using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Kiss.Utils;

namespace Kiss
{

    /// <summary>
    /// A class that finds types by looping assemblies in the 
    /// currently executing AppDomain. Only assemblies whose names matches
    /// certain patterns are investigated and an optional list of assemblies
    /// referenced by <see cref="AssemblyNames"/> are always investigated.
    /// </summary>
    public class AppDomainTypeFinder : ITypeFinder
    {
        private string assemblySkipLoadingPattern = "^System|^mscorlib";

        #region props

        /// <summary>The app domain to look for types in.</summary>
        public virtual AppDomain App
        {
            get { return AppDomain.CurrentDomain; }
        }

        /// <summary>Gets the pattern for dlls that we know don't need to be investigated for content items.</summary>
        public string AssemblySkipLoadingPattern
        {
            get { return assemblySkipLoadingPattern; }
            set { assemblySkipLoadingPattern = value; }
        }

        #endregion

        /// <summary>Finds types assignable from of a certain type in the app domain.</summary>
        /// <param name="requestedType">The type to find.</param>
        /// <returns>A list of types found in the app domain.</returns>
        public virtual IList<Type> Find(Type requestedType)
        {
            List<Type> types = new List<Type>();
            foreach (Assembly a in GetAssemblies())
            {
                Type[] list;

                try
                {
                    list = a.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    list = ex.Types;

                    StringBuilder msg = new StringBuilder("Error getting types from assembly ");
                    msg.Append(a.FullName);

                    if (ex.LoaderExceptions != null && ex.LoaderExceptions.Length > 0)
                    {
                        msg.AppendLine();
                        msg.AppendLine("LoaderExceptions:");
                        foreach (Exception e in ex.LoaderExceptions)
                        {
                            msg.AppendLine(ExceptionUtil.WriteException(e));
                        }
                    }

                    if (ex.Types != null && ex.Types.Length > 0)
                    {
                        msg.AppendLine();
                        msg.AppendLine("Types:");
                        foreach (var item in ex.Types)
                        {
                            if (item == null) continue;
                            msg.AppendLine(item.Name);
                        }
                    }

                    msg.AppendLine(ExceptionUtil.WriteException(ex));

                    LogManager.GetLogger<AppDomainTypeFinder>().Error(msg.ToString());
                }

                foreach (Type t in list)
                {
                    if (t == null) continue;

                    if ((requestedType.IsInterface && t.GetInterface(requestedType.Name) != null) || requestedType.IsAssignableFrom(t))
                        types.Add(t);
                }

            }

            return types;
        }

        /// <summary>Gets tne assemblies related to the current implementation.</summary>
        /// <returns>A list of assemblies that should be loaded by the factory.</returns>
        public virtual IList<Assembly> GetAssemblies()
        {
            List<string> addedAssemblyNames = new List<string>();
            List<Assembly> assemblies = new List<Assembly>();

            AddAssembliesInAppDomain(addedAssemblyNames, assemblies);

            LoadMatchingAssemblies(addedAssemblyNames, App.BaseDirectory);

            return assemblies;
        }

        /// <summary>Iterates all assemblies in the AppDomain and if it's name matches the configured patterns add it to our list.</summary>
        /// <param name="addedAssemblyNames"></param>
        /// <param name="assemblies"></param>
        protected void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (Matches(assembly.FullName))
                {
                    if (!addedAssemblyNames.Contains(assembly.FullName))
                    {
                        assemblies.Add(assembly);
                        addedAssemblyNames.Add(assembly.FullName);
                    }
                }
            }
        }

        /// <summary>Check if a dll is one of the shipped dlls that we know don't need to be investigated.</summary>
        /// <param name="assemblyFullName">The name of the assembly to check.</param>
        /// <returns>True if the assembly should be loaded</returns>
        public virtual bool Matches(string assemblyFullName)
        {
            return !Matches(assemblyFullName, AssemblySkipLoadingPattern);
        }

        /// <summary>Check if a dll is one of the shipped dlls that we know don't need to be investigated.</summary>
        /// <param name="assemblyFullName">The assembly name to match.</param>
        /// <param name="pattern">The regular expression pattern to match against the assembly name.</param>
        /// <returns>True if the pattern matches the assembly name.</returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Makes sure matching assemblies in the supplied folder are loaded in the app domain.
        /// </summary>
        protected virtual void LoadMatchingAssemblies(List<string> loadedAssemblyNames, string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            foreach (string dllPath in Directory.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    Assembly a = Assembly.ReflectionOnlyLoadFrom(dllPath);
                    if (Matches(a.FullName) && !loadedAssemblyNames.Contains(a.FullName))
                    {
                        App.Load(a.FullName);
                    }
                }
                catch (BadImageFormatException)
                {
                    LogManager.GetLogger<AppDomainTypeFinder>().Debug("{0} is not a valid .net assembly.", dllPath);
                }
            }
        }

        /// <summary>
        /// find assembly of a certain name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Assembly FindAssembly(string name)
        {
            foreach (Assembly ass in GetAssemblies())
            {
                if (ass.GetName().Name == name || ass.FullName == name)
                    return ass;
            }

            return null;
        }
    }
}
