// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;

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
