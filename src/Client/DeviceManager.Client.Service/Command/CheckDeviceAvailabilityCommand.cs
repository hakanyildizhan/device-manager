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
    /// Command that checks the availability of a device item.
    /// </summary>
    public class CheckDeviceAvailabilityCommand : IAppCommand
    {
        private readonly IDataService _dataService;
        private readonly IFeedbackService _feedbackService;
        private readonly ILogService _logService;
        public string Name { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public CheckDeviceAvailabilityCommand(
            IDataService dataService, 
            IFeedbackService feedbackService, 
            ILogService<CheckDeviceAvailabilityCommand> logService)
        {
            _dataService = dataService;
            _feedbackService = feedbackService;
            _logService = logService;
        }

        public async Task<bool> Execute()
        {
            int deviceId = int.Parse(Parameters["deviceId"]);
            string deviceName = Parameters["deviceName"];

            var result = await _dataService.CheckDeviceAvailabilityAsync(deviceId);

            if (result == ApiCallResult.NotReachable)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "Server unreachable", "Cannot reach the server right now. Please try again later.");
                _logService.LogError($"Could not check availability for {deviceName}. Server unreachable");
            }
            else if (result == ApiCallResult.Failure || result == ApiCallResult.Unknown)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Warning, "Device unavailable", "This device is currently unavailable. Device status will be updated shortly.");
                _logService.LogError($"Could not check device availability for {deviceName}");
            }

            return result == ApiCallResult.Success;
        }
    }
}
