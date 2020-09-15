// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;

namespace DeviceManager.WindowsService.IoC
{
    public interface IAppServiceProvider : IServiceProvider
    {
        object GetService<T>();
    }
}