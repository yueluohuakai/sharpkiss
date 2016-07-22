#region File Comment
//+-------------------------------------------------------------------+
//+ FileName: 	    StringWrapper.cs
//+ File Created:   20090825
//+-------------------------------------------------------------------+
//+ Purpose:        
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 20090825        ZHLI Comment Created
//+-------------------------------------------------------------------+
#endregion

using System;
using System.Text;

namespace Kiss.Utils
{
    /// <summary>
    /// Surrounds a string builder with two strings. The prefix is appended 
    /// right away, the suffix is appended upon disposal.
    /// </summary>
    /// <example>
    /// using(new StringWrapper(myStringBuilder, "(", ")"))
    /// {
    /// 	myStringBuilder.Append("something that should be between the parenthesis");
    /// }
    /// </example>
    public class StringWrapper : IDisposable
    {
        private StringBuilder builder = null;
        private string suffix;

        public StringWrapper ( StringBuilder builder, string prefix, string suffix )
        {
            if ( !string.IsNullOrEmpty ( prefix ) )
                builder.Append ( prefix );
            this.builder = builder;
            this.suffix = suffix;
        }

        public void Dispose ( )
        {
            if ( builder != null && !string.IsNullOrEmpty ( suffix ) )
                builder.Append ( suffix );
        }
    }
}
