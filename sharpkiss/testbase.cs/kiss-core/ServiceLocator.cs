using System;
using System.Collections.Generic;
using Kiss.Ioc;
using Kiss.Plugin;

namespace Kiss
{
    /// <summary>
    /// use this class to access all service
    /// </summary>
    public sealed class ServiceLocator
    {
        private IServiceContainer _container;
        public IServiceContainer Container
        {
            get
            {
                if (_container == null)
                    throw new KissException("ServiceLocator is not initialized! please call ServiceLocator.Instance.Init() method when app startup.");

                return _container;
            }
        }

        private readonly SingleEntryGate gate = new SingleEntryGate();

        static ServiceLocator()
        {
            Singleton<ServiceLocator>.Instance = new ServiceLocator();
        }

        private ServiceLocator()
        {
        }

        /// <summary>
        /// init, must called to setup windsor container
        /// </summary>
        public void Init()
        {
            Init(null, false);
        }

        /// <summary>
        /// init, must called to setup windsor container
        /// </summary>
        /// <param name="action"></param>
        public void Init(Action action)
        {
            Init(action, true);
        }

        /// <summary>
        /// init, must called to setup windsor container
        /// </summary>
        public void Init(Action action, bool enablePlugins)
        {
            if (!gate.TryEnter())
                return;

            try
            {
                _container = new ServiceContainer();

                if (action != null)
                    action.Invoke();

                _container.ComponentCreated += _container_ComponentCreated;

                if (enablePlugins)
                {
                    PluginBootstrapper pluginBootstrapper = PluginBootstrapper.Instance;
                    pluginBootstrapper.InitializePlugins(pluginBootstrapper.GetPluginDefinitions());
                }
            }
            catch (Exception ex)
            {
                throw new KissException("ServiceLocator init failed!", ex);
            }
        }

        void _container_ComponentCreated(object sender, ComponentCreatedEventArgs e)
        {
            if (e.Instance != null && e.Instance is IAutoStart)
            {
                var startable = e.Instance as IAutoStart;
                if (startable != null)
                {
                    startable.Start();
                }
            }
        }

        /// <summary>Finds types assignable from of a certain type</summary>
        /// <param name="requestedType">The type to find</param>
        /// <param name="excludeSelf">If or not exclude requestedType</param>
        /// <returns>A list of types found</returns>
        public IList<Type> Find(Type requestedType, bool excludeSelf)
        {
            IList<Type> list = Container.Resolve<ITypeFinder>().Find(requestedType);

            if (excludeSelf)
                list.Remove(requestedType);

            return list;
        }

        /// <summary>
        /// instance
        /// </summary>
        public static ServiceLocator Instance { get { return Singleton<ServiceLocator>.Instance; } }

        #region Container Methods

        /// <summary>Resolves a service configured in the factory.</summary>
        public T Resolve<T>() where T : class
        {
            return Container.Resolve<T>();
        }

        public T SafeResolve<T>() where T : class
        {
            return Container.SafeResolve<T>();
        }

        public object Resolve(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }

        /// <summary>Resolves a named service configured in the factory.</summary>
        /// <param name="key">The name of the service to resolve.</param>
        /// <returns>An instance of the resolved service.</returns>        
        public object Resolve(string key)
        {
            return Container.Resolve(key);
        }

        /// <summary>Registers a component in the IoC container.</summary>
        /// <param name="serviceType">The type of service to provide.</param>
        /// <param name="classType">The type of component to register.</param>
        public void AddComponent(Type serviceType, Type classType)
        {
            Container.AddComponent(serviceType, classType);
        }

        /// <summary>Registers a component in the IoC container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="classType">The type of component to register.</param>
        public void AddComponent(string key, Type classType)
        {
            Container.AddComponent(key, classType);
        }

        /// <summary>Registers a component in the IoC container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="serviceType">The type of service to provide.</param>
        /// <param name="classType">The type of component to register.</param>
        public void AddComponent(string key, Type serviceType, Type classType)
        {
            Container.AddComponent(key, serviceType, classType);
        }

        /// <summary>Adds a component instance to the container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="serviceType">The type of service to provide.</param>
        /// <param name="instance">The service instance to add.</param>
        public void AddComponentInstance(string key, Type serviceType, object instance)
        {
            Container.AddComponentInstance(key, serviceType, instance);
        }

        /// <summary>Adds a component instance to the container.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public T AddComponentInstance<T>(T instance) where T : class
        {
            if (instance != null)
            {
                AddComponentInstance(typeof(T).Name, typeof(T), instance);
            }
            return instance as T;
        }

        #endregion
    }
}
