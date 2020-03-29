using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.TrayApp.IoC
{
    public interface IAppServiceProvider : IServiceProvider
    {
        object GetService<T>();
    }
}
