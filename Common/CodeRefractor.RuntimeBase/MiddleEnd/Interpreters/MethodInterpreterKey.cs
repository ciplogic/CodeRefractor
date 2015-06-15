#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase.Shared;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters
{
    public class MethodInterpreterKey : IComparable
    {
        readonly string _methodName;
        readonly Type[] _parameterList;
        readonly int _hash;

        public MethodInterpreterKey(MethodInterpreter interpreter, Type implementingType = null)
        {
            Interpreter = interpreter;
            var methodBase = Interpreter.Method;
            DeclaringType = methodBase.DeclaringType;
            ImplementingType = implementingType ?? methodBase.DeclaringType;
            _methodName = methodBase.Name;

            var parameterList = new List<Type>();
            if (!methodBase.IsStatic)
                parameterList.Add(methodBase.DeclaringType);
            parameterList.AddRange(methodBase.GetParameters().Select(par => par.ParameterType).ToList());
            var returnType = methodBase.GetReturnType();
            if (returnType != typeof (void))
                parameterList.Add(returnType);
            _parameterList = parameterList.ToArray();

            _hash = ComputeHash();
        }

        public Type DeclaringType { get; set; }
        public Type ImplementingType { get; set; }
        public MethodInterpreter Interpreter { get; }

        public int CompareTo(object obj)
        {
            var objKey = obj as MethodInterpreterKey;
            if (objKey == null)
                return -2;
            if (_hash == objKey._hash)
                return 0;
            if (_hash < objKey._hash)
                return -1;

            return 1;
        }

        public void AdjustDeclaringTypeByImplementingType()
        {
            var value = ImplementingType;
            var customAttr = value.GetCustomAttributeT<ExtensionsImplementation>();
            if (customAttr != null)
            {
                DeclaringType = customAttr.DeclaringType;
            }
        }


        int ComputeHash()
        {
            var baseHash = _methodName.GetHashCode();
            foreach (var parameter in _parameterList)
            {
                baseHash = (6 * baseHash) ^ parameter.GetHashCode();
            }
            return baseHash;
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override string ToString()
        {
            var declaringType = DeclaringType.ToCppMangling();
            var functionParams = _parameterList
                .Select(par => par.ToCppMangling())
                .ToArray();
            var paramString = string.Join(", ", functionParams);
            return $"{declaringType}.{_methodName}({paramString})";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var toCompareObj = (MethodInterpreterKey) obj;
            if (_hash != toCompareObj._hash)
                return false;
            if (_parameterList.Length != toCompareObj._parameterList.Length)
                return false;
            if (_methodName != toCompareObj._methodName)
                return false;
            for (var index = 0; index < _parameterList.Length; index++)
            {
                var type = _parameterList[index];
                var compareType = toCompareObj._parameterList[index];
                if (type != compareType)
                    return false;
            }

            return true;
        }
    }
}