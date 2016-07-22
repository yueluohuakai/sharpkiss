using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kiss.Ioc
{
    /// <summary>
    /// service container
    /// </summary>
    /// <remarks>
    /// only support singleton lifecircle
    /// </remarks>
    public class ServiceContainer : IServiceContainer
    {
        readonly IDictionary<Type, Type> types = new Dictionary<Type, Type>();
        readonly IDictionary<string, Type> types2 = new Dictionary<string, Type>();
        readonly IDictionary<Type, object> instances = new Dictionary<Type, object>();
        readonly IDictionary<string, object> instances2 = new Dictionary<string, object>();

        /// <summary>Registers a component in the IoC container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="classType">The type of component to register.</param>
        public void AddComponent(string key, Type classType)
        {
            if (string.IsNullOrEmpty(key) || classType == null)
                throw new ArgumentNullException();

            if (!types2.ContainsKey(key))
                types2[key] = classType;
        }

        public void AddComponent(Type serviceType, Type classType)
        {
            if (serviceType == null || classType == null)
                throw new ArgumentNullException();

            if (!types.ContainsKey(serviceType))
                types[serviceType] = classType;
        }

        /// <summary>Registers a component in the IoC container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="serviceType">The type of service to provide.</param>
        /// <param name="classType">The type of component to register.</param>
        public void AddComponent(string key, Type serviceType, Type classType)
        {
            AddComponent(key, classType);

            AddComponent(serviceType, classType);
        }

        /// <summary>Adds a component instance to the container.</summary>
        /// <param name="serviceType">The type of service to provide.</param>
        /// <param name="instance">The service instance to add.</param>
        public void AddComponentInstance(Type serviceType, object instance)
        {
            AddComponent(serviceType, instance.GetType());

            instances[serviceType] = instance;
        }

        /// <summary>Adds a component instance to the container.</summary>
        /// <param name="key">A unique key.</param>
        /// <param name="serviceType">The type of service to provide.</param>
        /// <param name="instance">The service instance to add.</param>
        public void AddComponentInstance(string key, Type serviceType, object instance)
        {
            AddComponent(key, serviceType, instance.GetType());

            instances[serviceType] = instance;
            instances2[key] = instance;
        }

        public T Resolve<T>() where T : class
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        /// 类型未注册时，不抛出异常，而是返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SafeResolve<T>() where T : class
        {
            return (T)Resolve(typeof(T), false);
        }

        /// <summary>Resolves a named service configured in the factory.</summary>
        /// <param name="key">The name of the service to resolve.</param>
        /// <returns>An instance of the resolved service.</returns>        
        public object Resolve(string key)
        {
            if (instances2.ContainsKey(key))
                return instances2[key];

            lock (instances)
            {
                if (instances2.ContainsKey(key))
                    return instances2[key];

                if (!types2.ContainsKey(key))
                    throw new ArgumentException(string.Format("Key {0} is not registed!", key));

                Type implementation = types2[key];

                object instance = CreateInstance(implementation);

                // fire component created event
                OnComponentCreated(new ComponentCreatedEventArgs(instance));

                instances2[key] = instance;

                return instance;
            }
        }

        public object Resolve(Type contract)
        {
            return Resolve(contract, true);
        }

        public object Resolve(Type contract, bool throwException)
        {
            if (instances.ContainsKey(contract))
                return instances[contract];

            lock (instances)
            {
                if (instances.ContainsKey(contract))
                    return instances[contract];

                Type implementation = null;

                if (contract.IsGenericType)
                {
                    Type genericType = contract.GetGenericTypeDefinition();
                    if (types.ContainsKey(genericType))
                    {
                        implementation = types[genericType];
                        implementation = implementation.MakeGenericType(contract.GetGenericArguments());
                    }
                }
                else
                {
                    if (types.ContainsKey(contract))
                        implementation = types[contract];
                }

                if (implementation == null)
                {
                    if (throwException)
                        throw new ArgumentException(string.Format("Type {0} is not registed!", contract.FullName));
                    return null;
                }

                object instance = CreateInstance(implementation);

                // fire component created event
                OnComponentCreated(new ComponentCreatedEventArgs(instance));

                instances[contract] = instance;

                return instance;
            }
        }

        private object CreateInstance(Type implementation)
        {
            object instance;
            ConstructorInfo constructor = implementation.GetConstructors()[0];
            ParameterInfo[] constructorParameters = constructor.GetParameters();

            if (constructorParameters.Length == 0)
            {
                instance = Activator.CreateInstance(implementation);
            }
            else
            {
                List<object> parameters = new List<object>(constructorParameters.Length);

                foreach (ParameterInfo parameterInfo in constructorParameters)
                    parameters.Add(Resolve(parameterInfo.ParameterType));

                instance = constructor.Invoke(parameters.ToArray());
            }
            return instance;
        }

        #region event

        public event EventHandler<ComponentCreatedEventArgs> ComponentCreated;

        protected virtual void OnComponentCreated(ComponentCreatedEventArgs e)
        {
            EventHandler<ComponentCreatedEventArgs> handler = ComponentCreated;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
