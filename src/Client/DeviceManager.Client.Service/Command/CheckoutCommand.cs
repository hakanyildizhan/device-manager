// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service.Model;
using DeviceManager.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service.Command
{
    /// <summary>
    /// Command that checks out a device item.
    /// </summary>
    public class CheckoutCommand : IAppCommand
    {
        private readonly IDataService _dataService;
        private readonly IFeedbackService _feedbackService;
        private readonly ILogService _logService;
        public string Name { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public CheckoutCommand(
            IDataService dataService, 
            IFeedbackService feedbackService, 
            ILogService<CheckoutCommand> logService)
        {
            _dataService = dataService;
            _feedbackService = feedbackService;
            _logService = logService;
        }

        public async Task<bool> Execute()
        {
            string userName = Parameters["userName"];
            int deviceId = int.Parse(Parameters["deviceId"]);
            string deviceName = Parameters["deviceName"];

            var result = await _dataService.CheckoutDeviceAsync(userName, deviceId);

            if (result == ApiCallResult.Success)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Information, "Device checked out successfully.");
                _logService.LogInformation($"Check-out of {deviceName} successful");

            }
            else if (result == ApiCallResult.NotReachable)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "Server unreachable", "Cannot reach the server right now. Please try again later.");
                _logService.LogError($"Check-out of {deviceName} failed");
            }
            else
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "Operation failed", "Could not check out device. Please try again later.");
                _logService.LogError($"Check-out of {deviceName} failed");
            }

            return result == ApiCallResult.Success;
        }
    }
}
