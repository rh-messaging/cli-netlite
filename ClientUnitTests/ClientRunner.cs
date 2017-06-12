﻿using System;
using System.Diagnostics;

namespace ClientUnitTests
{
    /// <summary>
    /// Enum of client types
    /// </summary>
    public enum ClientType
    {
        Sender,
        Receiver,
        Connector
    };

    /// <summary>
    /// Help class, provide running clients
    /// </summary>
    class ClientRunner
    {
        private string projectDir;

        /// <summary>
        /// Construnctor of class
        /// </summary>
        public ClientRunner()
        {
            this.projectDir = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        }

        /// <summary>
        /// Build path of exucutable client
        /// </summary>
        /// <param name="client">client string</param>
        /// <returns>path to executable</returns>
        private String getPath(String client)
        {
            return System.IO.Path.Combine(new String[] {
                this.projectDir,
                client + "\\bin\\Debug",
                "cli-netlite-" + client + ".exe" });
        }

        /// <summary>
        /// Runner method of client
        /// </summary>
        /// <param name="type">type of client</param>
        /// <param name="args">cmd args for client</param>
        /// <returns>ecode</returns>
        private int runClient(ClientType type, String args)
        {
            Process p = new Process();

            string client;
            if (type == ClientType.Sender)
                client = "sender";
            else if (type == ClientType.Receiver)
                client = "receiver";
            else
                client = "connector";

            p.StartInfo.FileName = this.getPath(client);
            Console.WriteLine(p.StartInfo.FileName);
            p.StartInfo.Arguments = args;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
            return p.ExitCode;
        }

        /// <summary>
        /// Public method for run sender
        /// </summary>
        /// <param name="args">cmd args</param>
        /// <returns>ecode</returns>
        public int RunSender(String args)
        {
            return this.runClient(ClientType.Sender, args);
        }

        /// <summary>
        /// Public method for run receiver
        /// </summary>
        /// <param name="args">cmd args</param>
        /// <returns>ecode</returns>
        public int RunReceiver(String args)
        {
            return this.runClient(ClientType.Receiver, args);
        }

        /// <summary>
        /// Public method for run connector
        /// </summary>
        /// <param name="args">cmd args</param>
        /// <returns>ecode</returns>
        public int RunConnector(String args)
        {
            return this.runClient(ClientType.Connector, args);
        }
    }
}