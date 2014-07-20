#region Usings

using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd;

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

        public void RegisterMethod(MethodInfo method, MethodInterpreter interpreter, ClosureEntities closure)
        {
            if (!method.IsVirtual)
                return;
            
            foreach (var virtualMethod in VirtualMethods)
            {
                var matchFound = virtualMethod.MethodMatches(method);
                if (matchFound)
                    return;
            }
          
            var declaringType = method.GetBaseDefinition().DeclaringType;
            var virtMethod = new VirtualMethodDescription(method, declaringType);
            virtMethod.MethodMatches(method);
            VirtualMethods.Add(virtMethod);
            if (interpreter != null && interpreter is CilMethodInterpreter)
            {
                (interpreter as CilMethodInterpreter).Process();
         
            }
            closure.UseMethod(method, interpreter);   
        }
    }
}