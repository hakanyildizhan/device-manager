// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using DeviceManager.Installer;
using System.Collections.Generic;

namespace DeviceManager.Client.TrayApp.Service
{
    public class WindowProgressBarService : IProgressBarService
    {
        private static Dictionary<string, ProgressWindowViewModel> _activeProgressBarList = new Dictionary<string, ProgressWindowViewModel>();

        public void Show(string title, string message, string statusMessage, string uniqueId)
        {
            ProgressWindow progressBar = new ProgressWindow();
            ProgressWindowViewModel viewModel = new ProgressWindowViewModel(progressBar);
            viewModel.Message = message;
            viewModel.WindowTitle = title;
            viewModel.StatusMessage = statusMessage;
            progressBar.DataContext = viewModel;
            progressBar.Show();
            _activeProgressBarList.Add(uniqueId, viewModel);
        }

        public void IncreaseProgress(string progressBarId, int newProgress)
        {
            if (!_activeProgressBarList.ContainsKey(progressBarId))
            {
                return;
            }

            _activeProgressBarList[progressBarId].CurrentProgress = newProgress;
        }

        public void Close(string progressBarId)
        {
            if (!_activeProgressBarList.ContainsKey(progressBarId))
            {
                return;
            }

            _activeProgressBarList[progressBarId].Close();
            _activeProgressBarList.Remove(progressBarId);
        }
    }
}
