using System;
using System.Collections.Generic;
using System.Text;

namespace Kiss.Plugin
{
    public class PluginBootstrapper
    {
        private PluginBootstrapper()
        {
        }

        public static readonly PluginBootstrapper Instance = new PluginBootstrapper();

        private List<string> initializedPlugins = new List<string>();

        internal Dictionary<string, PluginSetting> _pluginSettings = new Dictionary<string, PluginSetting>();

        /// <summary>Gets plugins in the current app domain using the type finder.</summary>
        /// <returns>An enumeration of available plugins.</returns>
        public IEnumerable<IPluginDefinition> GetPluginDefinitions()
        {
            List<IPluginDefinition> pluginDefinitions = new List<IPluginDefinition>();

            // autoinitialize plugins
            foreach (Type type in ServiceLocator.Instance.Resolve<ITypeFinder>().Find(typeof(IPluginInitializer)))
            {
                foreach (AutoInitAttribute plugin in type.GetCustomAttributes(typeof(AutoInitAttribute), true))
                {
                    plugin.InitializerType = type;

                    pluginDefinitions.Add(plugin);
                }
            }

            pluginDefinitions.Sort((a, b) => { return b.Priority.CompareTo(a.Priority); });
            return pluginDefinitions;
        }

        /// <summary>Invokes the initialize method on the supplied plugins.</summary>
        public void InitializePlugins(IEnumerable<IPluginDefinition> plugins)
        {
            StringBuilder log = new StringBuilder();

            ServiceLocator sl = ServiceLocator.Instance;

            PluginSettings settings = PluginConfig.Instance.Settings;

            List<Exception> exceptions = new List<Exception>();
            int count = 0, enable_count = 0;
            foreach (IPluginDefinition plugin in plugins)
            {
                InitPlugin(log, sl, settings, exceptions, ref count, ref enable_count, plugin);
            }

            Plugins.ReInit();

            // init again for plugins in plugin
            foreach (var plugin in GetPluginDefinitions())
            {
                InitPlugin(log, sl, settings, exceptions, ref count, ref enable_count, plugin);
            }

            Plugins.ReInit();

            if (exceptions.Count > 0)
            {
                string message = "While initializing {0} plugin(s) threw an exception. Please review the stack trace to find out what went wrong.";
                message = string.Format(message, exceptions.Count);

                foreach (Exception ex in exceptions)
                    message += Environment.NewLine + Environment.NewLine + "- " + ex.Message;

                throw new PluginInitException(message, exceptions.ToArray());
            }

            log.AppendFormat("plugins initialized. {1} of {0} is enable.", count, enable_count);

            LogManager.GetLogger<PluginBootstrapper>().Info(log.ToString());
        }

        private void InitPlugin(StringBuilder log, ServiceLocator sl, PluginSettings settings, List<Exception> exceptions, ref int count, ref int enable_count, IPluginDefinition plugin)
        {
            if (string.IsNullOrEmpty(plugin.Name) || initializedPlugins.Contains(plugin.Name))
                return;

            try
            {
                PluginSetting setting = settings.FindByName(plugin.Name);

                // remove it. add it later
                settings.Remove(setting);

                setting.Title = plugin.Title;
                setting.Description = plugin.Description;

                plugin.Init(sl, ref setting);

                _pluginSettings[plugin.Name] = setting;
                settings.Add(setting);

                initializedPlugins.Add(plugin.Name);

                count++;
                if (setting.Enable)
                    enable_count++;

                log.AppendFormat("{0}:{1} ", setting.Name, setting.Enable);
                log.AppendLine();
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }
    }
}
