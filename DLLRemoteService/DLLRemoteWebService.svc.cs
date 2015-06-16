using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using CommonTypes;
using DLLRemoteService.Data_Model;

namespace DLLRemoteService
{
    public class DLLRemoteWebService : IWebService
    {
        public DLLRemoteWebService()
        {
            ValidDLLs = new List<string>();
        }


        /// <summary>
        /// Ensures assemblies are valid and registered.
        /// </summary>
        /// <param name="path">Path to the DLL. Use paths from GetRegisteredDLLs()</param>
        /// <returns>Returns the loaded assembly, if the loading was successful.</returns>
        /// TODO: Find a way to enforce this on the server level
        private Assembly LoadAssembly(string path)
        {
            if (!ValidDLLs.Any())
                GetValidDLLs();

            if (!ValidDLLs.Contains(path))
                throw new Exception("Unable to load unregistered DLL.");
            else return Assembly.LoadFile(path); 
        }

        private List<string> GetValidDLLs()
        {
            var dataContext = new DataContext(ConfigurationManager.ConnectionStrings["DLLRemote"].ConnectionString);

            try
            {
                var RegisteredDLLs = dataContext.GetTable<RegisteredDLL>();
                var dllQuery =
                    from dll in RegisteredDLLs
                    select dll.DLLPath;

                ValidDLLs = dllQuery.ToList();
            }
            catch (Exception)
            {
            }

            return ValidDLLs;
        }

        public List<string> ValidDLLs { get; set; }

        /// <summary>
        /// Gets a list of registered DLLs.
        /// </summary>
        public List<string> GetRegisteredDLLs()
        {
            return GetValidDLLs();
        }

        /// <summary>
        /// Gets all methods of the DLL.
        /// </summary>
        /// <param name="path">Path to the DLL. Use paths from GetRegisteredDLLs()</param>
        /// <returns></returns>
        public List<string> GetDLLMethods(string path)
        {
            var assembly = LoadAssembly(path);
            if (assembly != null)
            {
                var names = (from type in assembly.GetTypes()
                    from method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    select type.FullName + ":" + method.Name).Distinct().ToList();
                return names;
            }
            return null;
        }

        /// <summary>
        /// Returns a Library object that is effectively a metadata class regarding the Path DLL. The Library object contains all methods and parameters.
        /// </summary>
        /// <param name="path">Path to the DLL. Use paths from GetRegisteredDLLs()</param>
        /// <returns>Library metadata object</returns>
        public Library GetLibraryInformation(string path)
        {
            var assembly = LoadAssembly(path);
            if (assembly != null)
            {
                var lib = new Library {Path = path};

                var methods = (from type in assembly.GetTypes()
                    from method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    select method).Distinct().ToList();

                methods.ForEach(method =>
                {
                    var temp = new Method {Name = method.Name, ReturnType = method.ReturnType.Name.ToString()};
                    method.GetParameters().OrderBy(param => param.Position)
                        .ToList()
                        .ForEach(
                            param =>
                                temp.Parameters.Add(new Parameter
                                {
                                    Type = param.ParameterType.Name,
                                    Name = param.Name,
                                    Ordinal = param.Position,
                                    Namespace = param.ParameterType.Namespace
                                }));
                    lib.Methods.Add(temp);
                });

                return lib;
            }
            return null;
        }

        /// <summary>
        /// Executes a method remotely.
        /// </summary>
        /// <param name="path">Path to the DLL. Use paths from GetRegisteredDLLs()</param>
        /// <param name="method">Name of the method.</param>
        /// <param name="parameters">List of parameters for the method. Must be in correct order (i.e. from Library object) and must be correctly typed or castable.</param>
        /// <returns></returns>
        public string ExecuteMethod(string path, string method, List<object> parameters)
        {
            string returnvalue = string.Empty;
            var assembly = LoadAssembly(path);
            if (assembly != null)
            {
                var methodObj = (from type in assembly.GetTypes()
                                 from tempmethod in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                 select tempmethod).Distinct().FirstOrDefault(m => String.Equals(m.Name, method, StringComparison.InvariantCultureIgnoreCase));

                if (methodObj != null)
                {
                    if (parameters != null) returnvalue = methodObj.Invoke(null, parameters.ToArray()).ToString();
                }
            }
            return returnvalue;
        }
    }
}