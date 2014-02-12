#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace Compiler.CodeWriter.Linker
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
            var declaringType = methodBase.DeclaringType;
            var typeToSearch = declaringType.ReversedType();
            var isGacType = typeToSearch.Assembly.GlobalAssemblyCache;

            if (isGacType)
                return null;
            return methodBase.Register();
        }

        public const string EscapeName = "NonEscapingArgs";

        public static Dictionary<int, bool> EscapingParameterData(this MethodBase info)
        {
            var interpreter = info.GetInterpreter();
            if (interpreter == null)
                return null;
            var calledMethod = interpreter.MidRepresentation;
            var otherMethodData = (Dictionary<int, bool>) calledMethod.GetAdditionalProperty(EscapeName);
            if (otherMethodData == null)
            {
                return null;
            }
            return otherMethodData;
        }
    }
}