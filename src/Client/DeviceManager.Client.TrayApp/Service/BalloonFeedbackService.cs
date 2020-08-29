// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using Hardcodet.Wpf.TaskbarNotification;
using DeviceManager.Client.Service;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace DeviceManager.Client.TrayApp.Service
{
    public class BalloonFeedbackService : IFeedbackService
    {
        public async Task ShowMessageAsync(MessageType messageType, string title, string message)
        {
            await App.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                (Application.Current.MainWindow as MainWindow).trayIcon.ShowBalloonTip(title, message, (BalloonIcon)messageType);
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        public async Task ShowMessageAsync(string title, string message)
        {
            await this.ShowMessageAsync(MessageType.None, title, message);
        }

        public async Task ShowMessageAsync(MessageType messageType, string message)
        {
            await this.ShowMessageAsync(messageType, "", message);
        }

        public async Task ShowMessageAsync(string message)
        {
            await this.ShowMessageAsync(MessageType.None, "", message);
        }
    }
}
