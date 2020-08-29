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
        /// <param name="query">Query to execute as an <see cref="IAppCommand"/> if the user makes that selection.</param>
        /// <param name="sender"></param>
        /// <param name="timeOut">Time after which the prompt should disappear. 0 means the prompt will be active until closed manually.</param>
        /// <param name="executeOnAction">Option which should trigger the execution of given query.</param>
        /// <param name="affirmativeOption">Content to display on the affirmative option of the prompt. Default is "Yes".</param>
        /// <param name="negativeOption">Content to display on the negative option of the prompt. Default is "No".</param>
        void ShowPrompt(string title, string message, string query, object sender, int timeOut, ExecuteOnAction executeOnAction, string affirmativeOption = "Yes", string negativeOption = "No");
    }
}
