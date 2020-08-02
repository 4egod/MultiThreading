using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
namespace Tests
{
    /// <summary>
    /// These tests demonstrate how to use CountdownEvent class.
    /// CountdownEvent represents a synchronization primitive that is signaled when its count reaches zero.
    /// </summary> 
    [TestClass]
    public class CountdownEventTests
    {
        // Initial count = 5.
        private static CountdownEvent countdownEvent = new CountdownEvent(5);

        /// <summary>
        /// Method that waits for a signal.
        /// </summary>
        private static void WaitForSignal()
        {
            Thread.Sleep(500);
            Debug.WriteLine(Task.CurrentId + " is calling signal.");

            // Send a signal.
            countdownEvent.Signal();
        }

        /// <summary>
        /// The following example shows how to use a CountdownEvent.
        /// </summary>
        [TestMethod]
        public void TestCountdownEvent()
        {
            Task.Factory.StartNew(WaitForSignal);
            Task.Factory.StartNew(WaitForSignal);
            Task.Factory.StartNew(WaitForSignal);
            Task.Factory.StartNew(WaitForSignal);
            Task.Factory.StartNew(WaitForSignal);

            // Will return when countdownEvent count reaches 0.
            countdownEvent.Wait();

            Debug.WriteLine("Signal has been called 5 times");
        }
    }
}
