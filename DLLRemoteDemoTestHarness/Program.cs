using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DLLRemoteDemoTestHarness.DLLRemoteServiceReference;
using Microsoft.CSharp;

namespace DLLRemoteDemoTestHarness
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var client = new WebServiceClient();

            var assemblies = client.GetRegisteredDLLs();
            var inputParameters = new List<object>();

            var library = client.GetLibraryInformation(@"C:\Remote DLL Library\DemoCalculator.dll");

            Console.WriteLine("DLL Path: {0}, {1} methods total", library.Path, library.Methods.Count());
            library.Methods.ToList().ForEach(method =>
            {
                var sb = new StringBuilder();
                sb.Append("  " + method.ReturnType.ToString() + " " + method.Name + "(");
                method.Parameters.ToList().ForEach(param =>
                {
                    sb.Append(param.Type + " " + param.Name);
                    if (method.Parameters.Last() != param)
                    {
                        sb.Append(", ");
                    }
                });
                sb.Append(")");
                Console.WriteLine(sb);

            });
            Console.WriteLine(Environment.NewLine);

            bool validInput = false;
            string input = null;
            while (!validInput)
            {
                Console.Write("Please enter a method to execute: ");
                input = Console.ReadLine();
                if (!library.ContainsMethod(input))
                {
                    Console.WriteLine("ERROR: Not a method of assembly. Please enter a valid method.");
                }
                else
                {
                    validInput = true;
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine("Now entering method parameters.");

                    var method = library.GetMethod(input);
                    method.Parameters.ForEach(param =>
                    {
                        Console.Write(" {0} {1} = ", param.Type, param.Name);
                        inputParameters.Add(Console.ReadLine());
                    });
                }
            }

            Console.WriteLine("User input {0} parameters. Now remotely executing method...", inputParameters.Count);

            //Make all possible casts
            var tempParams = new List<object>();
            for (int i = 0; i < inputParameters.Count; i++)
            {
                var tempparam = library.GetMethod(input).Parameters[i];

                Type type = Type.GetType(tempparam.Namespace + '.' + tempparam.Type);

                var itemToAdd = Convert.ChangeType(inputParameters[i], type);
                if (type != null && !tempParams.Contains(itemToAdd))
                    tempParams.Add(itemToAdd);
                
            }

            if (inputParameters.Count == tempParams.Count)
                inputParameters = tempParams;

            var response = client.ExecuteMethod(library.Path, input, inputParameters.ToArray());

            Console.WriteLine("Response: {0}", response);

            Console.ReadLine();
        }
    }
}