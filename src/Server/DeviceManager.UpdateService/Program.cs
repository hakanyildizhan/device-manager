using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.UpdateService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] servicesToRun = new ServiceBase[]
            {
                new Updater()
            };
            
            SetupApplication();
            ServiceBase.Run(servicesToRun);
        }

        static void SetupApplication()
        {
            // Set up Nlog config
            // check if the DeviceManager App Pool Roaming folder exists
            string logFile = "service.log";
            string devMgrServerFolder = @"C:\Users\DeviceManager\AppData\Roaming\DeviceManager";
            if (Directory.Exists(devMgrServerFolder))
            {
                logFile = Path.Combine(devMgrServerFolder, logFile);
            }

            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = logFile,
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}",
                ArchiveAboveSize = 5242880, // 5 MB
                MaxArchiveFiles = 2,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.DateAndSequence
            };
            var debugger = new NLog.Targets.DebuggerTarget("debugger");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, debugger);
            NLog.LogManager.Configuration = config;
        }
    }
}
