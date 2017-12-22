﻿using System;
using System.Threading;

namespace OzetteLibrary.Events
{
    public class AsyncResult : IAsyncResult
    {
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
