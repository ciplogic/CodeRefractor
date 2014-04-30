using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.TypeInfoWriter;

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

                var parametersString = GetParametersString(virtualMethod);
                var parametersCallString = "_this";
                sb.Append(virtualMethod.ReturnType.ToCppMangling());
                sb.Append(" ");
                sb.Append(methodName);
                sb.Append("_vcall(")
                    .AppendFormat(parametersString)
                    .AppendLine("){")
                    .AppendLine("switch(_this->_typeId)")
                    .AppendLine("{");
                foreach (var implementation in virtualMethod.UsingImplementations)
                {
                    var typeId = _typeTable.TypeTable.GetTypeId(implementation);

                    sb.AppendFormat("case {0}:",typeId).AppendLine();

                    var isVoid = virtualMethod.BaseMethod.ReturnType == typeof(void);
                    if (!isVoid)
                    {
                        sb.Append("return ");
                    }

                    var methodImpl = implementation.GetMethod(virtualMethod.Name).ClangMethodSignature();
                    sb
                        .AppendFormat("{0}(", methodImpl)
                        .AppendFormat("{0});", parametersCallString)
                        .AppendLine();
                    if (isVoid)
                    {
                        sb.Append("return;").AppendLine();
                    }
                }
                sb.AppendLine("}");
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