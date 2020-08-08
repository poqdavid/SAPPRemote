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

// <copyright file="Player.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the Player class.</summary>
namespace SAPPRemote
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Media;
    using System.Windows.Threading;

    internal static class Player
    {
        internal static SolidColorBrush GetColor(IColor color)
        {
            byte r = (byte)color.R;
            byte g = (byte)color.G;
            byte b = (byte)color.B;

            SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(r, g, b));

            brush.Freeze();
            return brush;
        }

        internal static IColor GetColor(SolidColorBrush color)
        {
            int r = (int)color.Color.R;
            int g = (int)color.Color.G;
            int b = (int)color.Color.B;

            IColor ic = new IColor(r, g, b);

            return ic;
        }

        internal static string GetTeamText(int code)
        {
            return code switch
            {
                0 => "Red",
                1 => "Blue",
                _ => "None",
            };
        }

        internal static int GetListIndex(ObservableCollection<PlayerData> plist, int pindex)
        {
            int temp = -1;
            foreach (PlayerData PD in plist)
            {
                if (PD.Index == pindex)
                {
                    temp = plist.IndexOf(PD);
                }
            }

            return temp;
        }

        internal static int GetListIndex(List<PlayerData> plist, int pindex)
        {
            int temp = -1;
            foreach (PlayerData PD in plist)
            {
                if (PD.Index == pindex)
                {
                    temp = plist.IndexOf(PD);
                }
            }

            return temp;
        }

        internal static PlayerData GetData(string json)
        {
            return Json.GetPlayerData(json);
        }

        internal static PlayerStat GetStat(string json)
        {
            PlayerStat temp = new PlayerStat
            {
                Score = Json.Get_int(json, "score"),
                Kills = Json.Get_int(json, "kills"),
                Assists = Json.Get_int(json, "assists"),
                Deaths = Json.Get_int(json, "deaths"),
                Suicides = Json.Get_int(json, "suicides"),
                Betrays = Json.Get_int(json, "betrays")
            };
            return temp;
        }

        internal static string GetStat_str(string json)
        {
            return string.Format("Score: {0}\nKills: {1}\nAssists: {2}\nDeaths: {3}\nSuicides: {4}\nBetrays: {5}", Json.Get_int(json, "score"), Json.Get_int(json, "kills"), Json.Get_int(json, "assists"), Json.Get_int(json, "deaths"), Json.Get_int(json, "suicides"), Json.Get_int(json, "betrays"));
        }
    }

    public class IColor
    {
        private int defaultB = 255;
        private int defaultG = 255;
        private int defaultR = 255;

        public IColor(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        [JsonProperty("b")]
        public int B { get { return this.defaultB; } set { this.defaultB = value; } }

        [JsonProperty("g")]
        public int G { get { return this.defaultG; } set { this.defaultG = value; } }

        [JsonProperty("r")]
        public int R { get { return this.defaultR; } set { this.defaultR = value; } }
    }

    public class PlayerData
    {
        private int defaultAssists = 0;
        private int defaultBetrays = 0;
        private IColor defaultColor = new IColor(255, 255, 255);
        private readonly SolidColorBrush defaultiTeam = Brushes.Transparent;

        private int defaultDeaths = 0;
        private int defaultIndex = 0;
        private int defaultKills = 0;
        private string defaultName = "";
        private int defaultScore = 0;
        private int defaultSuicides = 0;
        private int defaultTeam = 0;

        [JsonProperty("assists")]
        public int Assists { get { return this.defaultAssists; } set { this.defaultAssists = value; } }

        [JsonProperty("betrays")]
        public int Betrays { get { return this.defaultBetrays; } set { this.defaultBetrays = value; } }

        [JsonProperty("color")]
        public IColor IPlayerColor { get { return this.defaultColor; } set { this.defaultColor = value; } }

        [JsonIgnore]
        public SolidColorBrush PlayerColor { get { return Player.GetColor(this.IPlayerColor); } set { } }

        [JsonProperty("deaths")]
        public int Deaths { get { return this.defaultDeaths; } set { this.defaultDeaths = value; } }

        [JsonProperty("index")]
        public int Index { get { return this.defaultIndex; } set { this.defaultIndex = value; } }

        [JsonProperty("kills")]
        public int Kills { get { return this.defaultKills; } set { this.defaultKills = value; } }

        [JsonProperty("name")]
        public string Name { get { return this.defaultName; } set { this.defaultName = value; } }

        [JsonProperty("score")]
        public int Score { get { return this.defaultScore; } set { this.defaultScore = value; } }

        [JsonProperty("suicides")]
        public int Suicides { get { return this.defaultSuicides; } set { this.defaultSuicides = value; } }

        [JsonProperty("team")]
        public int ITeam { get { return this.defaultTeam; } set { this.defaultTeam = value; } }

        [JsonIgnore]
        public string IndexST { get { return this.defaultIndex + ". "; } }

        [JsonIgnore]
        public List<MenuData> MenuItems { get { return SAPPTcpClient.ISettings.MenuItems; } set { SAPPTcpClient.ISettings.MenuItems = value; } }

        [JsonIgnore]
        public string Stats { get { return string.Format("Score: {0}\nKills: {1}\nAssists: {2}\nDeaths: {3}\nSuicides: {4}\nBetrays: {5}", this.Score, this.Kills, this.Assists, this.Deaths, this.Suicides, this.Betrays); } set { } }
    }

    public class PlayersStat
    {
        ///<summary>
        /// Default value for .
        ///</summary>
        private Players<PlayerData> defaultPlayers = new Players<PlayerData>() { };

        ///<summary>
        /// Gets or sets a value indicating whether to use   or not.
        ///</summary>
        ///<value>Use .</value>
        [JsonProperty("players")]
        public Players<PlayerData> Players { get { return this.defaultPlayers; } set { this.defaultPlayers = value; } }
    }

    public class PlayerStat
    {
        public int Score { get; set; }
        public int Kills { get; set; }
        public int Assists { get; set; }
        public int Deaths { get; set; }
        public int Suicides { get; set; }
        public int Betrays { get; set; }
    }

    public class Players<PlayerData> : ObservableCollection<PlayerData>
    {
        private readonly Dispatcher dispatcherUIThread;

        private delegate void SetItemCallback(int index, PlayerData item);

        private delegate void RemoveItemCallback(int index);

        private delegate void ClearItemsCallback();

        private delegate void InsertItemCallback(int index, PlayerData item);

        private delegate void MoveItemCallback(int oldIndex, int newIndex);

        public Dispatcher Dispatcher
        {
            get { return dispatcherUIThread; }
        }

        public Players(Dispatcher dispatcher)
        {
            dispatcherUIThread = dispatcher;
        }

        public Players()
        {
            dispatcherUIThread = Dispatcher.CurrentDispatcher;
        }

        /*        public Players(IEnumerable<PlayerData> collection)
                    : this()
                {
                    AddRange(collection);
                }*/

        protected override void SetItem(int index, PlayerData item)
        {
            if (dispatcherUIThread.CheckAccess())
            {
                base.SetItem(index, item);
            }
            else
            {
                dispatcherUIThread.Invoke(DispatcherPriority.Send,
                    new SetItemCallback(SetItem), index, new object[] { item });
            }
        }

        protected override void RemoveItem(int index)
        {
            if (dispatcherUIThread.CheckAccess())
            {
                base.RemoveItem(index);
            }
            else
            {
                dispatcherUIThread.Invoke(DispatcherPriority.Send,
                    new RemoveItemCallback(RemoveItem), index);
            }
        }

        protected override void ClearItems()
        {
            if (dispatcherUIThread.CheckAccess())
            {
                base.ClearItems();
            }
            else
            {
                dispatcherUIThread.Invoke(DispatcherPriority.Send,
                    new ClearItemsCallback(ClearItems));
            }
        }

        protected override void InsertItem(int index, PlayerData item)
        {
            if (dispatcherUIThread == null)
            {
                base.InsertItem(index, item);
            }
            else if (dispatcherUIThread.CheckAccess())
            {
                base.InsertItem(index, item);
            }
            else
            {
                dispatcherUIThread.Invoke(DispatcherPriority.Send,
                    new InsertItemCallback(InsertItem), index, new object[] { item });
            }
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (dispatcherUIThread.CheckAccess())
            {
                base.MoveItem(oldIndex, newIndex);
            }
            else
            {
                dispatcherUIThread.Invoke(DispatcherPriority.Send,
                    new MoveItemCallback(MoveItem), oldIndex, new object[] { newIndex });
            }
        }
    }
}