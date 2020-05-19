using DeviceManager.Client.Service;
using DeviceManager.Client.Service.Model;
using DeviceManager.Client.TrayApp.Windows;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class DeviceItemViewModel : BaseViewModel, IDisposable
    {
        private IDataService _dataService => (IDataService)ServiceProvider.GetService<IDataService>();
        private IFeedbackService _feedbackService => (IFeedbackService)ServiceProvider.GetService<IFeedbackService>();
        private ILogService<DeviceItemViewModel> _logService => (ILogService<DeviceItemViewModel>)ServiceProvider.GetService<ILogService<DeviceItemViewModel>>();
        private IConfigurationService _configService => (IConfigurationService)ServiceProvider.GetService<IConfigurationService>();

        private string _name;
        private bool _isAvailable;
        private string _usedBy;
        private bool _usedByMe;
        private string _usedByFriendly;
        private string _connectedModuleInfo;
        private DateTime? _checkoutDate;
        private Timer _usageTimer;

        /// <summary>
        /// Reminder prompt window associated with this device item.
        /// </summary>
        private Window _reminderWindow;

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
        public int UsagePromptDuration => _configService.GetUsagePromptDuration();

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
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

        public DeviceItemViewModel()
        {
            CheckoutOrReleaseCommand = new RelayCommand(async () => await CheckoutOrReleaseAsync());
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
                var result = await _dataService.CheckDeviceAvailabilityAsync(this.Id);
                if (result == ApiCallResult.Success)
                {
                    result = await _dataService.CheckoutDeviceAsync(Utility.GetCurrentUserName(), this.Id);
                    if (result == ApiCallResult.Success)
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Information, "Device checked out successfully.");
                        _logService.LogInformation("Check-out successful");
                        IsAvailable = false;
                        UsedBy = Utility.GetCurrentUserName();
                    }
                    else if (result == ApiCallResult.NotReachable)
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Error, "Server unreachable", "Cannot reach the server right now. Please try again later.");
                        _logService.LogError("Check-out failed");
                    }
                    else
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Error, "Operation failed", "Could not check out device. Please try again later.");
                        _logService.LogError("Check-out failed");
                    }
                }
                else if (result == ApiCallResult.NotReachable)
                {
                    await _feedbackService.ShowMessageAsync(MessageType.Error, "Server unreachable", "Cannot reach the server right now. Please try again later.");
                    _logService.LogError("Could not check device availability. Server unreachable");
                }
                else
                {
                    await _feedbackService.ShowMessageAsync(MessageType.Warning, "Device unavailable", "This device is currently unavailable. Device status will be updated shortly.");
                    _logService.LogError("Could not check device availability");
                    // TODO: get latest status for this specific device
                }
            });
        }

        private async Task CheckinAsync()
        {
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                if (UsedByMe)
                {
                    var result = await _dataService.CheckinDeviceAsync(Utility.GetCurrentUserName(), this.Id);
                    if (result == ApiCallResult.Success)
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Information, "Device is released successfully.");
                        _logService.LogInformation("Check-in succeeded");
                        IsAvailable = true;
                        UsedBy = null;
                    }
                    else if (result == ApiCallResult.NotReachable)
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Error, "Server unreachable", "Cannot reach the server right now. Please try again later.");
                        _logService.LogError("Check-in failed. Server unreachable");
                    }
                    else
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Error, "Operation failed", "Could not release device. Please try again later.");
                        _logService.LogError("Check-in failed");
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

        private async void UsageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_reminderWindow != null)
            {
                return;
            }

            await App.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                _reminderWindow = new ReminderWindow();
                ReminderWindowViewModel reminderWindowViewModel = new ReminderWindowViewModel(_reminderWindow);
                reminderWindowViewModel.Subscribe(this);
                _reminderWindow.DataContext = reminderWindowViewModel;
                _reminderWindow.Topmost = true;
                _reminderWindow.ShowDialog();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        internal async void HandleCheckinOnReminder(object sender, ApiCallResult e)
        {
            switch (e)
            {
                case ApiCallResult.Success:
                    await _feedbackService.ShowMessageAsync(MessageType.Information, "Device is released successfully.");
                    _logService.LogInformation("Check-in succeeded");
                    IsAvailable = true;
                    UsedBy = null;
                    break;
                case ApiCallResult.NotReachable:
                    await _feedbackService.ShowMessageAsync(MessageType.Error, "Server unreachable", "Cannot reach the server right now. Please try again later.");
                    _logService.LogError("Check-in failed. Server unreachable");
                    break;
                case ApiCallResult.Failure:
                case ApiCallResult.Unknown:
                default:
                    await _feedbackService.ShowMessageAsync(MessageType.Error, "Operation failed", "Could not release device. Please try again later.");
                    _logService.LogError("Check-in failed");
                    break;
            }

            _reminderWindow = null;
        }

        /// <summary>
        /// Handles the closing of the reminder by any means, i.e. by clicking "No" to check the device item back in, clicking "Yes, keep using" button, or via timeout due to inaction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void HandleReminderClose(object sender, ReminderResponse reminderResponse)
        {
            _reminderWindow = null;

            // if reminder window was closed without checkin, restart the reminder popup timer
            if (reminderResponse != ReminderResponse.Checkin)
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
            _reminderWindow = null;
        }
    }
}