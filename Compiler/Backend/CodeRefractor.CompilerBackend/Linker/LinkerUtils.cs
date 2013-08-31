using System;
using System.Reflection;
using CodeRefractor.CompilerBackend.OuputCodeWriter;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Linker
{
    public static class LinkerUtils
    {
        public static string ComputedValue(this IdentifierValue identifierValue)
        {
            var constValue = identifierValue as ConstValue;
            if(constValue==null)
            {
                return identifierValue.Name;
            }
            var computeType = identifierValue.ComputedType();
            if (computeType == typeof(string))
            {
                var stringTable = LinkingData.Instance.Strings;
                var stringId = stringTable.GetStringId((string) constValue.Value);
                
                return String.Format("_str({0})", stringId);
            }
            return constValue.Name;
        }

        public static MethodInterpreter GetInterpreter(this MethodData methodData)
        {
            var methodBase = methodData.Info;
            return GetInterpreter(methodBase);
        }

        private static MethodInterpreter GetInterpreter(this MethodBase methodBase)
        {
            var typeData = (ClassTypeData) ProgramData.UpdateType(methodBase.DeclaringType);
            var interpreter = typeData.GetInterpreter(methodBase.ToString());
            return interpreter;
        }

        public static MethodInterpreter GetInterpreter(this MetaMidRepresentation methodData)
        {
            return methodData.Method.GetInterpreter();
        }

        public static MetaMidRepresentation GetMethod(this MethodBase midrepresentation)
        {
            var methodName = midrepresentation.WriteHeaderMethod(false);
            MetaMidRepresentation result;
            return !LinkerInterpretersTable.Instance.Methods.TryGetValue(methodName, out result) ? null : result;
        }
    }
}