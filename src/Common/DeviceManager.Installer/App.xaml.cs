// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace DeviceManager.Installer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Process _process;
        private ProgressWindow _progressBar;
        private string _installerPath;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length < 2 || string.IsNullOrEmpty(e.Args[0]) || string.IsNullOrEmpty(e.Args[1]) ||
                !File.Exists(e.Args[1]))
            {
                Environment.Exit(0xA0); // bad arguments
            }

            string updateVersion = e.Args[0];
            _installerPath = e.Args[1];

            _progressBar = new ProgressWindow();
            ProgressWindowViewModel viewModel = new ProgressWindowViewModel(_progressBar);
            viewModel.WindowTitle = "Device Manager";
            viewModel.Message = "Installing...";
            viewModel.StatusMessage = $"v{updateVersion}";
            viewModel.IsIndeterminate = true;
            _progressBar.DataContext = viewModel;
            _progressBar.Show();

            // Install update
            Task.Run(() =>
            {
                _process = new Process();
                _process.StartInfo = new ProcessStartInfo("msiexec", $@"/i ""{_installerPath}"" /qn SILENT=1");
                _process.StartInfo.CreateNoWindow = true;
                _process.StartInfo.UseShellExecute = true;
                _process.EnableRaisingEvents = true;
                _process.Exited += OnProcessExited;
                
                bool started = _process.Start();

                if (!started)
                {
                    App.Current.Dispatcher.Invoke(new Action(delegate ()
                    {
                        MessageBox.Show(_progressBar, $"An error occured while installing the update.\r\nPlease try installing the update manually. Installer path: {_installerPath}", "Device Manager update installation", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                    Environment.Exit(3); // probably the file was not found
                }
            });
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            if (_process.ExitCode == 0)
            {
                string executablePath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Hakan Yildizhan\DeviceManager", "ExecutablePath", string.Empty).ToString();

                if (!string.IsNullOrEmpty(executablePath) && File.Exists(executablePath))
                {
                    Task.Run(() =>
                    {
                        Process process = new Process();
                        process.StartInfo = new ProcessStartInfo(executablePath);
                        process.Start();
                    });
                }
            }
            else
            {
                App.Current.Dispatcher.Invoke(new Action(delegate ()
                {
                    MessageBox.Show(_progressBar, $"An error occured while installing the update. Error code: {_process.ExitCode}.\r\nPlease try installing the update manually. Installer path: {_installerPath}", "Device Manager update installation", MessageBoxButton.OK, MessageBoxImage.Error);
                }));
            }

            Environment.Exit(_process.ExitCode);
        }
    }
}
