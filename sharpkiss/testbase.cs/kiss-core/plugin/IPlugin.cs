using System;
using Kiss.Security;

namespace Kiss.Plugin
{
    /// <summary>
    /// 插件的接口，该接口定义了一个插件的基本属性
    /// </summary>
    public interface IPlugin : IEquatable<IPlugin>
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 插件类型
        /// </summary>
        Type Decorates { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// 当前用户是否适用
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool IsAuthorized(Principal user);

        /// <summary>
        /// 是否启用
        /// </summary>
        bool IsEnabled { get; }
    }
}
