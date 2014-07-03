#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.RuntimeBase.TypeInfoWriter
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
            Name = method.GetMethodName();
            ReturnType = method.ReturnType;
            Parameters = method.GetParameters().Select(par => par.ParameterType).ToArray();

            BaseType = baseType;
            //Handle Interfaces
            var interfacewithmethod = baseType.GetInterfaces().FirstOrDefault(g =>baseType.GetInterfaceMap(g).TargetMethods.Contains(method));
            if (interfacewithmethod!=null)
            {
                BaseMethod = interfacewithmethod.GetMethod(method.Name.Replace(interfacewithmethod.Name+".",""),Parameters); //Remove explicit name
            }
            else
          
            BaseMethod = BaseType.GetMethod(method.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Parameters, null);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} (...)",
                ReturnType.Name, Name);
        }

        public bool MethodMatches(MethodInfo method, bool addToImplementations = true)
        {
            if (method.GetMethodName() != Name)
                return false;
            var declaringType = method.DeclaringType;
            if (BaseType.IsSubclassOf(declaringType))
                return false;

            if (ReturnType != method.ReturnType)
                return false;
            var arguments = method.GetParameters().Select(par => par.ParameterType).ToArray();

            if (arguments.Length != Parameters.Length)
                return false;

            for (var index = 0; index < arguments.Length; index++)
            {
                var argument = arguments[index];
                var parameter = Parameters[index];
                if (argument != parameter)
                    return false;
            }
            if (addToImplementations)
            {   
//                //Also add all subclasses that use this method
//                foreach (var argument in declaringType)
//                {
//                    
//                }
                var allInterfaceImplementors = declaringType.Assembly.GetTypes()
    .Where(p => declaringType.IsAssignableFrom(p));

                foreach (var allInterfaceImplementor in allInterfaceImplementors)
                {
                    UsingImplementations.Add(allInterfaceImplementor);
                    
                }
                UsingImplementations.Add(declaringType);
            }
            return true;
        }
    }
}