using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    /// <summary>
    /// These tests demonstrate how to use Mutex class.
    /// Mutex is a synchronization primitive that can also be used for interprocess synchronization.
    /// </summary> 
    [TestClass]
    public class MutexTests
    {
        // Create a new Mutex. The creating thread does not own the mutex.
        private static Mutex _mutex = new Mutex();

        /// <summary>
        /// This method represents a resource that must be synchronized so that only one thread at a time can enter.
        /// </summary>
        private static void CriticalMethod()
        {
            Debug.WriteLine($"Try to enter to critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            try
            {
                // Blocks the current thread until the current WaitHandle receives a signal.
                _mutex.WaitOne();

                Debug.WriteLine($"Process critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                // Code to synchronize the resource should be placed here.

                // Simulate some work.
                Thread.Sleep(1000);

                Debug.WriteLine($"End critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            }
            finally
            {
                _mutex.ReleaseMutex();

                Debug.WriteLine($"Released the mutex (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            }
        }

        /// <summary>
        /// This example shows how a Mutex is used to synchronize access to a protected resource. 
        /// Because each calling thread is blocked until it acquires ownership of the mutex, it must
        /// call the ReleaseMutex method to release ownership of the thread.
        /// </summary>
        [TestMethod]
        public void TestMutexThreadBlock()
        {
            // Create multiple threads to understand Mutex
            for (int i = 1; i <= 3; i++)
            {
                Thread threadObject = new Thread(CriticalMethod);
                threadObject.Start();
            }

            Thread.Sleep(4000);
        }

        /// <summary>
        /// Same as above, but used Tasks.
        /// </summary>
        [TestMethod]
        public void TestMutexTaskBlock()
        {
            Task[] tasks = new Task[3];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(CriticalMethod);
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// In the following example, each thread calls the WaitOne(Int32) method to acquire the mutex. 
        /// If the time-out interval elapses, the method returns false, and the thread neither acquires 
        /// the mutex nor gains access to the resource the mutex protects. The ReleaseMutex method is 
        /// called only by the thread that acquires the mutex.
        /// </summary>
        [TestMethod]
        public void TestMutexThreadBlockWithTimeout()
        {
            var caller = new ThreadStart(() =>
            {
                Debug.WriteLine($"Try to enter to critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                // Wait until it is safe to enter, and do not enter if the request times out.
                if (_mutex.WaitOne(1000))
                {
                    try
                    {
                        Debug.WriteLine($"Process critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                        // Code to synchronize the resource should be placed here.

                        // Simulate some work.
                        Thread.Sleep(2000);

                        Debug.WriteLine($"End critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                    }
                    finally
                    {
                        _mutex.ReleaseMutex();

                        Debug.WriteLine($"Has released the mutex (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                    }
                }
                else
                {
                    Debug.WriteLine($"Has not accessed critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                }
            });

            // Create multiple threads to understand Mutex
            for (int i = 1; i <= 3; i++)
            {
                Thread threadObject = new Thread(caller);
                threadObject.Start();
            }

            Thread.Sleep(4000);
        }
    }
}
