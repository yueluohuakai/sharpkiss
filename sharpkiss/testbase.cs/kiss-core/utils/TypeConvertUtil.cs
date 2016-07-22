using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace Kiss.Utils
{
    /// <summary>
    /// use this util to convert type
    /// </summary>
    public static class TypeConvertUtil
    {
        /// <summary>
        /// convert object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(object value)
        {
            Type t = typeof(T);

            object obj = ConvertTo(value, t);

            if (t.IsValueType)
            {
                if (obj == null) return default(T);

                return (T)obj;
            }

            return (T)obj;
        }

        /// <summary>
        /// convert to
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object ConvertTo(object value, Type targetType)
        {
            // check for value = null    
            if (value == null || value is DBNull)
                return null;

            // do we have a nullable type?
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                NullableConverter nc = new NullableConverter(targetType);
                targetType = nc.UnderlyingType;
            }

            if (targetType.IsEnum) // if enum use parse
                return Enum.Parse(targetType, value.ToString(), false);
            else
            {
                // if we have a custom type converter then use it
                TypeConverter td = TypeDescriptor.GetConverter(targetType);
                if (td.CanConvertFrom(value.GetType()))
                {
                    if (td.IsValid(value))
                        return td.ConvertFrom(value);

                    return Activator.CreateInstance(targetType);
                }
                else // otherwise use the changetype
                    return Convert.ChangeType(value, targetType);
            }
        }

        /// <summary>
        /// convert namevaluecollection to obj
        /// </summary>
        public static void ConvertFrom(object old, NameValueCollection nv, params string[] ignores)
        {
            if (old == null) return;

            List<string> ignore_list = new List<string>();

            foreach (var item in ignores)
            {
                ignore_list.Add(item.ToLower());
            }

            Type t = old.GetType();

            foreach (var p in t.GetProperties())
            {
                if (!p.CanWrite || nv[p.Name] == null || ignore_list.Contains(p.Name.ToLower()))
                    continue;

                p.SetValue(old,
                    ConvertTo(nv[p.Name].Trim(), p.PropertyType),
                    null);
            }
        }
    }
}
