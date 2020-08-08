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

// <copyright file="Json.cs" company="POQDavid">
// Copyright (c) POQDavid. All rights reserved.
// </copyright>
// <author>POQDavid</author>
// <summary>This is the Json class.</summary>
namespace SAPPRemote
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;

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
            try
            {
                JToken.Parse(json_string);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets property from json string.
        /// </summary>
        internal static string Get_str(string json_string, string key)
        {
            string temp = "";
            try
            {
                if (IsValid(json_string))
                {
                    JToken token = JObject.Parse(json_string);
                    token = token[key];

                    temp = token.ToString();
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
            return temp;
        }

        /// <summary>
        /// Gets property from json int.
        /// </summary>
        internal static int Get_int(string json_string, string key)
        {
            int temp = 0;
            try
            {
                if (IsValid(json_string))
                {
                    JToken token = JObject.Parse(json_string);
                    token = token[key];

                    temp = int.Parse(token.ToString());
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
            return temp;
        }

        /// <summary>
        /// Gets property from json RemoteConsoleOpcode.
        /// </summary>
        internal static Server.RemoteConsoleOpcode Get_rco(string json_string)
        {
            Server.RemoteConsoleOpcode temp = 0;
            try
            {
                if (IsValid(json_string))
                {
                    JToken token = JObject.Parse(json_string);
                    token = token["opcode"];

                    temp = (Server.RemoteConsoleOpcode)int.Parse(token.ToString());
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
            return temp;
        }

        public static string GenerateString(object jsonobj)
        {
            var s = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace // without this, you end up with duplicates.
            };

            return JsonConvert.SerializeObject(jsonobj, Formatting.None, s);
        }

        public static PlayerData GetPlayerData(string json_string)
        {
            PlayerData temp = new PlayerData();
            try
            {
                //string json_string = File.ReadAllText(SAPPRemote.MainWindow.SettingPath);
                if (Json.IsValid(json_string))
                {
                    var s = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Replace // without this, you end up with duplicates.
                    };

                    temp = JsonConvert.DeserializeObject<PlayerData>(json_string, s);
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
            return temp;
        }

        public static PlayersStat GetPlayersStat(string json_string)
        {
            PlayersStat temp = new PlayersStat();
            try
            {
                //string json_string = File.ReadAllText(SAPPRemote.MainWindow.SettingPath);
                if (Json.IsValid(json_string))
                {
                    var s = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Replace // without this, you end up with duplicates.
                    };

                    temp = JsonConvert.DeserializeObject<PlayersStat>(json_string, s);
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
            return temp;
        }

        public static ServerStat GetServerStat(string json_string)
        {
            ServerStat temp = new ServerStat();
            try
            {
                //string json_string = File.ReadAllText(SAPPRemote.MainWindow.SettingPath);
                if (Json.IsValid(json_string))
                {
                    var s = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Replace // without this, you end up with duplicates.
                    };

                    temp = JsonConvert.DeserializeObject<ServerStat>(json_string, s);
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
            return temp;
        }

        public static TeamChange GetTeamChange(string json_string)
        {
            TeamChange temp = new TeamChange();
            try
            {
                //string json_string = File.ReadAllText(SAPPRemote.MainWindow.SettingPath);
                if (Json.IsValid(json_string))
                {
                    var s = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Replace // without this, you end up with duplicates.
                    };

                    temp = JsonConvert.DeserializeObject<TeamChange>(json_string, s);
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
            return temp;
        }

        public static NewGame GetNewGame(string json_string)
        {
            NewGame temp = new NewGame();
            try
            {
                //string json_string = File.ReadAllText(SAPPRemote.MainWindow.SettingPath);
                if (Json.IsValid(json_string))
                {
                    var s = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Replace // without this, you end up with duplicates.
                    };

                    temp = JsonConvert.DeserializeObject<NewGame>(json_string, s);
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
            return temp;
        }

        public static PlayerLeave GetPlayerLeave(string json_string)
        {
            PlayerLeave temp = new PlayerLeave();
            try
            {
                //string json_string = File.ReadAllText(SAPPRemote.MainWindow.SettingPath);
                if (Json.IsValid(json_string))
                {
                    var s = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ObjectCreationHandling = ObjectCreationHandling.Replace // without this, you end up with duplicates.
                    };

                    temp = JsonConvert.DeserializeObject<PlayerLeave>(json_string, s);
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
            return temp;
        }
    }
}