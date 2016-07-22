using System;

namespace Kiss.Ioc
{
    public interface IServiceContainer
    {
        void AddComponent(string key, Type classType);
        void AddComponent(string key, Type serviceType, Type classType);
        void AddComponent(Type serviceType, Type classType);
        void AddComponentInstance(string key, Type serviceType, object instance);
        void AddComponentInstance(Type serviceType, object instance);
        event EventHandler<ComponentCreatedEventArgs> ComponentCreated;
        object Resolve(string key);
        object Resolve(Type contract);
        T Resolve<T>() where T : class;
        T SafeResolve<T>() where T : class;
    }

    public class ComponentCreatedEventArgs : EventArgs
    {
        public object Instance { get; set; }

        public ComponentCreatedEventArgs()
        {
        }

        public ComponentCreatedEventArgs(object instance)
        {
            Instance = instance;
        }
    }
}
