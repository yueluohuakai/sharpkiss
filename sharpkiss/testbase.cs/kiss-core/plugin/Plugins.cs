using Kiss.Security;
using Kiss.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kiss.Plugin
{
    public static class Plugins
    {
        private static IList<IPlugin> plugins = null;

        static Plugins()
        {
            plugins = FindPlugins();
        }

        internal static void ReInit()
        {
            plugins = FindPlugins();
        }

        /// <summary>Gets plugins found in the environment sorted and filtered by the given user.</summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="user">The user that should be authorized for the plugin.</param>
        /// <returns>An enumeration of plugins.</returns>
        public static IEnumerable<T> GetPlugins<T>(Principal user) where T : class, IPlugin
        {
            foreach (T plugin in GetPlugins<T>())
                if (plugin.IsAuthorized(user))
                    yield return plugin;
        }

        public static IEnumerable<T> GetPlugins<T>() where T : class, IPlugin
        {
            foreach (IPlugin plugin in plugins)
                if (plugin is T)
                    yield return plugin as T;
        }

        /// <summary>Finds and sorts plugin defined in known assemblies.</summary>
        /// <returns>A sorted list of plugins.</returns>
        private static IList<IPlugin> FindPlugins()
        {
            List<IPlugin> foundPlugins = new List<IPlugin>();
            foreach (Assembly assembly in ServiceLocator.Instance.Resolve<ITypeFinder>().GetAssemblies())
            {
                foreach (IPlugin plugin in FindPluginsIn(assembly))
                {
                    if (plugin.Name == null)
                        throw new KissException("A plugin in the assembly '{0}' has no name. The plugin is likely defined on the assembly ([assembly:...]). Try assigning the plugin a unique name and recompiling.", assembly.FullName);

                    if (foundPlugins.Contains(plugin))
                        throw new KissException("A plugin of the type '{0}' named '{1}' is already defined, assembly: {2}", plugin.GetType().FullName, plugin.Name, assembly.FullName);

                    foundPlugins.Add(plugin);
                }
            }
            foundPlugins.Sort((a, b) => { return a.SortOrder.CompareTo(b.SortOrder); });
            return foundPlugins;
        }

        private static IEnumerable<IPlugin> FindPluginsIn(Assembly a)
        {
            Type[] types;
            try
            {
                types = a.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }
            catch (Exception ex)
            {
                LogManager.GetLogger<PluginBootstrapper>().Error(ExceptionUtil.WriteException(ex));

                types = new Type[] { };
            }

            foreach (Type t in types)
            {
                if (t == null) continue;
                foreach (IPlugin attribute in t.GetCustomAttributes(typeof(IPlugin), false))
                {
                    if (attribute.Name == null)
                        attribute.Name = t.Name;
                    attribute.Decorates = t;

                    yield return attribute;
                }
            }
        }
    }
}
