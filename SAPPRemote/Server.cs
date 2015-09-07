// <copyright file="Server.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the Server class.</summary>
namespace SAPPRemote
{
	using System;
	
	/// <summary>
	/// Description of Server.
	/// </summary>
	public class Server
	{
		internal enum RemoteConsoleOpcode
		{
			RC_LOGIN = 1,
			RC_QUERY = 2,
			RC_CIN = 3,
			RC_COUT = 4,
			RC_CHAT = 5,
			RC_PJOIN = 6,
			RC_PLEAVE = 7,
			RC_TEAMCHANGE = 8,
			RC_NEWGAME = 9
		}
		
		public Server()
		{
		}
	}
}
