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

using MaterialDesignColors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace SAPPRemote
{
    public class ColorDataList
    {
        private string defaultColorName = "";
        private string defaultColorMetadata = "";
        private bool defaultColorEnabled = true;

        public ColorDataList(string colorname, string colormetadata, bool colorenabled)
        {
            this.ColorName = colorname;
            this.ColorMetadata = colormetadata;
            this.ColorEnabled = colorenabled;
        }

        [JsonProperty("ColorName")]
        public string ColorName { get { return this.defaultColorName; } set { this.defaultColorName = value; } }

        [JsonProperty("ColorMetadata")]
        public string ColorMetadata { get { return this.defaultColorMetadata; } set { this.defaultColorMetadata = value; } }

        [JsonProperty("ColorEnabled")]
        public bool ColorEnabled { get { return this.defaultColorEnabled; } set { this.defaultColorEnabled = value; } }
    }

    public class ColorData : INotifyPropertyChanged
    {
        private List<ColorDataList> defaultColorDataList = new List<ColorDataList>() { };

        [JsonProperty("ColorDataList")]
        public List<ColorDataList> ColorDataList { get { return this.defaultColorDataList; } set { this.defaultColorDataList = value; } }

        /// <summary>
        /// Saves the Color data in selected path.
        /// </summary>
        public void Save()
        {
            if (!Directory.Exists(SAPPRemote.SAPPTcpClient.AppDataPath))
            {
                Directory.CreateDirectory(SAPPRemote.SAPPTcpClient.AppDataPath);
            }

            var s = new JsonSerializerSettings();
            s.ObjectCreationHandling = ObjectCreationHandling.Replace; // without this, you end up with duplicates.

            SwatchesProvider swatchesProvider = new SwatchesProvider();
            List<string> ColorsList = swatchesProvider.Swatches.Select(a => a.Name).ToList();

            ColorData cd = new ColorData();

            cd.ColorDataList.Add(new SAPPRemote.ColorDataList("------------Recommended------------", "SEP", false));
            foreach (string st in ColorsList)
            {
                cd.ColorDataList.Add(new SAPPRemote.ColorDataList(st, "REC", true));
            }
            cd.ColorDataList.Add(new SAPPRemote.ColorDataList("------------Recommended------------", "SEP", false));

            cd.ColorDataList.Add(new SAPPRemote.ColorDataList("------------Others------------", "SEP", false));
            foreach (PropertyInfo c in typeof(Colors).GetProperties())
            {
                Color cx = ((Color)c.GetValue(null));
                cd.ColorDataList.Add(new SAPPRemote.ColorDataList(c.Name, cx.ToString(), true));
            }
            cd.ColorDataList.Add(new SAPPRemote.ColorDataList("------------Others------------", "SEP", false));

            File.WriteAllText(SAPPRemote.SAPPRemoteUI.ColorDataPath, JsonConvert.SerializeObject(cd, Formatting.Indented, s));
        }

        public void Load()
        {
            try
            {
                string json_string = File.ReadAllText(SAPPRemote.SAPPRemoteUI.ColorDataPath);
                if (Json.IsValid(json_string))
                {
                    var s = new JsonSerializerSettings();
                    s.NullValueHandling = NullValueHandling.Ignore;
                    s.ObjectCreationHandling = ObjectCreationHandling.Replace; // without this, you end up with duplicates.

                    SAPPRemote.SAPPRemoteUI.IColorData = JsonConvert.DeserializeObject<ColorData>(json_string, s);
                }
                else
                {
                    Save();
                    Load();
                }
            }
            catch (Exception)
            {
                Save();
                Load();
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
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}