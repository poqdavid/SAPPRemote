// <copyright file="TcpClient.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the myTcpClient class.</summary>
namespace SAPPRemote
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using System.Windows;
	using System.Windows.Controls;
	
	// State object for receiving data from remote device.
	public  class StateObject
	{
		// Client socket.
		public  TcpClient workSocket = null;
		// Size of receive buffer.
		public const int BufferSize = 256;
		// Receive buffer.
		public   byte[] buffer = new byte[BufferSize];
		// Received data string.
		public string sb;
	}
	
	/// <summary>
	/// Description of TcpClient.
	/// </summary>
	public partial class myTcpClient : Application
	{
		private static ManualResetEvent connectDone = new ManualResetEvent(false);
		private static ManualResetEvent sendDone = new ManualResetEvent(false);
		private static ManualResetEvent receiveDone = new ManualResetEvent(false);
		
		private static SAPPRemote.SAPPRemoteUI iSAPPRemoteUI = new SAPPRemoteUI();
		private static bool isConnected = false;
		public static System.Net.Sockets.TcpClient clientSocket;
		
		public static ServerStat SS = new ServerStat();
		public static PlayersStat PSS = new PlayersStat();
		public static NewGame NG = new NewGame();
		public static string response = String.Empty;

		public myTcpClient()
		{
			iSAPPRemoteUI.Show();
		}

		public static void Connect(string ip_port, string username, string password)
		{
			if (isConnected == false) {
				clientSocket = new System.Net.Sockets.TcpClient();

				string ip = ip_port.Split(':')[0];
				int port = int.Parse(ip_port.Split(':')[1]);

				clientSocket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), clientSocket.Client);
            
				iSAPPRemoteUI.updater.Start();
				iSAPPRemoteUI.SetServerStatText("Server stats loading...");
			}
		}
		
		public static void Disconnect()
		{
			try {

				iSAPPRemoteUI.updater.Stop();
				iSAPPRemoteUI.playerslist.Clear();

				if (isConnected) {
					iSAPPRemoteUI.SetTitle("SAPP Remote > Offline");
					iSAPPRemoteUI.SetServerStatText("Not connected to any server");
					iSAPPRemoteUI.textBox_console.CheckAppendText("Disconnected from the server.\n");
				}
				
				clientSocket.Close();
				clientSocket.Client.Shutdown(SocketShutdown.Both);
        	
			} catch (Exception) {
			
			}
			isConnected = false;
		}
 
		private static void ConnectCallback(IAsyncResult ar)
		{
			try {
				Socket client = (Socket)ar.AsyncState;

				client.EndConnect(ar);

				Send(clientSocket, Json.GenerateString(new Login(SAPPRemoteUI.ISettings.UserName, SAPPRemoteUI.CreateMD5(SAPPRemoteUI.ISettings.Password))));
				Receive(clientSocket);
				isConnected = true;
			} catch (Exception ex) {
				Disconnect();
				MessageBox.Show(ex.Message, "SAPPRemote>ConnectCallback", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			connectDone.Set();
		}
		
		public static void Send(TcpClient client, String data)
		{
			try {
				byte[] byteData = Encoding.UTF8.GetBytes(data + '\n');

				client.Client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
				sendDone.WaitOne();
			} catch (Exception) {
			}
		}
		       
		private static void SendCallback(IAsyncResult ar)
		{
			try {
				TcpClient client = (TcpClient)ar.AsyncState;

				client.Client.EndSend(ar);
				 
				sendDone.Set();
			} catch (Exception ex) {
				Disconnect();
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
		
		static bool loadplayerslist = false;
		
		private static void Receive(TcpClient client)
		{
			try {
				StateObject state = new StateObject();
				state.workSocket = client;
				
				client.Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), state);
				
				receiveDone.WaitOne();
			} catch (Exception ex) {
				Disconnect();
			}
		}

		private static void ReceiveCallback(IAsyncResult ar)
		{
			try {
				StateObject state = (StateObject)ar.AsyncState;
				TcpClient client = state.workSocket;

				int bytesRead = client.Client.EndReceive(ar);
				
				if (bytesRead > 0) {
					state.sb += Encoding.Default.GetString(state.buffer, 0, bytesRead);
					 
					if (state.sb.Contains("\n")) {
						if (state.sb.Length > 1) {
							response = state.sb;
							state.sb = "";
							foreach (string data_str in response.Split('\n')) {
								if (data_str.Length != 0) {
									msg(data_str);
								}
							}
							
							receiveDone.Set();
						}
					}  
					client.Client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
				} 
 
			} catch (Exception ex) {
				Disconnect();
			}
		}

		private static void LoadPlayersStat(Players<PlayerData> temp)
		{
			try {
				while (loadplayerslist) {
					if (iSAPPRemoteUI.playerslist.ToList().Count == 0) {
						foreach (PlayerData PD in temp) {
							if (!iSAPPRemoteUI.playerslist.ToList().Contains(PD)) {
								PD.CM = iSAPPRemoteUI.CM;
								iSAPPRemoteUI.playerslist.Add(PD);
							}
						}       
					}
					loadplayerslist = false;
				}
                            
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
                             
			try {                            
				foreach (PlayerData PD in temp) {
					PD.CM = iSAPPRemoteUI.CM;
					iSAPPRemoteUI.playerslist[Player.GetListIndex(iSAPPRemoteUI.playerslist, PD.Index)] = PD;
				}
			} catch (Exception ex) {
							 
			}
		}
        
		public static void msg(string temp)
		{
			switch (Json.get_rco(temp)) {
				case Server.RemoteConsoleOpcode.RC_LOGIN:
					{
						iSAPPRemoteUI.SetTitle(" > Online");
						iSAPPRemoteUI.textBox_console.CheckAppendText("Connected to the server.\n");
						iSAPPRemoteUI.textBox_console.CheckAppendText("Logged in, admin level: " + Json.get_str(temp, "level") + "\n");
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
						iSAPPRemoteUI.textBox_console.CheckAppendText(Json.get_str(temp, "text") + "\n");
					}
					return;
				case Server.RemoteConsoleOpcode.RC_CHAT:
					{
						PlayerData PD = iSAPPRemoteUI.playerslist.ToList()[Player.GetListIndex(iSAPPRemoteUI.playerslist.ToList(), Json.get_int(temp, "index"))];
                        
						switch (Json.get_int(temp, "type")) {
							case 0: //All
								{
									iSAPPRemoteUI.textBox_console.CheckAppendText("[GLOBAL] " + PD.Name + ": " + Json.get_str(temp, "message") + "\n");
								}
								return;
							case 1: //Team
								{
									iSAPPRemoteUI.textBox_console.CheckAppendText("[TEAM] " + PD.Name + ": " + Json.get_str(temp, "message") + "\n");
								}
								return;
							case 2: //Vehicle
								{
									iSAPPRemoteUI.textBox_console.CheckAppendText("[VEHICLE] " + PD.Name + ": " + Json.get_str(temp, "message") + "\n");
								}
								return;
							default:
								{
									iSAPPRemoteUI.textBox_console.CheckAppendText("[OTHER] " + PD.Name + ": " + Json.get_str(temp, "message") + "\n");
								}
								return;
						}
					}
				case Server.RemoteConsoleOpcode.RC_PJOIN:
					{
						try {
							iSAPPRemoteUI.updater.Stop();
							PlayerData tempplayer = Player.GetData(temp);

							if (!iSAPPRemoteUI.playerslist.ToList().Contains(tempplayer)) {
								tempplayer.CM = iSAPPRemoteUI.CM;
								iSAPPRemoteUI.playerslist.Add(tempplayer);
							}
							iSAPPRemoteUI.textBox_console.CheckAppendText("Player Joined, Name: " + tempplayer.Name + "\n");
							iSAPPRemoteUI.updater.Start();
						} catch (Exception ex) {
							 
						}
					}
					return;
				case Server.RemoteConsoleOpcode.RC_PLEAVE:
					{
						try {
							iSAPPRemoteUI.updater.Stop();
							int pindex = Player.GetListIndex(iSAPPRemoteUI.playerslist.ToList(), Json.get_int(temp, "index"));
							PlayerData PD = iSAPPRemoteUI.playerslist[pindex];
							iSAPPRemoteUI.playerslist.RemoveAt(pindex);
							 
							iSAPPRemoteUI.textBox_console.CheckAppendText("Player Quit, Name: " + PD.Name + "\n");
							iSAPPRemoteUI.updater.Start();
						} catch (Exception ex) {
							 
						}
					}
					return;
				case Server.RemoteConsoleOpcode.RC_TEAMCHANGE:
					{
						try {
							iSAPPRemoteUI.updater.Stop();
							TeamChange TC = Json.GetTeamChange(temp);
							int pindex = Player.GetListIndex(iSAPPRemoteUI.playerslist.ToList(), TC.Index);
							PlayerData PD = new PlayerData();
							PD = iSAPPRemoteUI.playerslist[pindex];
							 
							iSAPPRemoteUI.playerslist.RemoveAt(pindex);

							iSAPPRemoteUI.textBox_console.CheckAppendText("Teamchange, " + PD.Name + " changed to the " + Player.GetTeamText(TC.iTeam) + " team\n");
							iSAPPRemoteUI.playerslist.Add(PD);
							
							iSAPPRemoteUI.updater.Start();
						} catch (Exception ex) {
							 
						}
					}
					return;
				case Server.RemoteConsoleOpcode.RC_NEWGAME:
					{
						try {
							iSAPPRemoteUI.updater.Stop();
							NG = Json.GetNewGame(temp);
							iSAPPRemoteUI.textBox_console.CheckAppendText("New game, " + NG.ToString() + "\n");
							foreach (PlayerData PD in iSAPPRemoteUI.playerslist.ToList()) {
								iSAPPRemoteUI.textBox_console.CheckAppendText("Player Quit, Name: " + PD.Name + "\n");
							}
							iSAPPRemoteUI.playerslist.Clear();
							SS = NG.ToServerStat(SS);
							iSAPPRemoteUI.textblock_serverstat.SetText(SS.ToString());
							iSAPPRemoteUI.updater.Start();
							
						} catch (Exception ex) {
							 
						}
					}
					return;
			}
		}
	}
}
