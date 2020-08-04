using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;

namespace Tests
{
    /// <summary>
    /// These tests demonstrate how to use ThreadLocal<T> class. 
    /// ThreadLocal<T> provides thread-local storage for both static and
    /// instance fields, and allows you to specify default values.  
    /// </summary>
    [TestClass]
    public class ThreadLocalTests
    {
        // Initialize new static ThreadLocal<int> object and set it's value to 5.
        private static ThreadLocal<int> threadLocalStatic = new ThreadLocal<int>(() => 5);

        // Initialize new instance ThreadLocal<int> object and set it's value to 7.
        private ThreadLocal<int> threadLocalInstance = new ThreadLocal<int>(() => 7);

        [TestMethod]
        public void TestThreadLocal(int value)
        {
            // Try to change staic field.
            new Thread(() => threadLocalInstance.Value = 77).Start();

            // Try to change instance field.
            new Thread(() => threadLocalStatic.Value = 55).Start();

            // Print values in the third thread.
            new Thread(() => Debug.WriteLine($"ThreadLocal instance value: {threadLocalInstance}\n" +
                $"ThreadLocal static value: {threadLocalStatic}\n")).Start();
        }
    }
}
