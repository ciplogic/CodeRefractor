using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.CodeWriter.TypeInfoWriter
{
    public static class VirtualMethodTableCodeWriter
    {
        public static string GenerateTypeTableCode(VirtualMethodTable typeTable, List<MethodInterpreter> closure)
        {
            var methodNames = new HashSet<string>
                (
                closure.Select(m => m.Method.Name)
                );
            var sb = new StringBuilder();

            var validVirtMethods = new List<VirtualMethodDescription>();
            foreach (var virtualMethod in typeTable.VirtualMethods)
            {
                if (!methodNames.Contains(virtualMethod.Name))
                    continue;
                var implementations = virtualMethod.UsingImplementations
                    .Where(type => typeTable.TypeTable.HasType(type))
                    .ToList();
                if (implementations.Count != 0)
                    validVirtMethods.Add(virtualMethod);

            }
            foreach (var virtualMethod in validVirtMethods)
            {
                sb.Append("typedef ");
                sb.Append(virtualMethod.ReturnType.ToCppMangling());

                sb.Append(" (*");
                sb.Append(virtualMethod.Name);
                sb.Append("VirtPtr)(");
                sb.AppendFormat("const {0} &_this", virtualMethod.BaseType.ToDeclaredVariableType());
                sb.AppendLine(");");
            }

            sb
                .AppendLine("struct VImpl {")
                .AppendLine("	int _typeId;")
                .AppendLine("   void* _method;")
                .AppendLine("};");

            foreach (var virtualMethod in validVirtMethods)
            {
                sb.AppendFormat("VImpl impl{0}[{1}];", virtualMethod.Name, virtualMethod.UsingImplementations.Count)
                    .AppendLine();
            }
            sb.AppendLine("void setupTypeTable(){");
            foreach (var virtualMethod in validVirtMethods)
            {
                var index = 0;
                foreach (var implementation in virtualMethod.UsingImplementations)
                {
                    var itemName = string.Format("impl{0}[{1}]", virtualMethod.Name, index);
                    var typeId = typeTable.TypeTable.GetTypeId(implementation);
                    sb.AppendFormat("{0}._typeId = {1};", itemName, typeId)
                      .AppendLine();
                    var methodImpl = implementation.GetMethod(virtualMethod.Name).ClangMethodSignature();
                    sb.AppendFormat("{0}._method = &{1};", itemName, methodImpl)
                      .AppendLine();                    
                    index++;
                }
                
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}