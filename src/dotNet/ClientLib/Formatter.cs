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
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Amqp;
using Amqp.Types;
using Newtonsoft.Json;

namespace ClientLib
{
    /// <summary>
    /// Class for formating message to text output
    /// </summary>
    public class Formatter
    {
        private const string None = "None";
        private const string EmptyDict = "{}";
        private const string EmptyList = "[]";


        /// <summary>
        /// Print message in upstream format
        /// </summary>
        /// <param name="msg">msg object</param>
        /// <param name="hashContent"></param>
        public static void PrintMessage(Message msg, bool hashContent)
        {            
            Console.WriteLine("Message(");
                if (msg.Header != null) Console.WriteLine(msg.Header.ToString());
                if (msg.DeliveryAnnotations != null) Console.WriteLine(msg.DeliveryAnnotations.ToString());
                if (msg.MessageAnnotations != null) Console.WriteLine(msg.MessageAnnotations.ToString());
                if (msg.Properties != null) Console.WriteLine(msg.Properties.ToString());
                if (msg.ApplicationProperties != null) Console.WriteLine(msg.ApplicationProperties.ToString());
                if (msg.Body != null) Console.WriteLine("body:{0}", hashContent ? Hash(msg.Body.ToString()): msg.Body.ToString());
                if (msg.Footer != null) Console.WriteLine(msg.Footer.ToString());
            Console.WriteLine(")");
        }

        /// <summary>
        /// Print message as python dict
        /// </summary>
        /// <param name="msg">message object</param>
        /// <param name="hashContent"></param>
        public static void PrintMessageAsDict(Message msg, bool hashContent)
        {         
            Dictionary<string, object> msgDict = new Dictionary<string,object>();
            msgDict.Add("durable", msg.Header.Durable);
            msgDict.Add("ttl", msg.Header.Ttl);
            msgDict.Add("delivery_count", msg.Header.DeliveryCount);
            msgDict.Add("priority", (uint)msg.Header.Priority);
            msgDict.Add("first_aquirer", msg.Header.FirstAcquirer);
            msgDict.Add("id", msg.Properties.MessageId);
            msgDict.Add("reply_to", msg.Properties.ReplyTo);
            msgDict.Add("subject", msg.Properties.Subject);
            msgDict.Add("creation-time", msg.Properties.CreationTime);
            msgDict.Add("absolute-expiry-time", msg.Properties.AbsoluteExpiryTime);
            msgDict.Add("content_encoding", msg.Properties.ContentEncoding);
            msgDict.Add("content_type", msg.Properties.ContentType);
            msgDict.Add("correlation_id", msg.Properties.CorrelationId);
            msgDict.Add("user_id", msg.Properties.UserId);
            msgDict.Add("group-id", msg.Properties.GroupId);
            msgDict.Add("group-sequence", msg.Properties.GroupSequence);
            msgDict.Add("reply-to-group-id", msg.Properties.ReplyToGroupId);
            msgDict.Add("content", hashContent ? Hash(msg.Body) : msg.Body);
            msgDict.Add("properties", msg.ApplicationProperties);
            msgDict.Add("message-annotations", msg.MessageAnnotations);
            Console.WriteLine(FormatMap(msgDict));
        }

        /// <summary>
        /// Print message as python dict (keys are named by AMQP standard)
        /// </summary>
        /// <param name="msg">message object</param>
        /// <param name="hashContent"></param>
        public static void PrintMessageAsInterop(Message msg, bool hashContent)
        {
            Dictionary<string, object> msgDict = new Dictionary<string, object>();
            msgDict.Add("durable", msg.Header.Durable);
            msgDict.Add("ttl", msg.Header.Ttl);
            msgDict.Add("delivery-count", msg.Header.DeliveryCount);
            msgDict.Add("priority", (uint)msg.Header.Priority);
            msgDict.Add("first-aquirer", msg.Header.FirstAcquirer);
            msgDict.Add("id", RemoveIDPrefix(msg.Properties.MessageId));
            msgDict.Add("to", msg.Properties.To);
            msgDict.Add("address", msg.Properties.To);
            msgDict.Add("reply-to", msg.Properties.ReplyTo);
            msgDict.Add("subject", msg.Properties.Subject);
            msgDict.Add("creation-time", msg.Properties.CreationTime);
            msgDict.Add("absolute-expiry-time", msg.Properties.AbsoluteExpiryTime);
            msgDict.Add("content-encoding", msg.Properties.ContentEncoding);
            msgDict.Add("content-type", msg.Properties.ContentType);
            msgDict.Add("correlation-id", RemoveIDPrefix(msg.Properties.CorrelationId));
            msgDict.Add("user-id", msg.Properties.UserId);
            msgDict.Add("group-id", msg.Properties.GroupId);
            msgDict.Add("group-sequence", msg.Properties.GroupSequence);
            msgDict.Add("reply-to-group-id", msg.Properties.ReplyToGroupId);
            msgDict.Add("content", hashContent ? Hash(msg.Body) : msg.Body);
            msgDict.Add("properties", msg.ApplicationProperties);
            //msgDict.Add("message-annotations", msg.MessageAnnotations);
            Console.WriteLine(FormatMap(msgDict));
        }

        /// <summary>
        /// Print message as python dict (keys are named by AMQP standard)
        /// </summary>
        /// <param name="msg">message object</param>
        /// <param name="hashContent"></param>
        public static void PrintMessageAsJson(Message msg, bool hashContent)
        {
            Dictionary<string, object> msgDict = new Dictionary<string, object>();
            msgDict.Add("durable", msg.Header.Durable);
            msgDict.Add("ttl", msg.Header.Ttl);
            msgDict.Add("delivery-count", msg.Header.DeliveryCount);
            msgDict.Add("priority", (uint)msg.Header.Priority);
            msgDict.Add("first-aquirer", msg.Header.FirstAcquirer);
            msgDict.Add("id", RemoveIDPrefix(msg.Properties.MessageId));
            msgDict.Add("to", msg.Properties.To);
            msgDict.Add("address", msg.Properties.To);
            msgDict.Add("reply-to", msg.Properties.ReplyTo);
            msgDict.Add("subject", msg.Properties.Subject);
            msgDict.Add("creation-time", msg.Properties.CreationTime);
            msgDict.Add("absolute-expiry-time", msg.Properties.AbsoluteExpiryTime);
            msgDict.Add("content-encoding", msg.Properties.ContentEncoding);
            msgDict.Add("content-type", msg.Properties.ContentType);
            msgDict.Add("correlation-id", RemoveIDPrefix(msg.Properties.CorrelationId));
            msgDict.Add("user-id", msg.Properties.UserId);
            msgDict.Add("group-id", msg.Properties.GroupId);
            msgDict.Add("group-sequence", msg.Properties.GroupSequence);
            msgDict.Add("reply-to-group-id", msg.Properties.ReplyToGroupId);
            msgDict.Add("content", hashContent ? Hash(msg.Body) : msg.Body);
            msgDict.Add("properties", msg.ApplicationProperties);
            Console.WriteLine(JsonConvert.SerializeObject(msgDict));
        }

        /// <summary>
        /// Print statistic info
        /// </summary>
        /// <param name="in_dict">stats obejct</param>
        public static void PrintStatistics(Dictionary<string, object> in_dict)
        {
            Console.WriteLine("STATS " + FormatMap(in_dict));
        }

        /// <summary>
        /// Help method for replace ID: prefix
        /// </summary>
        /// <param name="idString">string for ID replace</param>
        /// <returns>string without ID:</returns>
        private static string RemoveIDPrefix(string idString)
        {
            if (!String.IsNullOrEmpty(idString))
                return idString.Replace("ID:", String.Empty);
            return idString;
        }

        /// <summary>
        /// Escape quote
        /// </summary>
        /// <param name="in_str">input string</param>
        /// <returns>string with quote</returns>
        public static string QuoteStringEscape(string in_str)
        {
            List<char> data = new List<char>();
            char pattern = '\'';

            for (int i = 0; i < in_str.Length; i++)
            {
                if (in_str[i] == pattern)
                {
                    data.Add('\\');
                }
                data.Add(in_str[i]);
            }

            string strData = new string(data.ToArray());
            return strData;
        }


        /// <summary>
        /// Format boolean to string
        /// </summary>
        /// <param name="inData">boolean</param>
        /// <returns>string</returns>
        public static string FormatBool(bool inData)
        {
            if (inData)
            {
                return "True";
            }
            return "False";
        }

        /// <summary>
        /// Format date as unix timestamp
        /// </summary>
        /// <param name="inData">date time</param>
        /// <returns>timestamp</returns>
        public static string FormatDate(DateTime inData)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (inData - sTime).TotalSeconds.ToString();
        } 

        /// <summary>
        /// Format string
        /// </summary>
        /// <param name="inData">streing</param>
        /// <returns>string</returns>
        public static string FormatString(string inData)
        {
            string strData = Formatter.None;

            if (!String.IsNullOrEmpty(inData))
            {
                strData = "\"";
                strData = strData + QuoteStringEscape(inData) + "\"";
            }
            return strData;
        }

        /// <summary>
        /// Format byte array as string
        /// </summary>
        /// <param name="inData">byte array</param>
        /// <returns>string</returns>
        public static string FormatByteArray(byte[] inData)
        {
            return FormatString(System.Text.Encoding.Default.GetString(inData));
        }

        /// <summary>
        /// Format integer as string
        /// </summary>
        /// <param name="inData">integer</param>
        /// <returns>string</returns>
        public static string FormatInt(int inData)
        {
            return inData.ToString();
        }

        /// <summary>
        /// Format unsingned integer as string
        /// </summary>
        /// <param name="inData">uint</param>
        /// <returns>string</returns>
        public static string FormatUInt(uint inData)
        {
            return inData.ToString();
        }

        /// <summary>
        /// Format 16bit uint as string
        /// </summary>
        /// <param name="inData">uint 16</param>
        /// <returns>string</returns>
        public static string FormatUInt16(UInt16 inData)
        {
            return inData.ToString();
        }

        /// <summary>
        /// Format long as string
        /// </summary>
        /// <param name="inData">long</param>
        /// <returns>string</returns>
        public static string FormatLong(long inData)
        {
            return inData.ToString();
        }

        /// <summary>
        /// Format ulong as string
        /// </summary>
        /// <param name="inData">ulong</param>
        /// <returns>string</returns>
        public static string FormatULong(ulong inData)
        {
            return inData.ToString();
        }

        /// <summary>
        /// Format fload as string
        /// </summary>
        /// <param name="inData">float</param>
        /// <returns>string</returns>
        public static string FormatFloat(float inData)
        {
            if (inData == Math.Floor(inData))
                return inData.ToString("F1");
            return inData.ToString();
        }

        /// <summary>
        /// Format double as string
        /// </summary>
        /// <param name="inData">double</param>
        /// <returns>string</returns>
        public static string FormatFloat(double inData)
        {
            if (inData == Math.Floor(inData))
                return inData.ToString("F1");
            return inData.ToString();
        }

        /// <summary>
        /// Format dict as python dict
        /// </summary>
        /// <param name="inDict">map</param>
        /// <returns>string</returns>
        public static string FormatMap(Dictionary<string, object> inDict)
        {
            if (inDict != null)
            {
                if (inDict.Count == 0)
                {
                    return Formatter.EmptyDict;
                }
                else
                {
                    StringBuilder dict = new StringBuilder();
                    string delimiter = "";
                    dict.Append("{");
                    foreach(KeyValuePair<string, object> pair in inDict)
                    {
                        dict.Append(delimiter).Append(FormatString(pair.Key)).Append(": ").Append(FormatVariant(pair.Value));
                        delimiter = ", ";
                    }
                    dict.Append("}");
                    return dict.ToString();
                }

            }
            return Formatter.None;
        }

        /// <summary>
        /// Format AMQP map as python dict
        /// </summary>
        /// <param name="inDict">amqp map</param>
        /// <returns>string</returns>
        public static string FormatMap(Map inDict)
        {
            if (inDict != null)
            {
                if (inDict.Count == 0)
                {
                    return Formatter.EmptyDict;
                }
                else
                {
                    StringBuilder dict = new StringBuilder();
                    string delimiter = "";
                    dict.Append("{");
                    foreach (object key in inDict.Keys)
                    {
                        dict.Append(delimiter).Append(FormatString(key.ToString())).Append(": ").Append(FormatVariant(inDict[key]));
                        delimiter = ", ";
                    }
                    dict.Append("}");
                    return dict.ToString();
                }
            }
            return Formatter.None;
        }

        /// <summary>
        /// Format list as string
        /// </summary>
        /// <param name="inData">collection (arrayList, list)</param>
        /// <returnsstring></returns>
        public static string FormatList(Collection<object> inData)
        {
            if (inData != null)
            {
                if (inData.Count == 0)
                {
                    return Formatter.EmptyList;
                }
                else
                {
                    StringBuilder list = new StringBuilder();
                    string delimiter = "";
                    list.Append("[");
                    foreach(object data in inData)
                    {
                        list.Append(delimiter).Append(FormatVariant(data));
                        delimiter = ", ";
                    }
                    list.Append("]");
                    return list.ToString();
                }
            }
            return Formatter.None;
        }

        /// <summary>
        /// Format AMQP list as string
        /// </summary>
        /// <param name="inData">AMQP list</param>
        /// <returns>string</returns>
        public static string FormatList(List inData)
        {
            if (inData != null)
            {
                if (inData.Count == 0)
                {
                    return Formatter.EmptyList;
                }
                else
                {
                    StringBuilder list = new StringBuilder();
                    string delimiter = "";
                    list.Append("[");
                    foreach (object data in inData)
                    {
                        list.Append(delimiter).Append(FormatVariant(data));
                        delimiter = ", ";
                    }
                    list.Append("]");
                    return list.ToString();
                }
            }
            return Formatter.None;
        }

        /// <summary>
        /// Format priority
        /// </summary>
        /// <param name="inData">byte</param>
        /// <returns>string</returns>
        public static string FormatPriority(byte inData)
        {
            string strData = inData.ToString();
            if (inData == 0)
            {
                strData = Formatter.None;
            }
            return strData;
        }

        /// <summary>
        /// Format sec to string ms
        /// </summary>
        /// <param name="inData">byte</param>
        /// <returns>string in ms</returns>
        public static string FormatTTL(byte inData)
        {
            double sec_data = inData / 1000;
            return sec_data.ToString();
        }

        /// <summary>
        /// Format data to string
        /// </summary>
        /// <param name="inData">object</param>
        /// <returns>string</returns>
        public static string FormatVariant(object inData)
        {
            if (inData == null)
            {
                return Formatter.None;
            }
            else if (inData.GetType() == typeof(Int32))
            {
                return FormatInt((int)inData);
            }
            else if (inData.GetType() == typeof(long))
            {
                return FormatLong((long)inData);
            }
            else if (inData.GetType() == typeof(UInt32))
            {
                return FormatUInt((uint)inData);
            }
            else if (inData.GetType() == typeof(UInt16))
            {
                return FormatUInt16((UInt16)inData);
            }
            else if (inData.GetType() == typeof(UInt64))
            {
                return FormatULong((ulong)inData);
            }
            else if (inData.GetType() == typeof(Boolean))
            {
                return FormatBool((bool)inData);
            }
            else if (inData.GetType() == typeof(Single))
            {
                return FormatFloat((float)inData);
            }
            else if (inData.GetType() == typeof(Double))
            {
                return FormatFloat((double)inData);
            }
            else if (inData.GetType() == typeof(String))
            {
                return FormatString((string)inData);
            }            
            else if (inData.GetType() == typeof(Dictionary<string, object>))
            {
                return FormatMap((Dictionary<string, object>)inData);
            }
            else if (inData.GetType() == typeof(Collection<object>))
            {
                return FormatList((Collection<object>)inData);
            }
            else if (inData.GetType() == typeof(List))
            {
                return FormatList((List)inData);
            }
            else if (inData.GetType() == typeof(Map))
            {
                return FormatMap((Map)inData);
            }
            else if (inData.GetType() == typeof(Amqp.Framing.ApplicationProperties))
            {
                return FormatMap((inData as Amqp.Framing.ApplicationProperties).Map);
            }
            else if (inData.GetType() == typeof(Amqp.Framing.MessageAnnotations))
            {
                return FormatMap((inData as Amqp.Framing.MessageAnnotations).Map);
            }
            else if (inData.GetType() == typeof(byte[]))
            {
                return FormatByteArray(inData as byte[]);
            }
            else if (inData.GetType() == typeof(DateTime))
            {
                return FormatDate((DateTime)inData);
            }
            else
            {
                return FormatString(inData.ToString());
            }
        }

        /// <summary>
        /// Method for run formating
        /// </summary>
        /// <param name="msg">message object</param>
        /// <param name="options">agruments of client</param>
        public static void LogMessage(Message msg, SenderReceiverOptions options)
        {
            var hashContent = options.HashContent;

            if (options.LogMsgs == "body")
            {
                Console.WriteLine(hashContent ? Hash(msg.Body) : msg.Body);
            }
            else if (options.LogMsgs.Equals("dict"))
            {
                Formatter.PrintMessageAsDict(msg, hashContent);
            }
            else if (options.LogMsgs.Equals("upstream"))
            {
                Formatter.PrintMessage(msg, hashContent);
            }
            else if (options.LogMsgs.Equals("interop"))
            {
                Formatter.PrintMessageAsInterop(msg, hashContent);
            }
            else if (options.LogMsgs.Equals("json"))
            {
                Formatter.PrintMessageAsJson(msg, hashContent);
            }
        }

        private static object Hash(object msgBody)
        {
            byte[] hash;
            SHA1 sha = new SHA1CryptoServiceProvider();
            if (msgBody is byte[])
                hash = sha.ComputeHash((byte[]) msgBody);
            else if (msgBody is string)
                hash = sha.ComputeHash(Encoding.UTF8.GetBytes((string) msgBody));
            else
                hash = sha.ComputeHash(Encoding.UTF8.GetBytes(FormatVariant(msgBody)));
            return String.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}
