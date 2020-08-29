// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using DeviceManager.Client.TrayApp.Command;
using DeviceManager.Client.TrayApp.Service;
using DeviceManager.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class DeviceItemViewModel : BaseViewModel, IDisposable
    {
        private IConfigurationService _configService => (IConfigurationService)ServiceProvider.GetService<IConfigurationService>();
        private ILogService<DeviceItemViewModel> _logService => (ILogService<DeviceItemViewModel>)ServiceProvider.GetService<ILogService<DeviceItemViewModel>>();
        private IPromptService _promptService => (IPromptService)ServiceProvider.GetService<IPromptService>();

        private string _deviceName;
        private string _header;
        private bool _isAvailable;
        private string _usedBy;
        private bool _usedByMe;
        private string _usedByFriendly;
        private string _connectedModuleInfo;
        private DateTime? _checkoutDate;
        private Timer _usageTimer;

        /// <summary>
        /// Whether reminder prompt associated with this device item is being shown.
        /// </summary>
        private bool _reminderActive;

        /// <summary>
        /// Event that the <see cref="MainWindowViewModel"/> subscribes to in order to be alerted whenever a state change happens on this device item.
        /// </summary>
        public event EventHandler StateChanged;

        public int Id { get; set; }

        /// <summary>
        /// Frequency at which the reminder popup will be displayed if this device item is being used by the current user.
        /// </summary>
        public int UsagePromptInterval
        {
            get
            {
                int usagePromptInterval = _configService.GetUsagePromptInterval();
#if DEBUG
                return usagePromptInterval;
#else
                if (usagePromptInterval > ServiceConstants.Settings.USAGE_PROMPT_INTERVAL_MAXIMUM ||
                    usagePromptInterval < ServiceConstants.Settings.USAGE_PROMPT_INTERVAL_MINIMUM)
                {
                    return ServiceConstants.Settings.USAGE_PROMPT_INTERVAL_DEFAULT;
                }
                else
                {
                    return usagePromptInterval;
                }
#endif
            }
        }

        /// <summary>
        /// The duration of the reminder popup to stay on the screen.
        /// </summary>
        public int UsagePromptDuration
        {
            get
            {
                int promptTimeout = _configService.GetUsagePromptDuration();
#if DEBUG
                return promptTimeout;
#else
                if (promptTimeout < ServiceConstants.Settings.USAGE_PROMPT_DURATION_MINIMUM)
                {
                    return ServiceConstants.Settings.USAGE_PROMPT_DURATION_DEFAULT;
                }
                else
                {
                    return promptTimeout;
                }
#endif
            }
        }

        /// <summary>
        /// Only the hardware item name part of <see cref="Name"/> shown on the menu item.
        /// </summary>
        public string DeviceName
        {
            get { return _deviceName; }
            set
            {
                if (_deviceName != value)
                {
                    _deviceName = value;
                    OnPropertyChanged(nameof(DeviceName));
                }
            }
        }

        /// <summary>
        /// Header that will be shown on the menu item. This will include name, hardware info as well as address information.
        /// </summary>
        public string Header
        {
            get { return _header; }
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged(nameof(Header));
                }
            }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                if (_isAvailable != value)
                {
                    _isAvailable = value;
                    OnPropertyChanged(nameof(IsAvailable));
                }
            }
        }

        public string UsedBy
        {
            get { return _usedBy; }
            set
            {
                if (_usedBy != value)
                {
                    _usedBy = value;
                    UsedByMe = _usedBy == Utility.GetCurrentUserName();
                    OnPropertyChanged(nameof(UsedBy));
                    OnPropertyChanged(nameof(UsedByMe));
                    OnPropertyChanged(nameof(Tooltip));
                }
            }
        }

        public string UsedByFriendly
        {
            get { return _usedByFriendly; }
            set
            {
                if (_usedByFriendly != value)
                {
                    _usedByFriendly = value;
                    OnPropertyChanged(nameof(UsedByFriendly));
                    OnPropertyChanged(nameof(Tooltip));
                }
            }
        }

        public string ConnectedModuleInfo
        {
            get { return _connectedModuleInfo; }
            set
            {
                if (_connectedModuleInfo != value)
                {
                    _connectedModuleInfo = value;
                    OnPropertyChanged(nameof(ConnectedModuleInfo));
                    OnPropertyChanged(nameof(Tooltip));
                }
            }
        }

        public bool UsedByMe 
        {
            get { return _usedByMe; }
            set
            {
                if (_usedByMe != value)
                {
                    _usedByMe = value;
                    NotifyStateChanged();

                    if (_usedByMe)
                    {
                        EnableTimer();
                    }
                    else
                    {
                        DisableTimer();
                    }
                }
            }
        }

        /// <summary>
        /// Date and time when this device item was checked out.
        /// If the device item is currently available, this value will be null.
        /// </summary>
        public DateTime? CheckoutDate
        {
            get { return _checkoutDate; }
            set
            {
                if (_checkoutDate != value)
                {
                    _checkoutDate = value;
                    OnPropertyChanged(nameof(Tooltip));
                }
            }
        }

        public string Tooltip { get { return this.GenerateTooltip(); } }

        public bool ExecutingCommand { get; set; }
        public ICommand CheckoutOrReleaseCommand { get; set; }

        /// <summary>
        /// Event handler to be invoked by the prompt controller. This handler manages the success state of the check-in action performed via prompt.
        /// </summary>
        public EventHandler<bool> CheckinOnReminder { get; set; }

        /// <summary>
        /// Event handler to be invoked by the prompt controller. This handler manages the closing of the check-in prompt.
        /// </summary>
        public EventHandler<PromptResult> ReminderClose { get; set; }

        public DeviceItemViewModel()
        {
            CheckoutOrReleaseCommand = new RelayCommand(async () => await CheckoutOrReleaseAsync());
            this.CheckinOnReminder += HandleCheckinOnReminder;
            this.ReminderClose += HandleReminderClose;
        }

        private async Task CheckoutOrReleaseAsync()
        {
            if (IsAvailable)
            {
                await CheckoutAsync();
            }
            else
            {
                await CheckinAsync();
            }
        }

        private async Task CheckoutAsync()
        {
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                CommandFactory commandFactory = CommandFactory.Instance;
                var success = await commandFactory.GetCommand($"CheckDeviceAvailability?deviceId={Id}&deviceName={DeviceName}")?.Execute();

                if (success)
                {
                    success = await commandFactory.GetCommand($"Checkout?userName={Utility.GetCurrentUserName()}&deviceId={Id}&deviceName={DeviceName}")?.Execute();

                    if (success)
                    {
                        IsAvailable = false;
                        UsedBy = Utility.GetCurrentUserName();
                    }
                }
            });
        }

        private async Task CheckinAsync()
        {
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                if (UsedByMe)
                {
                    CommandFactory commandFactory = CommandFactory.Instance;
                    var success = await commandFactory.GetCommand($"Checkin?userName={Utility.GetCurrentUserName()}&deviceId={Id}&deviceName={DeviceName}")?.Execute();

                    if (success)
                    {
                        IsAvailable = true;
                        UsedBy = null;
                    }
                }
            });
        }

        private void EnableTimer()
        {
            DisableTimer();
            _usageTimer = new Timer();
            _usageTimer.Elapsed += UsageTimer_Elapsed;
            _usageTimer.Interval = TimeSpan.FromSeconds(this.UsagePromptInterval).TotalMilliseconds;
            _usageTimer.Enabled = true;
            _usageTimer.Start();
        }

        private void UsageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // do nothing if the prompt is already being shown,
            // or the app is currently offline, as the user will not be able to check-in anyway
            if (_reminderActive == true || GlobalState.IsOffline)
            {
                return;
            }

            EventAggregator ag = EventAggregator.Instance;
            ag.Add(this, CheckinOnReminder);
            ag.Add(this, ReminderClose);

            _promptService.ShowPrompt(
                "Check-in reminder", 
                $"Do you still need to use {Header.Replace("\t", "  ")}?", 
                $"Checkin?deviceId={Id}&userName={Utility.GetCurrentUserName()}&deviceName={DeviceName}", 
                this,
                UsagePromptDuration,
                ExecuteOnAction.No,
                "Yes, keep using",
                "No, I'm done");

            _reminderActive = true;
        }

        internal void HandleCheckinOnReminder(object sender, bool success)
        {
            if (success)
            {
                IsAvailable = true;
                UsedBy = null;
            }

            _reminderActive = false;
        }

        /// <summary>
        /// Handles the closing of the reminder by any means, i.e. by clicking "No" to check the device item back in, clicking "Yes, keep using" button, or via timeout due to inaction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void HandleReminderClose(object sender, PromptResult promptResult)
        {
            // If the reminderActive flag is already false, this means that the device list was updated while the popup was on screen (see issue #45)
            // In that case, do not re-enable the timer & do nothing and return
            if (_reminderActive == false)
            {
                return;
            }

            _reminderActive = false;
            _logService.LogInformation($"Check-in prompt closed, reason: {promptResult}");

            // if the prompt was closed without checkin, restart the reminder timer
            if (promptResult != PromptResult.ActionPerformed)
            {
                EnableTimer();
            }
        }

        /// <summary>
        /// Called whenever there is a change on application settings that could have an impact on device items (time until reminder prompts show up etc.).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="changedSettings"></param>
        internal void HandleSettingsChanged(object sender, List<string> changedSettings)
        {
            // restart timer if reminder interval has changed & this item is already being used by current user
            if (changedSettings.Contains(ServiceConstants.Settings.USAGE_PROMPT_INTERVAL) && UsedByMe)
            {
                EnableTimer();
            }
        }

        private void DisableTimer()
        {
            if (_usageTimer != null)
            {
                _usageTimer.Elapsed -= UsageTimer_Elapsed;
                _usageTimer.Stop();
                _usageTimer.Dispose();
            }
        }

        /// <summary>
        /// Generates a tooltip for this <see cref="DeviceItemViewModel"/>, i.e.
        /// <para></para>
        /// Connected modules: [Im] [FWver], [HMI] [FWver]
        /// <para></para>
        /// Checked out by: [user]
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        private string GenerateTooltip()
        {
            StringBuilder sbTooltip = new StringBuilder();

            // Connected module info (if exists)
            if (!string.IsNullOrEmpty(this.ConnectedModuleInfo))
            {
                sbTooltip.Append("Connected modules: ");
                sbTooltip.Append(this.ConnectedModuleInfo);
            }

            if (!this.IsAvailable)
            {
                if (!string.IsNullOrEmpty(this.ConnectedModuleInfo))
                {
                    sbTooltip.AppendLine();
                }
                
                // Info about user that has the item checked out
                sbTooltip.Append("Checked out by: ");

                if (this.UsedBy == Utility.GetCurrentUserName())
                {
                    sbTooltip.Append("Me");
                }
                else if (!string.IsNullOrEmpty(this.UsedByFriendly))
                {
                    sbTooltip.Append(this.UsedByFriendly);
                }
                else
                {
                    sbTooltip.Append(this.UsedBy);
                }

                // Checkout date
                if (this.CheckoutDate != null)
                {
                    sbTooltip.Append($" (at {this.CheckoutDate.Value.ToLocalTime().ToShortDateString()} {this.CheckoutDate.Value.ToLocalTime().ToString("HH:mm")})");
                }
            }

            return sbTooltip.ToString();
        }

        /// <summary>
        /// Tears down any resources associated with this device item.
        /// </summary>
        public void Dispose()
        {
            DisableTimer();

            // unsubscribe all StateChanged event handlers
            var subscriberList = this.StateChanged?.GetInvocationList();
            if (subscriberList != null && subscriberList.Any())
            {
                foreach (var subscriber in subscriberList)
                {
                    this.StateChanged -= (subscriber as EventHandler);
                }
            }

            this.CheckinOnReminder -= HandleCheckinOnReminder;
            this.ReminderClose -= HandleReminderClose;
            EventAggregator ag = EventAggregator.Instance;
            ag.Remove(this, CheckinOnReminder);
            ag.Remove(this, ReminderClose);

            _reminderActive = false;
        }

        /// <summary>
        /// Should be invoked to declare that the check-in state for this device item has changed.
        /// </summary>
        private void NotifyStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}