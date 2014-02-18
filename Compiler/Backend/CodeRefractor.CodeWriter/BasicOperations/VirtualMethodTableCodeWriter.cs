using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.CodeWriter.TypeInfoWriter;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.CodeWriter.BasicOperations
{
    public class VirtualMethodTableCodeWriter
    {
        private readonly VirtualMethodTable _typeTable;
        private readonly List<VirtualMethodDescription> _validVirtualMethods;

        public VirtualMethodTableCodeWriter(VirtualMethodTable typeTable, List<MethodInterpreter> closure)
        {
            _typeTable = typeTable;
            var methodNames = GetAllMethodNames(closure);
            _validVirtualMethods = CalculateValidVirtualMethods(typeTable, methodNames);
        }

        public static List<VirtualMethodDescription> CalculateValidVirtualMethods(VirtualMethodTable typeTable, HashSet<string> methodNames)
        {
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
            return validVirtMethods;
        }

        public string GenerateTypeTableCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// --- Begin definition of virtual method tables ---");
            sb.AppendLine("void setupTypeTable();")
                .AppendLine();

            
            foreach (var virtualMethod in _validVirtualMethods)
            {
                var methodName = virtualMethod.BaseMethod.ClangMethodSignature();
                var parametersString = GetParametersString(virtualMethod); 

                sb.Append("typedef ");
                sb.Append(virtualMethod.ReturnType.ToCppMangling());

                sb.Append(" (*");
                sb.Append(methodName);
                sb.Append("VirtPtr)(");
                sb.AppendFormat(parametersString);
                sb.AppendLine(");");

                sb.Append(virtualMethod.ReturnType.ToCppMangling());
                sb.Append(" ");
                sb.Append(methodName);
                sb.Append("_vcall(");
                sb.AppendFormat(parametersString);
                sb.AppendLine(");");
            }

            foreach (var virtualMethod in _validVirtualMethods)
            {
                var methodName = virtualMethod.BaseMethod.ClangMethodSignature();
                var implCount = virtualMethod.UsingImplementations.Count;
                sb.AppendFormat("int virt_typeId_{0}[{1}];", methodName, implCount)
                    .AppendLine();
                sb.AppendFormat("{0}VirtPtr methods_{0}[{1}];", methodName, implCount)
                    .AppendLine();


                var parametersString = GetParametersString(virtualMethod);
                var parametersCallString = "_this";
                sb.Append(virtualMethod.ReturnType.ToCppMangling());
                sb.Append(" ");
                sb.Append(methodName);
                sb.Append("_vcall(");
                sb.AppendFormat(parametersString);
                sb.AppendLine("){");
                sb.AppendLine("int typeId = _this->_typeId;");
                sb.AppendFormat("for(int i= 0; i<{0};i++)",implCount)
                    .AppendLine();

                sb.AppendLine("{");
                sb.AppendFormat("if(virt_typeId_{0}[i] == typeId)",methodName)
                    .AppendLine();
                if (virtualMethod.BaseMethod.ReturnType != typeof (void))
                {
                    sb.Append("return ");
                }
                sb.AppendFormat("methods_{0}[i](", methodName);
                sb.AppendFormat("{0});", parametersCallString)
                    .AppendLine();
                sb.AppendLine("}");
                if (virtualMethod.BaseMethod.ReturnType != typeof(void))
                {
                    sb.Append("return 0;");
                }
                sb.AppendLine("}");
            }

            sb.AppendLine("// --- End of definition of virtual method tables ---");
            return sb.ToString();
        }

        private static string GetParametersString(VirtualMethodDescription virtualMethod)
        {
            var parametersString = string.Format("const {0} &_this", virtualMethod.BaseType.ToDeclaredVariableType());
            return parametersString;
        }

        public string GenerateVirtualFunctionMappingCode()
        {
            var sb = new StringBuilder();
            sb.AppendLine("void setupTypeTable(){");
            foreach (var virtualMethod in _validVirtualMethods)
            {
                var methodName = virtualMethod.BaseMethod.ClangMethodSignature();
                var index = 0;
                foreach (var implementation in virtualMethod.UsingImplementations)
                {
                    var typeId = _typeTable.TypeTable.GetTypeId(implementation);
                    sb.AppendFormat("virt_typeId_{0}[{1}] = {2};", methodName, index, typeId)
                        .AppendLine();
                    var methodImpl = implementation.GetMethod(virtualMethod.Name).ClangMethodSignature();
                    sb.AppendFormat("methods_{0}[{1}] = ({0}VirtPtr){2};", methodName, index, methodImpl)
                        .AppendLine();
                    index++;
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static HashSet<string> GetAllMethodNames(List<MethodInterpreter> closure)
        {
            var methodNames = new HashSet<string>
                (
                closure.Select(m => m.Method.Name)
                );
            return methodNames;
        }
    }
}