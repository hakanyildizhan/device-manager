// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using NLog;
using System.IO;

namespace DeviceManager.Service
{
    public static class NLogConfig
    {
        public static void Initialize()
        {
            var config = new NLog.Config.LoggingConfiguration();
            string logfilePath = Path.Combine(Utility.GetAppRoamingFolder(), "server.log");
            
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = logfilePath,
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}"
            };

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;
        }
    }
}
