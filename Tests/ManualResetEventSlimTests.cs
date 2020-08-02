using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    /// <summary>
    /// These tests demonstrate how to use ManualResetEvent class.
    /// ManualResetEventSlim represents a thread synchronization event that, when signaled, must be reset manually. 
    /// This class is a lightweight alternative to ManualResetEvent.
    /// </summary> 
    [TestClass]
    public class ManualResetEventSlimTests
    {
        // initialize as unsignaled
        private static ManualResetEventSlim eventSlim = new ManualResetEventSlim(false);

        /// <summary>
        /// Method that waits for a signal.
        /// </summary>
        private static void WaitForSignal()
        {
            Debug.WriteLine($"Waiting for a signal (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // The event will reset automatically after signal received
            eventSlim.Wait();

            Debug.WriteLine($"The signal has been received (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// The following example shows how to use a ManualResetEventSlim.
        /// </summary>
        [TestMethod]
        public void TestManualResetEventSlim()
        {
            Debug.WriteLine($"Hello from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            Task.Factory.StartNew(WaitForSignal);
            Task.Factory.StartNew(WaitForSignal);
            Task.Factory.StartNew(WaitForSignal);

            Thread.Sleep(1000);

            Debug.WriteLine($"Set signal from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // All tasks should receive a signal.
            eventSlim.Set();

            Thread.Sleep(1000);

            // All tasks will not wait for a signal because the signal is already set.
            Task.Factory.StartNew(WaitForSignal);
            Thread.Yield();
            Task.Factory.StartNew(WaitForSignal);
            Thread.Yield();
            Task.Factory.StartNew(WaitForSignal);
            Thread.Yield();

            Thread.Sleep(1000);

            // Reset the signal.
            eventSlim.Reset();

            // All tasks will wait a signal again.
            Task.Factory.StartNew(WaitForSignal);
            Task.Factory.StartNew(WaitForSignal);
            Task.Factory.StartNew(WaitForSignal);

            Thread.Sleep(1000);

            eventSlim.Set();
        }
    }
}
