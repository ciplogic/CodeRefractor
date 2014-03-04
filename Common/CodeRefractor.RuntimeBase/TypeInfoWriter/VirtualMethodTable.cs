using System.Collections.Generic;
using System.Reflection;

namespace CodeRefractor.CodeWriter.TypeInfoWriter
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

        public void RegisterMethod(MethodInfo method)
        {
            if(!method.IsVirtual || method.IsAbstract)
                return;
            var matchFound = false;
            foreach (var virtualMethod in VirtualMethods)
            {
                matchFound = virtualMethod.MethodMatches(method);
                if(matchFound)
                    break;
            }
            if (!matchFound)
            {
                var declaringType = method.GetBaseDefinition().DeclaringType;
                var virtMethod = new VirtualMethodDescription(method, declaringType);
                virtMethod.MethodMatches(method);
                VirtualMethods.Add(virtMethod);
            }
        }
    }
}