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
            string appRoamingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeviceManager");
            Directory.CreateDirectory(appRoamingFolder);
            string logFile = Path.Combine(appRoamingFolder, "log.txt");
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = logFile };
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;
        }
    }
}
