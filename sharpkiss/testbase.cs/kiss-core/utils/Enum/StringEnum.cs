#region File Comment
//+-------------------------------------------------------------------+
//+ File Created:	20:41 2007-2-7
//+-------------------------------------------------------------------+
//+ Purpose:    	    能够将String数组做为枚举类型
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
#endregion

#region References

using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace Kiss.Utils
{
    /// <summary>
    /// A Utility class can be used to treat <c>string</c> arrays as <c>enum</c>'s.  Use this class
    /// to parse a string to an enum or serialize an enum to a string.  Unlike direct use of an
    /// enum, this class allows the enum values to be decorated with <c>Description</c> attributes
    /// which can be used when serializing the enum.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class StringEnum<T>
    {
        /// <summary>
        /// Parses the specified value string to enum type.  If the string can not be parsed then
        /// and exception is thrown.  This method will first attempt to parse the string by matching it
        /// directly to the string representation of the enum value.  If a match is not found, then it will
        /// attempt to match the enum against each DescriptionAttribute applied to that enum (if any).
        /// </summary>
        /// <param name="enumString">The enum string.</param>
        /// <returns>An enum value of type T</returns>
        /// <exception cref="InvalidCastException"></exception>
        public static T Parse ( string enumString )
        {
            Type enumType = typeof ( T );
            foreach ( FieldInfo fi in enumType.GetFields ( ) )
            {
                if ( fi.Name.Equals ( enumString, StringComparison.CurrentCultureIgnoreCase ) )
                    return ( T ) fi.GetValue ( null );
                else
                {
                    object[ ] fieldAttributes = fi.GetCustomAttributes ( typeof ( DescriptionAttribute ), false );
                    foreach ( DescriptionAttribute attr in fieldAttributes )
                    {
                        if ( attr.Description.Equals ( enumString, StringComparison.CurrentCultureIgnoreCase ) )
                            return ( T ) fi.GetValue ( null );
                    }
                }
            }

            throw new InvalidCastException ( string.Format ( "Can't convert {0} to {1}", enumString,
                                                enumType ) );
        }

        /// <summary>
        /// 安全的解析指定的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T SafeParse ( string str )
        {
            if ( string.IsNullOrEmpty(str) )
                return default ( T );

            Type t = typeof ( T );

            if ( Enum.IsDefined ( t, str ) )
                return ( T ) Enum.Parse ( t, str );
            else
            {
                int i;
                if ( Int32.TryParse ( str, out i ) )
                {
                    if ( Enum.IsDefined ( t, i ) )
                        return ( T ) ( object ) i;
                }
            }

            return default ( T );
        }

        /// <summary>
        /// Parses the specified value string to enum type.  If the string can not be parsed then
        /// <c>false</c> is returne. This method will first attempt to parse the string by matching it
        /// directly to the string representation of the enum value.  If a match is not found, then it will
        /// attempt to match the enum against each DescriptionAttribute applied to that enum (if any).
        /// </summary>
        /// <param name="enumString">The value to parse.</param>
        /// <param name="enumerator">The enumerator type</param>
        /// <returns>
        /// 	<c>true</c> if the string value was successfully parsed.
        /// </returns>
        public static bool TryParse ( string enumString, ref T enumerator )
        {
            Type enumType = typeof ( T );
            foreach ( FieldInfo fi in enumType.GetFields ( ) )
            {
                if ( fi.Name.Equals ( enumString, StringComparison.CurrentCultureIgnoreCase ) )
                {
                    enumerator = ( T ) fi.GetValue ( null );
                    return true;
                }
                else
                {
                    object[ ] fieldAttributes = fi.GetCustomAttributes ( typeof ( DescriptionAttribute ), false );
                    foreach ( DescriptionAttribute attr in fieldAttributes )
                    {
                        if ( attr.Description.Equals ( enumString, StringComparison.CurrentCultureIgnoreCase ) )
                        {
                            enumerator = ( T ) fi.GetValue ( null );
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Uses the enum's DescriptionAttribute to generate the string translation of the enum value.  If
        /// no such attribute has been applied then the method will simply call T.ToString() on the enum.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns></returns>
        public static string ToString ( T enumValue )
        {
            Type enumType = typeof ( T );
            FieldInfo fi = enumType.GetField ( enumValue.ToString ( ) );

            //Get the Description attribute that has been applied to this enum
            object[ ] fieldAttributes = fi.GetCustomAttributes ( typeof ( DescriptionAttribute ), false );
            if ( fieldAttributes.Length > 0 )
            {
                DescriptionAttribute descAttr = fieldAttributes[ 0 ] as DescriptionAttribute;
                if ( descAttr != null )
                    return descAttr.Description;
            }

            //Enum does not have Description attribute so we return default string representation.
            return enumValue.ToString ( );
        }


        /// <summary>
        /// Returns an array of strings that represent the values of the enumerator.  If any of the 
        /// enum values have DescriptionAttribute's applied to them, then the value of the description
        /// is used in the List element.
        /// </summary>
        /// <returns></returns>
        public static string[ ] ToArray ( )
        {
            return ToList ( ).ToArray ( );
        }

        /// <summary>
        /// Returns a list of strings that represent the values of the enumerator.  If any of the 
        /// enum values have DescriptionAttribute's applied to them, then the value of the description
        /// is used in the List element.
        /// </summary>
        /// <returns></returns>
        public static List<string> ToList ( )
        {
            List<String> enumValues = new List<string> ( );
            Type enumType = typeof ( T );
            foreach ( FieldInfo fi in enumType.GetFields ( ) )
            {
                if ( fi.IsSpecialName == false )
                {
                    //Get the Description attribute that has been applied to this enum
                    object[ ] fieldAttributes = fi.GetCustomAttributes ( typeof ( DescriptionAttribute ), false );
                    if ( fieldAttributes.Length > 0 )
                    {
                        DescriptionAttribute descAttr = fieldAttributes[ 0 ] as DescriptionAttribute;
                        if ( descAttr != null )
                            enumValues.Add ( descAttr.Description );
                    }
                    else
                    {
                        //Enum does not have Description attribute so we return default string representation.
                        T enumValue = ( T ) fi.GetValue ( null );
                        enumValues.Add ( enumValue.ToString ( ) );
                    }
                }
            }

            return enumValues;
        }

        /// <summary>
        /// Returns a dictionary that maps enums to their descriptions.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<T, string> ToDictionary ( )
        {
            Dictionary<T , string> enumDictionary = new Dictionary<T, string> ( );

            Type enumType = typeof ( T );
            foreach ( FieldInfo fi in enumType.GetFields ( ) )
            {
                if ( fi.IsSpecialName == false && fi.IsPublic )
                {
                    T enumValue = ( T ) fi.GetValue ( null );

                    //Get the Description attribute that has been applied to this enum
                    object[ ] fieldAttributes = fi.GetCustomAttributes ( typeof ( DescriptionAttribute ), false );
                    if ( fieldAttributes.Length > 0 )
                    {
                        DescriptionAttribute descAttr = fieldAttributes[ 0 ] as DescriptionAttribute;
                        if ( descAttr != null && !enumDictionary.ContainsKey ( enumValue ) )
                            enumDictionary.Add ( enumValue, descAttr.Description );
                    }
                    else
                    {
                        //Enum does not have Description attribute so we return default string representation.
                        if ( !enumDictionary.ContainsKey ( enumValue ) )
                            enumDictionary.Add ( enumValue, enumValue.ToString ( ) );
                    }
                }
            }

            return enumDictionary;
        }


        /// <summary>
        /// Returns a list that contains all of the enum types.  This list can be enumerated
        /// against to get each enum value.
        /// </summary>
        /// <returns></returns>
        public static T[ ] GetKeys ( )
        {
            List<T> keys = new List<T> ( );

            Type enumType = typeof ( T );
            foreach ( FieldInfo fi in enumType.GetFields ( ) )
            {
                if ( fi.IsSpecialName == false )
                {
                    T enumValue = ( T ) fi.GetValue ( null );
                    keys.Add ( enumValue );
                }
            }

            return keys.ToArray ( );
        }
    }
}
