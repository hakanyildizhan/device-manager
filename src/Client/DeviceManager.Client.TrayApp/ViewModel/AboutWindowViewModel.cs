// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class AboutWindowViewModel : BaseViewModel
    {
        /// <summary>
        /// The window this view model controls.
        /// </summary>
        private Window _window;

        public string ApplicationVersion
        {
            get
            {
                return GetApplicationVersion();
            }
        }

        public ICommand CloseCommand { get; set; }

        public AboutWindowViewModel(Window window)
        {
            _window = window;
            CloseCommand = new RelayCommand(async () => await Close());
        }

        private async Task Close()
        {
            await App.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                _window?.Close();
                _window = null;
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        private string GetApplicationVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"v{version.Major}.{version.Minor}.{version.Revision}";
        }
    }
}
