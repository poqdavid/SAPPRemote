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
	
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		enum RemoteConsoleOpcode
		{
			RC_LOGIN = 1,
			RC_QUERY,
			RC_CIN,
			RC_COUT,
			RC_CHAT,
			RC_PJOIN,
			RC_PLEAVE,
			RC_TEAMCHANGE,
			RC_NEWGAME}

		;
        System.Net.Sockets.TcpClient clientSocket;

		StreamReader serverStream = default(StreamReader);
		Thread ctThread;

		string readData = null;

		public MainWindow()
		{
			InitializeComponent();

		}

		void textBox_command_KeyUp(object sender, KeyEventArgs e)
		{
			string outText = "";
			StreamWriter writer = new StreamWriter(clientSocket.GetStream());
			if (e.Key == Key.Enter) {
				outText = "{\"opcode\":3,\"command\":\"" + textBox_command.Text + "\"}";
			}
			writer.WriteLine(outText);
			writer.Flush();
		}
		void textBox_commandraw_KeyUp(object sender, KeyEventArgs e)
		{
			string outText = "";
			StreamWriter writer = new StreamWriter(clientSocket.GetStream());
			if (e.Key == Key.Enter) {
				outText = textBox_commandraw.Text;
			}
			writer.WriteLine(outText);
			writer.Flush();
		}
		void button_connect_Click(object sender, RoutedEventArgs e)
		{
			try {
                clientSocket = new System.Net.Sockets.TcpClient();
				string ip = textBox_ip_port.Text.Split(':')[0];
				int port = int.Parse(textBox_ip_port.Text.Split(':')[1]);
				clientSocket.Connect(ip, port);
				readData = "Conected to Chat Server ...";

				msg();

				StreamWriter writer = new StreamWriter(clientSocket.GetStream());

				
				writer.WriteLine("{\"opcode\":1,\"password\":\"" + textBox_password.Text + "\",\"username\":\"" + textBox_username.Text + "\"}");


				writer.Flush();

				ctThread = new Thread(getMessage);

				ctThread.Start();
			} catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
			 
		}
		
		void button_disconnect_Click(object sender, RoutedEventArgs e)
		{
			try {
				clientSocket.Close();
				ctThread.Abort();
			} catch (Exception) {
			}
		}
		
		public void msg()
		{

			textBox_console.CheckAppendText(" >> " + readData + Environment.NewLine);
           
			 
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

		
		
	}
    
    

	public static class TextBoxExtensions
	{
		public static void CheckAppendText(this TextBoxBase textBox, string msg, bool waitUntilReturn = false)
		{
 
			Action append = () => textBox.AppendText(msg);
			if (textBox.CheckAccess()) {
				append();
			} else if (waitUntilReturn) {
				textBox.Dispatcher.Invoke(append);
			} else {
				textBox.Dispatcher.BeginInvoke(append);
			}
		}
	}
}
