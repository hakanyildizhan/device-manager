using DeviceManager.Api.IoC;
using DeviceManager.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;
using Unity.Lifetime;

namespace DeviceManager.Api
{
    public static class IoCConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            container.RegisterType<IDeviceService, DeviceListService>(new TransientLifetimeManager());
            container.RegisterType<ISessionService, SessionService>(new TransientLifetimeManager());
            container.RegisterType<IUserService, UserService>(new TransientLifetimeManager());
            DependencyResolver.SetResolver(new UnityResolver(container));
        }
    }
}