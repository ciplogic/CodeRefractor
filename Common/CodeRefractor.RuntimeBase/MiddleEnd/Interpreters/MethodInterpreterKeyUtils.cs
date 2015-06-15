#region Uses

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.ClosureCompute;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters
{
    public static class MethodInterpreterKeyUtils
    {
        public static MethodInterpreterKey ToKey(this MethodInterpreter methodInterpreter, Type implementingType = null)
        {
            var resultKey = new MethodInterpreterKey(methodInterpreter, implementingType);
            resultKey.AdjustDeclaringTypeByImplementingType();
            return resultKey;
        }

        public static MethodInterpreterKey ToKey(this MethodBase methodbase, ClosureEntities closure)
        {
            var interpreter = closure.ResolveMethod(methodbase);
            var resultKey = new MethodInterpreterKey(interpreter, null);
            resultKey.AdjustDeclaringTypeByImplementingType();
            return resultKey;
        }

        internal static Dictionary<Type, Type> ReversedTypeMap(this Dictionary<Type, Type> map)
        {
            var result = new Dictionary<Type, Type>();
            foreach (var item in map)
            {
                result[item.Value] = item.Key;
            }
            return result;
        }
    }
}