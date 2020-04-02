using DeviceManager.Client.Service;
using DeviceManager.Client.Service.Model;
using StarterDeviceManager.Client.TrayApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private IDataService _dataService => (IDataService)ServiceProvider.GetService<IDataService>();
        private IFeedbackService _feedbackService => (IFeedbackService)ServiceProvider.GetService<IFeedbackService>();
        private IConfigurationService _configService => (IConfigurationService)ServiceProvider.GetService<IConfigurationService>();
        private Timer _timer;
        private string _userName;
        private string _friendlyName;
        private bool _editMode;
        private int _consecutiveFailedRefreshCount = 0;

        public bool ExecutingCommand { get; set; }
        public ObservableCollection<DeviceListViewModel> Devices { get; set; }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }
        public ICommand ExitCommand { get; set; }

        public string FriendlyName
        {
            get { return _friendlyName; }
            set
            {
                if (_friendlyName != value)
                {
                    _friendlyName = value;
                    OnPropertyChanged(nameof(FriendlyName));
                }
            }
        }

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                if (_editMode != value)
                {
                    _editMode = value;
                    OnPropertyChanged(nameof(EditMode));
                }
            }
        }

        public int RefreshInterval => _configService.GetRefreshInterval();

        public ICommand SetNameCommand { get; set; }
        public ICommand EnterEditModeCommand { get; set; }

        public MainWindowViewModel()
        {
            Devices = new ObservableCollection<DeviceListViewModel>();
            ExitCommand = new RelayCommand(() => Exit());
            EnterEditModeCommand = new RelayCommand(() => { EditMode = !EditMode; });
            SetNameCommand = new RelayParameterizedCommand(async (parameter) => await SetName(parameter));
            Task.Run(Initialize);
        }

        private async Task Initialize()
        {
            bool success = await Task.Run(RegisterUser);
            success = success && await Task.Run(GetDevicesAsync);
            success = success && await Task.Run(GetSettingsAsync);
            if (!success)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "Cannot contact the server.");
                return;
            }

            EnableTimer(TimerEvent.Refresh);
        }

        private async Task<bool> RegisterUser()
        {
            bool success = false;
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                UserInfo user = await _dataService.RegisterUserAsync(Utility.GetCurrentUserName());
                if (user != null)
                {
                    UserName = user.UserName;
                    FriendlyName = user.FriendlyName;
                    success = true;
                }
            });
            return success;
        }

        private async Task<bool> GetDevicesAsync()
        {
            bool success = false;
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                IEnumerable<Device> devices = await _dataService.GetDevicesAsync();

                if (devices != null && devices.Any())
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        devices.MapDeviceToViewModel().ToList().ForEach(d => Devices.Add(d));
                    });
                    success = true;
                }
            });
            return success;
        }

        private async Task<bool> GetSettingsAsync()
        {
            bool success = false;
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                Dictionary<string,string> settings = await _dataService.GetSettingsAsync();

                if (settings != null && settings.Any())
                {
                    await SaveSettings(settings);
                    success = true;
                }
            });
            return success;
        }

        /// <summary>
        /// Performs certain actions on each refresh, including:
        /// upserting app settings,
        /// updating device availability status,
        /// updating entire device list.
        /// </summary>
        /// <returns></returns>
        private async Task RefreshAsync()
        {
            Debug.WriteLine($"Refresh on {DateTime.Now.ToString()}");
            await _configService.LogRefresh();

            bool fullUpdateRequired = false;

            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                RefreshData refreshData = await _dataService.Refresh();

                if (refreshData == null)
                {
                    Debug.WriteLine($"Refresh failed.");
                    if (++_consecutiveFailedRefreshCount == AppConstants.FAILED_OPERATION_RETRIES)
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Error, "Cannot contact the server right now. Please try exiting and reopening the app again.");
                    }
                }
                else if (!refreshData.FullUpdateRequired)
                {
                    //upsert settings
                    await SaveSettings(refreshData.Settings);

                    foreach (var deviceListViewModel in Devices)
                    {
                        foreach (var deviceItemViewModel in deviceListViewModel.DeviceList)
                        {
                            DeviceSessionInfo sessionInfo = refreshData.DeviceSessionInfo.Single(s => s.DeviceId == deviceItemViewModel.Id);
                            deviceItemViewModel.IsAvailable = sessionInfo.IsAvailable;
                            deviceItemViewModel.UsedBy = sessionInfo.UsedBy;
                            deviceItemViewModel.UsedByFriendly = sessionInfo.UsedByFriendly;
                        }
                    }
                    await _configService.LogSuccessfulRefresh();

                    // has the connection been re-established?
                    if (_consecutiveFailedRefreshCount >= AppConstants.FAILED_OPERATION_RETRIES)
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Information, "Connection to the server has been re-established successfuly.");
                    }

                    _consecutiveFailedRefreshCount = 0;
                }
                else // full device list update is required
                {
                    fullUpdateRequired = true;
                }
            });

            if (fullUpdateRequired)
            {
                // stop the timer, get updated list, then Enable timer again

                DisableTimer();
                bool success = await GetDevicesAsync();
                if (!success)
                {
                    // retry action
                    for (int i = 0; i < AppConstants.FAILED_OPERATION_RETRIES; i++)
                    {
                        success = await RunDelayedCommandAsync(() => this.RefreshInterval, async () =>
                        {
                            return await GetDevicesAsync();
                        });

                        if (success) break;
                    }
                    if (!success)
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Error, "Cannot contact the server right now. Please try exiting and reopening the app again.");
                        return;
                    }
                    else
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Information, "Device list is updated.");
                        await _configService.LogSuccessfulRefresh();
                        EnableTimer(TimerEvent.Refresh);
                    }
                }
                else
                {
                    await _feedbackService.ShowMessageAsync(MessageType.Information, "Device list is updated.");
                    await _configService.LogSuccessfulRefresh();
                    EnableTimer(TimerEvent.Refresh);
                }
            }
        }

        private async Task SaveSettings(Dictionary<string,string> settings)
        {
            foreach (var setting in settings)
            {
                await _configService.SetAsync(setting.Key, setting.Value);
            }
        }

        private async Task SetName(object parameter)
        {
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                if ((parameter is string) && !string.IsNullOrEmpty((string)parameter))
                {
                    bool result = await _dataService.SetUsernameAsync(this.UserName, (string)parameter);
                    if (result)
                    {
                        FriendlyName = (string)parameter;
                    }
                    else
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Warning, "Could not update user name.");
                    }
                }

                EditMode = false;
            });
        }

        private void EnableTimer(TimerEvent timerEvent)
        {
            DisableTimer();
            _timer = new Timer();
            _timer.Elapsed += (sender, e) => OnTimedEvent(sender, e, timerEvent);
            _timer.Interval = TimeSpan.FromSeconds(this.RefreshInterval).TotalMilliseconds;
            _timer.Enabled = true;
            _timer.Start();
            Debug.WriteLine("Timer started.");
        }

        private async void OnTimedEvent(object sender, ElapsedEventArgs e, TimerEvent timerEvent)
        {
            // update interval if it has changed
            int currentInterval = this.RefreshInterval;
            if (_timer.Interval != TimeSpan.FromSeconds(currentInterval).TotalMilliseconds)
            {
                _timer.Interval = TimeSpan.FromSeconds(currentInterval).TotalMilliseconds;
            }
            
            switch (timerEvent)
            {
                case TimerEvent.RegisterUser:
                    await RegisterUser();
                    break;
                case TimerEvent.GetDevices:
                    await GetDevicesAsync();
                    break;
                case TimerEvent.Refresh:
                    await RefreshAsync();
                    break;
                default:
                    // Not implemented
                    DisableTimer();
                    break;
            }
        }

        private void DisableTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        private void Exit()
        {
            DisableTimer();
            (System.Windows.Application.Current.MainWindow as MainWindow).trayIcon.Visibility = System.Windows.Visibility.Hidden;
            System.Windows.Application.Current.Shutdown();
        }
    }
}
