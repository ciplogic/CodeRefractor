using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public static class MethodInterpreterKeyUtils
    {
		public static MethodInterpreterKey ToKey(this MethodInterpreter methodInterpreter, Type implementingType = null)
        {
			var result = new MethodInterpreterKey(methodInterpreter, implementingType);
            return result;
        }

        public static MethodInterpreterKey ToKey(this MethodBase methodbase, ProgramClosure closure)
        {
            foreach (var m in closure.MethodClosure)
            {
                if (m.Value.Method == methodbase)
                    return m.Key;
            }
            return null;
        }
        

		internal static Dictionary<Type, Type> ReversedTypeMap(this Dictionary<Type, Type> map)
		{
			var result = new Dictionary<Type, Type>();
			foreach (var item in map)
			{
				result [item.Value] = item.Key;
			}
			return result;
		}

    }
}