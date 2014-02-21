using System;
using System.Collections.Generic;

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public static class MethodInterpreterKeyUtils
    {
		public static MethodInterpreterKey ToKey(this MethodInterpreter methodInterpreter, Type implementingType = null)
        {
			var result = new MethodInterpreterKey(methodInterpreter, implementingType);
            return result;
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