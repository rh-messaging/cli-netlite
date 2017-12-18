/*
 * Copyright 2017 Red Hat Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Amqp.Framing;

namespace ClientLib
{
    public static class ReturnCode
    {
        public const int ERROR_SUCCESS = 0;
        public const int ERROR_ARG = 2;
        public const int ERROR_OTHER = 1;
    }

    public class Utils
    {
        /// <summary>
        /// Get unix timestamp
        /// </summary>
        /// <returns>count of ms from 1970</returns>
        public static double GetTime()
        {
            return (0.001 * (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        /// <summary>
        /// sleep for next loop action
        /// </summary>
        /// <param name="in_ts">initial timestamp</param>
        /// <param name="in_count">number of iterations</param>
        /// <param name="in_duration">total time of all iterations</param>
        /// <param name="in_indx"next iteration index></param>
        public static void Sleep4Next(double in_ts, int in_count, int in_duration,
                                      int in_indx)
        {
            if ((in_duration > 0) && (in_count > 0))
            {
                double cumulative_dur = (1.0 * in_indx * in_duration) / in_count;
                while (true)
                {
                    if (GetTime() - in_ts - cumulative_dur > -0.01)
                        break;
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// timestamping initializer for tsData based on optMode
        /// </summary>
        /// <param name="optMode">timestamping mode (if contains string 'perf' then enabled)</param>
        /// <returns>tsData</returns>
        public static List<double> TsInit(string optMode)
        {
            List<double> tsData = new List<double>();
            if (optMode.IndexOf("perf") > -1)
            {
                for (int i = 0; i < 7; i++)
                {
                    tsData.Add(-1.0);
                }
                tsData.Add(Double.MaxValue);
                tsData.Add(0.0);
                tsData.Add(0.0);
            }
            return tsData;
        }

        /// <summary>
        ///  add timestamp of a step (step) to timestamp store/array (tsData)
        /// </summary>
        /// <param name="tsData">an initialized arraylist</param>
        /// <param name="step">the timestamp step</param>
        /// <param name="optMode">timestamping mode (if contains string "perf:(in_step)" then enabled)</param>
        public static void TsSnapStore(List<double> tsData, char step, string optMode)
        {
            if ((tsData != null) && (tsData.Count > 0) && (optMode.IndexOf("perf") > -1))
                tsData[(int)step - (int)('A')] = GetTime();
        }

        /// <summary>
        ///  add timestamp of a step (step) to timestamp store/array (tsData)
        /// </summary>
        /// <param name="tsData">an initialized arraylist</param>
        /// <param name="step">the timestamp step</param>
        /// <param name="optMode">timestamping mode (if contains string "perf:(in_step)" then enabled)</param>
        /// <param name="in_msg_ts">message</param>
        public static void TsSnapStore(List<double> tsData, char step, string optMode, double msg)
        {
            if ((tsData != null) && (tsData.Count > 0) && (optMode.IndexOf("perf") > -1))
            {
                if ((step == 'L') && (msg > -1))
                {
                    // latency manipulation
                    double int_lat = GetTime() - msg;
                    if (int_lat < tsData[7])
                    {
                        tsData[7] = int_lat;
                    }
                    if (int_lat > tsData[8])
                    {
                        tsData[8] = int_lat;
                    }
                    tsData[9] += int_lat;
                }
                else
                {
                    if (optMode.IndexOf(step) > -1)
                    {
                        //regular timestamp snap
                        tsData[(int)step - (int)('A')] = GetTime();
                    }
                }
            }
        }

        /// <summary>
        /// Report ts data
        /// </summary>
        /// <param name="tsData">array of ts data</param>
        /// <param name="msgCnt">count of messages</param>
        /// <param name="msgSize">size of messages</param>
        /// <param name="ecode">ecode of operation</param>
        /// <returns>string representation for log</returns>
        public static string TsReport(List<double> tsData, int msgCnt, int msgSize, int ecode)
        {
            Dictionary<string, object> int_result = null;
            if ((tsData != null) && (tsData.Count > 0))
            {
                int_result = new Dictionary<string, object>();
                int_result["msg-cnt"] = msgCnt;
                int_result["msg-size"] = msgSize;
                int_result["char-size"] = sizeof(Char);
                int_result["ecode"] = ecode;

                string timestamp;
                for (int i = 0; i < 7; i++)
                {
                    timestamp = "ts-" + (char)(i + (int)('A'));
                    if (tsData[i] < 0.0)
                    {
                        int_result[timestamp] = null;

                    }
                    else
                    {
                        int_result[timestamp] = tsData[i];

                    }
                }
                int_result["lat-min"] = tsData[7];
                int_result["lat-max"] = tsData[8];
                int_result["lat-acc"] = tsData[9];

                if (msgCnt > 0)
                {
                    int_result["lat-avg"] = tsData[9] / msgCnt;
                }
            }

            return Formatter.FormatMap(int_result);
        }
    }

    /// <summary>
    /// Extension class for set settlement mode
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convert int to SenderSettlementMode
        /// </summary>
        /// <param name="mode">int mode</param>
        /// <returns>SenderSettlementMode</returns>
        public static SenderSettleMode GetSenderFlag (this SettlementMode mode)
        {
            switch (mode) {
                case SettlementMode.AtMostOnce:
                    return SenderSettleMode.Settled;
                case SettlementMode.AtLeastOnce:
                    return SenderSettleMode.Unsettled;
                case SettlementMode.ExactlyOnce:
                    return SenderSettleMode.Unsettled;
            }
            throw new ArgumentException ();
        }

        /// <summary>
        /// Convert int to ReceiverSettleMode
        /// </summary>
        /// <param name="mode">int mode</param>
        /// <returns>ReceiverSettleMode</returns>
        public static ReceiverSettleMode GetReceiverFlag (this SettlementMode mode)
        {
            switch (mode) {
                case SettlementMode.AtMostOnce:
                    return ReceiverSettleMode.First;
                case SettlementMode.AtLeastOnce:
                    return ReceiverSettleMode.First;
                case SettlementMode.ExactlyOnce:
                    return ReceiverSettleMode.Second;
            }
            throw new ArgumentException ();
        }
    }

    /// <summary>
    /// Store int settle mode
    /// </summary>
    public enum SettlementMode
    {
        AtMostOnce,
        AtLeastOnce,
        ExactlyOnce
    }
}
