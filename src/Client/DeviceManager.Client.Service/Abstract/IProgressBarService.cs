// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Client.Service
{
    public interface IProgressBarService
    {
        /// <summary>
        /// Shows a progress bar. Given unique ID can later be used to identify a specific progress bar and update its state.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="statusMessage"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        void Show(string title, string message, string statusMessage, string uniqueId);

        /// <summary>
        /// Updates the progress of the progress bar with given ID to the given value.
        /// </summary>
        /// <param name="progressBarId"></param>
        /// <param name="newProgress"></param>
        void IncreaseProgress(string progressBarId, int newProgress);

        /// <summary>
        /// Dismisses the progress bar.
        /// </summary>
        /// <param name="uniqueId"></param>
        void Close(string progressBarId);
    }
}
