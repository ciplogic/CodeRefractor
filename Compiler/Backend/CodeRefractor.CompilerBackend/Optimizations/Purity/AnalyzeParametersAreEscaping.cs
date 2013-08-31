using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    class AnalyzeParametersAreEscaping : ResultingGlobalOptimizationPass
    {
        private const string EscapeName = "NonEscapingArgs";
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;
            var escaping = new Dictionary<int, bool>();
            foreach (var op in operations)
            {
                switch (op.Kind)
                {
                    case LocalOperation.Kinds.Assignment:
                        var right = op.GetAssignment().Right as LocalVariable;
                        if(right == null || right.Kind != VariableKind.Argument)continue;
                        escaping[right.Id] = true; 
                        break;

                    case LocalOperation.Kinds.Call:
                        var methodData = (MethodData)op.Value;
                        var calledMethod = methodData.Info.GetMethod();
                        var otherMethodData = (Dictionary<int, bool>)calledMethod.GetAdditionalProperty(EscapeName);
                        if (otherMethodData==null)
                            break;
                        for (var i = 0; i < methodData.Parameters.Count; i++)
                        {
                            var parameter = methodData.Parameters[i];
                            var argCall = parameter as ArgumentVariable;
                            if (argCall == null)
                                continue;
                            if(!argCall.ComputedType().IsClass)
                                continue;
                            if(otherMethodData.ContainsKey(argCall.Id))
                            {
                                escaping[argCall.Id] = true;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            intermediateCode.SetAdditionalValue(EscapeName, escaping);
        }
    }
}
