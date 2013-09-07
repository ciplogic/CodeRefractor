#region Usings

using System;
using System.Reflection;
using System.Text;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class CppFullFileMethodWriter
    {
        public static MetaLinker CreateLinkerFromEntryPoint(this MethodInfo definition)
        {
            var linker = new MetaLinker();
            linker.SetEntryPoint(definition);
            MetaLinker.ComputeDependencies(definition);
            linker.Interpret();
            MetaLinkerOptimizer.OptimizeMethods();
            return linker;
        }

        public static string WriteHeaderMethod(this MethodBase methodBase, bool writeEndColon = true)
        {
            var retType = methodBase.GetReturnType().ToCppName();
            var sb = new StringBuilder();
            var arguments = methodBase.GetArgumentsAsText();

            sb.AppendFormat("{0} {1}({2})",
                            retType, methodBase.ClangMethodSignature(), arguments);
            if (writeEndColon)
                sb.Append(";");

            sb.AppendLine();
            return sb.ToString();
        }
    }
}