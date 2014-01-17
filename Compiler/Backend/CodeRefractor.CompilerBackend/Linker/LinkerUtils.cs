using System;
using System.Reflection;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.FrontEnd;
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
            if (constValue == null)
            {
                return identifierValue.Name;
            }
            var computeType = identifierValue.ComputedType();
            if (computeType.ClrTypeCode == TypeCode.String)
            {
                var stringTable = LinkingData.Instance.Strings;
                var stringId = stringTable.GetStringId((string)constValue.Value);

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
            var declaringType = methodBase.DeclaringType;
            var typeToSearch = declaringType.ReversedType();
            var isGacType = typeToSearch.Assembly.GlobalAssemblyCache;

            if (isGacType)
                return null;
            return methodBase.Register();
        }
    }
}