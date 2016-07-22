using System;
using Kiss.Plugin;
using Kiss.Utils;

namespace Kiss.Repository
{
    [AutoInit(Title = "Repository", Priority = 9)]
    public class RepositoryInitializer : IPluginInitializer
    {
        #region IPluginInitializer Members

        public void Init(ServiceLocator sl, ref PluginSetting s)
        {
            if (!s.Enable)
                return;

            RepositoryPluginSetting setting = new Repository.RepositoryPluginSetting(s);
            s = setting;

            string type1 = setting["type1"];
            string type2 = setting["type2"];
            if (StringUtil.IsNullOrEmpty(type1) && StringUtil.IsNullOrEmpty(type2))
            {
                setting.Enable = false;

                LogManager.GetLogger<RepositoryInitializer>().Debug("RepositoryInitializer is disabled. type1 and type2 is null.");
            }
            else
            {
                if (StringUtil.HasText(type1))
                {
                    try
                    {
                        Type t1 = Type.GetType(type1, true, true);
                        sl.AddComponent("kiss.repository_1", typeof(IRepository<>), t1);
                    }
                    catch (Exception ex)
                    {
                        LogManager.GetLogger<RepositoryInitializer>().Error("RepositoryInitializer Error." + ExceptionUtil.WriteException(ex));
                    }
                }

                if (StringUtil.HasText(type2))
                {
                    try
                    {
                        Type t2 = Type.GetType(type2, true, true);
                        sl.AddComponent("kiss.repository_2", typeof(IRepository<,>), t2);
                    }
                    catch (Exception ex)
                    {
                        LogManager.GetLogger<RepositoryInitializer>().Error("RepositoryInitializer Error." + ExceptionUtil.WriteException(ex));                        
                    }                    
                }
            }

            foreach (var item in Plugin.Plugins.GetPlugins<DbProviderAttribute>())
            {
                sl.AddComponent(item.ProviderName, item.Decorates);
            }
        }

        #endregion
    }
}
