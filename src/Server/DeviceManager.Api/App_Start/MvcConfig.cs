using DeviceManager.Api.IoC;
using DeviceManager.Service;
using DeviceManager.Service.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;
using Unity.Lifetime;

namespace DeviceManager.Api
{
    public static class MvcConfig
    {
        /// <summary>
        /// Sets up initial configuration for the MVC part of the application.
        /// </summary>
        public static void Register()
        {
            ConfigureDIContainer();
        }

        /// <summary>
        /// Registers types for dependency injection.
        /// </summary>
        private static void ConfigureDIContainer()
        {
            var container = new UnityContainer();
            container.RegisterType<IIdentityService, IdentityService>(new TransientLifetimeManager());
            container.RegisterType<IDeviceService, DeviceService>(new TransientLifetimeManager());
            container.RegisterType<ISessionService, SessionService>(new TransientLifetimeManager());
            container.RegisterType<IClientService, ClientService>(new TransientLifetimeManager());
            container.RegisterType<ISettingsService, SettingsService>(new TransientLifetimeManager());
            container.RegisterType(typeof(ILogService<>), typeof(NLogLogger<>));
            container.RegisterFactory<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication);
            IdentityStartup.RegisterServices(container);
            DependencyResolver.SetResolver(new UnityResolverMVC(container));
        }
    }
}