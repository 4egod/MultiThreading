using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace Tests
{
    /// <summary>
    /// These tests demonstrate how to use Thread class. 
    /// </summary> 
    [TestClass]
    public class ThreadsTests
    {
        private static object locker = new object();

        private static bool done = false;

        /// <summary>
        /// A simple method that writes to debug.
        /// </summary>
        private void DoSimpleWork()
        {
            Debug.WriteLine($"Hello from DoSimpleWork() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// A simple method that simulates work.
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

        /// <summary>
        /// A simple method that writes to Debug.
        /// </summary>
        /// <param name="value">Value to write.</param>
        private void Print(object value)
        {
            Debug.WriteLine(value.ToString());
        }

        /// <summary>
        /// Create a new Thread and then start it in background.
        /// </summary>
        [TestMethod]
        public void TestThreadStart()
        {
            Debug.WriteLine($"Hello from TestThreadStart() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            Thread t = new Thread(DoSimpleWork);
            t.Start();

            // Simulating some work in the main thread.
            Thread.Sleep(500); // sleep main thread for 500 milliseconds
            //Thread.Sleep(TimeSpan.FromHours(1)); // sleep for 1 hour

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Create a new Thread, start it in background and wait for the termination.
        /// </summary>
        [TestMethod]
        public void TestThreadStartJoin()
        {
            Debug.WriteLine($"Hello from TestThreadStartJoin() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            Thread t = new Thread(DoHardWork);
            t.Start();

            // Use the Thread.Join method to make the calling thread wait for the termination of the thread being stopped.
            t.Join();
            //t.Join(100); // Wait 100 ms for termination
            //t.Join(TimeSpan.FromHours(1)); // Wait 1 hour for termination

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Some class with instance and static methods.
        /// </summary>
        public class ThreadWrapper
        {
            /// <summary>
            /// Instance method.
            /// </summary>
            public void InstanceMethod()
            {
                Debug.WriteLine($"Hello from InstanceMethod() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            }

            /// <summary>
            /// Static method.
            /// </summary>
            public static void StaticMethod()
            {
                Debug.WriteLine($"Hello from StaticMethod() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            }
        }

        /// <summary>
        /// The following code example creates two new threads to call instance and static methods on another object.
        /// </summary>
        [TestMethod]
        public void TestThreadStartInstanceOrStatic()
        {
            Debug.WriteLine($"Hello from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            ThreadWrapper tw = new ThreadWrapper();

            // Create the thread object, passing in the tw.InstanceMethod method using a ThreadStart delegate.
            Thread InstanceCaller = new Thread(new ThreadStart(tw.InstanceMethod));

            // Start the thread.
            InstanceCaller.Start();
            Debug.WriteLine($"Hello again from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            InstanceCaller.Join();

            // Create the thread object, passing in the ThreadWrapper.StaticMethod method using a ThreadStart delegate.
            Thread StaticCaller = new Thread(new ThreadStart(ThreadWrapper.StaticMethod));

            // Start the thread.
            StaticCaller.Start();
            Debug.WriteLine($"Hello again from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            StaticCaller.Join();

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// The following code example shows context switching between threads.
        /// NET461 result:    ZYYYYYYYYYYYYYYXXXZZZZZZZZZZZZZZYYYXXXXXXXXXXXXXXZZZYYYYYYYYYYYYYYXXXXZZZZZZZZZZZZZYYYYYXXXXXXXXXXXXXZZZZYYYYYYYYYYYYYXXXXZZZZZZZZZZZZZYYYYXXXXXXXXXXXXXZZZZYYYYYYYYYYYYYXXXXXZZZZZZZZZZZZYYYYYXXXXXXXXXXXXXZZZZYYYYYYYYYYYYYYXXXXZZZZZZZZZZZZZYYYYYXXXXXXXXXXXXXZZZZYYYYYYYYYYXXXZZZZZZZZZZZZZZZXXXXXXXXXXX
        /// NETCORE31 result: ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXZZZZZZZZXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXZZZZZZZZZZZZZYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYY
        /// </summary>
        [TestMethod]
        public void TestThreadContextSwitching()
        {
            var caller = new ParameterizedThreadStart((object letter) =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Debug.Write(letter);
                }  
            });

            // First worker thread.
            var t1 = new Thread(caller);
            t1.Start("X");

            // Second worker thread.
            var t2 = new Thread(caller);
            t2.Start("Y");

            // Call from the main thread.
            caller("Z");

            t1.Join();
            t2.Join();
        }

        /// <summary>
        /// The following code example shows using local stack for variables. In this case we don't use the heap.
        /// </summary>
        [TestMethod]
        public void TestThreadLocalMemory()
        {
            var caller = new ThreadStart(() =>
            {
                // Using local variable for index.
                for (int i = 0; i < 10; i++)
                {
                    Debug.Write($"{i} ");
                }
            });

            // Worker thread.
            var t = new Thread(caller);
            t.Start();

            // Main thread.
            caller();
        }

        /// <summary>
        /// Unsafe sharing data between threads.
        /// </summary>
        [TestMethod]
        public void TestThreadUnsafeSharing()
        {
            var action = new Action(() =>
            {
                if (!done) 
                {
                    Debug.WriteLine($"Done (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                    done = true;
                }
            });

            Thread t = new Thread(new ThreadStart(action));
            t.Start();
            action();

            // "Done" being printed twice instead of once. 
        }

        /// <summary>
        /// Safe sharing data between threads.
        /// </summary>
        [TestMethod]
        public void TestThreadSafeSharing()
        {
            var action = new Action(() =>
            {
                lock (locker)
                {
                    if (!done)
                    {
                        Debug.WriteLine($"Done (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                        done = true;
                    }
                }
            });

            Thread t = new Thread(new ThreadStart(action));
            t.Start();
            action();

            // "Done" being printed only once. 
        }

        /// <summary>
        /// Safe sharing data between threads using Monitor.
        /// </summary>
        [TestMethod]
        public void TestThreadSafeSharingWithMonitor()
        {
            var action = new Action(() =>
            {
                // This is what "lock" exactly doing behind the scene
                Monitor.Enter(locker);
                try
                {
                    if (!done)
                    {
                        Debug.WriteLine($"Done (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
                        done = true;
                    }
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            });

            Thread t = new Thread(new ThreadStart(action));
            t.Start();
            action();

            // "Done" being printed only once. 
        }

        /// <summary>
        /// Deadlock example
        /// Deadlock is a situation where two or more threads are frozen in their execution because they are waiting for each other to finish
        /// </summary>
        [TestMethod]
        public void TestThreadDeadlock()
        {
            object l1 = new object();
            object l2 = new object();

            var a1 = new Action(() =>
            {
                lock (l1)
                {
                    Thread.Sleep(500);
                    lock (l2)
                    {
                    }
                }
            });

            var a2 = new Action(() =>
            {
                lock (l2)
                {
                    Thread.Sleep(500);
                    lock (l1)
                    {
                    }
                }
            });

            Thread t1 = new Thread(a1.Invoke);
            Thread t2 = new Thread(a2.Invoke);
            t1.Start();
            t2.Start();

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                Debug.WriteLine(t1.ThreadState);
                Debug.WriteLine(t2.ThreadState);
            }
        }

        /// <summary>
        /// The ThreadWithState class contains the information needed for a task, and the method that executes the task.
        /// </summary>
        public class ThreadWithState
        {
            // State information used in the task.
            private readonly string _greeting;

            public ThreadWithState(string greeting)
            {
                _greeting = greeting;
            }

            // The thread procedure performs the task, using state.
            public void ThreadProc()
            {
                Debug.WriteLine($"{_greeting} from ThreadWithState.ThreadProc() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            }
        }

        /// <summary>
        /// Passing Data to a Thread
        /// </summary>
        [TestMethod]
        public void TestThreadPassingData()
        {
            Debug.WriteLine($"Hello from TestThreadPassingData() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // A lambda expression execution.
            new Thread(() =>
            {
                Debug.WriteLine($"Hello from another thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            }).Start();

            // Anonymous methods.
            new Thread(delegate() 
            {
                Debug.WriteLine($"Hello from another thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            }).Start();

            // Pass an argument into Thread’s Start method (ParameterizedThreadStart delegate).
            Thread t = new Thread(Print);
            t.Start($"Hello from another thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            t.Join();

            // Use instance of ThreadWithState warpper.
            var tws = new ThreadWithState("Hello");
            t = new Thread(tws.ThreadProc);
            t.Start();
            t.Join();

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Wrapper class which contains callback delegate and thread function.
        /// </summary>
        public class SumWrapper
        {
            public delegate void ResultCallback(int result);

            private int _number;

            private ResultCallback _callback;

            public SumWrapper(int number, ResultCallback callback)
            {
                _number = number;
                _callback = callback;
            }

            // Thread function which will calculate the sum of the numbers.
            public void CalculateSum()
            {
                int res = 0;
                for (int i = 1; i <= _number; i++)
                {
                    res += i;
                }

                // Before the end of the thread function call the callback method.
                _callback?.Invoke(res);
            }
        }

        /// <summary>
        /// Retrieve data from a Thread Function using the callback method.
        /// </summary>
        [TestMethod]
        public void TestThreadRetriveData()
        {
            Debug.WriteLine($"Hello from TestThreadRetriveData() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            var callback = new SumWrapper.ResultCallback((int res) =>
            {
                Debug.WriteLine($"Result: {res} (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
            });

            var sum = new SumWrapper(10, callback);

            var t = new Thread(sum.CalculateSum);
            t.Start();
            t.Join();

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Exception Handling
        /// Any try/catch/finally blocks in scope when a thread is created are of no relevance to the thread when it starts executing. 
        /// </summary>
        [TestMethod]
        public void TestThreadExceptionHandling()
        {
            // Right way
            new Thread(() =>
            {
                try
                {
                    // ...
                    throw null; // The NullReferenceException will get caught below
                }
                catch (Exception)
                {
                    // Typically log the exception, and/or signal another thread that we've stuck
                }
            }).Start();

            // Wrong way
            try
            {
                // This will throw unhandled exception.
                //new Thread(() => throw null).Start();
            }
            catch
            {
                // We'll never get here!
                Debug.WriteLine("Exception!");
            }
        }

        /// <summary>
        /// The following example demonstrates how ManualResetEvent works as a signal.
        /// The example starts with a ManualResetEvent in the unsignaled state (that is, false is passed to the constructor).
        /// </summary>
        [TestMethod]
        public void TestThreadSignaling()
        {
            Debug.WriteLine($"Hello from TestThreadSignaling() (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            var waitHandle = new AutoResetEvent(false);

            new Thread(() =>
            {
                DoHardWork();
                waitHandle.Set();
            }).Start();

            waitHandle.WaitOne(); // wait for signal from the child thread.

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Use static variable as a thread-local for each thread. Each thread then sees a separate copy of local.
        /// </summary>
        [ThreadStatic]
        private static int local;

        private static int shared;

        [TestMethod]
        public void TestThreadLocalFields()
        {
            // Assign data in the first thread.
            new Thread(() => local = shared = 7).Start();

            // Print data in the second one.
            new Thread(() => Debug.WriteLine($"Local: {local}\nShared: {shared}")).Start();
        }
    }
}
