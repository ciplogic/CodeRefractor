#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public static class MethodInterpreterKeyUtils
    {
        public static MethodInterpreterKey ToKey(this MethodInterpreter methodInterpreter, Type implementingType = null)
        {
            var result = new MethodInterpreterKey(methodInterpreter, implementingType);
            return result;
        }

        public static MethodInterpreterKey ToKey(this MethodBase methodbase, ClosureEntities closure)
        {
            return closure.ResolveMethod(methodbase).ToKey();

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