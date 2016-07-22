using System;
using System.Collections;
using System.Data;

namespace Kiss.Utils
{
    public static class DataUtil
    {
        public static T SafePopulate<T> ( IDataReader rdr , string key )
        {
            object o = rdr[ key ];
            if ( o == null || o is DBNull )
                return default ( T );

            return ( T ) o;
        }

        public static T SafePopulate<T> ( IDataReader rdr , string key , T defaultValue )
        {
            object o = rdr[ key ];
            if ( o == null || o is DBNull )
                return default ( T );

            return defaultValue;
        }

        public static object Convent2Bindable ( object obj )
        {
            if ( obj == null )
                return null;

            if ( obj is IList )
            {
                if ( ( ( IList ) obj ).Count == 0 )
                    return null;
                return obj;
            }
            else
            {
                return new object[ ] { obj };
            }

        }

        public static Hashtable GetHashtable ( IDataReader rdr , DataTable schemaTable )
        {
            AssertUtils.ArgumentTrue ( !rdr.IsClosed , "rdr.IsClosed" );

            Hashtable ht = new Hashtable ( );

            foreach ( DataRow row in schemaTable.Rows )
            {
                string columnName = row.ItemArray[ 0 ].ToString ( );
                ht[ columnName ] = rdr[ columnName ];
            }

            return ht;
        }
    }
}
