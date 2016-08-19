using Kiss.Caching;
using Kiss.Plugin;
using Kiss.Utils;
using System;
using System.Collections.Generic;

namespace Kiss
{
    /// <summary>
    /// 缓存操作的入口
    /// </summary>
    public sealed class JCache
    {
        private static readonly ILogger _logger = LogManager.GetLogger<JCache>();
        private static readonly CachePluginSetting setting = PluginSettings.Get<CacheInitializer>() as CachePluginSetting;

        /// <summary>
        /// 缓存provider
        /// </summary>
        public static ICacheProvider GetProvider(string cacheKey)
        {
            if (!setting.Enabled) return null;

            string providerName = setting.DefaultProviderName;

            foreach (string key in setting.Configs.Keys)
            {
                if (cacheKey.StartsWith(key, StringComparison.InvariantCultureIgnoreCase))
                {
                    providerName = setting.Configs[key];
                    break;
                }
            }

            if (string.IsNullOrEmpty(providerName)) return null;

            return ServiceLocator.Instance.Resolve(string.Format("kiss.cache.{0}", providerName)) as ICacheProvider;
        }

        public static readonly Dictionary<string, string> Config = new Dictionary<string, string>();


        private JCache()
        {
        }

        /// <summary>
        /// 将对象插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public static void Insert(string key, object obj)
        {
            Insert(key, obj, setting.ValidFor);
        }

        /// <summary>
        /// 将对象插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="validFor"></param>
        public static void Insert(string key, object obj, TimeSpan validFor)
        {
            ICacheProvider p = GetProvider(key);
            if (p != null)
            {
                p.Insert(GetCacheKey(key), obj, validFor);
            }
        }

        /// <summary>
        /// 从缓存获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            ICacheProvider p = GetProvider(key);
            if (p != null)
            {
                return p.Get(GetCacheKey(key));
            }

            return null;
        }

        /// <summary>
        /// 从缓存获取对象列表
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static IDictionary<string, object> Get(IEnumerable<string> keys)
        {
            List<string> keylist = new List<string>(keys);

            if (keylist.Count == 0) return null;

            ICacheProvider p = GetProvider(keylist[0]);
            if (p != null)
                return p.Get(StringUtil.ToStringArray<string>(new List<string>(keys).ToArray(), delegate(string key) { return GetCacheKey(key); }));

            return null;
        }

        /// <summary>
        /// 从缓存获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            return (T)Get(key);
        }

        /// <summary>
        /// 移除缓存中的对象
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(params string[] keys)
        {
            if (keys.Length == 0) return;

            ICacheProvider p = GetProvider(keys[0]);
            if (p != null)
                p.Remove(StringUtil.ToStringArray<string>(keys, delegate(string key) { return GetCacheKey(key); }));
        }

        private static string GetCacheKey(string key)
        {
            if (string.IsNullOrWhiteSpace(setting.Namespace))
                return key;

            return string.Format("{0}.{1}", setting.Namespace, key);
        }
    }
}