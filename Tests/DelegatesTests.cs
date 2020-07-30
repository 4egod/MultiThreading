using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace Tests
{
    /// <summary>
    /// This tests demonstrate how to use asynchronous programming in C# using delegates. 
    /// This code is mostly legacy and doesn't supported in modern .NET Core.
    /// </summary>
    [TestClass]
    public class DelegatesTests
    {
        /// <summary>
        /// A simple method that writes to debug.
        /// </summary>
        private void DoSimpleWork()
        {
            Debug.WriteLine($"Hello from DoSimpleWork() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// A simple method that simulating work.
        /// </summary>
        private void DoHardWork()
        {
            Debug.WriteLine($"Begin DoHardWork() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Attention! Bad code
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
            }

            Debug.WriteLine($"End DoHardWork() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        private delegate void DoWorkDelegate();

        /// <summary>
        /// Synchronous call.
        /// </summary>
        [TestMethod]
        public void TestSyncCall()
        {
            Debug.WriteLine($"Hello from TestSyncCall() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Synchronous call on the same thread.
            DoSimpleWork();
        }

        /// <summary>
        /// Synchronous call with delegate's invoke.
        /// </summary>
        [TestMethod]
        public void TestDelegateInvoke()
        {
            Debug.WriteLine($"Hello from TestDelegateCall() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Create the delegate.
            var caller = new DoWorkDelegate(DoSimpleWork);

            // Call the delegated method on the same thread.
            caller();
        }

#if NET461
        /// <summary>
        /// simple usage of asynchronous begin/end pattern.
        /// The BeginInvoke method initiates the asynchronous call. 
        /// The EndInvoke method retrieves the results of the asynchronous call. 
        /// </summary>
        [TestMethod]
        public void TestBeginEndPatternSimple()
        {
            Debug.WriteLine($"Hello from TestSimpleBeginEndPattern() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Create the delegate.
            var caller = new DoWorkDelegate(DoSimpleWork);

            // The BeginInvoke method initiates the asynchronous call. 
            // BeginInvoke returns an IAsyncResult, which can be used to monitor the progress of the asynchronous call.
            IAsyncResult result = caller.BeginInvoke(null, null);

            // Simulating some work in the main thread.
            Thread.Sleep(0);
            Debug.WriteLine($"Hello again from TestSimpleBeginEndPattern() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // The EndInvoke method retrieves the results of the asynchronous call.
            // It can be called any time after BeginInvoke. 
            // If the asynchronous call has not completed, EndInvoke blocks the calling thread until it completes.
            caller.EndInvoke(result);

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Waiting for an Asynchronous Call with WaitHandle.
        /// You can obtain a WaitHandle by using the AsyncWaitHandle property of the IAsyncResult returned by BeginInvoke. 
        /// The WaitHandle is signaled when the asynchronous call completes, and you can wait for it by calling the WaitOne method.
        /// </summary>
        [TestMethod]
        public void TestBeginEndPatternWithWaitHandle()
        {
            Debug.WriteLine($"Hello from TestSimpleBeginEndPattern() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Create the delegate.
            var caller = new DoWorkDelegate(DoHardWork);

            IAsyncResult result = caller.BeginInvoke(null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            caller.EndInvoke(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Polling for Asynchronous Call Completion.
        /// You can use the IsCompleted property of the IAsyncResult returned by BeginInvoke to discover when the asynchronous call completes.
        /// You might do this when making the asynchronous call from a thread that services the user interface. 
        /// Polling for completion allows the calling thread to continue executing while the asynchronous call executes on a ThreadPool thread.
        /// </summary>
        [TestMethod]
        public void TestBeginEndPatternWithPolling()
        {
            Debug.WriteLine($"Hello from TestBeginEndPatternWithPolling() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Create the delegate.
            var caller = new DoWorkDelegate(DoHardWork);

            IAsyncResult result = caller.BeginInvoke(null, null);

            // Poll while simulating work.
            while (result.IsCompleted == false)
            {
                Debug.WriteLine($"Hello again from TestBeginEndPatternWithPolling() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                Thread.Sleep(250); // Attention! Bad code
            }

            caller.EndInvoke(result);

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Executing a Callback Method When an Asynchronous Call Completes.
        /// If the thread that initiates the asynchronous call does not need to be the thread that processes the results,
        /// you can execute a callback method when the call completes. The callback method is executed on a ThreadPool thread.
        /// </summary>
        [TestMethod]
        public void TestBeginEndPatternWithCallback()
        {
            Debug.WriteLine($"Hello from TestBeginEndPatternWithCallback() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            var doWork = new DoWorkDelegate(DoHardWork);

            var callback = new AsyncCallback(Callback);

            IAsyncResult ar = doWork.BeginInvoke(callback, doWork);

            // The callback is made on a ThreadPool thread. ThreadPool threads
            // are background threads, which do not keep the application running
            // if the main thread ends.
            Thread.Sleep(2000);

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Executing a Callback Method When an Asynchronous Call Completes.
        /// The main thread will finish before the second one ends. 
        /// </summary>
        [TestMethod]
        public void TestBeginEndPatternWithCallbackAbortException()
        {
            Debug.WriteLine($"Hello from TestBeginEndPatternWithCallbackAbortException() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            var doWork = new DoWorkDelegate(DoHardWork);

            var callback = new AsyncCallback(Callback);

            IAsyncResult ar = doWork.BeginInvoke(callback, doWork);

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Will rise ThreadAbortException on the second thread.
        }

        /// <summary>
        /// The callback method must have the same signature as the DoWorkDelegate.
        /// </summary>
        /// <param name="ar">IAsyncResult, which can be used to monitor the progress of the asynchronous call.</param>
        private void Callback(IAsyncResult ar)
        {
            // Retrieve the delegate.
            var caller = ar.AsyncState as DoWorkDelegate;

            caller.EndInvoke(ar);
        }
#endif
    }
}
