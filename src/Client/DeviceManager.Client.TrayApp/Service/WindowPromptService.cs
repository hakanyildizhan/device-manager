// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using DeviceManager.Client.TrayApp.ViewModel;
using DeviceManager.Client.TrayApp.Windows;
using System;

namespace DeviceManager.Client.TrayApp.Service
{
    /// <summary>
    /// Prompt service that brings up a classic popup window on screen. Should be preferred over toast notifications on older Windows environments.
    /// </summary>
    public class WindowPromptService : IPromptService
    {
        public async void ShowPrompt(string title, string message, string query, object sender, int timeOut, ExecuteOnAction confirmationOptions)
        {
            await App.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                ReminderWindow reminderWindow = new ReminderWindow();
                ReminderWindowViewModel reminderWindowViewModel = new ReminderWindowViewModel(reminderWindow);
                reminderWindowViewModel.PromptMessage = message;
                reminderWindowViewModel.PromptTitle = title;
                reminderWindowViewModel.PromptTimeout = timeOut;

                reminderWindowViewModel.Subscribe((DeviceItemViewModel)sender);
                reminderWindow.DataContext = reminderWindowViewModel;
                reminderWindow.Topmost = true;
                reminderWindow.ShowDialog();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
