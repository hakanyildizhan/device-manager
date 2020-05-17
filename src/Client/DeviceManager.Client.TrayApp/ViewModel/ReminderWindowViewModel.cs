using DeviceManager.Client.Service;
using DeviceManager.Client.Service.Model;
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
    public class ReminderWindowViewModel : BaseViewModel
    {
        private IDataService _dataService => (IDataService)ServiceProvider.GetService<IDataService>();

        /// <summary>
        /// The window this view model controls.
        /// </summary>
        private Window _window;

        private string _deviceNameOnPrompt => DeviceItem?.Name.Replace("\t", "  ");

        private Timer _promptTimeout;

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
        public int PromptTimeout => 60;

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
            _promptTimeout = new Timer();
            _promptTimeout.Elapsed += Timeout_Reached;
            _promptTimeout.Interval = TimeSpan.FromSeconds(this.PromptTimeout).TotalMilliseconds;
            _promptTimeout.Enabled = true;
            _promptTimeout.Start();
        }

        private void DisposeTimer()
        {
            if (_promptTimeout != null)
            {
                _promptTimeout.Stop();
                _promptTimeout.Dispose();
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
