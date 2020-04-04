using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service
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
