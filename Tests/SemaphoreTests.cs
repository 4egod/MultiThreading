using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    /// <summary>
    /// These tests demonstrate how to use Semaphore class. 
    /// Semaphore is a synchronization primitive that limits the number of threads that can access a resource or pool of resources concurrently.
    /// In other words, we can say that Semaphore allows one or more threads to enter into the critical section and execute the task concurrently with thread safety.
    /// </summary> 
    [TestClass]
    public class SemaphoreTests
    {
        // A semaphore that simulates a limited resource pool.
        private static Semaphore _semaphore;

        /// <summary>
        /// A method that allows only a limited number of threads to access the critical section.
        /// </summary>
        private static void CriticalMethod()
        {
            Debug.WriteLine($"Try to enter to critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            try
            {
                // Blocks the current thread until the current WaitHandle receives a signal.
                _semaphore.WaitOne();

                Debug.WriteLine($"Process critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                // Simulate some work.
                Thread.Sleep(1000);

                Debug.WriteLine($"End critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            }
            finally
            {
                _semaphore.Release();

                Debug.WriteLine($"Released the semaphore (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            }
        }

        /// <summary>
        /// This example shows how a local Semaphore object is used to synchronize access to a critical section. 
        /// </summary>
        [TestMethod]
        public void TestSemaphoreThread()
        {
            // Create a semaphore that can satisfy up to three concurrent requests. Use an initial count of 2,
            // so 2 threads can have access to a critical section concurrently.
            _semaphore = new Semaphore(2, 3);

            // Create multiple threads to understand Semaphore
            for (int i = 1; i <= 10; i++)
            {
                Thread threadObject = new Thread(CriticalMethod);
                threadObject.Start();
            }

            Thread.Sleep(11000);
        }

        /// <summary>
        /// Same as above, but used Tasks.
        /// </summary>
        [TestMethod]
        public void TestSemaphoreTask()
        {
            // Create a semaphore that can satisfy up to three concurrent requests. Use an initial count of 2,
            // so 2 threads can have access to a critical section concurrently.
            _semaphore = new Semaphore(2, 3);

            Task[] tasks = new Task[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(CriticalMethod);
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Using an initial count of zero for semaphore.
        /// </summary>
        [TestMethod]
        public void TestSemaphoreTask2()
        {
            // Create a semaphore that can satisfy up to three concurrent requests. Use an initial count of zero,
            // so that the entire semaphore count is initially owned by the main program thread.
            _semaphore = new Semaphore(0, 3);

            Task[] tasks = new Task[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(CriticalMethod);
            }

            // Wait for two seconds, to allow all the threads to start and to block on the semaphore.
            Thread.Sleep(2000);

            // The main thread starts out holding the entire semaphore count. 
            // Calling Release(3) brings the semaphore count back to its maximum value, and
            // allows the waiting threads to enter the semaphore, up to three at a time.
            Debug.WriteLine($"Call Release(3) from the main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            _semaphore.Release(3);

            Task.WaitAll(tasks);

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }


        /// <summary>
        /// Example of using SemaphoreSlim class.
        /// Semaphores are of two types: local semaphores and named system semaphores. 
        /// Local semaphores are local to an application, system semaphores are visible throughout the operating system
        /// and are suitable for inter-process synchronization. The SemaphoreSlim is a lightweight alternative to the
        /// Semaphore class that doesn't use Windows kernel semaphores. Unlike the Semaphore class, the SemaphoreSlim 
        /// class doesn't support named system semaphores. You can use it as a local semaphore only. The SemaphoreSlim 
        /// class is the recommended semaphore for synchronization within a single app.
        /// </summary>
        [TestMethod]
        public void TestSemaphoreSlim()
        {
            // Create a semaphore that can satisfy up to three concurrent requests.
            var semaphore = new SemaphoreSlim(3);

            var action = new Action(() =>
            {
                Debug.WriteLine($"Try to enter to critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                try
                {
                    // Blocks the current thread until the current WaitHandle receives a signal.
                    semaphore.Wait();

                    Debug.WriteLine($"Process critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                    // Simulate some work.
                    Thread.Sleep(1000);

                    Debug.WriteLine($"End critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                }
                finally
                {
                    semaphore.Release();

                    Debug.WriteLine($"Released the semaphore (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                }
            });

            Task[] tasks = new Task[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(action);
            }

            Task.WaitAll(tasks);

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        [TestMethod]
        public async Task TestSemaphoreSlimAsync()
        {
            // Create a semaphore that can satisfy up to three concurrent requests.
            var semaphore = new SemaphoreSlim(3);

            Task[] tasks = new Task[10];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    Debug.WriteLine($"Try to enter to critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                    try
                    {
                        // Blocks the current thread until the current WaitHandle receives a signal.
                        await semaphore.WaitAsync();

                        Debug.WriteLine($"Process critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

                        // Simulate some work.
                        await Task.Delay(1000);

                        Debug.WriteLine($"End critical section (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                    }
                    finally
                    {
                        semaphore.Release();

                        Debug.WriteLine($"Released the semaphore (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                    }
                });
            }

            await Task.WhenAll(tasks);

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }
    }
}
