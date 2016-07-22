
namespace Kiss.Plugin
{
    /// <summary>
    /// Classes implementing this interface can serve as plug in initializers. 
    /// If one of these classes is referenced by a AutoInitAttribute 
    /// it's initialize methods will be invoked during initialization.</summary>
    public interface IPluginInitializer
    {
        void Init(ServiceLocator sl, ref PluginSetting setting);
    }
}
