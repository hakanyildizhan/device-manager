// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
using DeviceManager.Service;
using DeviceManager.Update;
using DeviceManager.WindowsService.Jobs;
using System;
using Unity;

namespace DeviceManager.WindowsService.IoC
{
    public class UnityServiceProvider : IAppServiceProvider
    {
        private static readonly Lazy<IAppServiceProvider> lazy = new Lazy<IAppServiceProvider>(() => new UnityServiceProvider());
        public static IAppServiceProvider Instance { get { return lazy.Value; } }

        private IUnityContainer _container;

        private UnityServiceProvider()
        {
            _container = BuildUnityContainer();
        }

        /// <summary>Gets the service object of the specified type.</summary>
        /// <returns>A service object of type <paramref name="serviceType" />.-or- null if there is no service object of type <paramref name="serviceType" />.</returns>
        /// <param name="serviceType">An object that specifies the type of service object to get. </param>
        public object GetService(Type serviceType)
        {
            //Delegates the GetService to the Containers Resolve method
            return _container.Resolve(serviceType);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns></returns>
        public object GetService<T>()
        {
            //Delegates the GetService to the Containers Resolve method
            return _container.Resolve<T>();
        }

        private IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            container.RegisterType(typeof(ILogService<>), typeof(NLogLogger<>));
            container.RegisterSingleton<IScheduleParser, CrontabParser>();
            container.RegisterSingleton<ITokenStore, DBTokenStore>();
            container.RegisterSingleton<IManifestParser, XmlManifestParser>();
            container.RegisterSingleton<IUpdateChecker, UpdateChecker>();
            container.RegisterSingleton<ISettingsService, SettingsService>();
            container.RegisterType<IJob, CheckForUpdate>();
            container.RegisterType<IJob, DownloadAndInstallUpdate>();
            container.RegisterSingleton<IJobFactory, JobFactory>();

            return container;
        }
    }
}
