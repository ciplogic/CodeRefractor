using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Runtime;

namespace CodeRefractor.CodeWriter.Linker
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

        public static MethodInterpreter GetInterpreter(this MethodData methodData, CrRuntimeLibrary crRuntime)
        {
            var methodBase = methodData.Info;
            return GetInterpreter(methodBase, crRuntime);
        }

        public static MethodInterpreter GetInterpreter(this MethodBase methodBase, CrRuntimeLibrary crRuntime)
        {
            var declaringType = methodBase.DeclaringType;
            var typeToSearch = declaringType.ReversedType(crRuntime);
            var isGacType = typeToSearch.Assembly.GlobalAssemblyCache;
            if (isGacType)
            {
                var interpreter = new MethodInterpreter(methodBase);
                if (crRuntime.ResolveInterpreter(interpreter.ToKey()))
                {
                    return interpreter;
                }
            
                return null;
            }
            return methodBase.Register();
        }

        public const string EscapeName = "NonEscapingArgs";

        public static Dictionary<int, bool> EscapingParameterData(this MethodBase info, CrRuntimeLibrary crRuntime)
        {
            var interpreter = info.GetInterpreter(crRuntime);
            if (interpreter == null)
                return null;
            var calledMethod = interpreter.MidRepresentation;
            var otherMethodData = (Dictionary<int, bool>)calledMethod.GetAdditionalProperty(EscapeName);
            if (otherMethodData == null)
            {
                return null;
            }
            return otherMethodData;
        }
        public static bool[] BuildEscapingBools(this MethodBase method, CrRuntimeLibrary crRuntime)
        {
            var parameters = method.GetParameters();
            var escapingBools = new bool[parameters.Length + 1];

            var escapeData = method.EscapingParameterData(crRuntime);
            if (escapeData != null)
            {
                foreach (var escaping in escapeData)
                {
                    if (escaping.Value)
                        escapingBools[escaping.Key] = true;
                }
            }
            else
            {
                for (var index = 0; index <= parameters.Length; index++)
                {
                    escapingBools[index] = true;
                }
            }
            return escapingBools;
        }
    }
}