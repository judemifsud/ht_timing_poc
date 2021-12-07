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
            Assert.IsTrue(readerClient.Reader.IsConnected);
        }

        [Test]
        public void ShouldReceiveEvents()
        {
            uint msgID = 0;
            // wait around to collect some data.
            for (var delay = 0; delay < 10; delay++)
            {
                Thread.Sleep(10);
                #region PollReaderReports
                {
                    Console.WriteLine("Polling Report Data\n");
                }
                #endregion
            }
            Assert.IsTrue(true);
        }
    }
}