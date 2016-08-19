﻿using Kiss.Utils;
using Kiss.Web.UrlMapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Caching;

namespace Kiss.Web.Area
{
    public class AreaUrlMappingProvider : IUrlMappingProvider
    {
        private const string kCACHE_KEY = "__AreaUrlMappingProvider_cache_key__";
        private UrlMappingConfig config;
        private Dictionary<string, UrlMappingItemCollection> _urlMappings = new Dictionary<string, UrlMappingItemCollection>();
        private Dictionary<string, Dictionary<int, NavigationItem>> _menuItems = new Dictionary<string, Dictionary<int, NavigationItem>>();
        private UrlMappingItemCollection _manualGlobalRoutes = new UrlMappingItemCollection();
        private Dictionary<string, UrlMappingItemCollection> _manualItems = new Dictionary<string, UrlMappingItemCollection>();
        private static readonly object _synclock = new object();

        private ILogger _logger;
        private ILogger logger
        {
            get
            {
                if (_logger == null)
                    _logger = LogManager.GetLogger<AreaUrlMappingProvider>();
                return _logger;
            }
        }

        private DateTime _latestRefresh;

        private CacheDependency _fileDependency;

        public void Initialize(UrlMappingConfig config)
        {
            AssertUtils.ArgumentNotNull(config, "urlmappingConfig");

            this.config = config;

            RefreshUrlMappingData();
        }

        public DateTime LastRefreshTime
        {
            get { return _latestRefresh; }
        }

        public void RefreshUrlMappings()
        {
            RefreshUrlMappingData();
        }

        public UrlMappingItemCollection UrlMappings
        {
            get
            {
                RefreshUrlMappingData();

                IArea site = JContext.Current.Area;

                if (!_urlMappings.ContainsKey(site.AreaKey))
                {
                    logger.Info("routes not exist! area={0}", site.VirtualPath);
                    return new UrlMappingItemCollection();
                }

                if (_manualItems.ContainsKey(site.AreaKey))
                    return UrlMappingItemCollection.Combin(_manualGlobalRoutes, UrlMappingItemCollection.Combin(_manualItems[site.AreaKey], _urlMappings[site.AreaKey]));

                return UrlMappingItemCollection.Combin(_manualGlobalRoutes, _urlMappings[site.AreaKey]);
            }
        }

        public Dictionary<int, NavigationItem> MenuItems { get { return GetMenuItemsBySite(JContext.Current.Area); } }

        public Dictionary<int, NavigationItem> GetMenuItemsBySite(IArea site)
        {
            RefreshUrlMappingData();

            if (!_menuItems.ContainsKey(site.AreaKey))
            {
                logger.Info("menu not exist! area={0}", site.VirtualPath);
                return new Dictionary<int, NavigationItem>();
            }

            return _menuItems[site.AreaKey];
        }

        public UrlMappingItemCollection GetUrlsBySite(IArea site)
        {
            RefreshUrlMappingData();

            if (!_urlMappings.ContainsKey(site.AreaKey))
            {
                logger.Info("url not exist! area={0}", site.VirtualPath);
                return new UrlMappingItemCollection();
            }

            return _urlMappings[site.AreaKey];
        }

        protected void RefreshUrlMappingData()
        {
            if (HttpContext.Current.Cache[kCACHE_KEY] != null)
                return;

            lock (_synclock)
            {
                if (HttpContext.Current.Cache[kCACHE_KEY] != null) return;

                _urlMappings.Clear();
                _menuItems.Clear();

                List<string> routefiles = new List<string>();

                string root = ServerUtil.MapPath("~");

                foreach (var item in AreaInitializer.Areas.Keys)
                {
                    if (item.Equals("/"))
                        routefiles.Add(Path.Combine(root, "App_Data" + Path.DirectorySeparatorChar + "routes.config"));
                    else
                        routefiles.Add(Path.Combine(root, item.Substring(1) + Path.DirectorySeparatorChar + "routes.config"));
                }

                foreach (var item in routefiles)
                {
                    string vp = Path.GetFileName(Path.GetDirectoryName(item)).ToLowerInvariant();
                    if (string.Equals(vp, "App_Data", StringComparison.InvariantCultureIgnoreCase))
                        vp = "/";
                    else
                        vp = "/" + vp;

                    if (!AreaInitializer.Areas.ContainsKey(vp))
                        throw new WebException("virtual path not found: {0}", vp);

                    IArea site = AreaInitializer.Areas[vp];

                    UrlMappingItemCollection routes = new UrlMappingItemCollection();
                    Dictionary<int, NavigationItem> menus = new Dictionary<int, NavigationItem>();

                    XmlUrlMappingProvider.ParseXml(item, routes, menus, IncomingQueryStringBehavior.PassThrough);

                    _urlMappings[site.AreaKey] = routes;
                    _menuItems[site.AreaKey] = menus;
                }

                _fileDependency = new CacheDependency(Path.Combine(root, "App_Data" + Path.DirectorySeparatorChar + "routes.config"));
                HttpRuntime.Cache.Insert(kCACHE_KEY, "dummyValue", _fileDependency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.High, null);

                _latestRefresh = DateTime.Now;
            }
        }

        public void Dispose()
        {
        }

        public void AddMapping(UrlMappingItem item)
        {
            _manualGlobalRoutes.Merge(item);
        }

        public void AddMapping(string siteKey, UrlMappingItem item)
        {
            UrlMappingItemCollection coll;

            if (_manualItems.ContainsKey(siteKey))
                coll = _manualItems[siteKey];
            else
            {
                coll = new UrlMappingItemCollection();
                _manualItems[siteKey] = coll;
            }

            coll.Merge(item);
        }
    }
}
