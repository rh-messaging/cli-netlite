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
using Amqp;
using Amqp.Framing;
using Amqp.Listener;

namespace ClientLib
{
    /// <summary>
    /// Represent together properties of sender and receiver
    /// </summary>
    public class CoreClient
    {
        ///private members
        protected int exitCode;   
        protected List<double> ptsdata;
        protected double ts;
        protected Address address;
        protected Session session;
        protected Connection connection;
        protected ContainerHost containerHost;

        /// <summary>
        /// Constructor of core class
        /// </summary>
        public CoreClient()
        {
            this.exitCode = ReturnCode.ERROR_OTHER;

            //add trapper for unhandled exception (System.Net.Sockets)
            AppDomain.CurrentDomain.UnhandledException += this.UnhandledExceptionTrapper;
        }

        #region help methods
        /// <summary>
        /// Method for parse arguments from command line
        /// </summary>
        /// <param name="args">argument from command line</param>
        /// <param name="typeArguments">sender or receiver</param>
        /// <returns>options</returns>
        protected void ParseArguments(string[] args, Options typeArguments)
        {
            try {
                var unrecognized = typeArguments.Parse(args);
                if (unrecognized.Count > 0)
                {
                    typeArguments.PrintHelp();
                    foreach (var item in unrecognized)
                        Console.WriteLine("ERROR: {{ 'cause': {0}}}", item);
                    Environment.Exit(ReturnCode.ERROR_ARG);
                }
            }
            catch
            {
                typeArguments.PrintHelp();
                Environment.Exit(ReturnCode.ERROR_ARG);
            }
            this.SetUpCliLogging(typeArguments);
        }

        /// <summary>
        /// Method for enabling cli logging
        /// </summary>
        /// <param name="options">parsed options from cmd</param>
        private void SetUpCliLogging(Options options)
        {
            if((options as BasicOptions).LogLib.ToUpper() == "TRANSPORT_FRM")
            {
                Trace.TraceLevel = TraceLevel.Frame;
                Trace.TraceListener = (l, f, a) => Console.WriteLine(
                        DateTime.Now.ToString("[hh:mm:ss.fff]") + " " + string.Format(f, a));
            }
        }
        #endregion

        #region Connection and session methods
        /// <summary>
        /// Method for set address
        /// </summary>
        /// <param name="url">string url</param>
        protected void SetAddress(string url)
        {
            this.address = new Address(url);
        }

        /// <summary>
        /// Method for create connection
        /// </summary>
        protected void CreateConnection(ConnectionOptions options)
        {
            if (options.ConnSSL)
            {
                Connection.DisableServerCertValidation = true;
                this.address = new Address(options.Url.Replace("amqp://", "amqps://"));
            }
            Open open = new Open()
            {
                ContainerId = Guid.NewGuid().ToString()
            };
            if (options.FrameSize > -1)
                open.MaxFrameSize = (uint)options.FrameSize;
            if (options.Heartbeat > -1)
                open.IdleTimeOut = (uint)options.Heartbeat;

            this.connection = new Connection(this.address, null, open, null);
        }

        /// <summary>
        /// Method for create container listener
        /// </summary>
        /// <param name="options">receiver options</param>
        protected void CreateContainerHost(ReceiverOptions options)
        {
            Uri addressUri = new Uri("amqp://localhost:" + options.RecvListenerPort);
            this.containerHost = new ContainerHost(addressUri);
        }

        /// <summary>
        /// Method for create session
        /// </summary>
        protected void CreateSession()
        {
            this.session = new Session(this.connection);
        }

        /// <summary>
        /// Private method to close session
        /// </summary>
        private void CloseSession()
        {
            if (this.session != null)
                this.session.Close();
        }

        /// <summary>
        /// Method for close link
        /// </summary>
        /// <param name="link">sender or receiver link</param>
        protected void CloseLink(Link link)
        {
            if (link != null)
                link.Close();
        }

        /// <summary>
        /// Method for close session and connection
        /// </summary>
        protected void CloseConnection()
        {
            if (this.connection != null)
            {
                this.CloseSession();
                this.connection.Close();
            }
        }
        #endregion

        #region exception methods
        /// <summary>
        /// Method for handling argument exception
        /// </summary>
        /// <param name="ex">exception</param>
        /// <param name="options">parsed options</param>
        protected void ArgumentExceptionHandler(Exception ex, Options options)
        {
            Console.Error.WriteLine(ex.StackTrace);
            Console.Error.WriteLine("Invalid command option: " + ex.Message);
            options.PrintHelp();
            this.exitCode = ReturnCode.ERROR_ARG;
        }

        /// <summary>
        /// Method for handling other exception
        /// </summary>
        /// <param name="ex">exception</param>
        /// <param name="options">parsed options</param>
        protected void OtherExceptionHandler(Exception ex, Options options)
        {
            Console.Error.WriteLine(ex.StackTrace);
            Console.Error.WriteLine("ERROR: {{'cause': '{0}'}}", ex.Message.ToString());

            if (options is SenderOptions || options is ReceiverOptions)
            {
                Utils.TsSnapStore(this.ptsdata, 'G', (options as SenderReceiverOptions).LogStats);
            }

            //report timestamping
            if (this.ptsdata != null && this.ptsdata.Count > 0)
            {
                Console.WriteLine("STATS " + Utils.TsReport(this.ptsdata, -1, -1, 1));
            }
            this.exitCode = ReturnCode.ERROR_OTHER;
        }

        /// <summary>
        /// Method to rap unhandled exception
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="ex">exception</param>
        void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs ex)
        {
            Console.Error.WriteLine("ERROR: {{'cause': '{0}'}}", ex.ToString());
            Environment.Exit(ReturnCode.ERROR_OTHER);
        }
        #endregion
    }
}