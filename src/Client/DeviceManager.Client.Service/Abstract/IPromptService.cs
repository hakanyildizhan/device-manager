// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Client.Service
{
    public interface IPromptService
    {
        /// <summary>
        /// Shows a prompt with given title and message which will stay on screen until given timeout (in seconds) is exceeded. Given query is executed only when the option specified in <see cref="ExecuteOnAction"/> parameter is selected by the user.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="query"></param>
        /// <param name="sender"></param>
        /// <param name="timeOut"></param>
        /// <param name="executeOnAction"></param>
        void ShowPrompt(string title, string message, string query, object sender, int timeOut, ExecuteOnAction executeOnAction);
    }
}
