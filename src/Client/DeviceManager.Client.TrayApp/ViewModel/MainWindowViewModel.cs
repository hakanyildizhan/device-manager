// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using DeviceManager.Client.Service.Model;
using DeviceManager.Client.TrayApp.Windows;
using DeviceManager.Common;
using DeviceManager.Update;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private ILogService<MainWindowViewModel> _logService => (ILogService<MainWindowViewModel>)ServiceProvider.GetService<ILogService<MainWindowViewModel>>();
        private IRedundantConfigService _redundantConfigService => (IRedundantConfigService)ServiceProvider.GetService<IRedundantConfigService>();
        private IUpdateManager _updater => (IUpdateManager)ServiceProvider.GetService<IUpdateManager>();
        private IPromptService _prompter => (IPromptService)ServiceProvider.GetService<IPromptService>();

        private Timer _timer;
        private string _userName;
        private string _friendlyName;
        private bool _editMode;
        private int _consecutiveFailedRefreshCount = 0;
        private int _consecutiveFailedInitializeCount = 0;
        private bool _initialized;
        private AboutWindow _aboutWindow;

        /// <summary>
        /// Invoked when there is a change in any setting that has an impact on application behavior.
        /// </summary>
        public EventHandler<List<string>> SettingsChanged { get; set; }

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

        /// <summary>
        /// Indicates whether the application has finished executing initialization tasks successfully.
        /// </summary>
        public bool Initialized
        {
            get { return _initialized; }
            set
            {
                if (_initialized != value)
                {
                    _initialized = value;
                    OnPropertyChanged(nameof(Initialized));
                }
            }
        }

        
        public int RefreshInterval
        {
            get
            {
                int refreshInterval = _configService.GetRefreshInterval();
#if DEBUG
                return refreshInterval;
#else
                if (refreshInterval > ServiceConstants.Settings.REFRESH_INTERVAL_MAXIMUM ||
                    refreshInterval < ServiceConstants.Settings.REFRESH_INTERVAL_MINIMUM)
                {
                    return ServiceConstants.Settings.REFRESH_INTERVAL_DEFAULT;
                }
                else
                {
                    return refreshInterval;
                }
#endif
            }
        }

        /// <summary>
        /// Returns true if the user currently has one or more checked-out items.
        /// </summary>
        public bool CheckedoutItemsExist 
        { 
            get
            {
                foreach (var deviceGroup in Devices)
                {
                    foreach (var device in deviceGroup.DeviceList)
                    {
                        if (device.UsedByMe)
                            return true;
                    }
                }
                return false;
            }
        }

        public ICommand SetNameCommand { get; set; }
        public ICommand ReleaseAllCommand { get; set; }
        public ICommand NavigateToServerInterfaceCommand { get; set; }
        public ICommand ShowAboutWindowCommand { get; set; }
        public ICommand EnterEditModeCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public MainWindowViewModel()
        {
            Devices = new ObservableCollection<DeviceListViewModel>();
            ExitCommand = new RelayCommand(() => Exit());
            ShowAboutWindowCommand = new RelayCommand(async () => await ShowAboutWindow());
            EnterEditModeCommand = new RelayCommand(() => { EditMode = !EditMode; });
            SetNameCommand = new RelayParameterizedCommand(async (parameter) => await SetName(parameter));
            ReleaseAllCommand = new RelayCommand(async () => await ReleaseAll());
            NavigateToServerInterfaceCommand = new RelayCommand(async () => await NavigateToServerInterface());
            Task.Run(InitializeAsync);
            _logService.LogInformation($"App initialized. Version: {Utility.GetApplicationVersion()}");
        }

        /// <summary>
        /// Executes initialization tasks including registering the user, getting full device list, and settings. After successfully executing these tasks, periodic refresh will be scheduled.
        /// In case of failure, the operation will be retried until it succeeds.
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            bool success = await Task.Run(RegisterUser);
            success = success && await Task.Run(GetDevicesAsync);
            success = success && await Task.Run(GetSettingsAsync);
            if (!success)
            {
                if (++_consecutiveFailedInitializeCount == AppConstants.FAILED_OPERATION_RETRIES)
                {
                    await HandleGoingOffline();
                }

                _logService.LogError("Could not initialize, will retry");
                EnableTimer(TimerEvent.Initialize);
            }
            else
            {
                if (++_consecutiveFailedInitializeCount >= AppConstants.FAILED_OPERATION_RETRIES)
                {
                    await HandleGoingOnline();
                }

                _consecutiveFailedInitializeCount = 0;
                Initialized = true;
                GlobalState.IsOffline = false;
                _redundantConfigService.StoreServerURL(await _configService.GetServerAddressAsync());
                EnableTimer(TimerEvent.Refresh);

                await _updater.CheckUpdate();
                if (_updater.UpdateIsAvailable)
                {
                    _logService.LogInformation($"Update v{_updater.Update.Version} is available");
                    _prompter.ShowPrompt("Update available", "Do you want to install the update?", "InstallUpdate", this, 0, ExecuteOnAction.Yes);
                }
            }
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
                    _logService.LogInformation("Registered user successfully");
                }
            });
            return success;
        }

        private async Task<bool> GetDevicesAsync()
        {
            bool success = false;
            await _configService.LogRefresh();
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                IEnumerable<Device> devices = await _dataService.GetDevicesAsync();

                if (devices != null && devices.Any())
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        TeardownDeviceList();
                        devices.MapDeviceToViewModel().ToList().ForEach(d =>
                        {
                            d.DeviceList.ToList().ForEach(di => 
                            {
                                // subscribe to SettingsChanged event, so that each device item gets updated settings
                                this.SettingsChanged += di.HandleSettingsChanged;

                                // subscribe to StateChanged event, so that this view model is made aware any time a device item's state changes
                                di.StateChanged += this.DeviceItemStateChanged;
                            });
                            Devices.Add(d);
                        });
                    });
                    OnPropertyChanged(nameof(CheckedoutItemsExist));
                    success = true;
                    _logService.LogInformation("Got device list successfully");
                    await _configService.LogSuccessfulRefresh();
                }
            });
            return success;
        }

        /// <summary>
        /// Disposes all resources associated with device items that are about to be removed.
        /// </summary>
        private void TeardownDeviceList()
        {
            // unsubscribe all SettingsChanged event handlers
            var subscriberList = this.SettingsChanged?.GetInvocationList();
            if (subscriberList != null && subscriberList.Any())
            {
                foreach (var subscriber in subscriberList)
                {
                    this.SettingsChanged -= (subscriber as EventHandler<List<string>>);
                }
            }

            // dispose and clear all device items
            Devices.ToList().ForEach(d => d.DeviceList.ToList().ForEach(di => 
            {
                di.Dispose();
            }));
            Devices.Clear();
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
                    _logService.LogInformation("Got settings successfully");
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
#if DEBUG
            await _feedbackService.ShowMessageAsync("Refresh started.");
#endif
            await _configService.LogRefresh();
            bool fullUpdateRequired = false;

            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                string lastSuccessfulRefreshTime = _configService.GetLastSuccessfulRefreshTime();
                RefreshResponse refreshData = await _dataService.Refresh(lastSuccessfulRefreshTime);

                if (refreshData == null)
                {
                    _logService.LogError("Refresh failed");
                    if (++_consecutiveFailedRefreshCount == AppConstants.FAILED_OPERATION_RETRIES)
                    {
                        await HandleGoingOffline();
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
                            deviceItemViewModel.CheckoutDate = sessionInfo.CheckoutDate;
                        }
                    }
                    await _configService.LogSuccessfulRefresh();

                    // has the connection been re-established?
                    if (_consecutiveFailedRefreshCount >= AppConstants.FAILED_OPERATION_RETRIES)
                    {
                        await HandleGoingOnline();
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
                _logService.LogInformation("Initializing full device update");
                await InitiateFullHardwareListUpdate();
            }
        }

        /// <summary>
        /// Performs necessary operations to indicate to the user that the connection to the server has been established after a certain period of time.
        /// </summary>
        /// <returns></returns>
        private async Task HandleGoingOnline()
        {
            await _feedbackService.ShowMessageAsync(MessageType.Information, "Connection to the server has been established successfuly.");
            _logService.LogInformation("Connection re-established");
            GlobalState.IsOffline = false;
        }

        /// <summary>
        /// Performs necessary operations to indicate to the user that the application is having issues establishing connection to the server.
        /// </summary>
        /// <returns></returns>
        private async Task HandleGoingOffline()
        {
            await _feedbackService.ShowMessageAsync(MessageType.Error, "Cannot contact the server right now.");
            GlobalState.IsOffline = true;
        }

        /// <summary>
        /// Starts a full hardware list update, as reported from the <see cref="RefreshResponse"/>, following these steps:
        /// <para></para>
        /// 1. Stop any active refresh timer
        /// <para></para>
        /// 2. Get updated hardware list. If initial attempt fails, retry for the amount specified by <see cref="AppConstants.FAILED_OPERATION_RETRIES"/>. The operation will not be retried further, since clients must have an updated list of devices to continue using the app.
        /// <para></para>
        /// 3. Re-enable the refresh timer
        /// </summary>
        /// <returns></returns>
        private async Task InitiateFullHardwareListUpdate()
        {
            // stop timer
            DisableTimer();

            // get updated list
            bool success = await GetDevicesAsync();

            if (!success) // retry in case of failure
            {
                for (int i = 0; i < AppConstants.FAILED_OPERATION_RETRIES; i++)
                {
                    success = await RunDelayedCommandAsync(
                        () => this.ExecutingCommand,
                        () => this.RefreshInterval,
                        async () =>
                        {
                            return await GetDevicesAsync();
                        });

                    if (success) break;
                }
            }

            if (!success)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "Cannot contact the server right now. Please try exiting and reopening the app again.");
                _logService.LogError("Could not get devices after several attempts");
            }
            else
            {
                await _feedbackService.ShowMessageAsync(MessageType.Information, "Device list is updated.");
                await _configService.LogSuccessfulRefresh();
                _logService.LogInformation("Device list is updated");
                EnableTimer(TimerEvent.Refresh);
            }
        }

        /// <summary>
        /// Saves settings into config file. 
        /// If there has been a change on any interval/duration setting, restarts corresponding timers with new interval values.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private async Task SaveSettings(Dictionary<string,string> settings)
        {
            List<string> changedSettings = new List<string>();

            foreach (var setting in settings)
            {
                if (setting.Key != ServiceConstants.Settings.LAST_DEVICELIST_UPDATE &&
                    setting.Key != ServiceConstants.Settings.SERVER_VERSION)
                {
                    string previousValue = _configService.Get(setting.Key);

                    if (previousValue != setting.Value)
                    {
                        changedSettings.Add(setting.Key);
                    }
                }

                await _configService.SetAsync(setting.Key, setting.Value);

                if (changedSettings.Any())
                {
                    HandleSettingsChanged(changedSettings);
                }
            }
        }

        /// <summary>
        /// Handles the changes in settings.
        /// </summary>
        /// <param name="changedSettings"></param>
        private void HandleSettingsChanged(List<string> changedSettings)
        {
            // If refresh interval has changed, restart the corresponding timer
            if (changedSettings.Contains(ServiceConstants.Settings.REFRESH_INTERVAL))
            {
                EnableTimer(TimerEvent.Refresh);
            }

            // if device item specific intervals/durations have changed
            // invoke SettingsChanged event to alert device items
            if (changedSettings.Contains(ServiceConstants.Settings.USAGE_PROMPT_INTERVAL) ||
                changedSettings.Contains(ServiceConstants.Settings.USAGE_PROMPT_DURATION))
            {
                this.SettingsChanged.Invoke(this, changedSettings);
            }

        }

        private async Task SetName(object parameter)
        {
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                if ((parameter is string) && !string.IsNullOrEmpty((string)parameter))
                {
                    var result = await _dataService.SetUsernameAsync(this.UserName, (string)parameter);
                    if (result == ApiCallResult.Success)
                    {
                        FriendlyName = (string)parameter;
                        _logService.LogInformation("Name set successfully");
                    }
                    else if (result == ApiCallResult.NotReachable)
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Warning, "Server unreachable, could not update user name.");
                        _logService.LogError("Could not set name. Server unreachable");
                    }
                    else
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Warning, "Could not update user name.");
                        _logService.LogError("Could not set name");
                    }
                }

                EditMode = false;
            });
        }

        /// <summary>
        /// Schedules given task to run periodically. Disables any schedules that were already running.
        /// </summary>
        /// <param name="timerEvent"></param>
        private void EnableTimer(TimerEvent timerEvent)
        {
            DisableTimer();
            _timer = new Timer();
            _timer.Elapsed += (sender, e) => OnTimedEvent(sender, e, timerEvent);
            _timer.Interval = TimeSpan.FromSeconds(this.RefreshInterval).TotalMilliseconds;
            _timer.Enabled = true;
            _timer.Start();
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
                case TimerEvent.Initialize:
                    await InitializeAsync();
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
            _logService.LogInformation("Shutting down");
            DisableTimer();
            (System.Windows.Application.Current.MainWindow as MainWindow).trayIcon.Visibility = System.Windows.Visibility.Hidden;
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// Shows the About window.
        /// </summary>
        /// <returns></returns>
        private async Task ShowAboutWindow()
        {
            if (_aboutWindow != null) // do not show the window twice
            {
                return;
            }

            await App.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                _aboutWindow = new AboutWindow();
                _aboutWindow.Closed += AboutWindow_Closed;
                AboutWindowViewModel aboutWindowViewModel = new AboutWindowViewModel(_aboutWindow);
                _aboutWindow.DataContext = aboutWindowViewModel;
                _aboutWindow.ShowDialog();
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        /// <summary>
        /// Opens a browser window and navigates to the server web interface URL.
        /// </summary>
        /// <returns></returns>
        private async Task NavigateToServerInterface()
        {
            var serverAddress = _configService.GetServerAddressAsync();
            System.Diagnostics.Process.Start(await serverAddress);
        }

        /// <summary>
        /// Releases all device items that are currently checked-out to the current user.
        /// </summary>
        /// <returns></returns>
        private async Task ReleaseAll()
        {
            var result = await _dataService.CheckinAllDevicesAsync(Utility.GetCurrentUserName());

            if (result == ApiCallResult.Success)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Information, "All items are released successfully.");
                _logService.LogInformation($"All items released successfully");

                // update status of previously checked out devices
                foreach (var deviceGroup in Devices)
                {
                    foreach (var device in deviceGroup.DeviceList)
                    {
                        device.IsAvailable = true;
                        device.UsedBy = null;
                    }
                }
            }
            else if (result == ApiCallResult.NotReachable)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "Server unreachable", "Cannot reach the server right now. Please try again later.");
                _logService.LogError($"Check-in of all items failed. Server unreachable");
            }
            else
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "Operation failed", "Could not release devices. Please try again later.");
                _logService.LogError($"Check-in of all items failed");
            }
        }

        private void AboutWindow_Closed(object sender, EventArgs e)
        {
            _aboutWindow = null;
        }

        /// <summary>
        /// Called automatically whenever a device item's check-in state changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceItemStateChanged(object sender, System.EventArgs e)
        {
            OnPropertyChanged(nameof(CheckedoutItemsExist));
        }
    }
}
