using System;

namespace Kiss.Utils
{
    /// <summary>
    /// 枚举的扩展方法类
    /// </summary>
    /// <example>
    /// enum PermissionTypes : int {
    ///     None = 0,
    ///     Read = 1,
    ///     Write = 2,
    ///     Modify = 4,
    ///     Delete = 8
    ///     Create = 16,
    ///     All = Read | Write | Modify | Delete | Create
    /// }
    /// PermissionTypes permissions = PermissionTypes.Read | PermissionTypes.Write;
    /// bool canRead = permissions.Has(PermissionTypes.Read); //true
    /// bool canDelete = permissions.Has(PermissionTypes.Delete); //false
    /// </example>
    public static class EnumerationExtensions
    {
        /// <summary>
        /// 判断值里是否包含指定的枚举值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="type">要判断的枚举值</param>
        /// <param name="value">指定的枚举值</param>
        /// <returns>包含，返回ture</returns>
        public static bool Has<T> ( this Enum type, T value )
        {
            try
            {
                return ( ( ( int ) ( object ) type & ( int ) ( object ) value ) == ( int ) ( object ) value );
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断值是否等于指定的枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Is<T> ( this Enum type, T value )
        {
            try
            {
                return ( int ) ( object ) type == ( int ) ( object ) value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 附加一个枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Add<T> ( this Enum type, T value )
        {
            try
            {
                return ( T ) ( object ) ( ( ( int ) ( object ) type | ( int ) ( object ) value ) );
            }
            catch ( Exception ex )
            {
                throw new ArgumentException (
                    string.Format (
                        "Could not append value from enumerated type '{0}'.",
                        typeof ( T ).Name
                        ), ex );
            }
        }

        /// <summary>
        /// 移除一个枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Remove<T> ( this Enum type, T value )
        {
            try
            {
                return ( T ) ( object ) ( ( ( int ) ( object ) type & ~( int ) ( object ) value ) );
            }
            catch ( Exception ex )
            {
                throw new ArgumentException (
                    string.Format (
                        "Could not remove value from enumerated type '{0}'.",
                        typeof ( T ).Name
                        ), ex );
            }
        }
    }
}
