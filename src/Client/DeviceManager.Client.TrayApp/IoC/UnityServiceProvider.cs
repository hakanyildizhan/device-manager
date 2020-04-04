using DeviceManager.Client.Service;
using DeviceManager.Client.TrayApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Client.TrayApp.IoC
{
    public class UnityServiceProvider : IAppServiceProvider
    {
        private static readonly Lazy<IAppServiceProvider> lazy = new Lazy<IAppServiceProvider>(() => new UnityServiceProvider());
        public static IAppServiceProvider Instance { get { return lazy.Value; } }

        private IUnityContainer _container;

        //public IUnityContainer UnityContainer => _container;

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
            container.RegisterSingleton<IConfigurationService, JsonConfigService>();
            container.RegisterType<IDataService, DataService>();
            container.RegisterType<IFeedbackService, BasicToastFeedbackService>();
            return container;
        }
    }
}
