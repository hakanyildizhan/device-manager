// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.IO;
using System.Linq;
using Windows.UI.Notifications;

namespace DeviceManager.Client.TrayApp.Service
{
    public class ToastProgressBarService : IProgressBarService
    {
        private static string _iconFilepath = Path.Combine(Utility.GetAppRoamingFolder(), "AppIcon.png");

        public void Show(string title, string message, string statusMessage, string uniqueId)
        {
            bool iconExists = File.Exists(_iconFilepath);

            var toastContent = new ToastContent()
            {
                Scenario = ToastScenario.Reminder,

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },

                            new AdaptiveProgressBar()
                            {
                                Title = message,
                                Value = new BindableProgressBarValue("progressValue"),
                                Status = new BindableString("progressStatus")
                            }
                        },

                        AppLogoOverride = !iconExists ? null : new ToastGenericAppLogo()
                        {
                            Source = @"file:///" + _iconFilepath,
                            HintCrop = ToastGenericAppLogoCrop.Default
                        },
                    }
                }
            };

            var toast = new ToastNotification(toastContent.GetXml());
            toast.Tag = uniqueId;
            toast.Data = new NotificationData();
            toast.Data.Values["progressValue"] = "0";
            toast.Data.Values["progressStatus"] = "Downloading update..";
            toast.Data.SequenceNumber = 0;
            DesktopNotificationManagerCompat.CreateToastNotifier().Show(toast);
        }

        public void IncreaseProgress(string progressBarId, int newProgress)
        {
            var data = new NotificationData
            {
                SequenceNumber = 0
            };

            data.Values["progressValue"] = newProgress.ToString();
            DesktopNotificationManagerCompat.CreateToastNotifier().Update(data, progressBarId);
        }

        public void Close(string progressBarId)
        {
            var progressBar = DesktopNotificationManagerCompat.History.GetHistory().FirstOrDefault(n => n.Tag == progressBarId);

            if (progressBar != null)
            {
                DesktopNotificationManagerCompat.CreateToastNotifier().Show(progressBar);
                DesktopNotificationManagerCompat.CreateToastNotifier().Hide(progressBar);
            }
        }
    }
}
