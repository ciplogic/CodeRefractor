#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using CodeRefractor.FrontEnd;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.MiddleEnd
{
    public class MethodInterpreter
    {
        public MethodBase Method { get; set; }
        public TypeDescription DeclaringType { get; set; }

        public List<Type> ClassSpecializationType = new List<Type>();
        public List<Type> MethodSpecializationType = new List<Type>();
        public MethodKind Kind { get; set; }

        public readonly AnalyzeProperties AnalyzeProperties = new AnalyzeProperties();

        public MetaMidRepresentation MidRepresentation = new MetaMidRepresentation();
        public readonly CppRepresentation CppRepresentation = new CppRepresentation();
        public readonly MethodDescription Description = new MethodDescription();

        public bool Interpreted { get; set; }

        public void SetDeclaringType(MethodBase method)
        {
            DeclaringType = new TypeDescription(method.DeclaringType);
            if (!DeclaringType.ClrType.IsGenericType)
            {
                Method = method;
                return;
            }
            Method = MethodInterpreterUtils.GetGenericMethod(method, DeclaringType);
            if (Method == null)
            {
                Method = DeclaringType.ClrType.GetMethod(method.Name);
            }
        }

        public MethodInterpreter(MethodBase method)
        {
            SetDeclaringType(method);
            var pureAttribute = method.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                AnalyzeProperties.IsPure = true;
            MidRepresentation.Vars.SetupArguments(Method);
        }

        public void Specialize()
        {
            if (!IsGenericDeclaringType()) return;
            var genTypeArguments = Method.DeclaringType.GetGenericArguments();
            foreach (var genTypeArgument in genTypeArguments)
            {
                ClassSpecializationType.Add(genTypeArgument);
            }
        }

        public MethodInterpreter Clone()
        {
            var result = new MethodInterpreter(Method)
            {
                MidRepresentation = MidRepresentation,
                ClassSpecializationType = ClassSpecializationType,
                Kind = Kind,
                MethodSpecializationType = MethodSpecializationType
            };
            return result;
        }

        public bool IsGenericDeclaringType()
        {
            return Method.DeclaringType.IsGenericType;
        }

        public override string ToString()
        {
            return Method.GenerateKey();
        }



        public void Process()
        {
            if (Kind != MethodKind.Default)
                return;
            if (Interpreted)
                return;
            if (HandlePlatformInvokeMethod(Method))
                return;
            if (Method.GetMethodBody() == null)
                return;
            var midRepresentationBuilder = new MethodMidRepresentationBuilder(this, Method);
            midRepresentationBuilder.ProcessInstructions();
            Interpreted = true;
        }

        private bool HandlePlatformInvokeMethod(MethodBase method)
        {
            var pinvokeAttribute = method.GetCustomAttribute<DllImportAttribute>();

            if (pinvokeAttribute == null)
                return false;
            Description.LibraryName = pinvokeAttribute.Value;
            Description.MethodName = method.Name;
            Description.CallingConvention = pinvokeAttribute.CallingConvention;
            Description.EntryPoint = pinvokeAttribute.EntryPoint;
            Kind = MethodKind.PlatformInvoke;
            return true;
        }


        public Dictionary<int, int> GetLabelTable()
        {
            return MidRepresentation.UseDef.GetLabelTable();
        }
    }
}