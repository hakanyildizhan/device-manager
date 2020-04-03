using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    public interface ILogService
    {
        void LogInformation(string message);
        void LogException(string message);
        void LogDebug(string message);
        void LogError(string message);
    }

    public interface ILogService<T> : ILogService { }
}
