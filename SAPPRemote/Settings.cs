// <copyright file="Settings.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the settings class.</summary>
namespace SAPPRemote
{
	// Import statements are placed here
	using System;
	using System.ComponentModel;
	using System.IO;
	using Newtonsoft;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System.Windows.Controls;
	using System.Windows;
	using System.Collections.Generic;

	/// <summary>
	/// And this part is for the settings :)
	/// <list type="bullet">
	/// <item>
	/// <term>Author</term>
	/// <description>POQDavid</description>
	/// </item>
	/// </list>
	/// </summary>
	public class Settings : INotifyPropertyChanged
	{
		///<summary>
		/// Default value for IP_Port.
		///</summary>
		private string defaultIP_Port = "localhost:2302";

		///<summary>
		/// Default value for UserName.
		///</summary>
		private string defaultUserName = "";

		///<summary>
		/// Default value for Password.
		///</summary>
		private string defaultPassword = "";

		///<summary>
		/// Default value for AutoConnect.
		///</summary>
		private bool? defaultAutoConnect = false;

		///<summary>
		/// Default value for .
		///</summary>
		private List<iMenuData> defaultiMenuItems = new List<iMenuData>(new iMenuData[] {
			new iMenuData("Kick", "k %index"),
			new iMenuData("Ban", "b %index"),
            new iMenuData("IP-Ban", "ipban %index")
        });

		///<summary>
		/// Gets or sets the IP_Port property.
		///</summary>
		///<value>IP:Port string.</value>
		[JsonProperty("IP_Port")]
		[DefaultValue("localhost:2302")]
		public string IP_Port {
			get { return this.defaultIP_Port; }
			set {
				this.defaultIP_Port = value;
				this.OnPropertyChanged("IP_Port");
			}
		}

		///<summary>
		/// Gets or sets the UserName property.
		///</summary>
		///<value>User Name string.</value>
		[JsonProperty("UserName")]
		[DefaultValue("")]
		public string UserName {
			get { return this.defaultUserName; }
			set {
				this.defaultUserName = value;
				this.OnPropertyChanged("UserName");
			}
		}

		///<summary>
		/// Gets or sets the Password property.
		///</summary>
		///<value>Password mode.</value>
		[JsonProperty("Password")]
		[DefaultValue("")]
		public string Password {
			get { return this.defaultPassword; }
			set {
				this.defaultPassword = value;
				this.OnPropertyChanged("Password");
			}
		}

		///<summary>
		/// Gets or sets a value indicating whether to use AutoConnect or not.
		///</summary>
		///<value>Use AutoConnect.</value>
		[JsonProperty("AutoConnect")]
		[DefaultValue(false)]
		public bool? AutoConnect {
			get { return this.defaultAutoConnect; }
			set {
				this.defaultAutoConnect = value;
				this.OnPropertyChanged("AutoConnect");
			}
		}

		///<summary>
		/// Gets or sets a value indicating whether to use   or not.
		///</summary>
		///<value>Use .</value>
		[JsonProperty("MenuItems")]
		public List<iMenuData> iMenuItems { get { return this.defaultiMenuItems; } set { this.defaultiMenuItems = value; } }

		/// <summary>
		/// Saves the plugin settings in selected path.
		/// </summary>
		public void SaveSetting()
		{
			var s = new JsonSerializerSettings();
			s.ObjectCreationHandling = ObjectCreationHandling.Replace; // without this, you end up with duplicates.

			File.WriteAllText(SAPPRemote.SAPPRemoteUI.SettingPath, JsonConvert.SerializeObject(SAPPRemote.SAPPRemoteUI.ISettings, Formatting.Indented, s));
		}

		/// <summary>
		/// Loads the plugin settings from the selected path.
		/// </summary>
		public void LoadSettings()
		{
			try {
				string json_string = File.ReadAllText(SAPPRemote.SAPPRemoteUI.SettingPath);
				if (Json.IsValid(json_string)) {
					var s = new JsonSerializerSettings();
					s.NullValueHandling = NullValueHandling.Ignore;
					s.ObjectCreationHandling = ObjectCreationHandling.Replace; // without this, you end up with duplicates.

					SAPPRemote.SAPPRemoteUI.ISettings = JsonConvert.DeserializeObject<Settings>(json_string, s);
				} else {
					SaveSetting();
					LoadSettings();
				}
			} catch (Exception) {
				SaveSetting();
				LoadSettings();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Notifies objects registered to receive this event that a property value has changed.
		/// </summary>
		/// <param name="propertyName">The name of the property that was changed.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null) {
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	public class iMenuData
	{
		public iMenuData(string text, string command)
		{
			this.Text = text;
			this.Command = command;
		}

		private string defaultText = "";
		private string defaultCommand = "";

		[JsonProperty("text")]
		public string Text { get { return this.defaultText; } set { this.defaultText = value; } }

		[JsonProperty("command")]
		public string Command { get { return this.defaultCommand; } set { this.defaultCommand = value; } }
    
	}
}
