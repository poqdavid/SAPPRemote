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

	internal static class Player
	{
		internal static SolidColorBrush GetColor(string color_str)
		{
            byte r = (byte)Json.get_int(color_str, "r");
            byte g = (byte)Json.get_int(color_str, "g");
            byte b = (byte)Json.get_int(color_str, "b");
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, r, g, b));
            brush.Freeze();
            return brush;
		}
		internal static SolidColorBrush GetTeamColor(int color_code)
		{
			switch (color_code) {
				case 0:
					return Brushes.Blue;
				case 1:
					return Brushes.Red;
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
		internal static PlayerData GetData(string json)
		{
			PlayerData temp = new PlayerData();
			temp.Name = Json.get_str(json, "name");
			temp.Index = Json.get_int(json, "index");
			temp.Stats = GetStat_str(json);
			temp.Team = GetTeamColor(Json.get_int(json, "team"));
			temp.PlayerColor = GetColor(Json.get_str(json,"color"));
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
	
	public class PlayerData
	{
		public string Name { get; set; }
		public int Index { get; set; }
		public string Stats { get; set; }

		// 0 = Blue 1 = Red
		private SolidColorBrush defaultTeam = Brushes.Black;
		public SolidColorBrush Team { get { return this.defaultTeam; } set { this.defaultTeam = value; } }
		private SolidColorBrush defaultPlayerColor = Brushes.Black;
		public SolidColorBrush PlayerColor { get { return this.defaultPlayerColor; } set { this.defaultPlayerColor = value; } }
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
