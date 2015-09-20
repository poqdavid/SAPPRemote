// <copyright file="Player.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the Player class.</summary>
namespace SAPPRemote
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Windows.Media;
	using System.Windows.Threading;
	using Newtonsoft.Json;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Windows.Controls;

	internal static class Player
	{
		internal static SolidColorBrush GetColor(iColor color)
		{
			byte r = (byte)color.R;
			byte g = (byte)color.G;
			byte b = (byte)color.B;
            
			SolidColorBrush brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            
			brush.Freeze();
			return brush;
		}
		internal static SolidColorBrush GetTeamColor(int color_code)
		{
			switch (color_code) {
				case 0:
					return Brushes.Red;
				case 1:
					return Brushes.Blue;
				default:
					return Brushes.Black;
			}
		}
		internal static int GetListIndex(ObservableCollection<PlayerData> plist, int pindex)
		{
			int temp = -1;
			foreach (PlayerData PD in plist) {
				if (PD.Index == pindex) {
					 
					temp = plist.IndexOf(PD);
				}
			}
			
			return temp;
		}
		internal static int GetListIndex(List<PlayerData> plist, int pindex)
		{
			int temp = -1;
			foreach (PlayerData PD in plist) {
				if (PD.Index == pindex) {
					 
					temp = plist.IndexOf(PD);
				}
			}
			
			return temp;
		}
		internal static PlayerData GetData(string json)
		{
			PlayerData temp = new PlayerData();
			temp = Json.GetPlayerData(json);
            
			return temp;
		}
		internal static PlayerStat GetStat(string json)
		{
			PlayerStat temp = new PlayerStat();
			temp.Score = Json.get_int(json, "score");
			temp.Kills = Json.get_int(json, "kills");
			temp.Assists = Json.get_int(json, "assists");
			temp.Deaths = Json.get_int(json, "deaths");
			;
			temp.Suicides = Json.get_int(json, "suicides");
			temp.Betrays = Json.get_int(json, "betrays");
			return temp;
		}
		internal static string GetStat_str(string json)
		{
			string tempstat = "";
			tempstat = string.Format("Score: {0}\nKills: {1}\nAssists: {2}\nDeaths: {3}\nSuicides: {4}\nBetrays: {5}", Json.get_int(json, "score"), Json.get_int(json, "kills"), Json.get_int(json, "assists"), Json.get_int(json, "deaths"), Json.get_int(json, "suicides"), Json.get_int(json, "betrays"));
			return tempstat;
		}
	}


	public class iColor
	{
		private int defaultB = 255;
		private int defaultG = 255;
		private int defaultR = 255;

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
		private iColor defaultColor = new iColor();
		private SolidColorBrush defaultiTeam = Brushes.Transparent;
		private static ContextMenu defaultCM;
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
		public iColor iPlayerColor { get { return this.defaultColor; } set { this.defaultColor = value; } }

		[JsonIgnore]
		public SolidColorBrush PlayerColor { get { return Player.GetColor(this.iPlayerColor); } set { } }

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
		public int iTeam { get { return this.defaultTeam; } set { this.defaultTeam = value; } }

		[JsonIgnore]
		public ContextMenu CM { get { return defaultCM; } set { defaultCM = value; } }

		[JsonIgnore]
		public SolidColorBrush Team {
			get {
				this.defaultiTeam = Player.GetTeamColor(this.iTeam);
				return this.defaultiTeam; 
			}
			set { this.defaultiTeam = value; }
		}

		[JsonIgnore]
		public string Stats { get { return string.Format("Score: {0}\nKills: {1}\nAssists: {2}\nDeaths: {3}\nSuicides: {4}\nBetrays: {5}", this.Score, this.Kills, this.Assists, this.Deaths, this.Suicides, this.Betrays); } set { } }
       
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
		private Dispatcher dispatcherUIThread;

		private delegate void SetItemCallback(int index, PlayerData item);
		private delegate void RemoveItemCallback(int index);
		private delegate void ClearItemsCallback();
		private delegate void InsertItemCallback(int index, PlayerData item);
		private delegate void MoveItemCallback(int oldIndex, int newIndex);
        

		public Dispatcher Dispatcher {
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

		public Players(IEnumerable<PlayerData> collection)
			: this()
		{
			// AddRange(collection);
		}

		protected override void SetItem(int index, PlayerData item)
		{
			if (dispatcherUIThread.CheckAccess()) {
				base.SetItem(index, item);
			} else {
				dispatcherUIThread.Invoke(DispatcherPriority.Send,
					new SetItemCallback(SetItem), index, new object[] { item });
			}
		}

		protected override void RemoveItem(int index)
		{
			if (dispatcherUIThread.CheckAccess()) {
				base.RemoveItem(index);
			} else {
				dispatcherUIThread.Invoke(DispatcherPriority.Send,
					new RemoveItemCallback(RemoveItem), index);
			}
		}

		protected override void ClearItems()
		{
			if (dispatcherUIThread.CheckAccess()) {
				base.ClearItems();
			} else {
				dispatcherUIThread.Invoke(DispatcherPriority.Send,
					new ClearItemsCallback(ClearItems));
			}
		}

		protected override void InsertItem(int index, PlayerData item)
		{
			if (dispatcherUIThread == null) {
				base.InsertItem(index, item);
			} else if (dispatcherUIThread.CheckAccess()) {
				base.InsertItem(index, item);
			} else {
				dispatcherUIThread.Invoke(DispatcherPriority.Send,
					new InsertItemCallback(InsertItem), index, new object[] { item });
			}
		}

		protected override void MoveItem(int oldIndex, int newIndex)
		{
			if (dispatcherUIThread.CheckAccess()) {
				base.MoveItem(oldIndex, newIndex);
			} else {
				dispatcherUIThread.Invoke(DispatcherPriority.Send,
					new MoveItemCallback(MoveItem), oldIndex, new object[] { newIndex });
			}
		}

 



 

 
	}
}
