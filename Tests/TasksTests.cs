using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    /// <summary>
    /// This tests demonstrate how to use Task class.
    /// Task-based Asynchronous Pattern (TAP). 
    /// </summary> 
    [TestClass]
    public class TasksTests
    {
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
        /// In the below example, we are creating the task object by using the Task class and then start executing it by calling the Start method on the Task object.
        /// Very similar as we use Threads.
        /// </summary>
        [TestMethod]
        public void TestTaskStart()
        {
            Debug.WriteLine($"Hello from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            var task = new Task(DoSimpleWork);

            // Start the task
            task.Start();

            Thread.Sleep(1000);

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Start the new task, run it and wait when it finished.
        /// </summary>
        [TestMethod]
        public void TestTaskStartAndWait()
        {
            Debug.WriteLine($"Hello from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // Create and run the task. There are few ways how to do it.
            var task = Task.Run(DoHardWork);
            //var task = Task.Factory.StartNew(DoHardWork); 

            // Do something else on main thread.

            task.Wait();

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// Start the new task and configure it for long running.
        /// </summary>
        [TestMethod]
        public void TestTaskStartAndLongRunning()
        {
            Debug.WriteLine($"Hello from main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");

            // A newly created thread that would be dedicated to this task and would be destroyed once your task would have been completed.
            var task = Task.Factory.StartNew(DoHardWork, TaskCreationOptions.LongRunning);

            task.Wait();

            Debug.WriteLine($"End of main thread (Thread ID: {Thread.CurrentThread.ManagedThreadId})");
        }

        /// <summary>
        /// In the following example, the CalculateSum method takes an input integer value and calculate the sum of the number starting from 1 to that number.
        /// </summary>
        [DataTestMethod]
        [DataRow(3)]
        [DataRow(10)]
        public void TestTaskReturnValue(int value)
        {
            Task<int> task = Task.Run(() =>
            {
                int res = 0;

                for (int count = 1; count <= value; count++)
                {
                    res += count;
                }

                return res;
            });

            Debug.WriteLine($"Result from worker thread: {task.Result}");
        }

        public class Person
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int Age { get; set; }
        }

        /// <summary>
        /// Returns complex type.
        /// </summary>
        [TestMethod]
        public void TestTaskReturnComplex()
        {
            Task<Person> task = Task.Run(() =>
            {
                var res = new Person()
                {
                    FirstName = "Vasya",
                    LastName = "Pupkin",
                    Age = 15
                };

                return res;
            });

            var person = task.Result;

            Debug.WriteLine($"Result from worker thread: {person.FirstName} {person.LastName} is {person.Age} years old.");
        }

        /// <summary>
        /// In the following example, the antecedent task returns an DateTime value.
        /// When it completes its execution, then it passes that value to the continuation task
        /// and that continuation task does some operation and returns the result as a string.
        /// </summary>
        [TestMethod]
        public void TestTaskChaining()
        {
            Task<DateTime> antecedent = Task.Run(() =>
            {
                // Do some operations here like IO, API call etc.
                Task.Delay(1000).Wait();

                return DateTime.Now;
            });

            Task<string> continuation = antecedent.ContinueWith(x =>
            {
                // Return final result.
                return "Timestamp is " + x.Result.ToString();
            });

            Debug.WriteLine("This will display before the result.");

            Debug.WriteLine(continuation.Result);
        }

        /// <summary>
        /// The ContinueWith method has some overloaded versions that you can use to configure with multiple options
        /// when the continuation will run.In this way, you can add different continuation methods that will run 
        /// when an exception occurred, when the Task is canceled, or the Task completed successfully. Let's see an example to understand this.
        /// </summary>
        [TestMethod]
        public void TestTaskContinueWithConditions()
        {
            Task<DateTime> antecedent = Task.Run(() =>
            {
                // Uncomment it to test faulting continuation.
                //throw null;

                return DateTime.Now;
            });

            // May use to handle exceptions.
            var task = antecedent.ContinueWith(x =>
            {
                Debug.WriteLine("Fault! Previous task has been faulted.");
            }, TaskContinuationOptions.OnlyOnFaulted);

            task = antecedent.ContinueWith(x =>
            {
                Debug.WriteLine("Success! Timestamp is " + x.Result.ToString());
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }


        /// <summary>
        /// When you wait for a task to complete (by calling its Wait method or accessing its Result property), 
        /// any unhandled exceptions are conveniently rethrown to the caller, wrapped in an AggregateException object.
        /// </summary>
        [TestMethod]
        public void TestTaskExceptionHandling()
        {
            int x = 0;

            Task<int> calc = Task.Run(() => 1 / x);

            try
            {
                Debug.WriteLine(calc.Result);
            }
            catch (AggregateException ex)
            {
                Debug.Write(ex.InnerException.Message);  // Attempted to divide by 0
            }
        }

        /// <summary>
        /// Same as before but used async/await pattern.
        /// </summary>
        [TestMethod]
        public async Task TestTaskExceptionHandlingAsync()
        {
            int x = 0;

            try
            {
                int res = await Task.Run(() => 1 / x);
            }
            catch (DivideByZeroException ex) 
            {
                Debug.Write(ex.Message);  
            }
        }

        /// <summary>
        /// Using ContinueWith method which gives you an ability to specify what should be done once Task finishes processing the operation.
        /// </summary>
        [TestMethod]
        public void TestTaskExceptionHandlingContinueWith()
        {
            int x = 0;

            Task<int> calc = Task.Run(() => 1 / x);

            calc.ContinueWith(t =>
            {
                t.Exception.Handle(ex =>
                {
                    Debug.Write(ex.Message);

                    return true;
                });
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Canceling tasks.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task TestTaskCacelation()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var t = Task.Run(async() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    if (token.IsCancellationRequested) token.ThrowIfCancellationRequested();

                    await Task.Delay(100);
                }

                return DateTime.Now.ToString();
            });

            await Task.Yield();

            // Comment it to let the task run.
            tokenSource.Cancel();

            try
            {
                Debug.WriteLine("Timestamp: " + await t);
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(ex.Message);

                Debug.WriteLine("\nTask status: {0}", t.Status);
            }
            finally
            {
                tokenSource.Dispose();
            }
        }
    }
}
