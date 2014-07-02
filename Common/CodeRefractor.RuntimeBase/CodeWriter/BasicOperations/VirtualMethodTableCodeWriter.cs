#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Backend.ComputeClosure;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.TypeInfoWriter;
using CodeRefractor.Util;

#endregion

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

        public static List<VirtualMethodDescription> CalculateValidVirtualMethods(VirtualMethodTable typeTable,
            HashSet<string> methodNames)
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

        public string GenerateTypeTableCode(Type[] types)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// --- Begin definition of virtual method tables ---");
            sb.AppendLine("System_Void setupTypeTable();")
                .AppendLine();


            foreach (var virtualMethod in _validVirtualMethods)
            {
                var isinterfaceMethod = virtualMethod.BaseMethod.DeclaringType.IsInterface;
                string methodName;
            
                 methodName = virtualMethod.BaseMethod.ClangMethodSignature();
                var parametersString = GetParametersString(virtualMethod,isinterfaceMethod);

//                sb.Append("typedef ");
//                sb.Append(virtualMethod.ReturnType.ToCppName(true,EscapingMode.Smart));
//
//                sb.Append(" (*");
//                sb.Append(methodName);
//                sb.Append("VirtPtr)(");
//                sb.AppendFormat(parametersString);
//                sb.AppendLine(");");

                sb.Append(virtualMethod.ReturnType.ToCppName(true, EscapingMode.Smart));
                sb.Append(" ");
                sb.Append(methodName);
                sb.Append("_vcall(");
                sb.AppendFormat(parametersString);
                sb.AppendLine(");");
            }

            foreach (var virtualMethod in _validVirtualMethods)
            {
                //Ignore instance only dispatch
                if(virtualMethod.UsingImplementations.All(k=>k.IsInterface))
                    continue;

                var isinterfaceMethod = virtualMethod.BaseMethod.DeclaringType.IsInterface;
               
                var methodName = virtualMethod.BaseMethod.ClangMethodSignature();

                var parametersString = GetParametersString(virtualMethod, isinterfaceMethod);
             
                sb.Append(virtualMethod.ReturnType.ToCppName(true,EscapingMode.Smart));
                sb.Append(" ");
                sb.Append(methodName);
                sb.Append("_vcall(")
                    .AppendFormat(parametersString)
                    .AppendLine("){")
                    .AppendLine("switch (_this->_typeId)")
                    .AppendLine("{");
                foreach (var implementation in virtualMethod.UsingImplementations)
                {
                    //Interfaces dont have concrete implementations
                    if (!implementation.IsInterface)
                    {
                        var typeId = _typeTable.TypeTable.GetTypeId(implementation);

                        sb.AppendFormat("case {0}:", typeId).AppendLine();

                        var isVoid = virtualMethod.BaseMethod.ReturnType == typeof(void);
                        if (!isVoid)
                        {
                            sb.Append("return ");
                        
                        }

                        //Handle Interfaces

                        var method = implementation.GetMethod(virtualMethod.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, virtualMethod.Parameters, null); 
                        //implementation.GetMethod(virtualMethod.Name, virtualMethod.Parameters);
                        var methodImpl = method.ClangMethodSignature();
                        var parametersCallString = GetCall(virtualMethod, method);

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

                // Deal with subclasses that don't override this method
                var remainingSubclasses = virtualMethod.BaseMethod.DeclaringType.ImplementorsOfT(types).Except(virtualMethod.UsingImplementations); //types.Where(k => k.ImplementorsOfT() is ());
               

                foreach (var implementation in remainingSubclasses)
                {
                    var typeId = _typeTable.TypeTable.GetTypeId(implementation);

                    sb.AppendFormat("case {0}:", typeId).AppendLine();

                    var isVoid = virtualMethod.BaseMethod.ReturnType == typeof(void);
                    if (!isVoid)
                    {
                        sb.Append("return ");
                    }

                    var method = implementation.GetMethod(virtualMethod.Name, virtualMethod.Parameters);
                    var methodImpl = method.ClangMethodSignature();
                    var parametersCallString = GetCall(virtualMethod, method);
                    sb
                        .AppendFormat("{0}(",methodImpl)
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

        private static string GetParametersString(VirtualMethodDescription virtualMethod, bool isinterfacemethod)
        {
            string parametersString;
            if (isinterfacemethod)
            {
                parametersString = string.Format("const {0} _this", typeof(object).ToDeclaredVariableType(true, EscapingMode.Smart));
            }
            else
            parametersString = string.Format("const {0} _this", virtualMethod.BaseType.ToDeclaredVariableType(true,EscapingMode.Smart));
            //Add Rest of parameters
            if (virtualMethod.Parameters.Length > 0)
            {
                var paramTypes = virtualMethod.Parameters;
                int c = 0;
                parametersString += ", ";
                parametersString = paramTypes.Aggregate(parametersString, (current, paramType) => current + (paramType.ToDeclaredVariableType(true, EscapingMode.Smart) + " param" + (c++) + ", "));
                
                    parametersString = parametersString.Substring(0, parametersString.Length - 2);
                
                                  
            }
            return parametersString;
        }

        private static string GetCall(VirtualMethodDescription virtualMethod, MethodInfo method)
        {
            var parametersString = string.Format("std::static_pointer_cast<{0}>(_this)", method.DeclaringType.ToCppName(true, EscapingMode.Unused));
            //Add Rest of parameters
            if (virtualMethod.Parameters.Length > 0)
            {
                var paramTypes = virtualMethod.Parameters;
                int c = 0;
                parametersString += ", ";
               
                parametersString = paramTypes.Aggregate(parametersString, (current, paramType) => current + " param" + (c++) + ", ");

                parametersString = parametersString.Substring(0, parametersString.Length - 2);


            }
            return parametersString;
        }

        public static HashSet<string> GetAllMethodNames(List<MethodInterpreter> closure)
        {
            var methodNames = new HashSet<string>
                (
                closure.Select(m => m.Method.GetMethodName())
                );
            return methodNames;
        }
    }
}