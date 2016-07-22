#region File Comment
//+-------------------------------------------------------------------+
//+ File Created:   2009-10-10
//+-------------------------------------------------------------------+
//+ History:
//+-------------------------------------------------------------------+
//+ 2009-10-10		zhli Comment Created
//+-------------------------------------------------------------------+
#endregion

using System;
using System.Globalization;

namespace Kiss.Utils
{
    /// <summary>
    /// Assertion utility methods that simplify things such as argument checks.
    /// </summary>
    public static class AssertUtils
    {
        /// <summary>
        /// Checks the value of the supplied <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentNullException"/> if it is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The object to check.</param>
        /// <param name="name">The argument name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/>.
        /// </exception>
        public static void ArgumentNotNull ( object argument , string name )
        {
            if ( argument == null )
            {
                throw new ArgumentNullException (
                    name ,
                    string.Format (
                        CultureInfo.InvariantCulture ,
                    "Argument '{0}' cannot be null." , name ) );
            }
        }

        /// <summary>
        /// Checks the value of the supplied <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentNullException"/> if it is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The object to check.</param>
        /// <param name="name">The argument name.</param>
        /// <param name="message">
        /// An arbitrary message that will be passed to any thrown
        /// <see cref="System.ArgumentNullException"/>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/>.
        /// </exception>
        public static void ArgumentNotNull ( object argument , string name , string message )
        {
            if ( argument == null )
            {
                throw new ArgumentNullException ( name , message );
            }
        }

        /// <summary>
        /// Checks the value of the supplied <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentException"/> if it is <see langword="false"/>.
        /// </summary>
        /// <param name="argument">The object to check.</param>
        /// <param name="name">The argument name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="false"/>.
        /// </exception>
        public static void ArgumentTrue ( bool argument , string name )
        {
            if ( !argument )
            {
                throw new ArgumentException (
                    name ,
                    string.Format (
                        CultureInfo.InvariantCulture ,
                    "Argument '{0}' cannot be false." , name ) );
            }
        }

        /// <summary>
        /// Checks the value of the supplied <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentNullException"/> if it is <see langword="false"/>.
        /// </summary>
        /// <param name="argument">The object to check.</param>
        /// <param name="name">The argument name.</param>
        /// <param name="message">
        /// An arbitrary message that will be passed to any thrown
        /// <see cref="System.ArgumentException"/>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="false"/>.
        /// </exception>
        public static void ArgumentTrue ( object argument , string name , string message )
        {
            if ( argument == null )
            {
                throw new ArgumentException ( name , message );
            }
        }

        /// <summary>
        /// Checks the value of the supplied string <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentException"/> if it is <see langword="null"/> or
        /// contains only whitespace character(s).
        /// </summary>
        /// <param name="argument">The string to check.</param>
        /// <param name="name">The argument name.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/> or
        /// contains only whitespace character(s).
        /// </exception>
        public static void ArgumentHasText ( string argument , string name )
        {
            if ( string.IsNullOrEmpty ( argument ) )
            {
                throw new ArgumentNullException (
                    name ,
                    string.Format (
                    CultureInfo.InvariantCulture ,
                    "Argument '{0}' cannot be null or resolve to an empty string : '{1}'." , name , argument ) );
            }
        }

        /// <summary>
        /// Checks the value of the supplied string <paramref name="argument"/> and throws an
        /// <see cref="System.ArgumentNullException"/> if it is <see langword="null"/> or
        /// contains only whitespace character(s).
        /// </summary>
        /// <param name="argument">The string to check.</param>
        /// <param name="name">The argument name.</param>
        /// <param name="message">
        /// An arbitrary message that will be passed to any thrown
        /// <see cref="System.ArgumentException"/>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="argument"/> is <see langword="null"/> or
        /// contains only whitespace character(s).
        /// </exception>
        public static void ArgumentHasText ( string argument , string name , string message )
        {
            if ( string.IsNullOrEmpty ( argument ) )
            {
                throw new ArgumentNullException ( name , message );
            }
        }

        /// <summary>
        /// Checks whether the specified <paramref name="argument"/> can be cast 
        /// into the <paramref name="requiredType"/>.
        /// </summary>
        /// <param name="argument">
        /// The argument to check.
        /// </param>
        /// <param name="argumentName">
        /// The name of the argument to check.
        /// </param>
        /// <param name="requiredType">
        /// The required type for the argument.
        /// </param>
        /// <param name="message">
        /// An arbitrary message that will be passed to any thrown
        /// <see cref="System.ArgumentException"/>.
        /// </param>
        public static void AssertArgumentType ( object argument , string argumentName , Type requiredType , string message )
        {
            if ( argument != null && requiredType != null && !requiredType.IsAssignableFrom ( argument.GetType ( ) ) )
            {
                throw new ArgumentException ( message , argumentName );
            }
        }
    }
}