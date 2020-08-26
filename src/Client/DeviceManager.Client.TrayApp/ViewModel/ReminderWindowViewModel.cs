// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using DeviceManager.Client.TrayApp.Command;
using DeviceManager.Client.TrayApp.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class ReminderWindowViewModel : BaseViewModel
    {
        /// <summary>
        /// The window this view model controls.
        /// </summary>
        private Window _window;

        private string _promptMessage;
        private Timer _promptActiveTimer;

        /// <summary>
        /// View model for the corresponding device item.
        /// </summary>
        public DeviceItemViewModel DeviceItem { get; set; }

        /// <summary>
        /// Event handler for handling a check-in attempt result.
        /// </summary>
        public EventHandler<bool> CheckinPerformed { get; set; }

        /// <summary>
        /// Event handler for handling the closing of the reminder.
        /// </summary>
        public EventHandler<PromptResult> ReminderClosed { get; set; }

        /// <summary>
        /// The message to be shown on the dialog. Must be set by the caller.
        /// </summary>
        public string PromptMessage 
        {
            get { return _promptMessage; }
            set
            {
                if (_promptMessage != value)
                {
                    _promptMessage = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        /// <summary>
        /// Title of the popup. Must be set by the caller.
        /// </summary>
        public string PromptTitle { get; set; }

        /// <summary>
        /// Duration in seconds for the prompt to be active on screen. Must be set by the caller
        /// </summary>
        public int PromptTimeout { get; set; } = ServiceConstants.Settings.USAGE_PROMPT_DURATION_DEFAULT;

        /// <summary>
        /// Window width.
        /// </summary>
        public double Width => CalculateWindowWidth();

        /// <summary>
        /// Window height.
        /// </summary>
        public double Height => 180;

        /// <summary>
        /// Window caption height.
        /// </summary>
        public double CaptionHeight => 30;

        /// <summary>
        /// A flag indicating that the check-in command is currently being executed.
        /// </summary>
        public bool ExecutingCommand { get; set; }

        public ICommand ReleaseCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public ReminderWindowViewModel(Window window)
        {
            _window = window;
            ReleaseCommand = new RelayCommand(async () => await ReleaseAsync());
            CloseCommand = new RelayCommand(async () => await Close(PromptResult.Dismissed));
            _window.ContentRendered += OnWindowShown;   
        }

        private void OnWindowShown(object sender, EventArgs e)
        {
            StartAutoCloseCountdown();
        }

        public void Subscribe(DeviceItemViewModel deviceItem)
        {
            DeviceItem = deviceItem;
        }

        private async Task Close(PromptResult reminderResult)
        {
            EventAggregator ag = EventAggregator.Instance;
            ag.Raise(DeviceItem, null, reminderResult);

            var subscriberList = this.CheckinPerformed?.GetInvocationList();
            if (subscriberList != null && subscriberList.Any())
            {
                foreach (var subscriber in subscriberList)
                {
                    this.CheckinPerformed -= (subscriber as EventHandler<bool>);
                }
            }
            subscriberList = this.ReminderClosed?.GetInvocationList();
            if (subscriberList != null && subscriberList.Any())
            {
                foreach (var subscriber in subscriberList)
                {
                    this.ReminderClosed -= (subscriber as EventHandler<PromptResult>);
                }
            }

            DisposeTimer();
            await App.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                _window.ContentRendered -= OnWindowShown;
                _window.Close();
            }));
        }

        /// <summary>
        /// Checks in a device when the user clicks "No" on reminder prompt.
        /// </summary>
        /// <returns></returns>
        private async Task ReleaseAsync()
        {
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                CommandFactory commandFactory = CommandFactory.Instance;
                var result = await commandFactory.GetCommand($"Checkin?userName={Utility.GetCurrentUserName()}&deviceId={DeviceItem.Id}&deviceName={DeviceItem.DeviceName}").Execute();

                EventAggregator ag = EventAggregator.Instance;
                ag.Raise(DeviceItem, null, result);

                await Close(PromptResult.ActionPerformed);
            });
        }

        private void StartAutoCloseCountdown()
        {
            _promptActiveTimer = new Timer();
            _promptActiveTimer.Elapsed += Timeout_Reached;
            _promptActiveTimer.Interval = TimeSpan.FromSeconds(this.PromptTimeout).TotalMilliseconds;
            _promptActiveTimer.Enabled = true;
            _promptActiveTimer.Start();
        }

        private void DisposeTimer()
        {
            if (_promptActiveTimer != null)
            {
                _promptActiveTimer.Stop();
                _promptActiveTimer.Dispose();
            }
        }

        /// <summary>
        /// Handles what happens when the prompt has stayed on the screen long enough.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Timeout_Reached(object sender, ElapsedEventArgs e)
        {
            DisposeTimer();
            await Close(PromptResult.Timeout);
        }

        /// <summary>
        /// Calculates the minimum width of the window base on length of the device name.
        /// </summary>
        /// <returns></returns>
        private double CalculateWindowWidth()
        {
            if (PromptMessage.Length > 45)
            {
                return 400 + 4 * (PromptMessage.Length - 45);
            }
            else
            {
                return 400;
            }
        }
    }
}
