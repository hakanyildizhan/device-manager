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
    /// Command that checks in a device item.
    /// </summary>
    public class CheckinCommand : IAppCommand
    {
        private readonly IDataService _dataService;
        private readonly IFeedbackService _feedbackService;
        private readonly ILogService _logService;
        public string Name { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public CheckinCommand(
            IDataService dataService, 
            IFeedbackService feedbackService, 
            ILogService<CheckinCommand> logService)
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

            var result = await _dataService.CheckinDeviceAsync(userName, deviceId);

            switch (result)
            {
                case ApiCallResult.Success:
                    await _feedbackService.ShowMessageAsync(MessageType.Information, "Device is released successfully.");
                    _logService.LogInformation($"Check-in of {deviceName} succeeded via reminder");
                    break;
                case ApiCallResult.NotReachable:
                    await _feedbackService.ShowMessageAsync(MessageType.Error, "Server unreachable", "Cannot reach the server right now. Please try again later.");
                    _logService.LogError($"Check-in of {deviceName} failed via reminder. Server unreachable");
                    break;
                case ApiCallResult.Failure:
                case ApiCallResult.Unknown:
                default:
                    await _feedbackService.ShowMessageAsync(MessageType.Error, "Operation failed", "Could not release device. Please try again later.");
                    _logService.LogError($"Check-in of {deviceName} via reminder failed");
                    break;
            }

            return result == ApiCallResult.Success;
        }
    }
}
