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
using System.Transactions;
using Amqp;
using Amqp.Framing;
using Amqp.Types;
using System.Threading.Tasks;
using Amqp.Listener;

namespace ClientLib
{ 
    /// <summary>
    /// Class represent receiver from amqp broker
    /// </summary>
    public class ReceiverClient : CoreClient
    {
        #region Help methods
        /// <summary>
        /// Method for prepare receiver link
        /// </summary>
        /// <param name="options">receiver options</param>
        /// <returns>build receiver link</returns>
        private ReceiverLink PeprareReceiverLink(ReceiverOptions options)
        {
            Source recvSource = new Source()
            {
                Address = options.Address
            };
            if (options.RecvBrowse)
                recvSource.DistributionMode = new Symbol("copy");

            //source
            if (!string.IsNullOrEmpty(options.MsgSelector))
            {
                Map filters = new Map
                {
                    {
                        new Symbol("filter"),
                        new DescribedValue(
                                new Symbol("apache.org:selector-filter:float"),
                                options.MsgSelector)
                    },
                    {
                        new Symbol("filter1"),
                        new DescribedValue(
                                new Symbol("apache.org:selector-filter:string"),
                                options.MsgSelector)
                    },
                    {
                        new Symbol("filter2"),
                        new DescribedValue(
                                new Symbol("apache.org:selector-filter:int"),
                                options.MsgSelector)
                    },
                    {
                        new Symbol("filter3"),
                        new DescribedValue(
                                new Symbol("apache.org:selector-filter:boolean"),
                                options.MsgSelector)
                    },
                    {
                        new Symbol("filter4"),
                        new DescribedValue(
                                new Symbol("apache.org:selector-filter:list"),
                                options.MsgSelector)
                    },
                    {
                        new Symbol("filter5"),
                        new DescribedValue(
                                new Symbol("apache.org:selector-filter:map"),
                                options.MsgSelector)
                    }
                };
                recvSource.FilterSet = filters;
            }
            Attach attach = new Attach()
            {
                Source = recvSource,
                Target = new Target(),
                SndSettleMode = options.Settlement.GetSenderFlag(),
                RcvSettleMode = options.Settlement.GetReceiverFlag(),
            };

            return new ReceiverLink(session, "Aac2receiver", attach, (l, a) => { });
        }
        #endregion

        #region Listener methods
        /// <summary>
        /// Method for init container listener
        /// </summary>
        /// <param name="options">receiver options</param>
        private void InitListener(ReceiverOptions options)
        {
            this.CreateContainerHost(options);
            this.containerHost.Open();
            this.containerHost.RegisterMessageProcessor(options.Address, new MessageProcessor(options, this.containerHost));
            System.Threading.Thread.Sleep(options.Timeout);
        }

        /// <summary>
        /// Private class for handling requests on listener
        /// </summary>
        class MessageProcessor : IMessageProcessor
        {
            int received;
            int count;
            ReceiverOptions options;
            ContainerHost host;

            /// <summary>
            /// Constructor of MessageProcessor
            /// </summary>
            /// <param name="options">receiver options</param>
            /// <param name="host">container host listener</param>
            public MessageProcessor(ReceiverOptions options, ContainerHost host)
            {
                this.received = 0;
                this.options = options;
                this.count = options.MsgCount;
                this.host = host;
            }

            public int Credit { get { return options.MsgCount; } }

            /// <summary>
            /// init of message processor
            /// </summary>
            /// <param name="messageContext">context of messsage</param>
            public void Process(MessageContext messageContext)
            {
                var task = this.ReplyAsync(messageContext);
            }

            /// <summary>
            /// Async tassk for handling requst
            /// </summary>
            /// <param name="messageContext">context of message</param>
            /// <returns>async task</returns>
            async Task ReplyAsync(MessageContext messageContext)
            {
                while (this.received < count)
                {
                    try
                    {
                        Message message = messageContext.Message;
                        Formatter.LogMessage(message, options);
                        this.received++;
                        messageContext.Complete();
                    }
                    catch (Exception exception)
                    {
                        Console.Error.WriteLine("ERROR: {{'cause': '{0}'}}", exception.Message);
                        Environment.Exit(ReturnCode.ERROR_OTHER);
                    }

                    await Task.Delay(500);
                }
                host.Close();
                Environment.Exit(ReturnCode.ERROR_SUCCESS);
            }
        }

        #endregion

        #region Receive methods
        /// <summary>
        /// Method for browse or selector receive
        /// </summary>
        /// <param name="receiver">receiver link</param>
        /// <param name="options">receiver options</param>
        private void ReceiveAll(ReceiverLink receiver, ReceiverOptions options)
        {
            Message message = null;

            while ((message = receiver.Receive(options.Timeout)) != null)
            {
                Formatter.LogMessage(message, options);
                Utils.TsSnapStore(this.ptsdata, 'F', options.LogStats);

                if (!String.IsNullOrEmpty(options.MsgSelector))
                {
                    receiver.Accept(message);
                }
            }
        }

        /// <summary>
        /// Method for transactional receiving messages
        /// </summary>
        /// <param name="receiver">receiver link</param>
        /// <param name="options">receiver options</param>
        private void TransactionReceive(ReceiverLink receiver, ReceiverOptions options)
        {
            bool txFlag = true;
            int nReceived = 0;
            Message message = null;

            while (txFlag && (nReceived < options.MsgCount || options.MsgCount == 0) && options.TxSize > 0)
            {
                using (var txs = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    for (int i = 0; i < options.TxSize; i++)
                    {
                        message = receiver.Receive(options.Timeout);
                        if (message != null)
                        {
                            receiver.Accept(message);
                            Formatter.LogMessage(message, options);
                            nReceived++;
                        }
                    }
                    if (options.TxAction.ToLower() == "commit")
                        txs.Complete();
                }

                if (message == null || (options.MsgCount > 0 && ((options.MsgCount - nReceived) < options.TxSize)))
                {
                    txFlag = false;
                }
            }
            using (var txs = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                while ((message = receiver.Receive(options.Timeout)) != null && (nReceived < options.MsgCount || options.MsgCount == 0))
                {
                    receiver.Accept(message);
                    Formatter.LogMessage(message, options);
                    nReceived++;
                }

                if (options.TxLoopendAction.ToLower() == "commit")
                    txs.Complete();
            }
        }

        /// <summary>
        /// Standart receiving
        /// </summary>
        /// <param name="receiver">receiver link</param>
        /// <param name="options">receiver options</param>
        private void Receive(ReceiverLink receiver, ReceiverOptions options)
        {
            int nReceived = 0;
            Message message;

            while (((message = receiver.Receive(options.Timeout)) != null) && (nReceived < options.MsgCount || options.MsgCount == 0))
            {
                if (options.Duration > 0)
                {
                    Utils.Sleep4Next(ts, options.MsgCount, options.Duration, nReceived + 1);
                }

                receiver.Accept(message);

                Formatter.LogMessage(message, options);
                nReceived++;

                if (options.ProccessReplyTo)
                {
                    SenderLink sender = new SenderLink(session, "reply-to-sender", message.Properties.ReplyTo);
                    sender.Send(message);
                    sender.Close();
                }

                if ((options.MsgCount > 0) && (nReceived == options.MsgCount))
                {
                    break;
                }

                Utils.TsSnapStore(this.ptsdata, 'F', options.LogStats);
            }
        }
        #endregion

        /// <summary>
        /// Main metho for receiver (receive messages)
        /// </summary>
        /// <param name="args">array arguments from command line</param>
        /// <returns>returncode</returns>
        public void Run(string[] args) 
        {
            ReceiverOptions options = new ReceiverOptions();

            try
            {
                this.ParseArguments(args, options);

                if (options.RecvListener)
                {
                    this.InitListener(options);
                }
                else
                {
                    //init timestamping
                    this.ptsdata = Utils.TsInit(options.LogStats);

                    Utils.TsSnapStore(this.ptsdata, 'B', options.LogStats);

                    this.SetAddress(options.Url);
                    this.CreateConnection(options);

                    Utils.TsSnapStore(this.ptsdata, 'C', options.LogStats);

                    this.CreateSession();

                    Utils.TsSnapStore(this.ptsdata, 'D', options.LogStats);

                    ReceiverLink receiver = this.PeprareReceiverLink(options);

                    Message message = new Message();

                    this.ts = Utils.GetTime();

                    Utils.TsSnapStore(this.ptsdata, 'E', options.LogStats);
                    int nReceived = 0;

                    if (options.Capacity > -1)
                        receiver.SetCredit(options.Capacity);

                    bool tx_batch_flag = String.IsNullOrEmpty(options.TxLoopendAction) ? (options.TxSize > 0) : true;

                    //receiving of messages
                    if (options.RecvBrowse || !String.IsNullOrEmpty(options.MsgSelector))
                        this.ReceiveAll(receiver, options);
                    else
                    {
                        if (tx_batch_flag)
                            this.TransactionReceive(receiver, options);
                        else
                            this.Receive(receiver, options);
                    }

                    if (options.CloseSleep > 0)
                    {
                        System.Threading.Thread.Sleep(options.CloseSleep);
                    }

                    //close connection and link
                    this.CloseLink(receiver);
                    this.CloseConnection();

                    Utils.TsSnapStore(this.ptsdata, 'G', options.LogStats);

                    //report timestamping
                    if (this.ptsdata.Count > 0)
                    {
                        Console.WriteLine("STATS " + Utils.TsReport(this.ptsdata,
                            nReceived, message.Body.ToString().Length * sizeof(Char), 0));
                    }
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
            Environment.Exit(exitCode);
        }
    }
}
