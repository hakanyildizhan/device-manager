﻿// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using DeviceManager.Client.TrayApp.Command;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace DeviceManager.Client.TrayApp.Service
{
    /// <summary>
    /// Prompt service that makes use of the Windows 10 Toast Notifications.
    /// </summary>
    public class ToastPromptService : IPromptService
    {
        [ClassInterface(ClassInterfaceType.None)]
        [ComSourceInterfaces(typeof(INotificationActivationCallback))]
        [Guid("EB975E8A-0662-4C64-9F19-81EB11672550"), ComVisible(true)]
        public class ToastNotificationActivator : NotificationActivator
        {
            public override void OnActivated(string invokedArgs, NotificationUserInput userInput, string appUserModelId)
            {
                // to be handled by the Toast_Activated event handler
            }
        }

        private static string _iconFilepath = Path.Combine(Utility.GetAppRoamingFolder(), "AppIcon.png");

        static ToastPromptService()
        {
            // Register AUMID and COM server (for MSIX/sparse package apps, this no-ops)
            DesktopNotificationManagerCompat.RegisterAumidAndComServer<ToastNotificationActivator>("DeviceManager.Client");

            // Register COM server and activator type
            DesktopNotificationManagerCompat.RegisterActivator<ToastNotificationActivator>();
        }

        private static Dictionary<object, ToastNotification> activeToasts = new Dictionary<object, ToastNotification>();

        public void ShowPrompt(string title, string message, string query, object sender, int timeOut, ExecuteOnAction executeOnAction)
        {
            bool iconExists = File.Exists(_iconFilepath);

            var actions = new ToastActionsCustom();

            switch (executeOnAction)
            {
                case ExecuteOnAction.Yes:
                    actions.Buttons.Add(new ToastButton("Yes", query));
                    actions.Buttons.Add(new ToastButton("No", string.Empty));
                    break;
                case ExecuteOnAction.No:
                    actions.Buttons.Add(new ToastButton("Yes", string.Empty));
                    actions.Buttons.Add(new ToastButton("No", query));
                    break;
                default:
                    break;
            }

            var audio = new ToastAudio();
            audio.Src = new Uri("ms-winsoundevent:Notification.Default");
            audio.Silent = false;
            audio.Loop = false;

            ToastContent toastContent = new ToastContent()
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

                            new AdaptiveText()
                            {
                                Text = message
                            }
                        },

                        AppLogoOverride = !iconExists ? null : new ToastGenericAppLogo()
                        {
                            Source = @"file:///" + _iconFilepath,
                            HintCrop = ToastGenericAppLogoCrop.Default
                        },
                    }
                },

                Actions = actions,
                Audio = audio
            };

            var toast = new ToastNotification(toastContent.GetXml());
            toast.ExpirationTime = DateTimeOffset.Now.AddSeconds(timeOut);
            activeToasts.Add(sender, toast);

            toast.Dismissed += Toast_Dismissed;
            toast.Activated += Toast_Activated;
            DesktopNotificationManagerCompat.CreateToastNotifier().Show(toast);
        }

        private void Toast_Activated(ToastNotification sender, object args)
        {
            var arguments = (ToastActivatedEventArgs)args;
            object subscriber = RemoveSubscriber(sender);

            if (string.IsNullOrEmpty(arguments.Arguments))
            {
                if (subscriber != null)
                {
                    EventAggregator ag = EventAggregator.Instance;
                    ag.Raise(subscriber, null, PromptResult.Dismissed);
                }
            }

            else
            {
                bool result = Task.Run(async () => await CommandFactory.Instance.GetCommand(arguments.Arguments).Execute()).Result;
                EventAggregator ag = EventAggregator.Instance;
                ag.Raise(subscriber, null, result);
                ag.Raise(subscriber, null, PromptResult.ActionPerformed);
            }
        }

        private void Toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            object subscriber = RemoveSubscriber(sender);

            if (subscriber != null)
            {
                EventAggregator ag = EventAggregator.Instance;
                ag.Raise(subscriber, null, args.Reason == ToastDismissalReason.UserCanceled ? PromptResult.Dismissed : PromptResult.Timeout);
            }
        }

        private object RemoveSubscriber(ToastNotification toast)
        {
            object subscriber = null;

            foreach (var entry in activeToasts)
            {
                if (entry.Value == toast)
                {
                    subscriber = entry.Key;
                }
            }

            if (subscriber != null)
            {
                activeToasts.Remove(subscriber);
            }

            return subscriber;
        }
    }
}
