using DeviceManager.Client.Service;
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

        public bool UsedByMe { get; set; }

        public bool ExecutingCommand { get; set; }
        public ICommand CheckoutOrReleaseCommand { get; set; }

        private IDataService _dataService => (IDataService)ServiceProvider.GetService<IDataService>();
        private IFeedbackService _feedbackService => (IFeedbackService)ServiceProvider.GetService<IFeedbackService>();

        public DeviceItemViewModel()
        {
            CheckoutOrReleaseCommand = new RelayCommand(async () => await CheckoutOrReleaseAsync());
        }

        private async Task CheckoutOrReleaseAsync()
        {
            if (IsAvailable)
            {
                await RunCommandAsync(() => this.ExecutingCommand, async () =>
                {
                    bool currentlyAvailable = await _dataService.CheckDeviceAvailabilityAsync(this.Id);
                    if (!currentlyAvailable)
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Warning, "Device unavailable", "This device is currently unavialable. Device list will be shortly updated to include latest changes.");
                    }
                    else
                    {
                        bool success = await _dataService.CheckoutDeviceAsync(Utility.GetCurrentUserName(), this.Id);
                        if (!success)
                        {
                            await _feedbackService.ShowMessageAsync(MessageType.Error, "Operation failed", "Could not check out device. Please try again later.");
                        }
                        else
                        {
                            await _feedbackService.ShowMessageAsync(MessageType.Information, "Device checked out successfully.");
                            IsAvailable = false;
                            UsedBy = Utility.GetCurrentUserName();
                        }
                    }
                });
            }
            else
            {
                await RunCommandAsync(() => this.ExecutingCommand, async () =>
                {
                    if (UsedByMe)
                    {
                        bool success = await _dataService.CheckinDeviceAsync(Utility.GetCurrentUserName(), this.Id);
                        if (!success)
                        {
                            await _feedbackService.ShowMessageAsync(MessageType.Error, "Operation failed", "Could not release device. Please try again later.");
                        }
                        else
                        {
                            await _feedbackService.ShowMessageAsync(MessageType.Information, "Device released successfully.");
                            IsAvailable = true;
                            UsedBy = null;
                        }
                    }
                });
            }
        }
    }
}
