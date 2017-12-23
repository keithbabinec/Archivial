using System;
using System.Threading;

namespace OzetteLibrary.Events
{
    /// <summary>
    /// An easy implementation of IAsyncResult.
    /// </summary>
    /// <remarks>
    /// Credit: Some of this implementation format/design (particularly AsyncWaitHandle) is borrowed from the AsyncResult 
    /// base class found under the (open sourced) System.Management.Automation namespace. Which can be viewed on the PowerShell 
    /// github page. All non-essential features have been stripped out, so this is pretty barebones.
    /// </remarks>
    public class AsyncResult : IAsyncResult
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AsyncResult()
        {
            ThreadSync = new object();
        }

        /// <summary>
        /// A wait handle for async operations.
        /// </summary>
        private ManualResetEvent _completedWaitHandle;

        /// <summary>
        /// Flag to indicate if the command was completed synchronously (always false).
        /// </summary>
        public bool CompletedSynchronously
        {
            get { return false; }
        }

        /// <summary>
        /// Flag to indicate if the async operation has completed yet.
        /// </summary>
        public bool IsCompleted
        {
            get;
            private set;
        }

        /// <summary>
        /// Not supported (returns null).
        /// </summary>
        public object AsyncState { get; }

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        internal object ThreadSync { get; set; }

        /// <summary>
        /// Gets a System.Threading.WaitHandle that is used to wait for an asynchronous
        /// operation to complete.
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (null == _completedWaitHandle)
                {
                    lock (ThreadSync)
                    {
                        if (null == _completedWaitHandle)
                        {
                            _completedWaitHandle = new ManualResetEvent(IsCompleted);
                        }
                    }
                }

                return _completedWaitHandle;
            }
        }

        /// <summary>
        /// Sets the async operation result to completed.
        /// </summary>
        internal void Complete()
        {
            lock (ThreadSync)
            {
                if (IsCompleted == false)
                {
                    IsCompleted = true;
                    if (null != _completedWaitHandle)
                    {
                        _completedWaitHandle.Set();
                    }
                }
            }
        }
    }
}
