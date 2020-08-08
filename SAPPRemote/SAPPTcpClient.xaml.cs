/*
 *      This file is part of SAPPRemote distribution (https://github.com/poqdavid/SAPPRemote or http://poqdavid.github.io/SAPPRemote/).
 *  	Copyright (c) 2016-2020 POQDavid
 *      Copyright (c) contributors
 *
 *      SAPPRemote is free software: you can redistribute it and/or modify
 *      it under the terms of the GNU General Public License as published by
 *      the Free Software Foundation, either version 3 of the License, or
 *      (at your option) any later version.
 *
 *      SAPPRemote is distributed in the hope that it will be useful,
 *      but WITHOUT ANY WARRANTY; without even the implied warranty of
 *      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *      GNU General Public License for more details.
 *
 *      You should have received a copy of the GNU General Public License
 *      along with SAPPRemote.  If not, see <https://www.gnu.org/licenses/>.
 */

// <copyright file="TcpClient.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the SAPPTcpClient class.</summary>
namespace SAPPRemote
{
    using NLog;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public TcpClient workSocket = null;

        // Size of receive buffer.
        public const int BufferSize = 256;

        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public string sb;
    }

    /// <summary>
    /// Description of TcpClient.
    /// </summary>
    public partial class SAPPTcpClient : Application
    {
        private static string appdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SAPPRemote");

        public static String AppDataPath { get { return appdataPath; } set { appdataPath = value; } }

        ///<summary>
        /// This is simply where App stores data and loads them from.
        ///</summary>
        ///<returns>directory for the plugin's data and settings.</returns>
        private static string settingPath = Path.Combine(appdataPath, "Default.sapp");

        ///<summary>
        /// Gets or sets the dataDir property.
        ///</summary>
        ///<value>Directory for the plugin's data and settings.</value>
        public static string SettingPath { get { return settingPath; } set { settingPath = value; } }

        ///<summary>
        /// This is a static member of the Settings.
        ///</summary>
        private static Settings iSettings = new Settings();

        ///<summary>
        /// Gets or sets the iSettings property.
        ///</summary>
        ///<value>Plugin Settings.</value>
        public static Settings ISettings { get { return iSettings; } set { iSettings = value; } }

        private static bool appJustStarted = true;
        private static readonly ManualResetEvent connectDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent sendDone = new ManualResetEvent(false);
        private static readonly ManualResetEvent receiveDone = new ManualResetEvent(false);

        private static SAPPRemote.SAPPRemoteUI iSAPPRemoteUI;
        private static bool isConnected = false;
        public static System.Net.Sockets.TcpClient clientSocket;

        public static ServerStat SS = new ServerStat();
        public static PlayersStat PSS = new PlayersStat();
        public static NewGame NG = new NewGame();
        public static string response = String.Empty;
        public static string tempbuffer = String.Empty;

        public static SAPPRemoteLogger logger = new SAPPRemoteLogger();

        public void InitializeComponent()
        {
            logger.Info("Initializing resources for SAPPTcpClient");
            System.Uri resourceLocater = new System.Uri("/SAPPRemote;V2.0.0.0;component/sapptcpclient.xaml", System.UriKind.Relative);
            System.Windows.Application.LoadComponent(this, resourceLocater);
            logger.Info("Initialized resources for SAPPTcpClient");

            iSAPPRemoteUI = new SAPPRemoteUI();
            iSAPPRemoteUI.Show();
        }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        public static void Main()
        {
            SAPPTcpClient app = new SAPPTcpClient();

            app.InitializeComponent();
            app.Run();
        }

        public SAPPTcpClient()
        {
            ReadArgs();
            iSettings.LoadSettings();
            logger.Info("Settings loaded");
            GlobalDiagnosticsContext.Set("ipport", iSettings.IP_Port.Split(':')[0]);
        }

        public static void Connect(string ip_port)
        {
            if (isConnected == false)
            {
                try
                {
                    string ip = ip_port.Split(':')[0];
                    int port = int.Parse(ip_port.Split(':')[1]);

                    logger.Info("Connecting to " + ip_port);
                    clientSocket = new System.Net.Sockets.TcpClient();

                    clientSocket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), clientSocket.Client);
                    logger.Info("Connected to " + ip_port);

                    iSAPPRemoteUI.updater.Start();
                    iSAPPRemoteUI.SetServerStatText("Server stats loading...");
                }
                catch (Exception ex)
                {
                    logger.Error(ex, ex.Message);
                }
            }
        }

        public static void Disconnect()
        {
            logger.Info("Disconnecting from the server");
            try
            {
                iSAPPRemoteUI.updater.Stop();
                iSAPPRemoteUI.playerslist.Clear();

                if (isConnected)
                {
                    Action append = () =>
                    {
                        iSAPPRemoteUI.SetTitle("SAPP Remote > Offline");
                        iSAPPRemoteUI.SetServerStatText("Not connected to any server");

                        Paragraph p1 = new Paragraph();
                        p1.Append("Disconnected from the server.", Brushes.Transparent, Brushes.Red, true, false, false);
                        iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                    };

                    if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                    {
                        append();
                    }
                    else
                    {
                        iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                    }
                }

                clientSocket.GetStream().Close();
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
            isConnected = false;
            logger.Info("Disconnected from the server");
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);
                isConnected = true;
                Send(clientSocket, Json.GenerateString(new Login(ISettings.UserName, CreateMD5(ISettings.Password))));
                Receive(clientSocket);
            }
            catch (Exception ex)
            {
                Disconnect();
                logger.Error(ex, ex.Message);
            }
            connectDone.Set();
        }

        public static string CreateMD5(string input)
        {
            MD5 md5Hash = System.Security.Cryptography.MD5.Create();

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static void Send(TcpClient client, String data)
        {
            try
            {
                byte[] byteData = Encoding.UTF8.GetBytes(data + '\n');

                client.Client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
                sendDone.WaitOne();
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient client = (TcpClient)ar.AsyncState;

                client.Client.EndSend(ar);

                sendDone.Set();
            }
            catch (Exception ex)
            {
                Disconnect();
                logger.Error(ex, ex.Message);
            }
        }

        public static void SendQUERY()
        {
            Send(clientSocket, Json.GenerateString(new { opcode = Server.RemoteConsoleOpcode.RC_QUERY }));
        }

        public static void SendQUERY_STATS()
        {
            Send(clientSocket, Json.GenerateString(new { opcode = Server.RemoteConsoleOpcode.RC_QUERY_STATS }));
        }

        private static bool loadplayerslist = false;

        private static void Receive(TcpClient client)
        {
            try
            {
                StateObject state = new StateObject
                {
                    workSocket = client
                };

                client.Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);

                receiveDone.WaitOne();
            }
            catch (Exception ex)
            {
                Disconnect();
                logger.Error(ex, ex.Message);
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;

                if (isConnected && state.workSocket.Client.Connected)
                {
                    TcpClient client = state.workSocket;

                    int bytesRead = client.Client.EndReceive(ar);

                    if (bytesRead > 0)
                    {
                        string UTF7_data = Encoding.UTF7.GetString(state.buffer, 0, bytesRead);
                        byte[] UTF7_byte = Encoding.UTF7.GetBytes(UTF7_data);
                        byte[] UTF8_byte = Encoding.Convert(Encoding.UTF7, Encoding.UTF8, UTF7_byte);

                        state.sb += Encoding.UTF8.GetString(UTF8_byte);

                        if (state.sb.Contains("\n"))
                        {
                            if (state.sb.Length > 1)
                            {
                                response = state.sb;
                                state.sb = "";
                                foreach (string data_str in response.Split("\n"))
                                {
                                    if (data_str.Length != 0)
                                    {
                                        tempbuffer += data_str.Replace("\n", "");
                                        //iSAPPRemoteUI.textBox_console.CheckAppendText(data_str + "\n");
                                        if (tempbuffer[0] == '{' && tempbuffer[^1] == '}')
                                        {
                                            Msg(tempbuffer);
                                            tempbuffer = String.Empty;
                                        }
                                    }
                                }

                                receiveDone.Set();
                            }
                        }
                        client.Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    }
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                logger.Error(ex, ex.Message);
            }
        }

        private static void LoadPlayersStat(Players<PlayerData> temp)
        {
            try
            {
                while (loadplayerslist)
                {
                    if (iSAPPRemoteUI.playerslist.ToList().Count == 0)
                    {
                        foreach (PlayerData PD in temp)
                        {
                            if (!iSAPPRemoteUI.playerslist.ToList().Contains(PD))
                            {
                                iSAPPRemoteUI.playerslist.Add(PD);
                            }
                        }
                    }
                    loadplayerslist = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }

            try
            {
                foreach (PlayerData PD in temp)
                {
                    iSAPPRemoteUI.playerslist[Player.GetListIndex(iSAPPRemoteUI.playerslist, PD.Index)] = PD;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
            }
        }

        public static void Msg(string temp)
        {
            switch (Json.Get_rco(temp))
            {
                case Server.RemoteConsoleOpcode.RC_LOGIN:
                    {
                        iSAPPRemoteUI.SetTitle(" > Online");
                        Action append = () =>
                        {
                            Paragraph p1 = new Paragraph();
                            p1.Append("Connected to the server.", Brushes.Transparent, Brushes.Green, true, false, false);

                            if (appJustStarted == true)
                            {
                                iSAPPRemoteUI.richtextBox_console.SetText(p1);
                                appJustStarted = false;
                            }
                            else
                            {
                                iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                            }

                            Paragraph p2 = new Paragraph();

                            p2.Append("Logged in, admin level: ", Brushes.Transparent, Brushes.Yellow, true, false, false);
                            p2.Append(Json.Get_str(temp, "level"), Brushes.Transparent, Brushes.Red, true, false, false);

                            iSAPPRemoteUI.richtextBox_console.CheckAppendText(p2);
                        };

                        if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                        {
                            append();
                        }
                        else
                        {
                            iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                        }

                        loadplayerslist = true;
                        SendQUERY();
                    }
                    return;

                case Server.RemoteConsoleOpcode.RC_QUERY:
                    {
                        SS = Json.GetServerStat(temp);

                        iSAPPRemoteUI.textblock_serverstat.SetText(SS.ToString());
                        LoadPlayersStat(SS.Players);
                    }
                    return;

                case Server.RemoteConsoleOpcode.RC_QUERY_STATS:
                    {
                        PSS = Json.GetPlayersStat(temp);
                        LoadPlayersStat(PSS.Players);
                    }
                    return;

                case Server.RemoteConsoleOpcode.RC_CIN:
                    {
                    }
                    return;

                case Server.RemoteConsoleOpcode.RC_COUT:
                    {
                        Action append = () =>
                        {
                            Paragraph p1 = new Paragraph();
                            p1.Append(Json.Get_str(temp, "text"), Brushes.Transparent, Brushes.Cyan, true, false, false);

                            iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                        };

                        if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                        {
                            append();
                        }
                        else
                        {
                            iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                        }
                    }
                    return;

                case Server.RemoteConsoleOpcode.RC_CHAT:
                    {
                        PlayerData PD = iSAPPRemoteUI.playerslist.ToList()[Player.GetListIndex(iSAPPRemoteUI.playerslist.ToList(), Json.Get_int(temp, "index"))];

                        switch (Json.Get_int(temp, "type"))
                        {
                            case 0: //All
                                {
                                    Action append = () =>
                                    {
                                        Paragraph p1 = new Paragraph();
                                        p1.Append("[GLOBAL] ", Brushes.Transparent, Brushes.LimeGreen, true, false, false);
                                        p1.Append(PD.Name + ": ", Brushes.Transparent, Brushes.Green, true, false, false);
                                        p1.Append(Json.Get_str(temp, "message"), Brushes.Transparent, Brushes.White, true, false, false);

                                        iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                                    };

                                    if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                                    {
                                        append();
                                    }
                                    else
                                    {
                                        iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                                    }
                                }
                                return;

                            case 1: //Team
                                {
                                    Action append = () =>
                                    {
                                        Paragraph p1 = new Paragraph();
                                        p1.Append("[TEAM] ", Brushes.Transparent, Brushes.Yellow, true, false, false);
                                        p1.Append(PD.Name + ": ", Brushes.Transparent, Brushes.Green, true, false, false);
                                        p1.Append(Json.Get_str(temp, "message"), Brushes.Transparent, Brushes.White, true, false, false);

                                        iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                                    };

                                    if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                                    {
                                        append();
                                    }
                                    else
                                    {
                                        iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                                    }
                                }
                                return;

                            case 2: //Vehicle
                                {
                                    Action append = () =>
                                    {
                                        Paragraph p1 = new Paragraph();
                                        p1.Append("[VEHICLE] ", Brushes.Transparent, Brushes.Blue, true, false, false);
                                        p1.Append(PD.Name + ": ", Brushes.Transparent, Brushes.Green, true, false, false);
                                        p1.Append(Json.Get_str(temp, "message"), Brushes.Transparent, Brushes.White, true, false, false);

                                        iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                                    };

                                    if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                                    {
                                        append();
                                    }
                                    else
                                    {
                                        iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                                    }
                                }
                                return;

                            default:
                                {
                                    Action append = () =>
                                    {
                                        Paragraph p1 = new Paragraph();
                                        p1.Append("[OTHER] ", Brushes.Transparent, Brushes.LimeGreen, true, false, false);
                                        p1.Append(PD.Name + ": ", Brushes.Transparent, Brushes.Green, true, false, false);
                                        p1.Append(Json.Get_str(temp, "message"), Brushes.Transparent, Brushes.White, true, false, false);

                                        iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                                    };

                                    if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                                    {
                                        append();
                                    }
                                    else
                                    {
                                        iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                                    }
                                }
                                return;
                        }
                    }
                case Server.RemoteConsoleOpcode.RC_PJOIN:
                    {
                        try
                        {
                            iSAPPRemoteUI.updater.Stop();
                            PlayerData tempplayer = Player.GetData(temp);

                            if (!iSAPPRemoteUI.playerslist.ToList().Contains(tempplayer))
                            {
                                // tempplayer.CM = iSAPPRemoteUI.CM;

                                iSAPPRemoteUI.playerslist.Add(tempplayer);
                            }

                            Action append = () =>
                            {
                                Paragraph p1 = new Paragraph();
                                p1.Append("Player Joined, Name: ", Brushes.Transparent, Brushes.Green, true, false, false);
                                p1.Append(tempplayer.Name, Brushes.Transparent, Brushes.Green, true, false, false);

                                iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                            };

                            if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                            {
                                append();
                            }
                            else
                            {
                                iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                            }
                            iSAPPRemoteUI.updater.Start();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, ex.Message);
                        }
                    }
                    return;

                case Server.RemoteConsoleOpcode.RC_PLEAVE:
                    {
                        try
                        {
                            iSAPPRemoteUI.updater.Stop();
                            int pindex = Player.GetListIndex(iSAPPRemoteUI.playerslist.ToList(), Json.Get_int(temp, "index"));
                            PlayerData PD = iSAPPRemoteUI.playerslist[pindex];
                            iSAPPRemoteUI.playerslist.RemoveAt(pindex);

                            Action append = () =>
                            {
                                Paragraph p1 = new Paragraph();
                                p1.Append("Player Quit, Name: ", Brushes.Transparent, Brushes.Red, true, false, false);
                                p1.Append(PD.Name, Brushes.Transparent, Brushes.Green, true, false, false);

                                iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                            };

                            if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                            {
                                append();
                            }
                            else
                            {
                                iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                            }
                            iSAPPRemoteUI.updater.Start();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, ex.Message);
                        }
                    }
                    return;

                case Server.RemoteConsoleOpcode.RC_TEAMCHANGE:
                    {
                        try
                        {
                            iSAPPRemoteUI.updater.Stop();
                            TeamChange TC = Json.GetTeamChange(temp);
                            int pindex = Player.GetListIndex(iSAPPRemoteUI.playerslist.ToList(), TC.Index);
                            PlayerData PD = new PlayerData();
                            PD = iSAPPRemoteUI.playerslist[pindex];

                            iSAPPRemoteUI.playerslist.RemoveAt(pindex);

                            Action append = () =>
                            {
                                Paragraph p1 = new Paragraph();
                                p1.Append("Teamchange, ", Brushes.Transparent, Brushes.Cyan, true, false, false);
                                p1.Append(PD.Name, Brushes.Transparent, Brushes.Green, true, false, false);
                                p1.Append(" changed to the ", Brushes.Transparent, Brushes.Cyan, true, false, false);
                                p1.Append(Player.GetTeamText(TC.ITeam), Brushes.Transparent, Brushes.Yellow, true, false, false);
                                p1.Append(" team", Brushes.Transparent, Brushes.Cyan, true, false, false);

                                iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                            };

                            if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                            {
                                append();
                            }
                            else
                            {
                                iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                            }
                            iSAPPRemoteUI.playerslist.Add(PD);

                            iSAPPRemoteUI.updater.Start();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, ex.Message);
                        }
                    }
                    return;

                case Server.RemoteConsoleOpcode.RC_NEWGAME:
                    {
                        try
                        {
                            iSAPPRemoteUI.updater.Stop();
                            NG = Json.GetNewGame(temp);

                            Action append = () =>
                            {
                                Paragraph p1 = new Paragraph();
                                p1.Append("New game, ", Brushes.Transparent, Brushes.Cyan, true, false, false);
                                p1.Append(NG.ToString(), Brushes.Transparent, Brushes.Cyan, true, false, false);

                                iSAPPRemoteUI.richtextBox_console.CheckAppendText(p1);
                            };

                            if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                            {
                                append();
                            }
                            else
                            {
                                iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append);
                            }
                            foreach (PlayerData PD in iSAPPRemoteUI.playerslist.ToList())
                            {
                                Action append2 = () =>
                                {
                                    Paragraph p2 = new Paragraph();
                                    p2.Append("Player Quit, Name: ", Brushes.Transparent, Brushes.Red, true, false, false);
                                    p2.Append(PD.Name, Brushes.Transparent, Brushes.Green, true, false, false);

                                    iSAPPRemoteUI.richtextBox_console.CheckAppendText(p2);
                                };

                                if (iSAPPRemoteUI.richtextBox_console.CheckAccess())
                                {
                                    append2();
                                }
                                else
                                {
                                    iSAPPRemoteUI.richtextBox_console.Dispatcher.BeginInvoke(append2);
                                }
                            }
                            iSAPPRemoteUI.playerslist.Clear();
                            SS = NG.ToServerStat(SS);
                            iSAPPRemoteUI.textblock_serverstat.SetText(SS.ToString());
                            iSAPPRemoteUI.updater.Start();
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, ex.Message);
                        }
                    }
                    return;
            }
        }

        private static void ReadArgs()
        {
            if (Environment.GetCommandLineArgs().Length > 0)
            {
                int i = 0;
                foreach (string arg in Environment.GetCommandLineArgs())
                {
                    if (arg.Contains(".sapp"))
                    {
                        settingPath = Environment.GetCommandLineArgs()[i];
                    }
                    i++;
                }
            }
        }
    }
}