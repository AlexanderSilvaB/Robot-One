using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Reflection;
using UnityEngine;

namespace Scripts.GameLanguage
{
    public class CodeRunner
    {
        public CodeResults Compile(string code)
        {
            CodeResults result = new CodeResults();
            result.Code = "using Scripts.GameLanguage; public class APIInstance : API { public void Run() { " + code + " } }";
            Debug.Log(result.Code);

            CodeDomProvider code_provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add(Assembly.GetEntryAssembly().Location);

            CompilerResults results = code_provider.CompileAssemblyFromSource(parameters, result.Code);
            List<string> errors = new List<string>();
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError compiler_error in results.Errors)
                {
                    string error = 
                        "Line: " + compiler_error.Line + ", " +
                        "Error Number: " + compiler_error.ErrorNumber +
                        ", " + compiler_error.ErrorText + "\n";
                    errors.Add(error);
                    Debug.Log(error);
                }
                result.CompiledAssembly = null;
            }
            else
            {
                result.CompiledAssembly = results.CompiledAssembly;
                Debug.Log("Success");
            }
            result.Errors = errors;

            return result;
        }
        public void Run(CodeResults code)
        {
            if (!code.Success)
                return;
            foreach (Type type in code.CompiledAssembly.GetTypes())
            {
                Debug.Log(type.Name);
                if (!type.IsClass || type.IsNotPublic) continue;

                Debug.Log("Created");
                API api = (API)Activator.CreateInstance(type);
                api.Connect();
                Debug.Log("Connect");
                try
                {
                    Debug.Log("Running");
                    api.Run();
                    Debug.Log("Runned");
                }
                catch(Exception ex)
                {
                    Debug.Log(ex.Message);
                }
                api.Disconnect();
                Debug.Log("Disconnect");
            }
        }
    }
}