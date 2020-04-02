using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    public static class ServiceConstants
    {
        public static class Settings
        {
            public const string REFRESH_INTERVAL = "refreshInterval";
            public const int REFRESH_INTERVAL_DEFAULT = 20;
            public const string LAST_REFRESH = "lastRefresh";
            public const string LAST_SUCCESSFUL_REFRESH = "lastSuccessfulRefresh";
        }
    }
}
