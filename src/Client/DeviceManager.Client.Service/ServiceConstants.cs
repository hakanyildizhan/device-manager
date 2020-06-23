// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

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
            public const string SERVER_VERSION = "serverVersion";
            public const string LAST_DEVICELIST_UPDATE = "lastDeviceListUpdate";
        }
    }
}
