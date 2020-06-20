// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using DeviceManager.Client.Service.Model;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class ReminderWindowViewModel : BaseViewModel
    {
        private IDataService _dataService => (IDataService)ServiceProvider.GetService<IDataService>();

        /// <summary>
        /// The window this view model controls.
        /// </summary>
        private Window _window;

        private string _deviceNameOnPrompt => DeviceItem?.Name.Replace("\t", "  ");
        private Timer _promptActiveTimer;
        private int _promptTimeout;
        private DeviceItemViewModel _deviceItem;

        /// <summary>
        /// View model for the corresponding device item.
        /// </summary>
        public DeviceItemViewModel DeviceItem 
        { 
            get { return _deviceItem; }
            set
            {
                if (_deviceItem != value)
                {
                    _deviceItem = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        /// <summary>
        /// Event handler for handling a check-in attempt result.
        /// </summary>
        public EventHandler<ApiCallResult> CheckinPerformed { get; set; }

        /// <summary>
        /// Event handler for handling the closing of the reminder.
        /// </summary>
        public EventHandler<ReminderResponse> ReminderClosed { get; set; }

        /// <summary>
        /// The message to be shown on the dialog.
        /// </summary>
        public string PromptMessage => PreparePromptMessage();

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
        /// Duration in seconds for the prompt to be active on screen.
        /// </summary>
        public int PromptTimeout
        {
            get
            {
#if DEBUG
                return _promptTimeout;
#else
                if (_promptTimeout < ServiceConstants.Settings.USAGE_PROMPT_DURATION_MINIMUM)
                {
                    return ServiceConstants.Settings.USAGE_PROMPT_DURATION_DEFAULT;
                }
                else
                {
                    return _promptTimeout;
                }
#endif
            }
            set
            {
                _promptTimeout = value;
            }
        }

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
            CloseCommand = new RelayCommand(async () => await Close(ReminderResponse.KeepUsing));
            _window.ContentRendered += OnWindowShown;   
        }

        private void OnWindowShown(object sender, EventArgs e)
        {
            StartAutoCloseCountdown();
        }

        public void Subscribe(DeviceItemViewModel deviceItem)
        {
            DeviceItem = deviceItem;
            this.PromptTimeout = DeviceItem.UsagePromptDuration;
            this.CheckinPerformed += DeviceItem.HandleCheckinOnReminder;
            this.ReminderClosed += DeviceItem.HandleReminderClose;
        }

        private async Task Close(ReminderResponse reminderResult)
        {
            this.ReminderClosed?.Invoke(this, reminderResult);
            this.CheckinPerformed -= DeviceItem.HandleCheckinOnReminder;
            this.ReminderClosed -= DeviceItem.HandleReminderClose;
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
                var result = await _dataService.CheckinDeviceAsync(Utility.GetCurrentUserName(), DeviceItem.Id);
                CheckinPerformed?.Invoke(this, result);
                await Close(ReminderResponse.Checkin);
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
            await Close(ReminderResponse.Timeout);
        }

        /// <summary>
        /// Returns the prompt message to be shown to the user.
        /// </summary>
        /// <returns></returns>
        private string PreparePromptMessage()
        {
            return $"Do you still need to use\r\n{_deviceNameOnPrompt}?";
        }

        /// <summary>
        /// Calculates the minimum width of the window base on length of the device name.
        /// </summary>
        /// <returns></returns>
        private double CalculateWindowWidth()
        {
            if (_deviceNameOnPrompt.Length > 45)
            {
                return 400 + 4 * (_deviceNameOnPrompt.Length - 45);
            }
            else
            {
                return 400;
            }
        }
    }
}
