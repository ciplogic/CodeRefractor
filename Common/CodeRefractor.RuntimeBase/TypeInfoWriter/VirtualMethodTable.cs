#region Usings

using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;

#endregion

namespace CodeRefractor.RuntimeBase.TypeInfoWriter
{
    public class VirtualMethodTable
    {
        private readonly TypeDescriptionTable _typeTable;
        public readonly List<VirtualMethodDescription> VirtualMethods = new List<VirtualMethodDescription>();

        public VirtualMethodTable(TypeDescriptionTable typeTable)
        {
            _typeTable = typeTable;
        }

        public TypeDescriptionTable TypeTable
        {
            get { return _typeTable; }
        }

        public void RegisterMethod(MethodInfo method, Dictionary<MethodInterpreterKey, MethodInterpreter> methodClosure)
        {
            if (!method.IsVirtual || method.IsAbstract)
                return;
            foreach (var virtualMethod in VirtualMethods)
            {
                var matchFound = virtualMethod.MethodMatches(method);
                if (matchFound)
                    return;
            }
            var isInClosure = false;
            foreach (var interpreter in methodClosure)
            {
                var key = interpreter.Key;
                if (key.Interpreter.Kind != MethodKind.Default)
                    continue;
                var methodInKey = key.Interpreter.Method;
                if (methodInKey.Name != method.Name)
                    continue;
                var keyParameterInfos = methodInKey.GetParameters();
                var parameterInfos = method.GetParameters();
                if (keyParameterInfos.Length != parameterInfos.Length)
                    continue;
                var parameterMatch = true;
                for (var index = 0; index < keyParameterInfos.Length; index++)
                {
                    var keyParameterInfo = keyParameterInfos[index];
                    var parameterInfo = parameterInfos[index];
                    if (keyParameterInfo.ParameterType != parameterInfo.ParameterType)
                    {
                        parameterMatch = false;
                        break;
                    }
                }
                if (!parameterMatch)
                    continue;
                isInClosure = true;
            }
            if (!isInClosure)
                return;
            var declaringType = method.GetBaseDefinition().DeclaringType;
            var virtMethod = new VirtualMethodDescription(method, declaringType);
            virtMethod.MethodMatches(method);
            VirtualMethods.Add(virtMethod);
        }
    }
}