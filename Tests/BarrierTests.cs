using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    /// <summary>
    /// This tests demonstrate how to use Barrier class.
    /// A System.Threading.Barrier is a synchronization primitive that enables multiple threads (known as participants)
    /// to work concurrently on an algorithm in phases. Each participant executes until it reaches the barrier point in
    /// the code. The barrier represents the end of one phase of work.
    /// </summary> 
    [TestClass]
    public class BarrierTests
    {
        /// <summary>
        /// In the following example, each of three threads writes the numbers 0 through 4, while keeping in step with the other threads
        /// </summary>
        [TestMethod]
        public void TestBarrier()
        {
            // Create the Barrier object, and supply a post-phase delegate to be invoked at the end of each phase.
            Barrier barrier = new Barrier(3, x => Debug.WriteLine(""));

            var action = new Action(() =>
            {
                for (int i = 0; i < 5; i++)
                {
                    Debug.Write(i + " ");

                    // End phase.
                    barrier.SignalAndWait();
                }
            });

            var t1 = Task.Run(action);
            var t2 = Task.Run(action);
            var t3 = Task.Run(action);

            Task.WaitAll(t1, t2, t3);
        }
    }
}
