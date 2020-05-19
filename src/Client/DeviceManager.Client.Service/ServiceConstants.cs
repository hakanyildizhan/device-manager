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
            public const int REFRESH_INTERVAL_DEFAULT = 60;
            public const int REFRESH_INTERVAL_MAXIMUM = 600;
            public const int REFRESH_INTERVAL_MINIMUM = 30;
            public const string LAST_REFRESH = "lastRefresh";
            public const string LAST_SUCCESSFUL_REFRESH = "lastSuccessfulRefresh";
            public const string USAGE_PROMPT_INTERVAL = "usagePromptInterval";
            public const int USAGE_PROMPT_INTERVAL_DEFAULT = 3600;
            public const int USAGE_PROMPT_INTERVAL_MINIMUM = 1800;
            public const int USAGE_PROMPT_INTERVAL_MAXIMUM = 28800;
            public const string USAGE_PROMPT_DURATION = "usagePromptDuration";
            public const int USAGE_PROMPT_DURATION_MINIMUM = 10;
            public const int USAGE_PROMPT_DURATION_DEFAULT = 60;
            public const string SERVER_ADDRESS = "serverAddress";
        }
    }
}
