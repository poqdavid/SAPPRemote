// <copyright file="Json.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the Json class.</summary>
namespace SAPPRemote
{
	using System;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	/// <summary>
	/// Description of Json.
	/// </summary>
	public class Json
	{
		public Json()
		{
			
		}
		
		/// <summary>
		/// Given the JSON string, validates if it's a correct
		/// JSON string.
		/// </summary>
		/// <param name="json_string">JSON string to validate.</param>
		/// <returns>true or false.</returns>
		internal static bool IsValid(string json_string)
		{
			try {
				JToken.Parse(json_string);
				return true;
			} catch (JsonReaderException) {
				return false;
			}
		}
		
		
		/// <summary>
		/// Gets property from json string.
		/// </summary>
		internal static string get_str(string json_string, string key)
		{
 
			string temp = "";
			try {
				if (IsValid(json_string)) {
					
					JToken token = JObject.Parse(json_string);
					token = token[key];
					 
					temp = token.ToString();

				} else {

				}
			} catch (Exception) {

			}
			return temp;
		}
		
		/// <summary>
		/// Gets property from json int.
		/// </summary>
		internal static int get_int(string json_string, string key)
		{
			int temp = 0;
			try {
				if (IsValid(json_string)) {
					
					JToken token = JObject.Parse(json_string);
					token = token[key];
					 
					temp = int.Parse(token.ToString());

				} else {

				}
			} catch (Exception) {

			}
			return temp;
		}
		
		/// <summary>
		/// Gets property from json RemoteConsoleOpcode.
		/// </summary>
		internal static Server.RemoteConsoleOpcode get_rco(string json_string)
		{  
			Server.RemoteConsoleOpcode temp = 0;
			try {
				if (IsValid(json_string)) {
					
					JToken token = JObject.Parse(json_string);
					token = token["opcode"];
					 
					temp = (Server.RemoteConsoleOpcode)int.Parse(token.ToString());

				} else {

				}
			} catch (Exception) {

			}
			return temp;
		}
	}
}
