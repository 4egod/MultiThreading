using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tests
{
    /// <summary>
    /// This tests demonstrate how to use Parallel class.
    /// Parallel provides support for parallel loops and regions.
    /// </summary> 
    [TestClass]
    public class ParallelTests
    {
        static void Print(int value)
        {
            Debug.Write(value + " ");
        }

        /// <summary>
        /// This example demonstrates several approaches to implementing a parallel for loop using multiple language constructs.
        /// </summary>
        [DataTestMethod]
        [DataRow(100)]
        public void TestParallelFor(int range)
        {
            Debug.WriteLine("Using a static method:");
            Parallel.For(0, range, Print);

            Debug.WriteLine("\nUsing an anonymous delegate:");
            Parallel.For(0, range, delegate (int ix) { Debug.Write(ix + " "); });

            Debug.WriteLine("\nUsing a lambda expression:");
            Parallel.For(0, range, (ix) => { Debug.Write(ix + " "); });
        }
    }
}
