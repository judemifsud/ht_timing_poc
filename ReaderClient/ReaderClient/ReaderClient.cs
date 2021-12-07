using System;
using Org.LLRP.LTK.LLRPV1;

namespace ReaderClient
{
    public class ReaderClient : IDisposable
    {
        private readonly LLRPClient _reader;
        private static int _eventCount;

        public ReaderClient()
        {
            _reader = new LLRPClient();
            Console.WriteLine("# Adding Event Handlers");
            _reader.OnReaderEventNotification += reader_OnReaderEventNotification;
            _reader.OnRoAccessReportReceived += reader_OnRoAccessReportReceived;

            Console.WriteLine("# Connecting To Reader");
            _reader.Open("", 5000, out _);
            Console.WriteLine("# Connected To Reader");
        }

        public LLRPClient Reader => _reader;

        public void Dispose()
        {
            Console.WriteLine("# Disconnecting from Reader");
            _reader.Close();
            _reader.Dispose();
            Console.WriteLine("# Disconnected from Reader");
        }
        
        static void reader_OnReaderEventNotification(MSG_READER_EVENT_NOTIFICATION msg)
        {
            // Events could be empty
            if (msg.ReaderEventNotificationData == null) return;

            // Just write out the LTK-XML for now
            _eventCount++;

            Console.WriteLine("======Reader Event " + _eventCount + "======" +
                              DateTime.Now);

            // this is how you would look for individual events of interest
            // Here I just dump the event
            if (msg.ReaderEventNotificationData.AISpecEvent != null)
                Console.WriteLine(msg.ReaderEventNotificationData.AISpecEvent.ToString());
            if (msg.ReaderEventNotificationData.AntennaEvent != null)
                Console.WriteLine(msg.ReaderEventNotificationData.AntennaEvent.ToString());
            if (msg.ReaderEventNotificationData.ConnectionAttemptEvent != null)
                Console.WriteLine(msg.ReaderEventNotificationData.ConnectionAttemptEvent.ToString());
            if (msg.ReaderEventNotificationData.ConnectionCloseEvent != null)
                Console.WriteLine(msg.ReaderEventNotificationData.ConnectionCloseEvent.ToString());
            if (msg.ReaderEventNotificationData.GPIEvent != null)
                Console.WriteLine(msg.ReaderEventNotificationData.GPIEvent.ToString());
            if (msg.ReaderEventNotificationData.HoppingEvent != null)
                Console.WriteLine(msg.ReaderEventNotificationData.HoppingEvent.ToString());
            if (msg.ReaderEventNotificationData.ReaderExceptionEvent != null)
                Console.WriteLine(msg.ReaderEventNotificationData.ReaderExceptionEvent.ToString());
            if (msg.ReaderEventNotificationData.ReportBufferLevelWarningEvent != null)
                Console.WriteLine(msg.ReaderEventNotificationData.ReportBufferLevelWarningEvent.ToString());
            if (msg.ReaderEventNotificationData.ReportBufferOverflowErrorEvent != null)
                Console.WriteLine(msg.ReaderEventNotificationData.ReportBufferOverflowErrorEvent.ToString());
            if (msg.ReaderEventNotificationData.ROSpecEvent != null)
                Console.WriteLine(msg.ReaderEventNotificationData.ROSpecEvent.ToString());
        }

        private static void reader_OnRoAccessReportReceived(MSG_RO_ACCESS_REPORT msg)
        {
            // Report could be empty
            if (msg.TagReportData != null)
            {
                // Loop through and print out each tag
                for (var i = 0; i < msg.TagReportData.Length; i++)
                {
                    // just write out the EPC as a hex string for now. It is guaranteed to be
                    // in all LLRP reports regardless of default configuration

                    var data = "";
                    // Get PC Bits
                    if (msg.TagReportData[i].AirProtocolTagData.Length == 1)
                    {
                        data += ((PARAM_C1G2_PC)(msg.TagReportData[i].AirProtocolTagData[0])).PC_Bits.ToString();
                    }

                    data += "\t";
                    data += msg.TagReportData[i].PeakRSSI.PeakRSSI.ToString ();

                    data += "\t";
                    if (msg.TagReportData[i].EPCParameter[0].GetType() == typeof(PARAM_EPC_96))
                    {
                        data += ((PARAM_EPC_96)(msg.TagReportData[i].EPCParameter[0])).EPC.ToHexString();
                    }
                    else
                    {
                        data += ((PARAM_EPCData)(msg.TagReportData[i].EPCParameter[0])).EPC.ToHexString();
                    }

                    #region CheckForAccessResults
                    // check for read data results
                    if ((msg.TagReportData[i].AccessCommandOpSpecResult != null))
                    {
                        // there had better be one (since that what I asked for
                        if (msg.TagReportData[i].AccessCommandOpSpecResult.Count == 1)
                        {
                            // it had better be the read result
                            if (msg.TagReportData[i].AccessCommandOpSpecResult[0].GetType()
                                == typeof(PARAM_C1G2ReadOpSpecResult))
                            {
                                var read = (PARAM_C1G2ReadOpSpecResult)msg.TagReportData[i].AccessCommandOpSpecResult[0];
                                data += "\t";
                                if (read.Result == ENUM_C1G2ReadResultType.Success)
                                {
                                    data += read.ReadData.ToHexWordString();
                                }
                                else
                                {
                                    data += read.Result.ToString();
                                }
                            }
                        }
                    }
                    #endregion

                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine(data);
                    Console.WriteLine("msg.TagReportData[{0}]: {1}", i, msg.TagReportData[i]);
                }
            }
        }
    }
}