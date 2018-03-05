namespace OzetteProtocol.Constants
{
    /// <summary>
    /// A constants class for connections related settings.
    /// </summary>
    public static class Connections
    {
        /// <summary>
        /// The size of the TCP connection receive side buffer.
        /// </summary>
        public const int ReceiveBufferSizeBytes = 10240;

        /// <summary>
        /// The number of milliseconds to wait if there are no pending TCP connections.
        /// </summary>
        public const int NoPendingConnectionsWaitRetryMilliseconds = 500;

        /// <summary>
        /// The number of milliseconds to wait if there are no outgoing messages to send.
        /// </summary>
        public const int NoOutgoingMessagesWaitRetryMilliseconds = 500;
    }
}
