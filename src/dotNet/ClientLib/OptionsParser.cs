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
using System.IO;
using Amqp.Types;
using Amqp.Framing;
using NDesk.Options;

namespace ClientLib
{
    /// <summary>
    /// Abstract class for parse help option
    /// </summary>
    public abstract class Options : OptionSet
    {
        protected readonly int _toSecConstant = 1000;

        /// <summary>
        /// Constructor
        /// </summary>
        public Options()
        {
            this.Add("h|help", "show this message",
                v => {
                    this.PrintHelp();
                    Environment.Exit(0);
                });
        }

        /// <summary>
        /// Public method for print help usage
        /// </summary>
        public void PrintHelp()
        {
            Console.WriteLine("<type_of_client> [opts]");
            this.WriteOptionDescriptions(Console.Out);
        }

        protected static bool ParseBoolOption(string value)
        {
            if (value == "yes" || value == "true" || value == "True")
                return true;
            if (value == "no" || value == "false" || value == "False")
                return false;
            throw new ArgumentException();
        }
    }

    /// <summary>
    /// Abstract slacc for store connection options fol clients
    /// </summary>
    public abstract class ConnectionOptions : Options
    {
        //Properties
        private string amqpPrefix = "amqp://";
        public string Url { get; protected set; }
        public int Heartbeat { get; protected set; }
        public string AuthMech { get; protected set; }
        public int FrameSize { get; private set; }
        public bool ConnSSL { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionOptions() : base()
        {
            //default values
            this.Url = "amqp://127.0.0.1:5672";
            this.Heartbeat = -1;
            this.AuthMech = String.Empty;
            this.FrameSize = -1;

            //add options
            this.Add("b|broker=", "-b VALUE, --broker-url VALUE  url of broker to connect to (default amqp://127.0.0.1:5672)",
                (string url) => { this.Url = this.amqpPrefix + url; });
            this.Add("conn-heartbeat=", "time in s to delay between heatbeat packet",
                (int heatbeat) => { this.Heartbeat = heatbeat * this._toSecConstant; });
            this.Add("conn-auth-mechanisms=", "VALUE  SASL mechanisms; currently supported PLAIN | GSSAPI | EXTERNAL",
                (string authMech) => { this.AuthMech = authMech; });
            this.Add("conn-max-frame-size=", "Set connection max frame size",
                (int frameSize) => { this.FrameSize = frameSize; });
            this.Add("conn-ssl=", "Enable ssl connection without verify host adn enable trustAll",
                (bool connSSL) => { this.ConnSSL = connSSL; });
        }
    }

    /// <summary>
    /// Abstract class for store link options for clients
    /// </summary>
    public abstract class LinkOptions : ConnectionOptions
    {
        //properties
        public SettlementMode Settlement { get; protected set; }
        public string Address { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public LinkOptions() : base()
        {
            //default values
            this.Settlement = SettlementMode.AtLeastOnce;
            this.Address = "examples";

            //add options
            this.Add("link-at-most-once", "Sets 0-ack fire-and-forget delivery",
                v => { this.Settlement = SettlementMode.AtMostOnce; });
            this.Add("link-at-least-once", "Sets 1-ack reliable delivery",
                v => { this.Settlement = SettlementMode.AtLeastOnce; });
            this.Add("link-exactly-once", "Sets 2-ack reliable delivery",
                v => { this.Settlement = SettlementMode.ExactlyOnce; });
            this.Add("a|address=", "queue/exchange name",
                (string address) => { this.Address = address; });
        }
    }

    /// <summary>
    /// Abstract class for store toegether options for all clients
    /// </summary>
    public abstract class BasicOptions : LinkOptions
    {
        //Properties
        public int MsgCount { get; protected set; }
        public int CloseSleep { get; protected set; }
        public string SyncMode { get; protected set; }
        public TimeSpan Timeout { get; protected set; }
        public string LogMsgs { get; protected set; }
        public string LogStats { get; protected set; }
        public string LogLib { get; protected set; }
        public bool HashContent { get; protected set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public BasicOptions() : base()
        {
            //default values
            this.MsgCount = 1;
            this.CloseSleep = 0;
            this.SyncMode = "none";
            this.Timeout = TimeSpan.FromSeconds(1);
            this.LogMsgs = "upstream";
            this.LogStats = String.Empty;
            this.LogLib = String.Empty;

            //add options
            this.Add("c|count=", "count of messages to send, receive or count of connections (default 1)",
                (int count) => { this.MsgCount = count; });
            this.Add("close-sleep=", "sleep before end of client",
                (int closeSleep) => { this.CloseSleep = closeSleep * this._toSecConstant; });
            this.Add("sync-mode=", "sync action",
                (string syncMode) => { this.SyncMode = syncMode; });
            this.Add("t|timeout=", "timeout",
                (int timeout) => { this.Timeout = TimeSpan.FromSeconds(timeout); });
            this.Add("log-msgs=", "log messages output [dict|body|upstream|interop]",
                (string logMsgs) => { this.LogMsgs = logMsgs; });
            this.Add("log-stats=", "report various statistic/debug information [endpoint]",
                (string logStats) => { this.LogStats = logStats; });
            this.Add("log-lib=", "client logging library level [TRANSPORT_FRM]",
                (string logLib) => { this.LogLib = logLib; });
            this.Add("msg-content-hashed=", "Display SHA-1 hash of message content in logged messages (yes/no)",
                (string hashContent) => { this.HashContent = ParseBoolOption(hashContent); });
        }
    }

    /// <summary>
    /// Abstract class for store sender and receiver toegether options
    /// </summary>
    public abstract class SenderReceiverOptions : BasicOptions
    {
        //Properties
        public int Duration { get; protected set; }
        public string DurationMode { get; protected set; }
        public int TxSize { get; protected set; }
        public string TxAction { get; protected set; }
        public string TxLoopendAction { get; protected set; }
        public int Capacity { get; private set; }

        public SenderReceiverOptions() : base()
        {
            this.Duration = 0;
            this.DurationMode = "before-send";
            this.TxSize = -1;
            this.TxAction = "commit";
            this.TxLoopendAction = String.Empty;
            this.Capacity = -1;

            this.Add("duration=", "message actions total duration",
                (int duration) => { this.Duration = duration; });
            this.Add("duration-mode=", "specifies where to wait; following values are supported: before-receive, after-receive-before-tx-action,after-receive-after-tx-action",
                (string logLib) => { this.LogLib = logLib; });
            this.Add("tx-size=", "transactional mode: batch message count size negative skips tx-action before exit",
                (int txSize) => { this.TxSize = txSize; });
            this.Add("tx-action=", "transactional action at the end of tx batch (commit, rollback, none)",
                (string txAction) => { this.TxAction = txAction; });
            this.Add("tx-endloop-action=", "transactional action at the end message processing loop",
                (string txEndloopAction) => { this.TxLoopendAction = txEndloopAction; });
            this.Add("capacity=", "set link's capacity",
                (int capacity) => { this.Capacity = capacity; });

        }

    }

    /// <summary>
    /// Class for parse sender client options
    /// </summary>
    public class SenderOptions : SenderReceiverOptions
    {
        // properties
        public string Id { get; private set; }
        public string To { get; private set; }
        public string ReplyTo { get; private set; }
        public string Subject { get; private set; }
        public bool Durable { get; private set; }
        public uint Ttl { get; private set; }
        public byte Priority { get; private set; }
        public string CorrelationId { get; private set; }
        public byte[] UserId { get; private set; }
        public string MsgContentType { get; private set; }
        public string Content { get; private set; }
        public string ContentFromFile { get; private set; }
        public string ContentType { get; private set; }
        public string PropertyType { get; private set; }
        public string GroupId { get; private set; }
        public int GroupSequence { get; private set; }
        public string ReplyToGroupId { get; private set; }

        public Dictionary<string, object> Properties { get; set; }
        public List ListContent { get; private set; }
        public Map MapContent { get; set; }
        public MessageAnnotations MessageAnnotations { get; set; }

        /// <summary>
        /// Read text from file
        /// </summary>
        /// <param name="filePath">string file path with name and extension of file</param>
        /// <returns>string text from file</returns>
        private static String ReadInputFile(string filePath)
        {
            string text = String.Empty;
            using (StreamReader reader = new StreamReader(filePath))
            {
                text = reader.ReadToEnd();
            }
            return text;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SenderOptions() : base()
        {
            //default values
            this.Id = String.Empty;
            this.To = String.Empty;
            this.ReplyTo = String.Empty;
            this.Subject = String.Empty;
            this.Durable = false;
            this.Ttl = 0;
            this.Priority = 0;
            this.CorrelationId = String.Empty;
            this.UserId = null;
            this.MsgContentType = String.Empty;
            this.Content = String.Empty;
            this.ContentFromFile = String.Empty;
            this.ContentType = "string";
            this.PropertyType = "string";
            this.GroupSequence = 0;

            this.Properties = new Dictionary<string, object>();
            this.ListContent = new List();
            this.MapContent = new Map();
            this.MessageAnnotations = new MessageAnnotations();

            //add options
            this.Add("msg-id=", "use the supplied id instead of generating one",
                (string id) => { this.Id = id; });
            this.Add("msg-to=", "amqp:to in message header",
                (string to) => { this.To = to; });
            this.Add("msg-reply-to=", "specify reply-to address",
                (string replyTo) => { this.ReplyTo = replyTo; });
            this.Add("msg-subject=", "specify reply-to subject",
                (string subject) => { this.Subject = subject; });
            this.Add("msg-property=", "specify reply-to property",
                (string property) => {
                    var (key, value) = ParseItem(property);
                    this.Properties.Add(key, value);
                });
            this.Add("property-type=", "specify message property type (overrides auto-cast feature)",
                (string propertyType) => { this.PropertyType = propertyType; });
            this.Add("msg-durable=", "send durable messages yes/no",
                (string durable) => { this.Durable = ParseBoolOption(durable); });
            this.Add("msg-ttl=", "message time-to-live (ms)",
                (uint ttl) => { this.Ttl = ttl; });
            this.Add("msg-priority=", "message priority",
                (byte priority) => { this.Priority = priority; });
            this.Add("msg-correlation-id=", "message correlation id",
                (string correlationId) => { this.CorrelationId = correlationId; });
            this.Add("msg-user-id=", "message user id",
                (string userId) => {
                    char[] uid = userId.ToCharArray();
                    List<byte> uidl = new List<byte>();
                    foreach (char c in uid)
                    {
                        uidl.Add((byte)c);
                    }
                    this.UserId = uidl.ToArray();
                });
            this.Add("msg-group-id=", "amqp message group id",
                (string groupId) => { this.GroupId = groupId; });
            this.Add("msg-group-seq=", "amqp message group sequence",
                (int groupSequence) => { this.GroupSequence = groupSequence; });
            this.Add("msg-reply-to-group-id=", "amqp message reply to group id",
                (string replyToGroupId) => { this.ReplyToGroupId = replyToGroupId; });
            this.Add("content-type=|msg-content-type=", "message content type; values string, int, long, float",
                (string contentType) => { this.ContentType = contentType; });
            this.Add("msg-content=", "specify a content",
                (string content) => { this.Content = content; });
            this.Add("L|msg-content-list-item=", "specify a multiple entries content",
                (string listItem) => {
                    this.ListContent.Add(ParseValue(listItem));
                });
            this.Add("M|msg-content-map-item=", "KEY=VALUE specify a map content",
                (string mapItem) => {
                    var (key, value) = ParseItem(mapItem);
                    this.MapContent.Add(key, value);
                });
            this.Add("msg-content-from-file=", "specify file name to load the content from",
                (string path) => { this.ContentFromFile = ReadInputFile(path); });
            this.Add("msg-annotation=", "specify amqp properties",
                (string annotation) => {
                    var (key, value) = ParseItem(annotation);
                    this.MessageAnnotations[new Symbol(key)] = value;
                });
        }

        public static (string, object) ParseItem(string mapItem)
        {
            char[] delimiters = {'=', '~'};
            int i = mapItem.IndexOfAny(delimiters);

            if (i == -1)
            {
                throw new ArgumentException();
            }

            var key = mapItem.Substring(0, i);
            var value = mapItem.Substring(i+1);

            if (mapItem[i] == '~')
            {
                return (key, AutoCast(value));
            }
            return (key, value);
        }

        public static object ParseValue(string value)
        {
            if (value.Length >= 1 && value[0] == '~')
            {
                return AutoCast(value.Substring(1));
            }

            return value;
        }

        private static object AutoCast(string value)
        {
            int intVal;
            double doubleVal;
            bool boolVal;

            if (int.TryParse(value, out intVal))
            {
                return intVal;
            }

            if (double.TryParse(value, out doubleVal))
            {
                return doubleVal;
            }

            if (Boolean.TryParse(value, out boolVal))
            {
                return boolVal;
            }

            if (value == string.Empty)
            {
                return null;
            }

            return value;
        }
    }

    /// <summary>
    /// Class for parse receiver client options
    /// </summary>
    public class ReceiverOptions : SenderReceiverOptions
    {
        //properties
        public string Action { get; private set; }
        public bool RecvBrowse { get; private set; }
        public string MsgSelector { get; private set; }
        public bool ProccessReplyTo { get; private set; }
        public bool RecvListener { get; private set; }
        public int RecvListenerPort { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ReceiverOptions() : base()
        {
            //default values
            this.Action = String.Empty;
            this.RecvListenerPort = 5672;

            //add options
            this.Add("action=", "action on acquired message [accept, release, reject]",
                (string action) => { this.Action = action; });
            this.Add("recv-browse=", "get all messages from queue without delete",
                (bool recvBrowse) => { this.RecvBrowse = recvBrowse; });
            this.Add("recv-selector=|msg-selector=", "get all messages on specific filter",
                (string recvSelector) => { this.MsgSelector = recvSelector; });
            this.Add("process-reply-to", "reply on reply_on address",
                (v) => { this.ProccessReplyTo = true; });
            this.Add("recv-listen=", "enable receiver as listener [true, false]",
                (bool recvListen) => { this.RecvListener = recvListen; });
            this.Add("recv-listen-port=", "port for p2p",
                (int port) => { this.RecvListenerPort = port; });
        }
    }

    /// <summary>
    /// Class for parse connector client options
    /// </summary>
    public class ConnectorOptions : BasicOptions
    {
        //properties
        public string ObjCtrl { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectorOptions() : base()
        {
            //default values
            this.ObjCtrl = "C";

            //add options
            this.Add("obj-ctrl=", "Optional creation object control based on <object-ids>",
                (string ctrl) => { this.ObjCtrl = ctrl; });
        }
    }
}
