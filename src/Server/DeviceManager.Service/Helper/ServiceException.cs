using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service
{
    public class ServiceException : Exception
    {
        public ServiceException(Exception innerException) 
            : base("Service exception occured", innerException)
        {
        }
    }
}
