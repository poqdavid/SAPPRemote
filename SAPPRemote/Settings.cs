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

// <copyright file="Settings.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the settings class.</summary>
namespace SAPPRemote
{
    using Newtonsoft.Json;

    // Import statements are placed here
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Controls;

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

        private GUI defaultGUI = new GUI(new XTheme("bluegrey"));

        ///<summary>
        /// Default value for .
        ///</summary>
        private List<MenuData> defaultiMenuItems = new List<MenuData>(new MenuData[] {
            new MenuData("Kick", "k %index"),
            new MenuData("Ban", "b %index"),
            new MenuData("IP-Ban", "ipban %index")
        });

        ///<summary>
        /// Gets or sets the IP_Port property.
        ///</summary>
        ///<value>IP:Port string.</value>
        [JsonProperty("IP_Port")]
        [DefaultValue("localhost:2302")]
        public string IP_Port
        {
            get { return this.defaultIP_Port; }
            set
            {
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
        public string UserName
        {
            get { return this.defaultUserName; }
            set
            {
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
        public string Password
        {
            get { return this.defaultPassword; }
            set
            {
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
        public bool? AutoConnect
        {
            get { return this.defaultAutoConnect; }
            set
            {
                this.defaultAutoConnect = value;
                this.OnPropertyChanged("AutoConnect");
            }
        }

        ///<summary>
        /// Gets or sets a value indicating whether to use   or not.
        ///</summary>
        ///<value>Use .</value>
        [JsonProperty("MenuItems")]
        public List<MenuData> MenuItems { get { return this.defaultiMenuItems; } set { this.defaultiMenuItems = value; } }

        [JsonIgnore]
        public List<MenuItem> XMenuItems
        {
            get
            {
                List<MenuItem> temp = new List<MenuItem>();
                foreach (MenuData imd in this.MenuItems)
                {
                    temp.Add(GenerateMenuItem(imd));
                }

                return temp;
            }
        }

        public MenuItem GenerateMenuItem(MenuData imd)
        {
            MenuItem MI = new MenuItem
            {
                Header = imd.Text,
                Tag = imd.Command
            };
            return MI;
        }

        public GUI GUI { get { return this.defaultGUI; } set { this.defaultGUI = value; this.OnPropertyChanged("GUI"); } }

        /// <summary>
        /// Saves the App settings in selected path.
        /// </summary>
        public void SaveSetting()
        {
            if (!Directory.Exists(SAPPRemote.SAPPTcpClient.AppDataPath))
            {
                Directory.CreateDirectory(SAPPRemote.SAPPTcpClient.AppDataPath);
            }

            var s = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace // without this, you end up with duplicates.
            };

            File.WriteAllText(SAPPRemote.SAPPTcpClient.SettingPath, JsonConvert.SerializeObject(SAPPRemote.SAPPRemoteUI.ISettings, Formatting.Indented, s));
        }

        /// <summary>
        /// Loads the App settings from the selected path.
        /// </summary>
        public void LoadSettings()
        {
            try
            {
                string json_string = File.ReadAllText(SAPPRemote.SAPPTcpClient.SettingPath);
                if (Json.IsValid(json_string))
                {
                    var s = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Replace // without this, you end up with duplicates.
                    };

                    SAPPRemote.SAPPRemoteUI.ISettings = JsonConvert.DeserializeObject<Settings>(json_string, s);
                }
                else
                {
                    SaveSetting();
                    LoadSettings();
                }
            }
            catch (Exception)
            {
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
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class XTheme : INotifyPropertyChanged
    {
        public XTheme(string color)
        {
            this.Color = color;
        }

        private string defaultColor = "bluegrey";

        [JsonProperty("Color")]
        public string Color { get { return this.defaultColor; } set { this.defaultColor = value; this.OnPropertyChanged("Color"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies objects registered to receive this event that a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class GUI
    {
        public GUI(XTheme theme)
        {
            this.XTheme = theme;
        }

        private XTheme defaultiTheme = new XTheme("bluegrey");

        [JsonProperty("Theme")]
        public XTheme XTheme { get { return this.defaultiTheme; } set { this.defaultiTheme = value; } }
    }

    public class MenuData
    {
        public MenuData(string text, string command)
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