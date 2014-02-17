using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeRefractor.CodeWriter.TypeInfoWriter
{
    public class VirtualMethodDescription
    {
        public HashSet<Type> UsingImplementations { get; private set; }


        public string Name { get; set; }
        public Type[] Parameters;
        public Type ReturnType;
        public Type BaseType;

        public MethodInfo BaseMethod { get; private set; }

        public VirtualMethodDescription(MethodInfo method, Type baseType)
        {
            UsingImplementations = new HashSet<Type>();
            Name = method.Name;
            ReturnType = method.ReturnType;
            Parameters = method.GetParameters().Select(par => par.ParameterType).ToArray();

            BaseType = baseType;
            BaseMethod = BaseType.GetMethod(Name, Parameters);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} (...)",
                ReturnType.Name, Name);
        }

        public bool MethodMatches(MethodInfo method, bool addToImplementations = true)
        {
            if (method.Name != Name)
                return false;
            var declaringType = method.DeclaringType;
            if (BaseType.IsSubclassOf(declaringType))
                return false;

            if (ReturnType != method.ReturnType)
                return false;
            var arguments = method.GetParameters().Select(par => par.ParameterType).ToArray();

            if (arguments.Length != Parameters.Length)
                return false;

            for (int index = 0; index < arguments.Length; index++)
            {
                var argument = arguments[index];
                var parameter = Parameters[index];
                if (argument != parameter)
                    return false;
            }
            if (addToImplementations)
                UsingImplementations.Add(declaringType);
            return true;
        }

    }
}
