using System;
using System.Collections;
using System.Web;

namespace Kiss.Utils
{
    /// <summary>
    /// use this class to do something in another app domain
    /// </summary>
    [Serializable]
    public class CrossDomain
    {
        const string KEY = "cross_domain_key";

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="currentDoamin">current app domain</param>
        /// <param name="callback">action</param>
        /// <param name="param">param</param>
        public CrossDomain(AppDomain currentDoamin, Action<Hashtable> callback, Hashtable param)
        {
            CallbackAction = callback;
            HostDomain = currentDoamin;
            Datas = param;
        }

        private Hashtable Datas { get; set; }
        private AppDomain HostDomain { get; set; }
        private Action<Hashtable> CallbackAction { get; set; }

        private void Callback()
        {
            CallbackAction(Datas ?? new Hashtable());

            HostDomain.DoCallBack(delegate
            {
                HttpRuntime.Cache.Insert(KEY, Datas);
            });
        }

        /// <summary>
        /// execute in another app domain, and return result 
        /// </summary>
        /// <returns></returns>
        public Hashtable Execute()
        {
            return Execute(null, null, null);
        }

        /// <summary>
        /// execute in another app domain, and return result 
        /// </summary>
        /// <param name="appdomainName"></param>
        /// <param name="appBase"></param>
        /// <param name="privateBinPath"></param>
        /// <returns></returns>
        public Hashtable Execute(string appdomainName, string appBase, string privateBinPath)
        {
            if (string.IsNullOrEmpty(appdomainName))
                appdomainName = Guid.NewGuid().ToString();

            AppDomain appdomain;

            if (!string.IsNullOrEmpty(appBase))

                appdomain = AppDomain.CreateDomain(appdomainName,
                         null,
                         new AppDomainSetup() { ApplicationBase = appBase, PrivateBinPath = privateBinPath });
            else
                appdomain = AppDomain.CreateDomain(appdomainName);

            try
            {
                appdomain.DoCallBack(Callback);
            }
            finally
            {
                AppDomain.Unload(appdomain);                
            }

            Hashtable ht = HttpRuntime.Cache[KEY] as Hashtable;
            if (ht != null)
                HttpRuntime.Cache.Remove(KEY);
            return ht;
        }
    }
}
