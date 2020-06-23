// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using NLog;
using System.IO;
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
