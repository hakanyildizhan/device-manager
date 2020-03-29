using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    public interface IFeedbackService
    {
        Task ShowMessageAsync(MessageType messageType, string title, string message);
        Task ShowMessageAsync(string title, string message);
        Task ShowMessageAsync(MessageType messageType, string message);
        Task ShowMessageAsync(string message);
    }
}
