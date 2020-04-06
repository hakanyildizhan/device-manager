using DeviceManager.Client.Service;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DeviceManager.Client.TrayApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Mutex mutex = new Mutex(true, "{D945067F-CF69-4850-8E06-3DDA6625BA76}");

        protected override void OnStartup(StartupEventArgs e)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                base.OnStartup(e);
                SetupApplication();

                MainWindow mainWindow = new MainWindow();
                mainWindow.Visibility = Visibility.Hidden;
                mainWindow.Show();
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("The application is already running.");
            }
        }

        private void SetupApplication()
        {
            // Set up Nlog config
            string logFile = Path.Combine(Utility.GetAppRoamingFolder(), "client.log");
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = logFile };
            var debugger = new NLog.Targets.DebuggerTarget("debugger");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, debugger);
            NLog.LogManager.Configuration = config;
        }
    }
}
