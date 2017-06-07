using System;
using System.Collections.Generic;
using Amqp;
using Amqp.Framing;

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
                        Console.WriteLine("unrecognized option: {0}", item);
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
                Trace.TraceListener = (f, a) => Console.WriteLine(
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
            Open open = new Open();
            open.ContainerId = Guid.NewGuid().ToString();
            if (options.FrameSize > -1)
                open.MaxFrameSize = (uint)options.FrameSize;
            if (options.Heartbeat > -1)
                open.IdleTimeOut = (uint)options.Heartbeat;

            this.connection = new Connection(this.address, null, open, null);
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
            Console.Error.WriteLine("Exception: {0}.", ex);

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
        /// <param name="e">exception</param>
        void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs ex)
        {
            Console.Error.WriteLine("Exception: {0}.", ex);
            Environment.Exit(ReturnCode.ERROR_OTHER);
        }
        #endregion
    }
}