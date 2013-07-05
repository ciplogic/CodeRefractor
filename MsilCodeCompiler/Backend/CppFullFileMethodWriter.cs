#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CodeRefractor.Compiler.FrontEnd;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.Compiler.Backend
{
    public static class CppFullFileMethodWriter
    {
        public static MetaLinker CreateLinkerFromEntryPoint(this MethodInfo definition,
                                                            List<OptimizationPass> optimizationPasses = null)
        {
            var linker = new MetaLinker();
            linker.SetEntryPoint(definition);
            linker.ComputeDependencies(definition);
            linker.ComputeLabels(definition);
            linker.EvaluateMethods();
            return linker;
        }

        public static string WriteHeaderMethod(this MethodBase methodBase, bool writeEndColon = true,
                                               Type mappedType = null)
        {
            var retType = methodBase.GetReturnType().ToCppMangling();
            var sb = new StringBuilder();
            var arguments = methodBase.GetArgumentsAsText();

            sb.AppendFormat("{0} {1}({2})",
                            retType, methodBase.ClangMethodSignature(mappedType), arguments);
            if (writeEndColon)
                sb.Append(";");

            sb.AppendLine();
            return sb.ToString();
        }

        public static string WritePInvokeDefinition(this MethodInterpreter methodBase, string methodDll)
        {
            var retType = methodBase.Method.GetReturnType().ToCppMangling();
            var sb = new StringBuilder();
            var arguments = methodBase.Method.GetArgumentsAsText();

            sb.AppendFormat("typedef {0} (*{1}_type)({2})",
                            retType, methodDll, arguments);

            sb.AppendLine(";");
            sb.AppendFormat("{0}_type {0};", methodDll);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}