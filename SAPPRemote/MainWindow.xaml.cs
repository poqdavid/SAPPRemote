// <copyright file="MainWindow.xaml.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the MainWindow class.</summary>
namespace SAPPRemote
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Windows.Data;
	using System.Windows.Documents;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using System.Windows.Navigation;
	using System.Windows.Shapes;
	using System.Windows.Threading;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System.Collections.ObjectModel;
	using System.Security.Cryptography;
	
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		///<summary>
		/// This is simply where plugin stores data and loads them from.
		///</summary>
		///<returns>directory for the plugin's data and settings.</returns>
		private static string settingPath = @"Settings.json";


		///<summary>
		/// This is a static member of the Settings.
		///</summary>
		private static Settings iSettings;

		///<summary>
		/// Gets or sets the dataDir property.
		///</summary>
		///<value>Directory for the plugin's data and settings.</value>
		public static string SettingPath { get { return settingPath; } set { settingPath = value; } }

		///<summary>
		/// Gets or sets the iSettings property.
		///</summary>
		///<value>Plugin Settings.</value>
		public static Settings ISettings { get { return iSettings; } set { iSettings = value; } }

		static Players<PlayerData> playerslist = new Players<PlayerData>();
		static ServerStat SS = new ServerStat();
		public DispatcherTimer updater = new DispatcherTimer();
		System.Net.Sockets.TcpClient clientSocket;

		StreamReader serverStream = default(StreamReader);
		Thread ctThread;

		string readData = null;
		private void ReadArgs()
		{
			if (Environment.GetCommandLineArgs().Length > 0) {
				int i = 0;
				foreach (string arg in Environment.GetCommandLineArgs()) {
					if (arg == "path") {
						settingPath = Environment.GetCommandLineArgs()[(i + 1)];
					}
					i++;
				}

			} else {

			}
		}
		public MainWindow()
		{
			ReadArgs();
			iSettings = new Settings();
			iSettings.LoadSettings();
			InitializeComponent();
			updater.Tick += updater_Tick;
			updater.Interval = new TimeSpan(0, 0, 3);

			textBox_ip_port.Text = iSettings.IP_Port;
			textBox_username.Text = iSettings.UserName;
			textBox_password.Password = iSettings.Password;
             
			checkbox_autoconnect.IsChecked = iSettings.AutoConnect;
            
			playerslist.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(playerslist_CollectionChanged);
        
			if (iSettings.AutoConnect == true) {
				Connect();
			}
		}

		public static string CreateMD5(string input)
		{
			// Use input string to calculate MD5 hash
			MD5 md5Hash = System.Security.Cryptography.MD5.Create();

			// Convert the input string to a byte array and compute the hash.
			byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < data.Length; i++) {
				sBuilder.Append(data[i].ToString("x2"));
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}
		
		private void updater_Tick(object sender, EventArgs e)
		{
			SendQUERY();
			//listBox_players.ItemsSource = playerslist;
			//listBox_players.ItemsSource = SS.Players.ToList() ;
		}

		void playerslist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			// listBox_players.ItemsSource = null;
			listBox_players.ItemsSource = playerslist;
			 
		}

		void textBox_command_KeyUp(object sender, KeyEventArgs e)
		{
			try {
				string outText = "";
				StreamWriter writer = new StreamWriter(clientSocket.GetStream());
				if (e.Key == Key.Enter) {
					outText = Json.GenerateString(new Command(textBox_command.Text));
					textBox_command.Text = "";
				} else {
					if (e.Key == Key.LeftCtrl) {
						outText = textBox_command.Text;
					}
				}
				writer.WriteLine(outText);
				writer.Flush();
			} catch (Exception) {
				Disconnect();
			}
		}
		
		void button_connect_Click(object sender, RoutedEventArgs e)
		{
			Connect();
		}
		public void SendQUERY()
		{
			try {
				StreamWriter writer = new StreamWriter(clientSocket.GetStream());
				writer.WriteLine(Json.GenerateString(new Query()));
				writer.Flush();
			} catch (Exception) {
				Disconnect();
			}
		}
		void Connect()
		{
			try {
				clientSocket = new System.Net.Sockets.TcpClient();
				string ip = textBox_ip_port.Text.Split(':')[0];
				int port = int.Parse(textBox_ip_port.Text.Split(':')[1]);
				clientSocket.Connect(ip, port);
				readData = "Conected to Server ...";
				textblock_serverstat.SetText("Server stats loading...");

				msg();

				StreamWriter writer = new StreamWriter(clientSocket.GetStream());
				writer.WriteLine(Json.GenerateString(new Login(textBox_username.Text, CreateMD5(textBox_password.Password))));


				writer.Flush();

				ctThread = new Thread(getMessage);

				ctThread.Start();
				updater.Start();
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
		
		void button_disconnect_Click(object sender, RoutedEventArgs e)
		{
			Disconnect();
		}

		void Disconnect()
		{
			try {
				this.Title = "SAPP Remote > Offline";
				updater.Stop();
				clientSocket.Close();
				ctThread.Abort();
				playerslist.Clear();
				textBox_console.CheckAppendText("> " + "Disconnect from server..." + Environment.NewLine);
				textblock_serverstat.SetText("Disconnect...");
			} catch (Exception) {
			}
		}
 
		bool loadplayerslist = false;
		public void msg()
		{
			string temp = readData;
			switch (Json.get_rco(temp)) {
				case Server.RemoteConsoleOpcode.RC_LOGIN:
					{
						this.SetTitle(" > Online");
						textBox_console.CheckAppendText("> Logged-in: Login LVL>" + Json.get_str(temp, "level") + Environment.NewLine);
						loadplayerslist = true;
					}
					return;
				case Server.RemoteConsoleOpcode.RC_QUERY:
					{

						SS = Json.GetServerStat(temp);
						string serverst = string.Format("Game: {8}\nSAPP Version {0}\nServer Name: {1}\nMap: {2} | GameType: {4}\nNoLead: {6} | Anticheat: {7}", SS.SappVersion, SS.ServerName, SS.Map, SS.Mode, SS.GameType.ToUpper(), SS.Players.ToList().Count, (SS.NoLead ? "ON" : "OFF"), (SS.AntiCheat ? "ON" : "OFF"), (SS.Running ? "Running" : "Not Running"));
						textblock_serverstat.SetText(serverst);
						try {
							while (loadplayerslist) {
								if (playerslist.ToList().Count == 0) {
									foreach (PlayerData PD in SS.Players) {
										if (!playerslist.ToList().Contains(PD)) {
                                             
											playerslist.Add(PD);
                                            
										}
									}
                                     
								}
								loadplayerslist = false;
							}
                            
						} catch (Exception ex) {
						}

                             
						try {
                             
							foreach (PlayerData PD in SS.Players) {
								playerslist[Player.GetListIndex(playerslist, PD.Index)] = PD;

							}
						} catch (Exception ex) {
							 
						}
					}
					return;
				case Server.RemoteConsoleOpcode.RC_CIN:
					{
						switch (Json.get_int(temp, "ret")) {
							case 0:
								{
									textBox_console.CheckAppendText("> " + "Command Faild!" + Environment.NewLine);
								}
								return;
							case 1:
								{
									//OK
								}
								return;
							case -1:
								{
									textBox_console.CheckAppendText("> " + "Sorry you don't have the premission for that command!" + Environment.NewLine);
								}
								return;
						}
					}
					return;
				case Server.RemoteConsoleOpcode.RC_COUT:
					{
						textBox_console.CheckAppendText("> " + Json.get_str(temp, "text") + Environment.NewLine);
					}
					return;
				case Server.RemoteConsoleOpcode.RC_CHAT:
					{
						PlayerData PD = playerslist.ToList()[Player.GetListIndex(playerslist.ToList(), Json.get_int(temp, "index"))];
                        
						switch (Json.get_int(temp, "type")) {
							case 0: //All
								{
									textBox_console.CheckAppendText("> " + PD.Name + " (Chat>All): " + Json.get_str(temp, "message") + Environment.NewLine);
								}
								return;
							case 1: //Team
								{
									textBox_console.CheckAppendText("> " + PD.Name + " (Chat>Team): " + Json.get_str(temp, "message") + Environment.NewLine);
								}
								return;
							case 2: //Vehicle
								{
									textBox_console.CheckAppendText("> " + PD.Name + " (Chat>Vehicle): " + Json.get_str(temp, "message") + Environment.NewLine);
								}
								return;
						}
					}
					return;
				case Server.RemoteConsoleOpcode.RC_PJOIN:
					{
						
						try {
							updater.Stop();
							PlayerData tempplayer = Player.GetData(temp);

							if (!playerslist.ToList().Contains(tempplayer)) {
								playerslist.Add(tempplayer);
							}
							textBox_console.CheckAppendText("> Player Joined, Name: " + tempplayer.Name + Environment.NewLine);
							updater.Start();
						} catch (Exception ex) {
							 
						}
						
					}
					return;
				case Server.RemoteConsoleOpcode.RC_PLEAVE:
					{
						
						try {
							updater.Stop();
							int pindex = Player.GetListIndex(playerslist.ToList(), Json.get_int(temp, "index"));
							PlayerData PD = playerslist[pindex];
							playerslist.RemoveAt(pindex);
							 
							textBox_console.CheckAppendText("> Player Quit, Name: " + PD.Name + Environment.NewLine);
							updater.Start();
						} catch (Exception ex) {
							 
						}
						
					}
					return;
				case Server.RemoteConsoleOpcode.RC_TEAMCHANGE:
					{
						//SendQUERY();
					}
					return;
				case Server.RemoteConsoleOpcode.RC_NEWGAME:
					{
						try {
							updater.Stop();
							
							foreach (PlayerData PD in playerslist.ToList()) {
								textBox_console.CheckAppendText("> Player Quit, Name: " + PD.Name + Environment.NewLine);
							}

							updater.Start();
						} catch (Exception ex) {
							 
						}
						playerslist.Clear();
					}
					return;
				default:
					{
						textBox_console.CheckAppendText("> " + temp + Environment.NewLine);
					}
					return;
			}
		}

		private void getMessage()
		{
			try {
				while (true) {

					serverStream = new StreamReader(clientSocket.GetStream(), Encoding.UTF7);

					string returndata = serverStream.ReadLine();

					readData = "" + returndata.ToString();

					msg();
				}
			} catch (Exception) {
				Disconnect();
			}
		}
		
		void textBox_console_TextChanged(object sender, TextChangedEventArgs e)
		{
			textBox_console.ScrollToEnd();
		}

		private void window_client_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try {
				clientSocket.Close();
				ctThread.Abort();
			} catch (Exception) {
			}
		}

		private void textBox_ip_port_TextChanged(object sender, TextChangedEventArgs e)
		{
			iSettings.IP_Port = textBox_ip_port.Text;
			iSettings.SaveSetting();
		}

		private void textBox_username_TextChanged(object sender, TextChangedEventArgs e)
		{
			iSettings.UserName = textBox_username.Text;
			iSettings.SaveSetting();
		}

		private void textBox_password_PasswordChanged(object sender, RoutedEventArgs e)
		{
			iSettings.Password = textBox_password.Password;
			iSettings.SaveSetting();
		}

		private void checkbox_autoconnect_Click(object sender, RoutedEventArgs e)
		{
			iSettings.AutoConnect = checkbox_autoconnect.IsChecked;
			iSettings.SaveSetting();
		}


		private void button_refresh_Click(object sender, RoutedEventArgs e)
		{
			SendQUERY();
		}


	}
}
