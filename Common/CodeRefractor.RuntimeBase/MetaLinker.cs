#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;
using CodeRefractor.Util;
using Mono.Cecil;
using Mono.Collections.Generic;
using Instruction = Mono.Cecil.Cil.Instruction;

#endregion

namespace CodeRefractor.RuntimeBase
{
    public static class MetaLinker
    {

        public static bool MethodMatches(this MethodBase method, MethodDefinition otherDefinition)
        {
            if ((method.GetMethodName() != otherDefinition.Name) || (otherDefinition.Name != method.Name))
                return false;
            var declaringType = method.DeclaringType;
        

            if (method.GetReturnType().FullName != otherDefinition.ReturnType.FullName)
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

        public static bool MethodMatches(this MethodReference otherDefinition, MethodBase method)
        {
            if ((method.GetMethodName() != otherDefinition.Name) && (otherDefinition.Name != method.Name))
                return false;
            var declaringType = method.DeclaringType;


            if (method.GetReturnType().FullName != otherDefinition.ReturnType.FullName)
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

     

        public static Collection<Instruction> GetInstructions(this MethodBase definition)
        {
            var foundMethod = definition.GetMethodDefinition();

            //And then get a CilWorker off of the method. This allows me to make modifications to this method’s CIL.

            return foundMethod.Body.Instructions;
        }

        public static MethodDefinition GetMethodDefinition(this MethodBase definition)
        {
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(definition.DeclaringType.Assembly.Location);
                //new AssemblyDefinition() definition.DeclaringType.Assembly.GetAssembly("CecilTestLibrary.dll");


            TypeReference type= assembly.MainModule.GetType(definition.DeclaringType.FullName.GetCecilName());//.FirstOrDefault(h => h.FullName == );

            MethodDefinition foundMethod = ((TypeDefinition)type).Methods.Single(m => definition.MethodMatches(m));
            return foundMethod;
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

        public static string GetClrName(this string name)
        {
            return name.Replace("/", "+").Replace("/", ".");
        }
        public static string GetCecilName(this string name)
        {
            return name.Replace("+", "/");
        }
        public static MethodBase GetMethod(this MethodReference definition)
        {

            var assembly = Assembly.Load(definition.Module.Assembly.FullName);


            var type = assembly.GetType(definition.DeclaringType.FullName.GetClrName());

            if (type == null) //Now would be a good time to use our own custom CLR
            {
                type = Assembly.GetAssembly(typeof(Object)).GetType(definition.DeclaringType.FullName);
                if (type == null)
                    return null;
            }

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance| BindingFlags.CreateInstance| BindingFlags.GetField| BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.SetField);
            MethodBase foundMethod = methods.FirstOrDefault(m => definition.MethodMatches(m));
            if (foundMethod == null)
                foundMethod = type.GetConstructors().FirstOrDefault(m => definition.MethodMatches(m));
            return foundMethod;
        }


        public static List<MethodInterpreter> ComputeDependencies(MethodBase definition)
        {
            var resultDict = new Dictionary<string, MethodInterpreter>();
            var body = definition.GetMethodBody();
            if (body == null)
            {
                return new List<MethodInterpreter>();
            }
            //Lets try cecil


           // var instructions = MsilReader.MethodBodyReader.GetInstructions(definition);
            var instructions = definition.GetInstructions();

            foreach (var instruction in instructions)
            {
                var opcodeStr = instruction.OpCode.Value;
                switch (opcodeStr)
                {
                    case OpcodeIntValues.CallVirt:
                    case OpcodeIntValues.Call:
                    case OpcodeIntValues.CallInterface:
                    {
                        //var operand =(MethodBase) instruction.Operand;
                        var operand = GetMethod((MethodReference)instruction.Operand);
                        if (operand == null)
                            break;
                        AddMethodIfNecessary(operand, resultDict);
                        break;
                    }
                    case OpcodeIntValues.NewObj:
                    {
                        var operand = GetMethod((MethodReference)instruction.Operand); ;// (ConstructorInfo) instruction.Operand;
                        if (operand == null)
                            break;
                        AddMethodIfNecessary(operand, resultDict);
                        break;
                    }
                }
            }
            return resultDict.Values.ToList();
        }


        private static void AddMethodIfNecessary(MethodBase methodBase, Dictionary<string, MethodInterpreter> resultDict)
        {
            if (MetaMidRepresentationOperationFactory.HandleRuntimeHelpersMethod(methodBase))
                return;
            var interpreter = methodBase.Register();
            resultDict[interpreter.ToString()] = interpreter;

            var declaringType = methodBase.DeclaringType;
            var isInGlobalAssemblyCache = declaringType.Assembly.GlobalAssemblyCache;
            if (isInGlobalAssemblyCache)
                return; //doesn't run on global assembly cache methods
            ComputeDependencies(methodBase);
        }


        public static void Interpret(CilMethodInterpreter methodInterpreter, CrRuntimeLibrary crRuntime)
        {
            if (methodInterpreter.Kind != MethodKind.CilInstructions)
                return;
            if (methodInterpreter.Interpreted)
                return;
            methodInterpreter.Process();
        }
    }
}