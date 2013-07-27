#region Usings

using System.Text;
using CodeRefractor.Compiler.FrontEnd;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.Compiler.Backend
{
    internal class CppClassCodeWriter
    {
        public string WriteCode(ClassInterpreter classInterpreter, TypeData data)
        {
            var sb = new StringBuilder();
            var classNs = classInterpreter.DeclaringType;
            sb.AppendFormat("namespace {0} {{", classNs.Namespace).AppendLine();
            sb.AppendFormat("struct {0} {{", classNs.Name).AppendLine();

            if (data != null)
            {
                foreach (var field in data.Fields)
                {
                    if (field.IsStatic)
                        continue;
                    sb.AppendFormat("{0} {1};", field.TypeData.Info.ToCppName(), field.Name)
                        .AppendLine();
                }
            }
            foreach (var method in classInterpreter.Methods)
            {
                var codeWriter = new MethodInterpreterCodeWriter
                {
                    Interpreter = method
                };
                sb.Append(codeWriter.WriteHeaderMethod());
            }
            sb.AppendFormat("}};").AppendLine();
            sb.AppendFormat("}}").AppendLine();
            return sb.ToString();
        }
    }
}