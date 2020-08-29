// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using DeviceManager.Client.TrayApp.Command;
using DeviceManager.Client.TrayApp.Service;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class PromptWindowViewModel : BaseViewModel
    {
        /// <summary>
        /// The window this view model controls.
        /// </summary>
        private Window _window;

        private string _promptMessage;
        private Timer _promptActiveTimer;

        /// <summary>
        /// The message to be shown on the dialog. Must be set by the caller.
        /// </summary>
        public string PromptMessage 
        {
            get { return _promptMessage; }
            set
            {
                if (_promptMessage != value)
                {
                    _promptMessage = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        /// <summary>
        /// Title of the popup. Must be set by the caller.
        /// </summary>
        public string PromptTitle { get; set; }

        /// <summary>
        /// Duration in seconds for the prompt to be active on screen. 0 (zero) means there is no timeout. Must be set by the caller.
        /// </summary>
        public int PromptTimeout { get; set; } = ServiceConstants.Settings.USAGE_PROMPT_DURATION_DEFAULT;

        /// <summary>
        /// Text to show on the affirmative button. Default is "Yes".
        /// </summary>
        public string YesButtonContent { get; set; } = "Yes";

        /// <summary>
        /// Text to show on the negative button. Default is "No".
        /// </summary>
        public string NoButtonContent { get; set; } = "No";

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
        /// A flag indicating that the check-in command is currently being executed.
        /// </summary>
        public bool ExecutingCommand { get; set; }

        /// <summary>
        /// Whether affirmative or negative option should run the <see cref="Query"/>.
        /// </summary>
        public ExecuteOnAction ExecuteOnAction { get; set; }

        /// <summary>
        /// The resource associated with this prompt, e.g. <see cref="DeviceItemViewModel"/>, <see cref="MainWindowViewModel"/>, etc.
        /// </summary>
        public object Owner { get; set; }

        /// <summary>
        /// The command query to run.
        /// </summary>
        public string Query { get; set; }

        public ICommand RunQueryCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public PromptWindowViewModel(Window window)
        {
            _window = window;
            RunQueryCommand = new RelayCommand(async () => await RunQuery());
            CloseCommand = new RelayCommand(async () => await Close(PromptResult.Dismissed));
            _window.ContentRendered += OnWindowShown;
        }

        private void OnWindowShown(object sender, EventArgs e)
        {
            if (this.PromptTimeout != 0)
            {
                StartAutoCloseCountdown();
            }
        }

        private async Task Close(PromptResult reminderResult)
        {
            EventAggregator ag = EventAggregator.Instance;
            ag.Raise(Owner, null, reminderResult);

            DisposeTimer();
            await App.Current.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                _window.ContentRendered -= OnWindowShown;
                _window.Close();
            }));
        }

        /// <summary>
        /// Runs the query when the user selects the option corresponding to the <see cref="ExecuteOnAction"/> variable.
        /// </summary>
        /// <returns></returns>
        private async Task RunQuery()
        {
            await RunCommandAsync(() => this.ExecutingCommand, async () =>
            {
                CommandFactory commandFactory = CommandFactory.Instance;
                var result = await commandFactory.GetCommand(Query)?.Execute();

                EventAggregator ag = EventAggregator.Instance;
                ag.Raise(Owner, null, result);

                await Close(PromptResult.ActionPerformed);
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
            await Close(PromptResult.Timeout);
        }

        /// <summary>
        /// Calculates the minimum width of the window base on length of the prompt message.
        /// </summary>
        /// <returns></returns>
        private double CalculateWindowWidth()
        {
            if (PromptMessage.Length > 45)
            {
                return 400 + 4 * (PromptMessage.Length - 45);
            }
            else
            {
                return 400;
            }
        }
    }
}
