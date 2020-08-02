using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace Tests
{
    /// <summary>
    /// These tests demonstrate how to use ThreadPool class. 
    /// </summary> 
    [TestClass]
    public class ThreadPoolTests
    {
#if !NET461
        [TestInitialize]
        public void Initialize()
        {
            Debug.WriteLine($"Pending work items: {ThreadPool.PendingWorkItemCount}, Completed work items {ThreadPool.CompletedWorkItemCount}");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Debug.WriteLine($"Pending work items: {ThreadPool.PendingWorkItemCount}, Completed work items {ThreadPool.CompletedWorkItemCount}");
        }
#endif

        [TestMethod]
        public void TestThreadPoolInfo()
        {
            int workerThreads, ioThreads;

            Debug.WriteLine($"Host processors count: {Environment.ProcessorCount}");

            ThreadPool.GetMaxThreads(out workerThreads, out ioThreads);

            Debug.WriteLine($"Max worker threads: {workerThreads}, Max IO threads: {ioThreads}");

            ThreadPool.GetMinThreads(out workerThreads, out ioThreads);

            Debug.WriteLine($"Min worker threads: {workerThreads}, Min IO threads: {ioThreads}");

            ThreadPool.SetMaxThreads(Environment.ProcessorCount * 2, Environment.ProcessorCount * 2);
        }

        [TestMethod]
        public void TestThreadPoolQueue()
        {
            var mre = new ManualResetEvent(false);

            var callback = new WaitCallback((object state) =>
            {
                for (int i = 0; i < 3; i++)
                {
                    Debug.WriteLine($"Get {i} from (Thread ID: {Thread.CurrentThread.ManagedThreadId}, IsBacground: {Thread.CurrentThread.IsBackground}, IsThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread})");
                    Thread.Sleep(1000);
                }

                mre.Set();
            });

            Debug.WriteLine($"Hello from TestThreadPoolQueue() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            ThreadPool.QueueUserWorkItem(callback);

#if !NET461
            Debug.WriteLine($"Pending work items: {ThreadPool.PendingWorkItemCount}, Completed work items {ThreadPool.CompletedWorkItemCount}");
#endif
            mre.WaitOne();

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        public class Person
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int Age { get; set; }
        }

        [TestMethod]
        public void ThreadPoolQueueWithState()
        {
            var mre = new ManualResetEvent(false);

            var callback = new WaitCallback((object state) =>
            {
                Person p = state as Person;

                string s = $"{p.FirstName} {p.LastName} is {p.Age} years old";

                if (p.Age <= 16)
                {
                    s += " and he is a teenager.";
                }
                else
                {
                    s += ".";
                }

                s += $" (Thread ID: {Thread.CurrentThread.ManagedThreadId}, IsThreadPoolThread: {Thread.CurrentThread.IsThreadPoolThread})";

                Debug.WriteLine(s);

                mre.Set();
            });

            var person = new Person()
            {
                FirstName = "Vasya",
                LastName = "Pupkin",
                Age = 15
            };

            Debug.WriteLine($"Hello from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            ThreadPool.QueueUserWorkItem(callback, person);

            mre.WaitOne();

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }
    }
}
