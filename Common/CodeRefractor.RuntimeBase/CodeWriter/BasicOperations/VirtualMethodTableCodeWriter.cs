#region Uses

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.ClosureCompute.Steps;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.RuntimeBase.TypeInfoWriter;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.CodeWriter.BasicOperations
{
    public class VirtualMethodTableCodeWriter
    {
        public static string GenerateTypeTableCode(TypeDescriptionTable table, ClosureEntities crRuntime)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// --- Begin definition of virtual implementingMethod tables ---");
            sb.AppendLine("System_Void setupTypeTable();")
                .AppendLine();


            var vcalls = crRuntime.AbstractMethods;

            WriteForwardVcalls(crRuntime, vcalls, sb);


            foreach (var virtualMethod in vcalls)
            {
                var methodName = virtualMethod.ClangMethodSignature(crRuntime);
                var parametersString = GetParametersString(virtualMethod, crRuntime);

                sb.Append(virtualMethod.ReturnType.ToCppName(EscapingMode.Smart));
                sb.Append(" ");
                sb.Append(methodName);
                sb.Append("_vcall(");
                sb.AppendFormat(parametersString);
                sb.AppendLine(") {");

                sb.AppendLine("switch (_this->_typeId)").AppendLine("{");

                var declaringType = virtualMethod.DeclaringType;

                var implementingTypes = declaringType.ImplementorsOfT(crRuntime); //.MappedTypes.Values);
                foreach (var implementingType in implementingTypes)
                {
                    if (implementingType.GetReversedMappedType(crRuntime) != implementingType)
                        continue;
                    var implementingMethod = AddVirtualMethodImplementations.GetImplementingMethod(implementingType,
                        virtualMethod);
//                    if (implementingMethod == null) //We should call the next implementingMethod in line ... not ignore this object
//                        continue;
                    if (implementingMethod != null)
                    {
                        if (implementingMethod.GetMethodBody() == null)
                            continue;
                        var declaringTypeImplementation =
                            implementingMethod.DeclaringType.GetReversedMappedType(crRuntime);

                        var typeId = table.GetTypeId(declaringTypeImplementation);

                        sb.AppendFormat("case {0}:", typeId).AppendLine();


                        var isVoid = virtualMethod.ReturnType == typeof (void);
                        if (!isVoid)
                        {
                            sb.Append("return ");
                        }


                        var methodImpl = implementingMethod.ClangMethodSignature(crRuntime);
                        var parametersCallString = GetCall(virtualMethod, implementingMethod, crRuntime);

                        sb
                            .AppendFormat("{0}(", methodImpl)
                            .AppendFormat("{0});", parametersCallString)
                            .AppendLine();
                        if (isVoid)
                        {
                            sb.Append("return;").AppendLine();
                        }
                    }
                    else
                    {
                        var typeId = table.GetTypeId(implementingType.GetReversedMappedType(crRuntime));

                        sb.AppendFormat("case {0}:", typeId).AppendLine();

                        var isVoid = virtualMethod.ReturnType == typeof (void);
                        if (!isVoid)
                        {
                            sb.Append("return ");
                        }


                        var method = implementingType.GetMethod(virtualMethod.Name,
                            virtualMethod.GetParameters().Select(j => j.ParameterType).ToArray());

                        var methodImpl = method.ClangMethodSignature(crRuntime);
                        var parametersCallString = GetCall(virtualMethod, method, crRuntime);
                        sb
                            .AppendFormat("{0}(", methodImpl)
                            .AppendFormat("{0});", parametersCallString)
                            .AppendLine();
                        if (isVoid)
                        {
                            sb.Append("return;").AppendLine();
                        }
                    }
                }

                sb.AppendLine("} //switch");
                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        static void WriteForwardVcalls(ClosureEntities crRuntime, HashSet<MethodInfo> vcalls, StringBuilder sb)
        {
            foreach (var virtualMethod in vcalls)
            {
                var methodName = virtualMethod.ClangMethodSignature(crRuntime);
                var parametersString = GetParametersString(virtualMethod, crRuntime);

                sb.Append(virtualMethod.ReturnType.ToCppName(EscapingMode.Smart));
                sb.Append(" ");
                sb.Append(methodName);
                sb.Append("_vcall(");
                sb.AppendFormat(parametersString);
                sb.AppendLine(");");
            }
        }

        static string GetParametersString(MethodInfo virtualMethod,
            ClosureEntities crRuntime)
        {
            var sb = new StringBuilder();
            var parameters = virtualMethod.GetParameters();
            var declaringType = virtualMethod.DeclaringType.GetReversedMappedType(crRuntime);
            if (declaringType.IsInterface) //Interface definitions are "objects"
            {
                sb.AppendFormat("const {0} _this", typeof(object).ToCppName());
            }
            else
            {
                sb.AppendFormat("const {0} _this", declaringType.ToCppName());
            }
            if (parameters.Length > 0)
            {
                sb.Append(", ");
                var arguments = string.Join(", ", parameters.Select(param => string.Format("{0} {1}",
                    param.ParameterType.GetReversedMappedType(crRuntime).ToCppName(),
                    param.Name)));
                sb.Append(arguments);
            }
            return sb.ToString();
        }

        static string GetCall(MethodInfo virtualMethod, MethodInfo implementingMethod, ClosureEntities crRuntime)
        {
            //Add Rest of parameters
            var parameters = virtualMethod.GetParameters();
            var usedArgs = implementingMethod.GetInterpreter(crRuntime) != null
                ? MidRepresentationUtils.GetUsedArguments(implementingMethod.GetInterpreter(crRuntime))
                : Enumerable.Repeat(true, parameters.Count() + 1).ToArray();
            // Sometime we dont have a implementingMethod interpreter ... why ?
            var pCount = 0;
            var parametersString = usedArgs[0]
                ? GetCorrectParameter(virtualMethod, implementingMethod, crRuntime, 0)
                : "";

            pCount++;
            if (parameters.Length <= 0)
                return parametersString;

            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(parametersString))
            {
                sb.Append(parametersString);
            }

            foreach (var info in parameters)
            {
                if (!string.IsNullOrEmpty(parametersString) && usedArgs[pCount])
                {
                    sb.Append(", " + info.Name);
                }
                pCount++;
            }


            //            sb.Append(", ");
            //            var arguments = string.Join(", ", parameters.Select(par=>par.Name));
            //            sb.Append(arguments);
            return sb.ToString();
        }

        static string GetCorrectParameter(MethodInfo virtualMethod, MethodInfo implementingMethod,
            ClosureEntities crRuntime, int parameter)
        {
            if (parameter == 0) //TODO: Add Unboxing Feature if Value types are found
            {
                var imParam = implementingMethod.DeclaringType;
                if (imParam.IsValueType)
                {
                    return string.Format("unbox_value<{0}>(_this)",
                        implementingMethod.DeclaringType.GetReversedMappedType(crRuntime).ToCppName(EscapingMode.Unused));
                }
                return string.Format("std::static_pointer_cast<{0}>(_this)",
                    implementingMethod.DeclaringType.GetReversedMappedType(crRuntime).ToCppName(EscapingMode.Unused));
            }
            return "";
        }
    }
}