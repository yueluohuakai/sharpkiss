using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.Remoting;

namespace Kiss.Utils
{
    /// <summary>
    /// Helper methods with regard to objects, types, properties, etc.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Not intended to be used directly by applications.
    /// </p>
    /// </remarks>
    internal sealed class ObjectUtils
    {
        #region Constants

        /// <summary>
        /// An empty object array.
        /// </summary>
        public static readonly object[] EmptyObjects = new object[ ] { };

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This is a utility class, and as such exposes no public constructors.
        /// </p>
        /// </remarks>
        private ObjectUtils ( )
        {
        }

        // CLOVER:ON

        #endregion

        #region Methods

        /// <summary>
        /// Convenience method to instantiate a <see cref="System.Type"/> using
        /// its no-arg constructor.
        /// </summary>
        /// <remarks>
        /// <p>
        /// As this method doesn't try to instantiate <see cref="System.Type"/>s
        /// by name, it should avoid <see cref="System.Type"/> loading issues.
        /// </p>
        /// </remarks>
        /// <param name="type">
        /// The <see cref="System.Type"/> to instantiate</param>
        /// <returns>A new instance of the <see cref="System.Type"/>.</returns>        
        public static object InstantiateType ( Type type )
        {
            if ( type == null )
            {
                throw new FatalObjectException (
                    "Cannot instantiate a null Type." );
            }
            if ( type.IsInterface )
            {
                throw new FatalObjectException (
                    string.Format (
                        CultureInfo.InvariantCulture , "Cannot instantiate an interface {0}." , type ) );
            }
            if ( type.IsAbstract )
            {
                throw new FatalObjectException (
                    string.Format (
                        CultureInfo.InvariantCulture , "Cannot instantiate an abstract class {0}." , type ) );
            }
            ConstructorInfo constructor = type.GetConstructor ( Type.EmptyTypes );
            if ( constructor == null )
            {
                throw new FatalObjectException (
                    string.Format (
                        CultureInfo.InvariantCulture , "Cannot instantiate a class that does not have a public no-argument constructor [{0}]." , type ) );
            }
            return ObjectUtils.InstantiateType ( constructor , ObjectUtils.EmptyObjects );
        }

        /// <summary>
        /// Convenience method to instantiate a <see cref="System.Type"/> using
        /// the given constructor.
        /// </summary>
        /// <remarks>
        /// <p>
        /// As this method doesn't try to instantiate <see cref="System.Type"/>s
        /// by name, it should avoid <see cref="System.Type"/> loading issues.
        /// </p>
        /// </remarks>
        /// <param name="constructor">
        /// The constructor to use for the instantiation.
        /// </param>
        /// <param name="arguments">
        /// The arguments to be passed to the constructor.
        /// </param>
        /// <returns>A new instance.</returns>        
        public static object InstantiateType ( ConstructorInfo constructor , object[ ] arguments )
        {
            if ( constructor == null )
            {
                throw new FatalObjectException (
                    "Cannot instantiate Type using a null constructor argument; " +
                        "does the Type have a no-arg constructor? Is the Type an interface?" );
            }

            if ( constructor.DeclaringType.ContainsGenericParameters )
            {
                throw new FatalObjectException (
                    string.Format (
                        CultureInfo.InvariantCulture , "Cannot instantiate an open generic type [{0}]." , constructor.DeclaringType ) );
            }
            try
            {
                return constructor.Invoke ( arguments );
            }
            catch ( Exception ex )
            {
                Type ctorType = constructor.DeclaringType;
                throw new FatalObjectException (
                    string.Format (
                        CultureInfo.InvariantCulture ,
                        "Cannot instantiate Type [{0}] using ctor [{1}] : '{2}'" , ctorType , constructor , ex.Message ) ,
                    ex );
            }
        }

        /// <summary>
        /// Checks whether the supplied <paramref name="instance"/> is not a transparent proxy and is
        /// assignable to the supplied <paramref name="type"/>. 
        /// </summary>
        /// <remarks>
        /// <p>
        /// Neccessary when dealing with server-activated remote objects, because the
        /// object is of the type TransparentProxy and regular <c>is</c> testing for assignable
        /// types does not work.
        /// </p>
        /// <p>
        /// Transparent proxy instances always return <see langword="true"/> when tested
        /// with the <c>'is'</c> operator (C#). This method only checks if the object
        /// is assignable to the type if it is not a transparent proxy.
        /// </p>
        /// </remarks>
        /// <param name="type">The target <see cref="System.Type"/> to be checked.</param>
        /// <param name="instance">The value that should be assigned to the type.</param>
        /// <returns>
        /// <see langword="true"/> if the supplied <paramref name="instance"/> is not a
        /// transparent proxy and is assignable to the supplied <paramref name="type"/>.
        /// </returns>
        public static bool IsAssignableAndNotTransparentProxy ( Type type , object instance )
        {
            if ( !RemotingServices.IsTransparentProxy ( instance ) )
            {
                return IsAssignable ( type , instance );
            }
            return false;
        }

        /// <summary>
        /// Determine if the given <see cref="System.Type"/> is assignable from the
        /// given value, assuming setting by reflection.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Considers primitive wrapper classes as assignable to the
        /// corresponding primitive types.
        /// </p>
        /// <p>
        /// For example used in an object factory's constructor resolution.
        /// </p>
        /// </remarks>
        /// <param name="type">The target <see cref="System.Type"/>.</param>
        /// <param name="obj">The value that should be assigned to the type.</param>
        /// <returns>True if the type is assignable from the value.</returns>
        public static bool IsAssignable ( Type type , object obj )
        {
            return ( type.IsInstanceOfType ( obj ) ||
                ( !type.IsPrimitive && obj == null ) ||
                ( type.Equals ( typeof ( bool ) ) && obj is Boolean ) ||
                ( type.Equals ( typeof ( byte ) ) && obj is Byte ) ||
                ( type.Equals ( typeof ( char ) ) && obj is Char ) ||
                ( type.Equals ( typeof ( sbyte ) ) && obj is SByte ) ||
                ( type.Equals ( typeof ( int ) ) && obj is Int32 ) ||
                ( type.Equals ( typeof ( short ) ) && obj is Int16 ) ||
                ( type.Equals ( typeof ( long ) ) && obj is Int64 ) ||
                ( type.Equals ( typeof ( float ) ) && obj is Single ) ||
                ( type.Equals ( typeof ( double ) ) && obj is Double ) );
        }

        /// <summary>
        /// Check if the given <see cref="System.Type"/> represents a
        /// "simple" property,
        /// i.e. a primitive, a <see cref="System.String"/>, a
        /// <see cref="System.Type"/>, or a corresponding array.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Used to determine properties to check for a "simple" dependency-check.
        /// </p>
        /// </remarks>
        /// <param name="type">
        /// The <see cref="System.Type"/> to check.
        /// </param>
        public static bool IsSimpleProperty ( Type type )
        {
            return type.IsPrimitive
                || type.Equals ( typeof ( string ) )
                || type.Equals ( typeof ( string[ ] ) )
                || IsPrimitiveArray ( type )
                || type.Equals ( typeof ( Type ) )
                || type.Equals ( typeof ( Type[ ] ) );
        }

        /// <summary>
        /// Check if the given class represents a primitive array,
        /// i.e. boolean, byte, char, short, int, long, float, or double.
        /// </summary>
        public static bool IsPrimitiveArray ( Type type )
        {
            return typeof ( bool[ ] ).Equals ( type )
                || typeof ( sbyte[ ] ).Equals ( type )
                || typeof ( char[ ] ).Equals ( type )
                || typeof ( short[ ] ).Equals ( type )
                || typeof ( int[ ] ).Equals ( type )
                || typeof ( long[ ] ).Equals ( type )
                || typeof ( float[ ] ).Equals ( type )
                || typeof ( double[ ] ).Equals ( type );
        }

        /// <summary>
        /// Resolves the supplied type name into a <see cref="System.Type"/>
        /// instance.
        /// </summary>
        /// <remarks>
        /// <p>
        /// If you require special <see cref="System.Type"/> resolution, do
        /// <b>not</b> use this method, but rather instantiate
        /// </p>
        /// </remarks>
        /// <param name="typeName">
        /// The (possibly partially assembly qualified) name of a
        /// <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the type cannot be resolved.
        /// </exception>
        public static Type ResolveType ( string typeName )
        {
            Type type = TypeRegistry.ResolveType ( typeName );
            if ( type == null )
            {
                type = internalTypeResolver.Resolve ( typeName );
            }
            return type;
        }

        /// <summary>
        /// Determine if the given objects are equal, returning <see langword="true"/>
        /// if both are <see langword="null"/> respectively <see langword="false"/>
        /// if only one is <see langword="null"/>.
        /// </summary>
        /// <param name="o1">The first object to compare.</param>
        /// <param name="o2">The second object to compare.</param>
        /// <returns>
        /// <see langword="true"/> if the given objects are equal.
        /// </returns>
        public static bool NullSafeEquals ( object o1 , object o2 )
        {
            return ( o1 == o2 || ( o1 != null && o1.Equals ( o2 ) ) );
        }

        /// <summary>
        /// Returns the first element in the supplied <paramref name="enumerator"/>.
        /// </summary>
        /// <param name="enumerator">
        /// The <see cref="System.Collections.IEnumerator"/> to use to enumerate
        /// elements.
        /// </param>
        /// <returns>
        /// The first element in the supplied <paramref name="enumerator"/>.
        /// </returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// If the supplied <paramref name="enumerator"/> did not have any elements.
        /// </exception>
        public static object EnumerateFirstElement ( IEnumerator enumerator )
        {
            return ObjectUtils.EnumerateElementAtIndex ( enumerator , 0 );
        }

        /// <summary>
        /// Returns the first element in the supplied <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">
        /// The <see cref="System.Collections.IEnumerable"/> to use to enumerate
        /// elements.
        /// </param>
        /// <returns>
        /// The first element in the supplied <paramref name="enumerable"/>.
        /// </returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// If the supplied <paramref name="enumerable"/> did not have any elements.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="enumerable"/> is <see langword="null"/>.
        /// </exception>
        public static object EnumerateFirstElement ( IEnumerable enumerable )
        {
            AssertUtils.ArgumentNotNull ( enumerable , "enumerable" );
            return ObjectUtils.EnumerateElementAtIndex ( enumerable.GetEnumerator ( ) , 0 );
        }

        /// <summary>
        /// Returns the element at the specified index using the supplied
        /// <paramref name="enumerator"/>.
        /// </summary>
        /// <param name="enumerator">
        /// The <see cref="System.Collections.IEnumerator"/> to use to enumerate
        /// elements until the supplied <paramref name="index"/> is reached.
        /// </param>
        /// <param name="index">
        /// The index of the element in the enumeration to return.
        /// </param>
        /// <returns>
        /// The element at the specified index using the supplied
        /// <paramref name="enumerator"/>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If the supplied <paramref name="index"/> was less than zero, or the
        /// supplied <paramref name="enumerator"/> did not contain enough elements
        /// to be able to reach the supplied <paramref name="index"/>.
        /// </exception>
        public static object EnumerateElementAtIndex ( IEnumerator enumerator , int index )
        {
            if ( index < 0 )
            {
                throw new ArgumentOutOfRangeException ( );
            }
            object element = null;
            int i = 0;
            while ( enumerator.MoveNext ( ) )
            {
                element = enumerator.Current;
                if ( ++i > index )
                {
                    break;
                }
            }
            if ( i < index )
            {
                throw new ArgumentOutOfRangeException ( );
            }
            return element;
        }

        /// <summary>
        /// Returns the element at the specified index using the supplied
        /// <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">
        /// The <see cref="System.Collections.IEnumerable"/> to use to enumerate
        /// elements until the supplied <paramref name="index"/> is reached.
        /// </param>
        /// <param name="index">
        /// The index of the element in the enumeration to return.
        /// </param>
        /// <returns>
        /// The element at the specified index using the supplied
        /// <paramref name="enumerable"/>.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If the supplied <paramref name="index"/> was less than zero, or the
        /// supplied <paramref name="enumerable"/> did not contain enough elements
        /// to be able to reach the supplied <paramref name="index"/>.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="enumerable"/> is <see langword="null"/>.
        /// </exception>
        public static object EnumerateElementAtIndex ( IEnumerable enumerable , int index )
        {
            AssertUtils.ArgumentNotNull ( enumerable , "enumerable" );
            return ObjectUtils.EnumerateElementAtIndex ( enumerable.GetEnumerator ( ) , index );
        }


        private static readonly MethodInfo Exception_InternalPreserveStackTrace =
            typeof ( Exception ).GetMethod ( "InternalPreserveStackTrace" , BindingFlags.Instance | BindingFlags.NonPublic );

        /// <summary>
        /// Convenience method that uses reflection to invoke 
        /// a method using the given <see cref="System.Reflection.MethodInfo"/>.
        /// </summary>
        /// <remarks>
        /// <p>
        /// This method catch the <see cref="System.Reflection.TargetInvocationException"/> 
        /// and then re-throw the InnerException preserving the stack trace.
        /// </p>
        /// </remarks>
        /// <param name="method">
        /// The <see cref="System.Reflection.MethodInfo"/> to invoke.
        /// </param>
        /// <param name="instance">
        /// The target object instance on which to invoke the method.
        /// Ignored if the method is static.
        /// </param>
        /// <param name="arguments">
        /// The arguments to be passed to the method.
        /// </param>
        /// <returns>The return value of the invoked method</returns>
        /// <exception cref="System.Exception">
        /// If invoking the method resulted in an exception.
        /// </exception>
        public static object InvokeMethod ( MethodInfo method , object instance , object[ ] arguments )
        {
            object returnValue = null;
            try
            {
                returnValue = method.Invoke ( instance , arguments );
            }
            catch ( TargetInvocationException ex )
            {
                Exception_InternalPreserveStackTrace.Invoke ( ex.InnerException , new Object[ ] { } );

                throw ex.InnerException;
            }
            return returnValue;
        }

        #endregion

        #region Fields

        private static readonly ITypeResolver internalTypeResolver
			= new CachedTypeResolver ( new TypeResolver ( ) );

        #endregion
    }
}