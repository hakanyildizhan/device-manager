using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service
{
    public interface ILogService
    {
        void LogInformation(string message);
        void LogException(Exception exception);
        void LogException(Exception exception, string message);
        void LogDebug(string message);
        void LogError(string message);
    }

    public interface ILogService<T> : ILogService { }
}
