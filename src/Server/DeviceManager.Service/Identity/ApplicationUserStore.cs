// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Unity;

namespace DeviceManager.Service.Identity
{
    public class ApplicationUserStore : UserStore<UserAccount>
    {
        public ApplicationUserStore([Dependency] DeviceManagerContext context)
            : base(context)
        {
        }
    }
}
