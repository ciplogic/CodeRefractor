#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.Analyze;
using CodeRefractor.CecilUtils;
using CodeRefractor.FrontEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;
using CodeRefractor.Util;
using Mono.Cecil;
using Mono.Collections.Generic;
using Instruction = Mono.Cecil.Cil.Instruction;

#endregion
using System;
using System.Linq;
using Mono.Cecil;

namespace Mono.Cecil.Tests
{
}
namespace CodeRefractor.RuntimeBase
{
    public static class MetaLinker
    {

        public static bool MethodMatches(this MethodBase method, MethodDefinition otherDefinition)
        {
            if ((method.GetMethodName() != otherDefinition.Name) || (otherDefinition.Name != method.Name))
                return false;
        
            var arguments = method.GetParameters().Select(par => par.ParameterType).ToArray();

            if (arguments.Length != otherDefinition.Parameters.Count)
                return false;

            for (var index = 0; index < arguments.Length; index++)
            {
                Type argument = arguments[index];
                ParameterDefinition parameter = otherDefinition.Parameters[index];
                if (argument.FullName != parameter.ParameterType.FullName)
                    return false;
            }
            
            return true;
        }

        public static MethodReference Import(this AssemblyDefinition assemblyDefinition, MethodBase methodBase)
        {
            if (assemblyDefinition == null) throw new ArgumentNullException("assemblyDefinition");
            return assemblyDefinition.MainModule.Import(methodBase);
        }
        public static Collection<Instruction> GetInstructions(this MethodBase definition)
        {
            var foundMethod = definition.GetMethodDefinition();

            //And then get a CilWorker off of the method. This allows me to make modifications to this method’s CIL.

            return foundMethod.Body.Instructions;
        }

        public static MethodDefinition GetMethodDefinition(this MethodBase definition)
        {
            return definition.ToDefinition();
        }


        public static Type GetClrType(this FieldReference definition)
        {

            var assembly = Assembly.Load(definition.Module.Assembly.FullName);


            var type = assembly.GetType(definition.FullName);

            if (type == null) //Now would be a good time to use our own custom CLR
            {
                type = Assembly.GetAssembly(typeof(Object)).GetType(definition.FullName);
                if (type == null)
                    return null;
            }


            return type;
        }
        public static Type GetClrType(this TypeReference definition)
        {

            var assembly = Assembly.Load(definition.Module.Assembly.FullName.GetClrName());


            var type = assembly.GetType(definition.FullName.GetClrName());

            if (type == null) //Now would be a good time to use our own custom CLR
            {
                type = Assembly.GetAssembly(typeof(Object)).GetType(definition.FullName.GetClrName());
                if (type == null)
                    return null;
            }


            return type;
        }

    }
}