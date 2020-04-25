using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
