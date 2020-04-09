using DeviceManager.Client.Service;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DeviceManager.Client.TrayApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupApplication();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Visibility = Visibility.Hidden;
            mainWindow.Show();
        }

        private void SetupApplication()
        {
            // Set up Nlog config
            string logFile = Path.Combine(Utility.GetAppRoamingFolder(), "client.log");
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") 
            { 
                FileName = logFile,
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}"
            };
            var debugger = new NLog.Targets.DebuggerTarget("debugger");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, debugger);
            NLog.LogManager.Configuration = config;
        }
    }
}
