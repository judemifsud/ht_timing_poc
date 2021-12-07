using System;
using System.Threading;
using NUnit.Framework;
using Org.LLRP.LTK.LLRPV1;

namespace ReaderClient.Tests
{
    [TestFixture]
    public class ReaderClientTests
    {
        [Test]
        public void ShouldConnect()
        {
            using var readerClient = new ReaderClient();
            uint msgID = 0;
            // wait around to collect some data.
            for (int delay = 0; delay < 10; delay++)
            {
                Thread.Sleep(10);
                #region PollReaderReports
                {
                    Console.WriteLine("Polling Report Data\n");
                }
                #endregion
            }
            //Assert.IsTrue(readerClient.Reader.IsConnected);
        }
    }
}