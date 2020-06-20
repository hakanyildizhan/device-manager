// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using NLog;
using System;

namespace DeviceManager.Client.TrayApp.Service
{
    public class NLogLogger<T> : ILogService<T>
    {
        private readonly ILogger _logger;
        public NLogLogger()
        {
            _logger = LogManager.GetLogger(typeof(T).FullName);
        }
        public void LogException(Exception exception)
        {
            _logger.Log(LogLevel.Error, exception);
        }

        public void LogException(Exception exception, string message)
        {
            _logger.Log(LogLevel.Error, exception, message);
        }


        public void LogInformation(string message)
        {
            _logger.Info(message);
        }

        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }
    }
}
