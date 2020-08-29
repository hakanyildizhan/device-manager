// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Windows;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class ProgressWindowViewModel : BaseViewModel
    {
        /// <summary>
        /// To store the progress bar.
        /// </summary>
        private Window _progressBar;

        private int _currentProgress;

        /// <summary>
        /// Window title.
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// Message to show above the progress bar.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Status message to show above the progress bar.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Set to true if progress cannot be determined.
        /// </summary>
        public bool IsIndeterminate { get; set; }

        /// <summary>
        /// Maximum progress. Default is 100.
        /// </summary>
        public int MaximumProgress { get; set; } = 100;

        /// <summary>
        /// Id for the progress bar.
        /// </summary>
        //public string UniqueId { get; set; }

        /// <summary>
        /// Modify this value to change progress level.
        /// </summary>
        public int CurrentProgress 
        { 
            get { return _currentProgress; }
            set
            {
                if (_currentProgress != value)
                {
                    _currentProgress = value;
                    OnPropertyChanged(nameof(CurrentProgress));
                }
            }
        }

        public ProgressWindowViewModel(Window progressBar)
        {
            _progressBar = progressBar;
        }

        public void Close()
        {
            _progressBar?.Close();
        }
    }
}
