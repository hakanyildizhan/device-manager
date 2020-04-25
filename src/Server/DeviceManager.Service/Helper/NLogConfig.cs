using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
