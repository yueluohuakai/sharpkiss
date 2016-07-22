using System;
using System.Collections;
using System.Reflection;

namespace Kiss.Utils
{
    /// <summary>
    /// Resolves a <see cref="System.Type"/> by name.
    /// </summary>
    internal class TypeResolver : ITypeResolver
    {
        /// <summary>
        /// Resolves the supplied <paramref name="typeName"/> to a
        /// <see cref="System.Type"/> instance.
        /// </summary>
        /// <param name="typeName">
        /// The unresolved name of a <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the supplied <paramref name="typeName"/> could not be resolved
        /// to a <see cref="System.Type"/>.
        /// </exception>
        public Type Resolve ( string typeName )
        {
            Type type = ResolveGenericType ( typeName );
            if ( type == null )
            {
                type = ResolveType ( typeName );
            }
            return type;
            //return ResolveType ( typeName );
        }

        /// <summary>
        /// Resolves the supplied generic <paramref name="typeName"/>,
        /// substituting recursively all its type parameters., 
        /// to a <see cref="System.Type"/> instance.
        /// </summary>
        /// <param name="typeName">
        /// The (possibly generic) name of a <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the supplied <paramref name="typeName"/> could not be resolved
        /// to a <see cref="System.Type"/>.
        /// </exception>
        private Type ResolveGenericType ( string typeName )
        {
            if ( string.IsNullOrEmpty ( typeName ) )
            {
                throw BuildTypeLoadException ( typeName );
            }
            GenericArgumentsInfo genericInfo = new GenericArgumentsInfo ( typeName );
            Type type = null;
            try
            {
                if ( genericInfo.ContainsGenericArguments )
                {
                    type = ObjectUtils.ResolveType ( genericInfo.GenericTypeName );
                    if ( !genericInfo.IsGenericDefinition )
                    {
                        string[] unresolvedGenericArgs = genericInfo.GetGenericArguments ( );
                        Type[] genericArgs = new Type[ unresolvedGenericArgs.Length ];
                        for ( int i = 0 ; i < unresolvedGenericArgs.Length ; i++ )
                        {
                            genericArgs[ i ] = ObjectUtils.ResolveType ( unresolvedGenericArgs[ i ] );
                        }
                        type = type.MakeGenericType ( genericArgs );
                    }
                }
            }
            catch ( Exception ex )
            {
                if ( ex is TypeLoadException )
                {
                    throw;
                }
                throw BuildTypeLoadException ( typeName , ex );
            }
            return type;
        }

        /// <summary>
        /// Resolves the supplied <paramref name="typeName"/> to a
        /// <see cref="System.Type"/>
        /// instance.
        /// </summary>
        /// <param name="typeName">
        /// The (possibly partially assembly qualified) name of a
        /// <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the supplied <paramref name="typeName"/> could not be resolved
        /// to a <see cref="System.Type"/>.
        /// </exception>
        private Type ResolveType ( string typeName )
        {
            if ( string.IsNullOrEmpty ( typeName ) )
            {
                throw BuildTypeLoadException ( typeName );
            }
            TypeAssemblyInfo typeInfo = new TypeAssemblyInfo ( typeName );
            Type type = null;
            try
            {
                type = ( typeInfo.IsAssemblyQualified ) ?
                     LoadTypeDirectlyFromAssembly ( typeInfo ) :
                     LoadTypeByIteratingOverAllLoadedAssemblies ( typeInfo );
            }
            catch ( Exception ex )
            {
                throw BuildTypeLoadException ( typeName , ex );
            }
            if ( type == null )
            {
                throw BuildTypeLoadException ( typeName );
            }
            return type;
        }

        /// <summary>
        /// Uses <see cref="System.Reflection.Assembly.LoadWithPartialName(string)"/>
        /// to load an <see cref="System.Reflection.Assembly"/> and then the attendant
        /// <see cref="System.Type"/> referred to by the <paramref name="typeInfo"/>
        /// parameter.
        /// </summary>
        /// <remarks>
        /// <p>
        /// <see cref="System.Reflection.Assembly.LoadWithPartialName(string)"/> is
        /// deprecated in .NET 2.0, but is still used here (even when this class is
        /// compiled for .NET 2.0);
        /// <see cref="System.Reflection.Assembly.LoadWithPartialName(string)"/> will
        /// still resolve (non-.NET Framework) local assemblies when given only the
        /// display name of an assembly (the behaviour for .NET Framework assemblies
        /// and strongly named assemblies is documented in the docs for the
        /// <see cref="System.Reflection.Assembly.LoadWithPartialName(string)"/> method).
        /// </p>
        /// </remarks>
        /// <param name="typeInfo">
        /// The assembly and type to be loaded.
        /// </param>
        /// <returns>
        /// A <see cref="System.Type"/>, or <see lang="null"/>.
        /// </returns>
        /// <exception cref="System.Exception">
        /// <see cref="System.Reflection.Assembly.LoadWithPartialName(string)"/>
        /// </exception>
        private static Type LoadTypeDirectlyFromAssembly ( TypeAssemblyInfo typeInfo )
        {
            Type type = null;
            Assembly assembly = Assembly.Load ( typeInfo.AssemblyName );
            if ( assembly != null )
            {
                type = assembly.GetType ( typeInfo.TypeName , true , true );
            }
            return type;
        }

        /// <summary>
        /// Uses <see cref="M:System.AppDomain.CurrentDomain.GetAssemblies()"/>
        /// to load the attendant <see cref="System.Type"/> referred to by 
        /// the <paramref name="typeInfo"/> parameter.
        /// </summary>
        /// <param name="typeInfo">
        /// The type to be loaded.
        /// </param>
        /// <returns>
        /// A <see cref="System.Type"/>, or <see lang="null"/>.
        /// </returns>
        private static Type LoadTypeByIteratingOverAllLoadedAssemblies ( TypeAssemblyInfo typeInfo )
        {
            Type type = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies ( );
            foreach ( Assembly assembly in assemblies )
            {
                type = assembly.GetType ( typeInfo.TypeName , false , false );
                if ( type != null )
                {
                    break;
                }
            }
            return type;
        }

        private static TypeLoadException BuildTypeLoadException ( string typeName )
        {
            return new TypeLoadException ( "Could not load type from string value '" + typeName + "'." );
        }

        private static TypeLoadException BuildTypeLoadException ( string typeName , Exception ex )
        {
            return new TypeLoadException ( "Could not load type from string value '" + typeName + "'." , ex );
        }


        #region Inner Class : GenericArgumentsInfo

        /// <summary>
        /// Holder for the generic arguments when using type parameters.
        /// </summary>
        /// <remarks>
        /// <p>
        /// Type parameters can be applied to classes, interfaces, 
        /// structures, methods, delegates, etc...
        /// </p>
        /// </remarks>
        internal class GenericArgumentsInfo
        {
            #region Constants

            /// <summary>
            /// The generic arguments prefix.
            /// </summary>
            public const char GenericArgumentsPrefix = '<';

            /// <summary>
            /// The generic arguments suffix.
            /// </summary>
            public const char GenericArgumentsSuffix = '>';

            /// <summary>
            /// The character that separates a list of generic arguments.
            /// </summary>
            public const char GenericArgumentsSeparator = ',';

            #endregion

            #region Fields

            private string unresolvedGenericTypeName;
            private string unresolvedGenericMethodName;
            private string[] unresolvedGenericArguments;

            #endregion

            #region Constructor (s) / Destructor

            /// <summary>
            /// Creates a new instance of the GenericArgumentsInfo class.
            /// </summary>
            /// <param name="value">
            /// The string value to parse looking for a generic definition
            /// and retrieving its generic arguments.
            /// </param>
            public GenericArgumentsInfo ( string value )
            {
                ParseGenericArguments ( value );
            }

            #endregion

            #region Properties

            /// <summary>
            /// The (unresolved) generic type name portion 
            /// of the original value when parsing a generic type.
            /// </summary>
            public string GenericTypeName
            {
                get { return unresolvedGenericTypeName; }
            }

            /// <summary>
            /// The (unresolved) generic method name portion 
            /// of the original value when parsing a generic method.
            /// </summary>
            public string GenericMethodName
            {
                get { return unresolvedGenericMethodName; }
            }

            /// <summary>
            /// Is the string value contains generic arguments ?
            /// </summary>
            /// <remarks>
            /// <p>
            /// A generic argument can be a type parameter or a type argument.
            /// </p>
            /// </remarks>
            public bool ContainsGenericArguments
            {
                get
                {
                    return ( unresolvedGenericArguments != null &&
                        unresolvedGenericArguments.Length > 0 );
                }
            }

            /// <summary>
            /// Is generic arguments only contains type parameters ?
            /// </summary>
            public bool IsGenericDefinition
            {
                get
                {
                    if ( unresolvedGenericArguments == null )
                        return false;

                    foreach ( string arg in unresolvedGenericArguments )
                    {
                        if ( arg.Length > 0 )
                            return false;
                    }
                    return true;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Returns an array of unresolved generic arguments types.
            /// </summary>
            /// <remarks>
            /// <p>
            /// A empty string represents a type parameter that 
            /// did not have been substituted by a specific type.
            /// </p>
            /// </remarks>
            /// <returns>
            /// An array of strings that represents the unresolved generic 
            /// arguments types or an empty array if not generic.
            /// </returns>
            public string[ ] GetGenericArguments ( )
            {
                if (unresolvedGenericArguments == null)
                    return new string[] { };

                return unresolvedGenericArguments;
            }

            private void ParseGenericArguments ( string originalString )
            {
                int argsStartIndex
                    = originalString.IndexOf ( GenericArgumentsPrefix );
                if ( argsStartIndex < 0 )
                {
                    unresolvedGenericTypeName = originalString;
                    unresolvedGenericMethodName = originalString;
                }
                else
                {
                    int argsEndIndex =
                        originalString.LastIndexOf ( GenericArgumentsSuffix );
                    if ( argsEndIndex != -1 )
                    {
                        unresolvedGenericMethodName = originalString.Remove (
                            argsStartIndex , argsEndIndex - argsStartIndex + 1 );

                        SplitGenericArguments ( originalString.Substring (
                            argsStartIndex + 1 , argsEndIndex - argsStartIndex - 1 ) );

                        unresolvedGenericTypeName = originalString.Replace (
                            originalString.Substring ( argsStartIndex , argsEndIndex - argsStartIndex + 1 ) ,
                            "`" + unresolvedGenericArguments.Length );
                    }
                }
            }

            private void SplitGenericArguments ( string originalArgs )
            {
                IList args = new ArrayList ( );

                int index = 0;
                int cursor = originalArgs.IndexOf ( GenericArgumentsSeparator , index );
                while ( cursor != -1 )
                {
                    string arg = originalArgs.Substring ( index , cursor - index );
                    if ( arg.Split ( GenericArgumentsPrefix ).Length ==
                        arg.Split ( GenericArgumentsSuffix ).Length )
                    {
                        args.Add ( arg.Trim ( ) );
                        index = cursor + 1;
                    }
                    cursor = originalArgs.IndexOf ( GenericArgumentsSeparator , cursor + 1 );
                }
                args.Add ( originalArgs.Substring ( index , originalArgs.Length - index ).Trim ( ) );

                unresolvedGenericArguments = new string[ args.Count ];
                args.CopyTo ( unresolvedGenericArguments , 0 );
            }

            #endregion
        }

        #endregion


        #region Inner Class : TypeAssemblyInfo

        /// <summary>
        /// Holds data about a <see cref="System.Type"/> and it's
        /// attendant <see cref="System.Reflection.Assembly"/>.
        /// </summary>
        internal class TypeAssemblyInfo
        {
            #region Constants

            /// <summary>
            /// The string that separates a <see cref="System.Type"/> name
            /// from the name of it's attendant <see cref="System.Reflection.Assembly"/>
            /// in an assembly qualified type name.
            /// </summary>
            public const string TypeAssemblySeparator = ",";

            #endregion

            #region Fields

            private string unresolvedAssemblyName;
            private string unresolvedTypeName;

            #endregion

            #region Constructor (s) / Destructor

            /// <summary>
            /// Creates a new instance of the TypeAssemblyInfo class.
            /// </summary>
            /// <param name="unresolvedTypeName">
            /// The unresolved name of a <see cref="System.Type"/>.
            /// </param>
            public TypeAssemblyInfo ( string unresolvedTypeName )
            {
                SplitTypeAndAssemblyNames ( unresolvedTypeName );
            }

            #endregion

            #region Properties

            /// <summary>
            /// The (unresolved) type name portion of the original type name.
            /// </summary>
            public string TypeName
            {
                get { return unresolvedTypeName; }
            }

            /// <summary>
            /// The (unresolved, possibly partial) name of the attandant assembly.
            /// </summary>
            public string AssemblyName
            {
                get { return unresolvedAssemblyName; }
            }

            /// <summary>
            /// Is the type name being resolved assembly qualified?
            /// </summary>
            public bool IsAssemblyQualified
            {
                get { return !string.IsNullOrEmpty ( AssemblyName ); }
            }

            #endregion

            #region Methods

            private void SplitTypeAndAssemblyNames ( string originalTypeName )
            {
                int typeAssemblyIndex
                    = originalTypeName.IndexOf ( TypeAssemblySeparator );
                if ( typeAssemblyIndex < 0 )
                {
                    unresolvedTypeName = originalTypeName;
                }
                else
                {
                    unresolvedTypeName = originalTypeName.Substring (
                        0 , typeAssemblyIndex ).Trim ( );
                    unresolvedAssemblyName = originalTypeName.Substring (
                        typeAssemblyIndex + 1 ).Trim ( );
                }
            }

            #endregion
        }

        #endregion
    }
}
