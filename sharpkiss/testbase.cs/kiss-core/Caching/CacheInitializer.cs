using Kiss.Plugin;

namespace Kiss.Caching
{
    /// <summary>
    /// cache initializer. use this class to create cache provider
    /// </summary>
    [AutoInit(Title = "Cache", Priority = 10)]
    class CacheInitializer : IPluginInitializer
    {
        #region IPluginInitializer Members

        public void Init(ServiceLocator sl, ref PluginSetting s)
        {
            if (!s.Enable) return;

            CachePluginSetting settings = new CachePluginSetting(s);
            s = settings;

            foreach (var item in Plugin.Plugins.GetPlugins<CacheProviderAttribute>())
            {
                sl.AddComponent(string.Format("kiss.cache.{0}", item.Name), typeof(ICacheProvider), item.Decorates);
            }
        }

        #endregion
    }
}
