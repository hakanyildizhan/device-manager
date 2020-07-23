// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.ComponentModel;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    /// <summary>
    /// This view model is incorporated into <see cref="BaseViewModel"/>, in order for all view models to share static, global application state.
    /// </summary>
    public class GlobalStateViewModel : INotifyPropertyChanged
    {
        private static bool _isOffline;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Indicates whether a certain number of previous initialization/refresh attemps have failed.
        /// </summary>
        public bool IsOffline
        {
            get { return _isOffline; }
            set
            {
                if (_isOffline != value)
                {
                    _isOffline = value;
                    OnPropertyChanged(nameof(IsOffline));
                }
            }
        }

        /// <summary>
        /// Call this to fire a <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
