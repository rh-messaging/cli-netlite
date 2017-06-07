//  ------------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation
//  All rights reserved. 
//  
//  Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this 
//  file except in compliance with the License. You may obtain a copy of the License at 
//  http://www.apache.org/licenses/LICENSE-2.0  
//  
//  THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, 
//  EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR 
//  CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR 
//  NON-INFRINGEMENT. 
// 
//  See the Apache Version 2.0 License for specific language governing permissions and 
//  limitations under the License.
//  ------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Transactions;

using Amqp;
using Amqp.Framing;

namespace ClientLib
{
    /// <summary>
    /// Class represent sender to amqp broker
    /// </summary>
    public class SenderClient : CoreClient
    {
        #region Content methods
        /// <summary>
        /// Method create method content 
        /// </summary>
        /// <param name="options">options from parse arguments from cmd</param>
        /// <returns>content</returns>
        static object CreateMsgContent(SenderOptions options, int indexOfMessage)
        {
            //TODO set content type
            object content = String.Empty;
            if (!(String.IsNullOrEmpty(options.Content)))
                content = options.Content;
            else if (options.ListContent.Count > 0)
                content = options.ListContent;
            else if (options.MapContent.Count > 0)
                content = options.MapContent;
            else if (!(String.IsNullOrEmpty(options.ContentFromFile)))
                content = options.ContentFromFile;
            return content;
        }

        /// <summary>
        /// Method create message
        /// </summary>
        /// <param name="options">options from parse arguments from cmd</param>
        /// <param name="nSent">count of send message</param>
        /// <returns>message</returns>
        static Message CreateMessage(SenderOptions options, int nSent)
        {
            object content = CreateMsgContent(options, nSent);
            Message msg = new Message(content);

            //msg properties            
            msg.Properties = new Properties();
            if (!String.IsNullOrEmpty(options.Id))
                msg.Properties.MessageId = options.Id;
            if (!String.IsNullOrEmpty(options.CorrelationId))
                msg.Properties.CorrelationId = options.CorrelationId;
            if (!String.IsNullOrEmpty(options.Subject))
                msg.Properties.Subject = options.Subject;
            if (!String.IsNullOrEmpty(options.ContentType))
                msg.Properties.ContentType = options.ContentType;
            if (options.UserId != null && options.UserId.Length != 0)
                msg.Properties.UserId = options.UserId;
            if (!String.IsNullOrEmpty(options.ReplyTo))
                msg.Properties.ReplyTo = options.ReplyTo;
            if (options.MessageAnnotations.Map.Count > 0)
                msg.MessageAnnotations = options.MessageAnnotations;
            if (!String.IsNullOrEmpty(options.GroupId))
                msg.Properties.GroupId = options.GroupId;
            if (!String.IsNullOrEmpty(options.GroupId))
                msg.Properties.GroupId = options.GroupId;
            msg.Properties.GroupSequence = (uint)options.GroupSequence;
            if (!String.IsNullOrEmpty(options.ReplyToGroupId))
                msg.Properties.ReplyToGroupId = options.ReplyToGroupId;

            //set up message header
            msg.Header = new Header();
            msg.Header.Durable = options.Durable;
            msg.Header.Priority = options.Priority;
            if (options.Ttl > 0)
                msg.Header.Ttl = options.Ttl;

            //set up application properties
            if (options.Properties.Count > 0)
            {
                //TODO - set up property type
                msg.ApplicationProperties = new ApplicationProperties();
                foreach (KeyValuePair<string, object> p in options.Properties)
                {
                    msg.ApplicationProperties[p.Key.ToString()] = p.Value;
                }
            }
            return msg;
        }
        #endregion

        #region Help method
        /// <summary>
        /// Method for return sender statistic dictionary
        /// </summary>
        /// <param name="snd">sender</param>
        /// <returns>dictionary object for sender stats</returns>
        static Dictionary<string, object> GetSenderStats(SenderLink snd)
        {
            Dictionary<string, object> stats = new Dictionary<string, object>();
            Dictionary<string, object> sender = new Dictionary<string, object>();
            //code
            stats["sender"] = sender;
            stats["timestamp"] = Utils.GetTime();
            return stats;
        }

        /// <summary>
        /// Prepare sender link with options
        /// </summary>
        /// <param name="options">sender options</param>
        /// <returns>builted sender link</returns>
        private SenderLink PrepareSender(SenderOptions options)
        {
            Attach attach = new Attach()
            {
                Source = new Source(),
                Target = new Target() { Address = options.Address },
                SndSettleMode = options.Settlement.GetSenderFlag(),
                RcvSettleMode = options.Settlement.GetReceiverFlag()
            };
            return new SenderLink(this.session, "sender-spout", attach, null);
        }
        #endregion

        #region Send methods
        /// <summary>
        /// Method for transactional sending of messages
        /// </summary>
        /// <param name="sender">sender link</param>
        /// <param name="options">options</param>
        private void TransactionSend(SenderLink sender, SenderOptions options)
        {
            int nSent = 0;
            bool txFlag = true;
            Message message;

            while (txFlag && options.TxSize > 0)
            {
                using (var txs = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    for (int i = 0; i < options.TxSize; i++)
                    {
                        message = CreateMessage(options, nSent);

                        if ((options.Duration > 0) && (options.DurationMode == "before-send"))
                            Utils.Sleep4Next(this.ts, options.MsgCount, (options.Duration), nSent + 1);

                        sender.Send(message, options.Timeout);

                        if ((options.Duration > 0) && (options.DurationMode == "after-send-before-tx-action"))
                            Utils.Sleep4Next(this.ts, options.MsgCount, (options.Duration), nSent + 1);

                        Formatter.LogMessage(message, options);

                        nSent++;
                    }

                    if (options.TxAction.ToLower() == "commit")
                        txs.Complete();

                    if ((options.Duration > 0) && (options.DurationMode == "after-send-after-tx-action"))
                        Utils.Sleep4Next(ts, options.MsgCount, (options.Duration), nSent);

                }
                //set up tx_batch_flag
                if ((options.MsgCount - nSent) < options.TxSize)
                {
                    txFlag = false;
                }
            }
            //rest of messages
            using (var txs = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                while (nSent < options.MsgCount)
                {
                    message = CreateMessage(options, nSent);
                    sender.Send(message, options.Timeout);
                    Formatter.LogMessage(message, options);
                    nSent++;
                }

                if (options.TxLoopendAction.ToLower() == "commit")
                    txs.Complete();
            }
        }

        /// <summary>
        /// Method for standart sending of messages
        /// </summary>
        /// <param name="sender">sender link</param>
        /// <param name="options">options</param>
        private void Send(SenderLink sender, SenderOptions options)
        {
            int nSent = 0;
            Message message;

            while ((nSent < options.MsgCount))
            {
                message = CreateMessage(options, nSent);
                if ((options.Duration > 0) && (options.DurationMode == "before-send"))
                    Utils.Sleep4Next(ts, options.MsgCount, (options.Duration), nSent + 1);

                sender.Send(message, options.Timeout);

                if ((options.Duration > 0) && ((options.DurationMode == "after-send-before-tx-action") ||
                        (options.DurationMode == "after-send-after-tx-action")))
                    Utils.Sleep4Next(ts, options.MsgCount, (options.Duration), nSent + 1);

                Formatter.LogMessage(message, options);
                nSent++;
            }
        }
        #endregion

        /// <summary>
        /// Main method of sender
        /// </summary>
        /// <param name="args">args from command line</param>
        /// <returns>int status exit code</returns>
        public void Run(string[] args)
        {
            SenderOptions options = new SenderOptions();

            try
            {
                this.ParseArguments(args, options);

                //init timestamping
                this.ptsdata = Utils.TsInit(options.LogStats);
                Utils.TsSnapStore(this.ptsdata, 'B', options.LogStats);

                this.SetAddress(options.Url);
                this.CreateConnection(options);

                Utils.TsSnapStore(this.ptsdata, 'C', options.LogStats);

                this.CreateSession(); 

                Utils.TsSnapStore(this.ptsdata, 'D', options.LogStats);

                SenderLink sender = this.PrepareSender(options);

                //enable transactions
                bool tx_batch_flag = String.IsNullOrEmpty(options.TxLoopendAction) ? (options.TxSize > 0) : true;

                Stopwatch stopwatch = new Stopwatch();
                TimeSpan timespan = new TimeSpan(0, 0, options.Timeout);

                stopwatch.Start();

                this.ts = Utils.GetTime();
                Utils.TsSnapStore(this.ptsdata, 'E', options.LogStats);

                //sending of messages
                if (tx_batch_flag)
                    this.TransactionSend(sender, options);
                else
                    this.Send(sender, options);

                if (options.LogStats.IndexOf("endpoints") > -1)
                {
                    Dictionary<string, object> stats = GetSenderStats(sender);
                    Formatter.PrintStatistics(stats);
                }

        
                Utils.TsSnapStore(this.ptsdata, 'F', options.LogStats);
                //close-sleep
                if (options.CloseSleep > 0)
                {
                    System.Threading.Thread.Sleep(options.CloseSleep);
                }

                ///close connection and link
                this.CloseLink(sender);
                this.CloseConnection();

                Utils.TsSnapStore(this.ptsdata, 'G', options.LogStats);

                if (this.ptsdata.Count > 0)
                {
                    Console.WriteLine("STATS " + Utils.TsReport(this.ptsdata, options.MsgCount,
                        options.Content.Length * sizeof(Char), 0));
                }
                this.exitCode = ReturnCode.ERROR_SUCCESS;
            }
            catch (ArgumentException ex)
            {
                this.ArgumentExceptionHandler(ex, options);
            }
            catch (Exception ex)
            {
                this.OtherExceptionHandler(ex, options);
            }
            finally
            {
                this.CloseConnection();
            }
            Environment.Exit(this.exitCode);
        }

    }
}
