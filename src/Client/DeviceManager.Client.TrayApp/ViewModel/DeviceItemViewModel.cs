﻿using DeviceManager.Client.Service;
using DeviceManager.Client.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class DeviceItemViewModel : BaseViewModel
    {
        private IDataService _dataService => (IDataService)ServiceProvider.GetService<IDataService>();
        private IFeedbackService _feedbackService => (IFeedbackService)ServiceProvider.GetService<IFeedbackService>();
        private ILogService<DeviceItemViewModel> _logService => (ILogService<DeviceItemViewModel>)ServiceProvider.GetService<ILogService<DeviceItemViewModel>>();
        private IConfigurationService _configService => (IConfigurationService)ServiceProvider.GetService<IConfigurationService>();

        public int Id { get; set; }
        public string Name { get; set; }
        public int UsagePromptInterval => _configService.GetUsagePromptInterval();

        private string _tooltip;
        private bool _isAvailable;
        private string _usedBy;
        private string _usedByFriendly;
        private Timer _usageTimer;
        private bool _promptActive = false;

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
                }
            }
        }

        public bool UsedByMe { get; set; }

        public string Tooltip
        {
            get { return _tooltip; }
            set
            {
                if (_tooltip != value)
                {
                    _tooltip = value;
                    OnPropertyChanged(nameof(Tooltip));
                }
            }
        }

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
                        EnableTimer();
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
                        DisableTimer();
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
            if (!_promptActive)
            {
                _promptActive = true;
                MessageBoxResult result = MessageBox.Show($"Click \"Yes\" to keep using\r\n\r\n{this.Name},\r\n\r\nor \"No\" to release it.", "Device Manager", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // do nothing
                        _promptActive = false;
                        break;
                    case MessageBoxResult.No:
                        _promptActive = false;
                        await CheckinAsync();
                        break;
                }
            }
        }

        private void DisableTimer()
        {
            if (_usageTimer != null)
            {
                _usageTimer.Stop();
                _usageTimer.Dispose();
            }
        }
    }
}
