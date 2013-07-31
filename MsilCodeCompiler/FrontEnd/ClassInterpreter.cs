#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

namespace CodeRefractor.Compiler.FrontEnd
{
    public class ClassInterpreter
    {
        public TypeData DeclaringType;
        public readonly List<MethodInterpreter> Methods = new List<MethodInterpreter>();

        public static string GetClassName(Type declaringType)
        {
            return declaringType.FullName;
        }

        public void Add(MethodInterpreter method)
        {
            Methods.Add(method);
        }

        public override string ToString()
        {
            return string.Format("Class {0}", DeclaringType.Name);
        }
    }
}