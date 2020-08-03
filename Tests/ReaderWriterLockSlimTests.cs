using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    /// <summary>
    /// This tests demonstrate how to use Task class.
    /// Represents a lock that is used to manage access to a resource, allowing multiple threads for reading or exclusive access for writing.
    /// ReaderWriterLockSlim is similar to ReaderWriterLock, but it has simplified rules for recursion and for upgrading and downgrading lock state.
    /// ReaderWriterLockSlim avoids many cases of potential deadlock. In addition, the performance of ReaderWriterLockSlim is significantly better
    /// than ReaderWriterLock. ReaderWriterLockSlim is recommended for all new development.
    /// </summary> 
    [TestClass]
    public class ReaderWriterLockSlimTests
    {
        /// <summary>
        /// An instance of ReaderWriterLockSlim is used to synchronize access to the Dictionary (numbers) that serves as the inner cache.
        /// </summary>
        private static ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        /// <summary>
        /// Rundom number generator.
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// Dictionary where key is a random number and value is thread's id where this number was generated.
        /// </summary>
        private static Dictionary<int, int> numbers = new Dictionary<int, int>();

        /// <summary>
        /// Bet a random number and continuously checking inner cache for that number. 
        /// </summary>
        private static void Read()
        {
            // Pick a number.
            int x = GetRandom();

            while (true)
            {
                locker.EnterReadLock();

                // Check if picked number exist in the inner cache.
                if (numbers.ContainsKey(x))
                {
                    Debug.WriteLine($"Bingo! {x} added by {numbers[x]} and picked by {Thread.CurrentThread.ManagedThreadId}");

                    locker.ExitReadLock();

                    break;
                }

                locker.ExitReadLock();                
            }            
        }

        /// <summary>
        /// Generates unique number and adds it to inner cache.
        /// </summary>
        private static void Write()
        {
            int i = 0;

            while (i < 50)
            {
                int key = GetRandom();

                locker.EnterWriteLock();

                if (numbers.ContainsKey(key))
                {
                    locker.ExitWriteLock();

                    continue;
                } 
                else
                {
                    numbers.Add(key, Thread.CurrentThread.ManagedThreadId);

                    locker.ExitWriteLock();

                    Debug.WriteLine($"{key} added by {Thread.CurrentThread.ManagedThreadId}");

                    Thread.Sleep(500);

                    i++;
                }
            }
        }

        /// <summary>
        /// Thread safe rundom number generator.
        /// </summary>
        /// <returns>Random number in a range from 0 to 99.</returns>
        private static int GetRandom()
        {
            lock (random)
            {
                return random.Next(0, 100);
            }
        }

        /// <summary>
        /// Creates two threads for writing that generate unique numbers from 0 to 99 and add them to the inner cache.
        /// Also creates 3 threads for reading.
        /// </summary>
        [TestMethod]
        public void TestReaderWriterLockSlim()
        {
            var tasks = new Task[5];

            for (int i = 0; i < tasks.Length; i++)
            {
                if (i % 2 == 0)
                {
                    tasks[i] = Task.Factory.StartNew(Read);
                }
                else
                {
                    tasks[i] = Task.Factory.StartNew(Write);
                }
            }

            Task.WaitAll(tasks);
        }
    }
}
