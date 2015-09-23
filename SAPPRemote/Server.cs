// <copyright file="Server.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the Server class.</summary>
namespace SAPPRemote
{
	using System;
	using Newtonsoft.Json;
	using System.ComponentModel;
	using System.Collections.Generic;
	using System.Windows.Media;
	using Newtonsoft.Json.Converters;
	
	/// <summary>
	/// Description of Server.
	/// </summary>
	public class Server
	{

		[Description("Property 1")]
		public enum RemoteConsoleOpcode
		{
			RC_LOGIN = 1,
			RC_QUERY,
			RC_QUERY_STATS,
			RC_QUERY_POS,
			RC_CIN,
			RC_COUT,
			RC_CHAT,
			RC_PJOIN,
			RC_PLEAVE,
			RC_TEAMCHANGE,
			RC_NEWGAME
		}
		
		[JsonProperty("opcode")]
		[Description("Property 1")]
		public static RemoteConsoleOpcode opcode { get; set; }
		
		public Server()
		{
		}
	}

	public class Login
	{
		public Login()
		{
        
		}

		public Login(string usern, string userp)
		{
			this.UserName = usern;
			this.Password = userp;
		}
		public Login(Server.RemoteConsoleOpcode opcode, string usern, string userp)
		{
			this.opCode = opcode;
			this.UserName = usern;
			this.Password = userp;
		}
		///<summary>
		/// Default value for opCode.
		///</summary>
		private Server.RemoteConsoleOpcode defaultopCode = Server.RemoteConsoleOpcode.RC_LOGIN;

		///<summary>
		/// Default value for UserName.
		///</summary>
		private string defaultUserName = "";

		///<summary>
		/// Default value for Password.
		///</summary>
		private string defaultPassword = "";

		///<summary>
		/// Gets or sets the opCode property.
		///</summary>
		///<value>opCode data.</value>
		[JsonProperty("opcode")]
		[DefaultValue(Server.RemoteConsoleOpcode.RC_LOGIN)]
		public Server.RemoteConsoleOpcode opCode { get { return this.defaultopCode; } set { this.defaultopCode = value; } }

		///<summary>
		/// Gets or sets the UserName property.
		///</summary>
		///<value>User Name string.</value>
		[JsonProperty("username")]
		[DefaultValue("")]
		public string UserName { get { return this.defaultUserName; } set { this.defaultUserName = value; } }

		///<summary>
		/// Gets or sets the Password property.
		///</summary>
		///<value>Password mode.</value>
		[JsonProperty("password")]
		[DefaultValue("")]
		public string Password { get { return this.defaultPassword; } set { this.defaultPassword = value; } }
	}

	public class Command
	{

		public Command()
		{
		}
		public Command(string command)
		{
			this.myCommand = command;
		}

		public Command(Server.RemoteConsoleOpcode opcode, string command)
		{
			this.opCode = opcode;
			this.myCommand = command;
		}

		///<summary>
		/// Default value for opCode.
		///</summary>
		private Server.RemoteConsoleOpcode defaultopCode = Server.RemoteConsoleOpcode.RC_CIN;

		///<summary>
		/// Default value for UserName.
		///</summary>
		private string defaultCommand = "";


		///<summary>
		/// Gets or sets the opCode property.
		///</summary>
		///<value>opCode data.</value>
		[JsonProperty("opcode")]
		[DefaultValue(Server.RemoteConsoleOpcode.RC_CIN)]
		public Server.RemoteConsoleOpcode opCode { get { return this.defaultopCode; } set { this.defaultopCode = value; } }

		///<summary>
		/// Gets or sets the UserName property.
		///</summary>
		///<value>User Name string.</value>
		[JsonProperty("command")]
		[DefaultValue("")]
		public string myCommand { get { return this.defaultCommand; } set { this.defaultCommand = value; } }

	}

	public class ServerStat
	{
		///<summary>
		/// Default value for .
		///</summary>
		private bool defaultAntiCheat = false;

		///<summary>
		/// Default value for .
		///</summary>
		private string defaultGameType = "";

		///<summary>
		/// Default value for .
		///</summary>
		private string defaultMap = "";

		///<summary>
		/// Default value for .
		///</summary>
		private string defaultMode = "";

		///<summary>
		/// Default value for .
		///</summary>
		private bool defaultNoLead = false;

		///<summary>
		/// Default value for .
		///</summary>
		private bool defaultRunning = false;

		///<summary>
		/// Default value for .
		///</summary>
		private string defaultSappVersion = "";

		///<summary>
		/// Default value for .
		///</summary>
		private string defaultServerName = "";

		///<summary>
		/// Default value for .
		///</summary>
		private bool? defaultTeams = false;

		///<summary>
		/// Default value for .
		///</summary>
		private string defaultVersion = "";

		///<summary>
		/// Default value for .
		///</summary>
		private Players<PlayerData> defaultPlayers = new Players<PlayerData>() { };

		///<summary>
		/// Gets or sets a value indicating whether to use  or not.
		///</summary>
		///<value>Use .</value>
		[JsonProperty("anticheat")]
		[DefaultValue(false)]
		public bool AntiCheat { get { return this.defaultAntiCheat; } set { this.defaultAntiCheat = value; } }

		///<summary>
		/// Gets or sets the  property.
		///</summary>
		///<value>  string.</value>
		[JsonProperty("gametype")]
		[DefaultValue("")]
		public string GameType { get { return this.defaultGameType; } set { this.defaultGameType = value; } }

		///<summary>
		/// Gets or sets the   property.
		///</summary>
		///<value>  string.</value>
		[JsonProperty("map")]
		[DefaultValue("")]
		public string Map { get { return this.defaultMap; } set { this.defaultMap = value; } }

		///<summary>
		/// Gets or sets the   property.
		///</summary>
		///<value>  string.</value>
		[JsonProperty("mode")]
		[DefaultValue("")]
		public string Mode { get { return this.defaultMode; } set { this.defaultMode = value; } }

		///<summary>
		/// Gets or sets a value indicating whether to use   or not.
		///</summary>
		///<value>Use .</value>
		[JsonProperty("no-lead")]
		[DefaultValue(false)]
		public bool NoLead { get { return this.defaultNoLead; } set { this.defaultNoLead = value; } }

		///<summary>
		/// Gets or sets a value indicating whether to use   or not.
		///</summary>
		///<value>Use .</value>
		[JsonProperty("players")]
		public Players<PlayerData> Players { get { return this.defaultPlayers; } set { this.defaultPlayers = value; } }

		///<summary>
		/// Gets or sets a value indicating whether to use   or not.
		///</summary>
		///<value>Use  .</value>
		[JsonProperty("running")]
		[DefaultValue(false)]
		public bool Running { get { return this.defaultRunning; } set { this.defaultRunning = value; } }

		///<summary>
		/// Gets or sets the   property.
		///</summary>
		///<value>  string.</value>
		[JsonProperty("sapp_version")]
		[DefaultValue("")]
		public string SappVersion { get { return this.defaultSappVersion; } set { this.defaultSappVersion = value; } }


		///<summary>
		/// Gets or sets the   property.
		///</summary>
		///<value>  string.</value>
		[JsonProperty("servername")]
		[DefaultValue("")]
		public string ServerName { get { return this.defaultServerName; } set { this.defaultServerName = value; } }

		///<summary>
		/// Gets or sets a value indicating whether to use   or not.
		///</summary>
		///<value>Use  .</value>
		[JsonProperty("teams")]
		[DefaultValue(false)]
		public bool? Teams { get { return this.defaultTeams; } set { this.defaultTeams = value; } }

		///<summary>
		/// Gets or sets the   property.
		///</summary>
		///<value>  string.</value>
		[JsonProperty("version")]
		[DefaultValue("")]
		public string Version { get { return this.defaultVersion; } set { this.defaultVersion = value; } }
		
		public override string ToString()
		{
			return string.Format("Game: {8}\nSAPP Version {0}\nServer Name: {1}\nMap: {2} | Gametype: {3}({4})\nNoLead: {6} | Anticheat: {7}", this.SappVersion, this.ServerName, this.Map, this.Mode, this.GameType, this.Players.Count, (this.NoLead ? "ON" : "OFF"), (this.AntiCheat ? "ON" : "OFF"), (this.Running ? "Running" : "Not Running"));
		}
    
	}

	public class TeamChange
	{
		private int defaultIndex = 0;

		private int defaultTeam = 0;
		private SolidColorBrush defaultiTeam = Brushes.Black;

		[JsonProperty("index")]
		public int Index { get { return this.defaultIndex; } set { this.defaultIndex = value; } }

		///<summary>
		/// Gets or sets the opCode property.
		///</summary>
		///<value>opCode data.</value>
		[JsonProperty("opcode")]
		public Server.RemoteConsoleOpcode opCode { get; set; }

		[JsonProperty("team")]
		public int iTeam { get { return this.defaultTeam; } set { this.defaultTeam = value; } }

	}

	public class NewGame
	{
		///<summary>
		/// Default value for .
		///</summary>
		private string defaultGameType = "";

		///<summary>
		/// Default value for .
		///</summary>
		private string defaultMap = "";

		///<summary>
		/// Default value for .
		///</summary>
		private string defaultMode = "";

		///<summary>
		/// Gets or sets the  property.
		///</summary>
		///<value>  string.</value>
		[JsonProperty("gametype")]
		[DefaultValue("")]
		public string GameType { get { return this.defaultGameType; } set { this.defaultGameType = value; } }

		///<summary>
		/// Gets or sets the   property.
		///</summary>
		///<value>  string.</value>
		[JsonProperty("map")]
		[DefaultValue("")]
		public string Map { get { return this.defaultMap; } set { this.defaultMap = value; } }

		///<summary>
		/// Gets or sets the   property.
		///</summary>
		///<value>  string.</value>
		[JsonProperty("mode")]
		[DefaultValue("")]
		public string Mode { get { return this.defaultMode; } set { this.defaultMode = value; } }
		
		public ServerStat ToServerStat(ServerStat temp)
		{
			temp.GameType = this.GameType;
			temp.Map = this.Map;
			temp.Mode = this.Mode;
			return temp;
		}
		
		public override string ToString()
		{
			return string.Format("Map: {0}, Gametype: {1}({2})", this.Map, this.Mode, this.GameType);
		}
	}

	public class PlayerLeave
	{

		private int defaultIndex = 0;

		[JsonProperty("index")]
		public int Index { get { return this.defaultIndex; } set { this.defaultIndex = value; } }

	}
}
