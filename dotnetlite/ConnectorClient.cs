using System;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using Amqp;

namespace Dotnetlite
{
    public class ConnectorClient : CoreClient
    {
        public void Run(string[] args)
        {
            Collection<Connection> con_list = new Collection<Connection>();
            Collection<Session> ssn_list = new Collection<Session>();
            Collection<SenderLink> snd_list = new Collection<SenderLink>();
            Collection<ReceiverLink> rcv_list = new Collection<ReceiverLink>();

            ConnectorOptions options = new ConnectorOptions();

            try
            {
                //parse args
                this.ParseArguments(args, options);

                //create connections
                if (Regex.IsMatch(options.ObjCtrl, "[CESR]"))
                {
                    //?Connection con = new Connection(new Address(options.Url),SaslProfile.External, new Open(), null);
                    for (int i = 0; i < options.MsgCount; i++)
                    {
                        Connection tmp_con = new Connection(new Address(options.Url));
                        con_list.Add(tmp_con);
                    }
                }

                //request to create at least a session (connection needed)
                if (Regex.IsMatch(options.ObjCtrl, "[ESR]"))
                {
                    try
                    {
                        foreach (Connection conn in con_list)
                        {
                            //if connection is opened
                            Session s = new Session(conn);
                            ssn_list.Add(s);
                            //if (options.syncMode == "action")
                            //s.Sync();
                            //else ssn_list.Add(null);
                        }
                    }
                    catch (AmqpException ae)
                    {
                        Console.Error.WriteLine(ae.Message);
                        for (int i = ssn_list.Count; i < con_list.Count; i++)
                        {
                            ssn_list.Add(null);
                            Environment.Exit(ReturnCode.ERROR_OTHER);
                        }
                    }

                    // further object require non-empty address
                    string address = "jms.queue.connection_tests";
                    //address = options.Address  ???

                    //not working currently
                    if (address != String.Empty)
                    {
                        // sender part (if address is specified)
                        // create senders for opened sessions
                        if (Regex.IsMatch(options.ObjCtrl, "[S]"))
                        {
                            try
                            {
                                int i = 0;
                                foreach (Session s in ssn_list)
                                {
                                    i++;
                                    if (s != null)
                                    {
                                        SenderLink snd = new SenderLink(s, "tmp_s" + i.ToString(), address);
                                        snd_list.Add(snd);
                                        //TODO
                                        if (options.SyncMode == "action")
                                        {
                                            //s.Sync();
                                        }
                                    }
                                    else
                                    {
                                        snd_list.Add(null);
                                    }
                                }
                            }
                            catch (AmqpException ae)
                            {
                                Console.Error.WriteLine(ae.Message);
                                for (int i = snd_list.Count; i < ssn_list.Count; i++)
                                {
                                    snd_list.Add(null);
                                }
                                Environment.Exit(ReturnCode.ERROR_OTHER);
                            }
                        }
                        if (Regex.IsMatch(options.ObjCtrl, "[R]"))
                        {
                            try
                            {
                                int i = 0;
                                foreach (Session s in ssn_list)
                                {
                                    i++;
                                    if (s != null)
                                    {
                                        ReceiverLink rcv = new ReceiverLink(s, "tmp_rcv" + i.ToString(), address);
                                        rcv_list.Add(rcv);
                                        if (options.SyncMode == "action")
                                        {
                                            //s.Sync();
                                        }
                                    }
                                    else
                                    {
                                        rcv_list.Add(null);
                                    }
                                }
                            }
                            catch (AmqpException ae)
                            {
                                Console.Error.WriteLine(ae.Message);
                                for (int i = rcv_list.Count; i < ssn_list.Count; i++)
                                {
                                    rcv_list.Add(null);
                                }
                                Environment.Exit(ReturnCode.ERROR_OTHER);
                            }
                        }
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
                if (options.CloseSleep > 0)
                    Thread.Sleep(options.CloseSleep);

                foreach (ReceiverLink rec in rcv_list)
                {
                    if (rec != null)
                    {
                        rec.Close();
                    }
                }
                foreach (SenderLink sen in snd_list)
                {
                    if (sen != null)
                    {
                        sen.Close();
                    }
                }
                foreach (Session ses in ssn_list)
                {
                    if (ses != null)
                    {
                        ses.Close();
                    }
                }
                foreach (Connection c in con_list)
                {
                    if (c != null)
                    {
                        c.Close();
                    }
                }
            }
            Environment.Exit(this.exitCode);
        }
    }
}
