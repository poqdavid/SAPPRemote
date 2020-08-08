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

using System;

namespace SAPPRemote
{
    public class SAPPRemoteLogger
    {
        private readonly NLog.Logger logger;

        public SAPPRemoteLogger()
        {
            logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public void Trace(Exception ex, string message)
        {
            logger.Trace(ex, message + ":\n" + ex.StackTrace);
        }

        public void Trace(string message)
        {
            logger.Trace(message);
        }

        public void Error(Exception ex, string message)
        {
            logger.Error(ex, message + ":\n" + ex.StackTrace);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Info(Exception ex, string message)
        {
            logger.Info(ex, message);
        }

        public void Info(string message)
        {
            logger.Info(message);
        }
    }
}