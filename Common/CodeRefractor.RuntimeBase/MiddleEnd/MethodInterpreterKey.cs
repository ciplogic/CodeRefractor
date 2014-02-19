using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MethodInterpreterKey
    {
        private readonly MethodInterpreter _interpreter;
        private readonly int _hash;
        private readonly string _methodName;
        private readonly Type[] _parameterList;


        public MethodInterpreterKey(MethodInterpreter interpreter)
        {
            _interpreter = interpreter;
            var methodBase = _interpreter.Method;
            _methodName = methodBase.Name;

            var parameterList = new List<Type>();
            if (!methodBase.IsStatic)
                parameterList.Add(methodBase.DeclaringType);
            parameterList.AddRange(methodBase.GetParameters().Select(par => par.ParameterType).ToList());
            var returnType = methodBase.GetReturnType();
            if (returnType != typeof(void))
                parameterList.Add(returnType);
            _parameterList = parameterList.ToArray();

            _hash = ComputeHash();
        }

        private int ComputeHash()
        {
            var baseHash = _methodName.GetHashCode();
            foreach (var parameter in _parameterList)
            {
                baseHash = (6*baseHash) ^ parameter.GetHashCode();
            }
            return baseHash;
        }

        public MethodInterpreter Interpreter
        {
            get { return _interpreter; }
        }

        public override int GetHashCode()
        {
            return _hash;
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
            for (int index = 0; index < _parameterList.Length; index++)
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