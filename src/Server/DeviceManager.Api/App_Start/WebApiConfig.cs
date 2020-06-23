// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Api.IoC;
using DeviceManager.Service;
using System.Web.Http;
using Unity;
using Unity.Lifetime;

namespace DeviceManager.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            NLogConfig.Initialize();

            // IoC container setup
            var container = new UnityContainer();
            container.RegisterType<IDeviceService, DeviceService>(new TransientLifetimeManager());
            container.RegisterType<ISessionService, SessionService>(new TransientLifetimeManager());
            container.RegisterType<IClientService, ClientService>(new TransientLifetimeManager());
            container.RegisterType<ISettingsService, SettingsService>(new TransientLifetimeManager());
            container.RegisterType(typeof(ILogService<>), typeof(NLogLogger<>));
            config.DependencyResolver = new UnityResolverWebApi(container);

            // Exception handler
            config.Filters.Add(new GenericExceptionFilterAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApiGet",
                routeTemplate: "api/{controller}"
            );
        }
    }
}
