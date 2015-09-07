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
		public DispatcherTimer updater = new DispatcherTimer();
		System.Net.Sockets.TcpClient clientSocket;

		StreamReader serverStream = default(StreamReader);
		Thread ctThread;

		string readData = null;
        private void ReadArgs()
        {
            if (Environment.GetCommandLineArgs().Length > 0)
            {
                int i = 0;
                foreach (string arg in Environment.GetCommandLineArgs())
                {
                    if (arg == "path")
                    {
                        settingPath = Environment.GetCommandLineArgs()[(i + 1)];
                    }
                    i++;
                }

            }
            else
            {

            }
        }
		public MainWindow()
		{
            ReadArgs();
            iSettings = new Settings();
            iSettings.LoadSettings();
			InitializeComponent();
			updater.Tick += updater_Tick;
			updater.Interval = new TimeSpan(0, 0, 1);

            textBox_ip_port.Text = iSettings.IP_Port;
            textBox_username.Text = iSettings.UserName;
            textBox_password.Text = iSettings.Password;
            checkbox_autoconnect.IsChecked = iSettings.AutoConnect;
            
			playerslist.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(playerslist_CollectionChanged);
            if (iSettings.AutoConnect == true) { Connect(); }
		}
		
		private void updater_Tick(object sender, EventArgs e)
		{
			StreamWriter writer = new StreamWriter(clientSocket.GetStream());	
			writer.WriteLine("{\"opcode\":2}");
			writer.Flush();
		}

		void playerslist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			 listBox_players.ItemsSource = null;
			 listBox_players.ItemsSource = playerslist;
		}

		void textBox_command_KeyUp(object sender, KeyEventArgs e)
		{
			try {
				string outText = "";
				StreamWriter writer = new StreamWriter(clientSocket.GetStream());
				if (e.Key == Key.Enter) {
					outText = "{\"opcode\":3,\"command\":\"" + textBox_command.Text + "\"}";
				} else {
					if (e.Key == Key.LeftCtrl) {
						outText = textBox_command.Text;
					}
				}
				writer.WriteLine(outText);
				writer.Flush();
			} catch (Exception) {
			}
		}
		
		void button_connect_Click(object sender, RoutedEventArgs e)
		{
            Connect();
		}
        void Connect()
        {
            try
            {
                clientSocket = new System.Net.Sockets.TcpClient();
                string ip = textBox_ip_port.Text.Split(':')[0];
                int port = int.Parse(textBox_ip_port.Text.Split(':')[1]);
                clientSocket.Connect(ip, port);
                 readData = "Conected to Server ...";

                msg();

                StreamWriter writer = new StreamWriter(clientSocket.GetStream());


                writer.WriteLine("{\"opcode\":1,\"password\":\"" + textBox_password.Text + "\",\"username\":\"" + textBox_username.Text + "\"}");


                writer.Flush();

                ctThread = new Thread(getMessage);

                ctThread.Start();
                updater.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
		
		void button_disconnect_Click(object sender, RoutedEventArgs e)
		{
            iSettings.AutoConnect = true;
			try {
				this.Title = "SAPP Remote > Offline";
				updater.Stop();
				clientSocket.Close();
				ctThread.Abort();
			} catch (Exception) {
			}
		}
		
		public void msg()
		{
			string temp = readData;
			switch (Json.get_rco(temp)) {
				case Server.RemoteConsoleOpcode.RC_LOGIN:
					{
						this.SetTitle("> Login LVL: " + Json.get_str(temp, "level"));
					}
					return;
				case Server.RemoteConsoleOpcode.RC_QUERY:
					{
						//textBox_console.CheckAppendText("> " + temp + Environment.NewLine);
					}
					return;
				case Server.RemoteConsoleOpcode.RC_CIN:
					{
                        switch (Json.get_int(temp, "ret"))
                        {
                            case 0:
                                {
                                    //OK
                                }
                                return;
                            case 1:
                                {
                                    textBox_console.CheckAppendText("> " + "Command Faild!" + Environment.NewLine);
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
                       PlayerData PD = playerslist[Player.GetListIndex(playerslist, Json.get_int(temp, "index"))];
                       switch (Json.get_int(temp, "type"))
                       {
                           case 0: //All
                               {
                                   textBox_console.CheckAppendText("> "+ PD.Name +" (Chat>All): "+ Json.get_str(temp, "message") + Environment.NewLine);
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
						PlayerData tempplayer = Player.GetData(temp);
						playerslist.Add(tempplayer);
                        textBox_console.CheckAppendText("> Player Joined, Name: " + tempplayer.Name + Environment.NewLine);
					}
					return;
				case Server.RemoteConsoleOpcode.RC_PLEAVE:
					{
                        try
                        {
                            int pindex = Player.GetListIndex(playerslist, Json.get_int(temp, "index"));
                            PlayerData PD = playerslist[pindex];
                            playerslist.RemoveAt(pindex);
                            textBox_console.CheckAppendText("> Player Quit, Name: " + PD.Name + Environment.NewLine);
                        }
                        catch (Exception) { }
					}
					return;
				case Server.RemoteConsoleOpcode.RC_TEAMCHANGE:
					{
						textBox_console.CheckAppendText("> TEAMCHANGE: " + temp + Environment.NewLine);
					}
					return;
				case Server.RemoteConsoleOpcode.RC_NEWGAME:
					{
						textBox_console.CheckAppendText("> NEWGAME: " + temp + Environment.NewLine);
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

					serverStream = new StreamReader(clientSocket.GetStream());

					string returndata = serverStream.ReadLine();

					readData = "" + returndata.ToString();

					msg();
				}
			} catch (Exception) {
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

        private void textBox_password_TextChanged(object sender, TextChangedEventArgs e)
        {
            iSettings.Password = textBox_password.Text;
            iSettings.SaveSetting();
        }

        private void checkbox_autoconnect_Click(object sender, RoutedEventArgs e)
        {
            iSettings.AutoConnect = checkbox_autoconnect.IsChecked;
            iSettings.SaveSetting();
        }

        private void window_client_Loaded(object sender, RoutedEventArgs e)
        {

 
        }
	}
}
