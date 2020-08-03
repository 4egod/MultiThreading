using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Tests
{
    /// <summary>
    /// The event-based asynchronous pattern (EAP) provides a simple means by which classes can offer
    /// multithreading capability without consumers needing to explicitly start or manage threads.
    /// Starting with the .NET Framework 4, the Task Parallel Library provides a new model for asynchronous and parallel programming.
    /// The EAP is just a pattern, so these features must be written by the implementer. 
    /// Just a few classes in the Framework follow this pattern, so instead of implementing this pattern I'll show how to use it on the following example.
    /// </summary>
    [TestClass]
    public class EAPTests
    {
        [TestMethod]
        public void TestEAP()
        {
            var wc = new WebClient();

            AutoResetEvent e = new AutoResetEvent(false);

            // Will be called
            wc.DownloadStringCompleted += (sender, args) =>
            {
                if (args.Cancelled)
                    Debug.WriteLine("Canceled");
                else if (args.Error != null)
                    Debug.WriteLine("Exception: " + args.Error.Message);
                else
                {
                    Debug.WriteLine(args.Result.Length + " chars were downloaded");
                    // We could update the UI from here...
                }

                e.Set();
            };

            wc.DownloadStringAsync(new Uri("https://ya.ru"));

            // We can cancel a pending asynchronous operation.
            //wc.CancelAsync();

            e.WaitOne();
        }
    }
}
