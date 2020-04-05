using DeviceManager.Client.Service;
using DeviceManager.Client.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class DeviceItemViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tooltip { get; set; }

        private bool _isAvailable;
        private string _usedBy;
        private string _usedByFriendly;

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

        public bool ExecutingCommand { get; set; }
        public ICommand CheckoutOrReleaseCommand { get; set; }

        private IDataService _dataService => (IDataService)ServiceProvider.GetService<IDataService>();
        private IFeedbackService _feedbackService => (IFeedbackService)ServiceProvider.GetService<IFeedbackService>();
        private ILogService<DeviceItemViewModel> _logService => (ILogService<DeviceItemViewModel>)ServiceProvider.GetService<ILogService<DeviceItemViewModel>>();

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
    }
}
