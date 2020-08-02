using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;

namespace Tests
{
    /// <summary>
    /// These tests demonstrate how to use AutoResetEvent class.
    /// AutoResetEvent represents a thread synchronization event that, when signaled, resets automatically after releasing a single waiting thread.
    /// </summary> 
    [TestClass]
    public class AutoResetEventTests
    {
        /// <summary>
        /// Create instance of ManualResetEvent in the unsignaled state (that is, false is passed to the constructor).
        /// </summary>
        private static AutoResetEvent event1 = new AutoResetEvent(false);
        private static AutoResetEvent event2 = new AutoResetEvent(false);

        /// <summary>
        /// Method that waits for a signal.
        /// </summary>
        private static void WaitForSignal()
        {
            Debug.WriteLine($"Waiting for a signal (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // The event will reset automatically after signal received
            event1.WaitOne();

            Debug.WriteLine($"The signal has been received (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// The following example shows how to use AutoResetEvent to release worker thread, by calling the Set method on the main thread.
        /// </summary>
        [TestMethod]
        public void TestAutoResetEvent()
        {
            Debug.WriteLine($"Hello from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Create new thread.
            var thread = new Thread(WaitForSignal);

            // Start the thread.
            thread.Start();

            // Simulate some work.
            Thread.Sleep(2000);

            Debug.WriteLine($"Set signal from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            event1.Set();
        }

        [TestMethod]
        public void TestAutoResetEventTwoThreads()
        {
            Debug.WriteLine($"Hello from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Create new thread.
            var thread = new Thread(WaitForSignal);

            // Create a second thread.
            var secondThread = new Thread(WaitForSignal);

            // Start the thread.
            thread.Start();
            secondThread.Start();

            // Simulate some work.
            Thread.Sleep(2000);

            Debug.WriteLine($"Set signal from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            event1.Set();

            Thread.Sleep(1000);

            // Set the signal second time 
            Debug.WriteLine($"Set signal from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            event1.Set();
        }

        [TestMethod]
        public void TestAutoResetEventTwoWay()
        {
            var action = new ThreadStart(() =>
            {
                Debug.WriteLine($"Hello from worker thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                Thread.Sleep(2000);

                Debug.WriteLine($"Set signal from worker thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                event1.Set();

                Debug.WriteLine($"Waiting for a signal from worker thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                event2.WaitOne();
                Debug.WriteLine($"Worker thread released (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                Debug.WriteLine($"End of worker thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            });

            Debug.WriteLine($"Hello from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Create new thread.
            var thread = new Thread(action);
            thread.Start();

            Debug.WriteLine($"Waiting for a signal from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            event1.WaitOne();
            Debug.WriteLine($"Main thread released (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            Thread.Sleep(2000);
            Debug.WriteLine($"Set signal from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            event2.Set();

            Thread.Sleep(1000);

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }
    }
}
