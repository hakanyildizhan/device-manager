using Hardcodet.Wpf.TaskbarNotification;
using DeviceManager.Client.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DeviceManager.Client.TrayApp.Service
{
    public class BasicToastFeedbackService : IFeedbackService
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
