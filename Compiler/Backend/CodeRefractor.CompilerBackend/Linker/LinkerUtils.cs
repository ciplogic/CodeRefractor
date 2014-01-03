using System;
using System.Reflection;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
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
            if (computeType.ClrTypeCode == TypeCode.String)
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

        public static MethodInterpreter GetInterpreter(this MethodBase methodBase)
        {
            var typeToSearch = methodBase.DeclaringType.ReversedType();
            var isGacType = typeToSearch.Assembly.GlobalAssemblyCache;
            
            if(isGacType)
                return null;
            var typeData = (ClassTypeData) ProgramData.UpdateType(methodBase.DeclaringType);
            var interpreter = typeData.GetInterpreter(methodBase);
            return interpreter;
        }

        public static MethodInterpreter GetInterpreter(this MetaMidRepresentation methodData)
        {
            return methodData.Method.GetInterpreter();
        }

        public static MethodInterpreter GetMethod(this MethodInterpreter midrepresentation)
        {
            var methodName = midrepresentation.Method.WriteHeaderMethod(false);
            MethodInterpreter result;
            return !LinkerInterpretersTable.Methods.TryGetValue(methodName, out result) ? null : result;
        }
    }
}