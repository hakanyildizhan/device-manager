using DeviceManager.Client.Service;
using DeviceManager.Client.Service.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private Timer _timer;
        private string _userName;
        private string _friendlyName;
        private bool _editMode;

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
            bool registerUserSuccess = await Task.Run(RegisterUser);
            if (registerUserSuccess)
            {
                await Task.Run(GetDevicesAsync);
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
                }
                else
                {
                    await _feedbackService.ShowMessageAsync(MessageType.Error, "Could not register current user.");
                    await EnableTimer(TimerEvent.RegisterUser);
                }
            });
            return success;
        }

        private async Task GetDevicesAsync()
        {
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                IEnumerable<Device> devices = await _dataService.GetDevicesAsync();

                if (devices != null && devices.Any())
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        devices.MapDeviceToViewModel().ToList().ForEach(d => Devices.Add(d));
                    });
                    
                    await _feedbackService.ShowMessageAsync(MessageType.Information, "Device list is updated.");
                }
                else
                {
                    await _feedbackService.ShowMessageAsync(MessageType.Warning, "Could not update device list.");
                    await EnableTimer(TimerEvent.GetDevices);
                }
            });
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
                        await _feedbackService.ShowMessageAsync(MessageType.Information, "User name is updated.");
                    }
                    else
                    {
                        await _feedbackService.ShowMessageAsync(MessageType.Warning, "Could not update user name.");
                    }
                }

                EditMode = false;
            });
        }

        private async Task EnableTimer(TimerEvent timerEvent)
        {
            double originalInterval = _timer != null ? _timer.Interval : 0;
            DisableTimer();
            if (originalInterval * 2 >= TimeSpan.FromSeconds(80).TotalMilliseconds)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "Cannot perform operation at this time. Please try exiting and reopening the app again.");
                return;
            }
            _timer = new Timer();
            _timer.Elapsed += (sender, e) => OnTimedEvent(sender, e, timerEvent);
            _timer.Interval = originalInterval == 0 ? TimeSpan.FromSeconds(10).TotalMilliseconds : originalInterval * 2;
            _timer.Enabled = true;
        }

        private async void OnTimedEvent(object sender, ElapsedEventArgs e, TimerEvent timerEvent)
        {
            switch (timerEvent)
            {
                case TimerEvent.RegisterUser:
                    await RegisterUser();
                    break;
                case TimerEvent.GetDevices:
                    await GetDevicesAsync();
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
