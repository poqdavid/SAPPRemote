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
	public partial class SAPPRemoteUI : Window
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

		public Players<PlayerData> playerslist = new Players<PlayerData>();

		public DispatcherTimer updater = new DispatcherTimer();
		public ContextMenu CM = new ContextMenu();

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

        
		public SAPPRemoteUI()
		{
			ReadArgs();
			updater.Tick += updater_Tick;
			updater.Interval = new TimeSpan(0, 0, 5);
			updater.Stop();
			iSettings = new Settings();
			iSettings.LoadSettings();
			InitializeComponent();


			textBox_ip_port.Text = iSettings.IP_Port;
			textBox_username.Text = iSettings.UserName;
			textBox_password.Password = iSettings.Password;
             
			checkbox_autoconnect.IsChecked = iSettings.AutoConnect;
            
			playerslist.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(playerslist_CollectionChanged);
			foreach (iMenuData imd in iSettings.iMenuItems) {
				CM.Items.Add(GenerateMenuItem(imd));
			}

		}

		public MenuItem GenerateMenuItem(iMenuData imd)
		{
			MenuItem MI = new MenuItem();
			MI.Click += new RoutedEventHandler(MenuItem_Click);
			MI.Header = imd.Text;
			MI.Tag = imd.Command;
			return MI;
		}

		void window_client_Loaded(object sender, RoutedEventArgs e)
		{
			if (iSettings.AutoConnect == true) {
				myTcpClient.Connect(textBox_ip_port.Text, textBox_username.Text, textBox_password.Password);
			}
		}

		public static string CreateMD5(string input)
		{
			MD5 md5Hash = System.Security.Cryptography.MD5.Create();

			byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

			StringBuilder sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++) {
				sBuilder.Append(data[i].ToString("x2"));
			}

			return sBuilder.ToString();
		}
		
		private void updater_Tick(object sender, EventArgs e)
		{
			myTcpClient.SendQUERY_STATS();
		}

		void playerslist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			listBox_players.ItemsSource = playerslist;
		}

		void textBox_command_KeyUp(object sender, KeyEventArgs e)
		{
			try {
				string outText = "";
				if (e.Key == Key.Enter) {
					
					outText = Json.GenerateString(new Command(textBox_command.Text));
					myTcpClient.Send(myTcpClient.clientSocket.Client, outText);
					textBox_command.Text = "";
					
				} else {
					if (e.Key == Key.LeftShift) {
						outText = textBox_command.Text;
						myTcpClient.Send(myTcpClient.clientSocket.Client, outText);
						textBox_command.Text = "";
					}
				}

			} catch (Exception) {
				myTcpClient.Disconnect();
				if (e.Key == Key.Enter) {
					textBox_command.Text = "";
				} else {
					if (e.Key == Key.LeftShift) {
						textBox_command.Text = "";
					}
				}
			}
		}
		
		void button_connect_Click(object sender, RoutedEventArgs e)
		{
			myTcpClient.Connect(textBox_ip_port.Text, textBox_username.Text, textBox_password.Password);
		}

		void button_disconnect_Click(object sender, RoutedEventArgs e)
		{
			myTcpClient.Disconnect();
		}

		void textBox_console_TextChanged(object sender, TextChangedEventArgs e)
		{
			textBox_console.ScrollToEnd();
		}

		private void window_client_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try {
				myTcpClient.Disconnect();
			} catch (Exception) {
			}
		}

		public void AppendConsoleText(string text)
		{
			textBox_console.CheckAppendText("> " + text + Environment.NewLine);
		}
		
		public void SetServerStatText(string text)
		{
			textblock_serverstat.SetText(text);
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
			myTcpClient.SendQUERY();
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem temp = (MenuItem)sender;
			PlayerData PD = (PlayerData)listBox_players.SelectedItem;
			string outText = "";
			outText = Json.GenerateString(new Command(temp.Tag.ToString().Replace("%index", PD.Index.ToString()).Replace("%name", PD.Name)));
			myTcpClient.Send(myTcpClient.clientSocket.Client, outText);
           
		}

	}
}
