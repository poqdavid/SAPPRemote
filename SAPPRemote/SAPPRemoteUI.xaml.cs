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

// <copyright file="MainWindow.xaml.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the MainWindow class.</summary>
namespace SAPPRemote
{
    using MaterialDesignColors;
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SAPPRemoteUI : Window
    {
        ///<summary>
        /// --------.
        ///</summary>
        ///<returns>------.</returns>
        private static string colordataPath = Path.Combine(SAPPTcpClient.AppDataPath, "Colors.sappdata");

        private static ColorData iColorData;

        ///<summary>
        /// Gets or sets the iSettings property.
        ///</summary>
        ///<value>Plugin Settings.</value>
        public static Settings ISettings { get { return SAPPTcpClient.ISettings; } set { SAPPTcpClient.ISettings = value; } }

        public static string ColorDataPath { get { return colordataPath; } set { colordataPath = value; } }

        public static ColorData IColorData { get { return iColorData; } set { iColorData = value; } }

        public Players<PlayerData> IPlayersList { get { return playerslist; } set { playerslist = value; } }

        public Players<PlayerData> playerslist;

        public DispatcherTimer updater = new DispatcherTimer();

        private PlayerData SelectedPD = null;

        public SAPPRemoteUI()
        {
            SAPPTcpClient.logger.Info("Initializing resources for SAPPRemoteUI");
            updater.Tick += Updater_Tick;
            updater.Interval = new TimeSpan(0, 0, 5);
            updater.Stop();

            iColorData = new ColorData();
            iColorData.Load();

            playerslist = new Players<PlayerData>();
            InitializeComponent();
            SAPPTcpClient.logger.Info("Initialized resources for SAPPRemoteUI");

            SAPPTcpClient.ISettings.PropertyChanged += ISettings_PropertyChanged;
            SAPPTcpClient.ISettings.GUI.XTheme.PropertyChanged += ITheme_PropertyChanged;

            this.theme_p.ItemsSource = iColorData.ColorDataList;
        }

        private void ITheme_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SAPPTcpClient.ISettings.SaveSetting();
        }

        private void ISettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SAPPTcpClient.ISettings.SaveSetting();
        }

        private void Window_client_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Updater_Tick(object sender, EventArgs e)
        {
            SAPPTcpClient.SendQUERY_STATS();
        }

        private void TextBox_command_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string outText = "";
                if (e.Key == Key.Enter)
                {
                    outText = Json.GenerateString(new Command(textBox_command.Text));
                    SAPPTcpClient.Send(SAPPTcpClient.clientSocket, outText);
                    textBox_command.Text = "";
                }
            }
            catch (Exception ex)
            {
                SAPPTcpClient.Disconnect();
                if (e.Key == Key.Enter)
                {
                    textBox_command.Text = "";
                }
                SAPPTcpClient.logger.Error(ex, ex.Message);
            }
        }

        private void Button_connect_Click(object sender, RoutedEventArgs e)
        {
            SAPPTcpClient.Connect(textBox_ip_port.Text);
        }

        private void Button_disconnect_Click(object sender, RoutedEventArgs e)
        {
            SAPPTcpClient.Disconnect();
        }

        private void RichtextBox_console_TextChanged(object sender, TextChangedEventArgs e)
        {
            richtextBox_console.ScrollToEnd();
        }

        private void Window_client_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                SAPPTcpClient.Disconnect();
            }
            catch (Exception ex)
            {
                SAPPTcpClient.logger.Error(ex, ex.Message);
            }
        }

        public void SetServerStatText(string text)
        {
            textblock_serverstat.SetText(text);
        }

        private void TextBox_password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            SAPPTcpClient.ISettings.Password = textBox_password.Password;
            SAPPTcpClient.ISettings.SaveSetting();
        }

        private void Button_refresh_Click(object sender, RoutedEventArgs e)
        {
            SAPPTcpClient.SendQUERY();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            MenuItemClick(mi);
        }

        private void MenuItemClick(MenuItem mi)
        {
            try
            {
                string cmd = mi.Tag.ToString().Replace("%index", SelectedPD.Index.ToString()).Replace("%name", SelectedPD.Name);
                string outText = Json.GenerateString(new Command(cmd));

                SAPPTcpClient.Send(SAPPTcpClient.clientSocket, outText);
            }
            catch (Exception ex) { SAPPTcpClient.logger.Error(ex, ex.Message); }
        }

        private void Theme_p_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.grid_loading.Visibility == Visibility.Hidden)
            {
                if (theme_p.SelectedItem is ColorDataList)
                {
                    if (((ColorDataList)this.theme_p.SelectedItem).ColorMetadata == "REC")
                    {
                        SetTheme(((ColorDataList)this.theme_p.SelectedItem).ColorName);
                    }
                    else if (((ColorDataList)this.theme_p.SelectedItem).ColorMetadata != "SEP")
                    {
                        string cdata = ((ColorDataList)this.theme_p.SelectedItem).ColorMetadata;
                        Color cx = (Color)System.Windows.Media.ColorConverter.ConvertFromString(cdata);
                        SetTheme(cx);
                    }
                }
            }
        }

        public void SetTheme(string cname)
        {
            var paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();
            SwatchesProvider swatchesProvider = new SwatchesProvider();
            Swatch color = swatchesProvider.Swatches.FirstOrDefault(a => a.Name == cname);
            paletteHelper.ReplacePrimaryColor(color);
        }

        public void SetTheme(Color c)
        {
            var paletteHelper = new MaterialDesignThemes.Wpf.PaletteHelper();

            paletteHelper.ChangePrimaryColor(c);
        }

        private void Window_client_ContentRendered(object sender, EventArgs e)
        {
            SAPPTcpClient.logger.Info("Finished rendering content for SAPPRemoteUI");

            if (theme_p.SelectedItem is ColorDataList)
            {
                if (((ColorDataList)this.theme_p.SelectedItem).ColorMetadata == "REC")
                {
                    SetTheme(((ColorDataList)this.theme_p.SelectedItem).ColorName);
                }
                else if (((ColorDataList)this.theme_p.SelectedItem).ColorMetadata != "SEP")
                {
                    string cdata = ((ColorDataList)this.theme_p.SelectedItem).ColorMetadata;
                    Color cx = (Color)System.Windows.Media.ColorConverter.ConvertFromString(cdata);
                    SetTheme(cx);
                }
            }
            this.grid_loading.Visibility = Visibility.Hidden;

            if (SAPPTcpClient.ISettings.AutoConnect == true)
            {
                SAPPTcpClient.Connect(textBox_ip_port.Text);
            }

            textBox_password.Password = SAPPTcpClient.ISettings.Password;
        }

        private void Button_setting_Click(object sender, RoutedEventArgs e)
        {
            DH_Settings.IsOpen = true;
        }

        private void ListBox_players_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_players.SelectedItem != null)
            {
                this.SelectedPD = (PlayerData)listBox_players.SelectedItem;
            }
        }
    }
}