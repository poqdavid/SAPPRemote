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
		private string defaultIP_Port = "127.0.0.1:12345";

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
		/// Gets or sets the IP_Port property.
		///</summary>
		///<value>IP:Port string.</value>
		[JsonProperty("IP_Port")]
		[DefaultValue("127.0.0.1:12345")]
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

		/// <summary>
		/// Saves the plugin settings in selected path.
		/// </summary>
		public void SaveSetting()
		{
			var s = new JsonSerializerSettings();
			s.ObjectCreationHandling = ObjectCreationHandling.Replace; // without this, you end up with duplicates.

			File.WriteAllText(SAPPRemote.MainWindow.SettingPath, JsonConvert.SerializeObject(SAPPRemote.MainWindow.ISettings, Formatting.Indented, s));
		}

		/// <summary>
		/// Loads the plugin settings from the selected path.
		/// </summary>
		public void LoadSettings()
		{
			try {
				string json_string = File.ReadAllText(SAPPRemote.MainWindow.SettingPath);
				if (Json.IsValid(json_string)) {
					var s = new JsonSerializerSettings();
					s.NullValueHandling = NullValueHandling.Ignore;
					s.ObjectCreationHandling = ObjectCreationHandling.Replace; // without this, you end up with duplicates.

					SAPPRemote.MainWindow.ISettings = JsonConvert.DeserializeObject<Settings>(json_string, s);
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
}
