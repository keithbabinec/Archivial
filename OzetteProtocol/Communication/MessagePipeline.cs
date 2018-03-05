using OzetteProtocol.Constants;
using OzetteProtocol.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace OzetteProtocol.Communication
{
    /// <summary>
    /// Contains raw I/O functionality for sending and receiving protocol base messages.
    /// </summary>
    public class MessagePipeline
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="listenPort">The tcp port to listen on.</param>
        public MessagePipeline(int listenPort)
        {
            if (listenPort <= 0)
            {
                throw new ArgumentException(nameof(listenPort) + " must be provided.");
            }

            ListeningPort = listenPort;

            ThreadSync = new object();
            lock (ThreadSync)
            {
                Running = false;
            }

            Outgoing = new ConcurrentQueue<BaseTransaction>();
            Sent = new ConcurrentDictionary<Guid, BaseTransaction>();
        }

        /// <summary>
        /// Starts the message pipeline.
        /// </summary>
        /// <remarks>
        /// The pipeline will not send or receive messages until this function is called.
        /// </remarks>
        public void Start()
        {
            lock (ThreadSync)
            {
                if (Running == false)
                {
                    Running = true;
                }
                else
                {
                    throw new InvalidOperationException("The pipeline is already running.");
                }
            }

            Thread receiver = new Thread(() => InternalReceiveWaiter());
            receiver.Start();

            Thread sender = new Thread(() => InternalSendWaiter());
            sender.Start();
        }

        /// <summary>
        /// Stops the pipeline.
        /// </summary>
        public void Stop()
        {
            lock (ThreadSync)
            {
                if (Running == true)
                {
                    Running = false;
                }
            }
        }

        /// <summary>
        /// A flag to indicate if the pipeline is running.
        /// </summary>
        private bool Running;

        /// <summary>
        /// A thread-synchronization object.
        /// </summary>
        private object ThreadSync;

        /// <summary>
        /// The TCP port number to listen for incoming responses.
        /// </summary>
        private int ListeningPort;

        /// <summary>
        /// A reference to the current listening tcp socket.
        /// </summary>
        private TcpListener Listener;

        /// <summary>
        /// A collection of outgoing transaction messages.
        /// </summary>
        private ConcurrentQueue<BaseTransaction> Outgoing;

        /// <summary>
        /// A collection of transactions waiting for responses.
        /// </summary>
        private ConcurrentDictionary<Guid, BaseTransaction> Sent;

        /// <summary>
        /// Begins sending a transaction message through the pipeline.
        /// </summary>
        /// <param name="message">Message</param>
        public void BeginSend(BaseTransaction message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.SetMessageStatus(TransactionPipelineState.Queued);
            Outgoing.Enqueue(message);
        }

        /// <summary>
        /// A long running function to receive responses.
        /// </summary>
        private void InternalReceiveWaiter()
        {
            while (true)
            {
                // TODO: add logging
                // TODO: add error handling

                // is the pipeline supposed to be running?
                // bail out if we received the quit signal.

                lock (ThreadSync)
                {
                    if (Running == false)
                    {
                        break;
                    }
                }

                // ensure the TCP socket is open and listening.

                PrepareListeningSocket();

                // check if we have any pending TCP connections.
                // if yes, then accept the connection and receive the payload.
                // otherwise wait a bit before checking again.

                if (Listener.Pending())
                {
                    TcpClient incomingConnection = Listener.AcceptTcpClient();
                    incomingConnection.ReceiveBufferSize = Connections.ReceiveBufferSizeBytes;

                    // after the connection is accepted, open the provided stream.
                    // read all of the bytes sent, and deserialize it into an object.

                    using (var stream = incomingConnection.GetStream())
                    {
                        byte[] bytes = new byte[Connections.ReceiveBufferSizeBytes];
                        int i;

                        while ((i = stream.Read(bytes, 0, incomingConnection.ReceiveBufferSize)) != 0)
                        {
                            using (var memoryStream = new MemoryStream(bytes))
                            {
                                var deserialized = (new BinaryFormatter()).Deserialize(memoryStream);

                                if (deserialized is BaseResponse)
                                {
                                    // pull the message from the sent storage.
                                    // save the response.
                                    // update the status that it has been received.

                                    var response = deserialized as BaseResponse;
                                    BaseTransaction transaction = null;

                                    if (Sent.TryRemove(response.TransactionID, out transaction))
                                    {
                                        transaction.Response = response;
                                        transaction.SetMessageStatus(TransactionPipelineState.Received);
                                    }
                                    else
                                    {
                                        // the message transaction wasn't found.
                                        // this means we received a response we didn't send, or we received a second response
                                        // from a message we already processed.

                                        // TODO: log warning/error
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(Connections.NoPendingConnectionsWaitRetryMilliseconds));
                }
            }
        }

        /// <summary>
        /// A long running function to send messages.
        /// </summary>
        private void InternalSendWaiter()
        {
            while (true)
            {
                // TODO: add logging
                // TODO: add error handling

                // is the pipeline supposed to be running?
                // bail out if we received the quit signal.

                lock (ThreadSync)
                {
                    if (Running == false)
                    {
                        break;
                    }
                }

                if (Outgoing.Count > 0)
                {
                    // we have a message to send.
                    // open a tcp client connection and send the message payload.

                    // TODO: add retry functionality

                    BaseTransaction nextTransaction = null;
                    if (Outgoing.TryDequeue(out nextTransaction))
                    {
                        nextTransaction.SetMessageStatus(TransactionPipelineState.Sending);

                        TcpClient tcpConnection = GetTcpClient(nextTransaction.TargetHostname, nextTransaction.TargetPort);

                        using (var networkStream = tcpConnection.GetStream())
                        {
                            byte[] messageBytes = null;

                            using (var memoryStream = new MemoryStream())
                            {
                                (new BinaryFormatter()).Serialize(memoryStream, nextTransaction.Message);
                                messageBytes = memoryStream.ToArray();
                            }

                            networkStream.Write(messageBytes, 0, messageBytes.Length);
                        }

                        Sent.TryAdd(nextTransaction.TransactionID, nextTransaction);
                        nextTransaction.SetMessageStatus(TransactionPipelineState.Sent);
                    }
                }
                else
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(Connections.NoOutgoingMessagesWaitRetryMilliseconds));
                }
            }
        }

        /// <summary>
        /// Prepares the incoming listening socket.
        /// </summary>
        private void PrepareListeningSocket()
        {
            if (Listener == null)
            {
                Listener = new TcpListener(IPAddress.Any, ListeningPort);
                Listener.Start();

                // TODO: find out if we need to set Listener.ExclusiveAddressUse flag
            }

            // TODO: add other connection/socket validation here
        }

        /// <summary>
        /// Returns a TcpClient connection object for the specified Server and Port.
        /// </summary>
        /// <param name="Server">Target hostname.</param>
        /// <param name="Port">Targert tcp port number.</param>
        /// <returns><c>TcpClient</c></returns>
        private TcpClient GetTcpClient(string Server, int Port)
        {
            // TODO: implement TcpClient connection caching.

            TcpClient client = new TcpClient(Server, Port);
            return client;
        }
    }
}
