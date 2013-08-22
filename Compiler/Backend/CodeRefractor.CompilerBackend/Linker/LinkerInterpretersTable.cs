using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.CompilerBackend.OuputCodeWriter;
using System.Reflection;

namespace CodeRefractor.CompilerBackend.Linker
{
    class LinkerInterpretersTable
    {
        static LinkerInterpretersTable()
        {
            Instance = new LinkerInterpretersTable();
        }
        public Dictionary<string, MetaMidRepresentation> Methods = 
            new Dictionary<string, MetaMidRepresentation>();
        public static LinkerInterpretersTable Instance { get; private set; }
        public static void Register(MetaMidRepresentation method)
        {
            var methodName = method.Method.WriteHeaderMethod(false);
            Instance.Methods[methodName] = method;
        }

        public static MetaMidRepresentation GetMethod(MethodBase midrepresentation)
        {
            var methodName = midrepresentation.WriteHeaderMethod(false);
            MetaMidRepresentation result;
            if (!Instance.Methods.TryGetValue(methodName, out result)) 
                return null;
            return result;
        }

    }
}
