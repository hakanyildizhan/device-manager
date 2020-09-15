// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Entity.Context;
using DeviceManager.WindowsService.IoC;
using System.ServiceProcess;
using Unity;

namespace DeviceManager.WindowsService
{
    public abstract class UpdateServiceBase : ServiceBase
    {
        internal static IAppServiceProvider ServiceProvider => UnityServiceProvider.Instance;
    }
}
