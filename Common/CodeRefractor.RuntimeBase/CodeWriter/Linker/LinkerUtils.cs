#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

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
            if (Type.GetTypeCode(computeType.ClrType) == TypeCode.String)
            {
                
                var stringTable = LinkingData.Instance.Strings;
                var stringId = stringTable.GetStringId((string) constValue.Value);
                    
                return String.Format("_str({0})", stringId);
            }
            return constValue.Name;
        }

        public static MethodInterpreter GetInterpreter(this CallMethodStatic callMethodStatic, ClosureEntities crRuntime)
        {
            return crRuntime.ResolveMethod(callMethodStatic.Info);
        }

        public static MethodInterpreter GetInterpreter(this MethodBase methodBase, ClosureEntities crRuntime)
        {
            return crRuntime.ResolveMethod(methodBase);
        }

        public const string EscapeName = "NonEscapingArgs";

        public static Dictionary<int, bool> EscapingParameterData(this MethodBase info, ClosureEntities crRuntime)
        {
            var interpreter = info.GetInterpreter(crRuntime) as CilMethodInterpreter;
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

        public static bool[] BuildEscapingBools(this MethodBase method, ClosureEntities crRuntime)
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