#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.Inliner
{
    //[Optimization(Category = OptimizationCategories.Inliner)]
    internal class SmallFunctionsInliner : ResultingOptimizationPass
    {
        public static int MaxLengthInliner = 200;
        public static int MaxLengthChildFunction = 5;

        public SmallFunctionsInliner()
            : base(OptimizationKind.Global)
        {
        }

        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            if (methodInterpreter.MidRepresentation.LocalOperations.Count > MaxLengthInliner) return;
            MethodInterpreter interpreter = null;
            MethodData methodData = null;
            var pos = 0;
            foreach (var localOperation in methodInterpreter.MidRepresentation.LocalOperations)
            {
                pos++;
                if (localOperation.Kind != OperationKind.Call) continue;

                methodData = (MethodData) localOperation;
                var methodBase = methodData.Info;
                interpreter = methodBase.Register();
                if (interpreter == null)
                    continue;
                if (methodInterpreter.MidRepresentation.GetMethodBody == null)
                    continue;
                if (methodInterpreter.MidRepresentation.LocalOperations.Count > MaxLengthChildFunction)
                    continue;
                break;
            }
            if (interpreter == null)
                return;
            InlineMethod(methodInterpreter.MidRepresentation, methodData, pos);
            Result = true;
        }

        public static void InlineMethod(
            MetaMidRepresentation intermediateCode,
            MethodData methodData,
            int pos)
        {
            var methodToInlineInterpreter = methodData.Interpreter;
            var mappedParameters = BuildMappedParameters(methodToInlineInterpreter,
                methodData);

            var mappedLocals = BuildMappedLocals(methodToInlineInterpreter,
                intermediateCode.Vars.LocalVars.Count);

            var mappedVregs = BuildMappedVregs(intermediateCode, methodToInlineInterpreter);

            var indexCall = pos;
            var assignment = (MethodData) intermediateCode.LocalOperations[indexCall];

            var localOperationsToInline = BuildLocalOperationsToInline(methodToInlineInterpreter,
                mappedParameters,
                assignment != null? assignment.Result:null);

            MergeVRegs(intermediateCode, methodToInlineInterpreter, mappedVregs);
            MergeLocalVariables(intermediateCode, methodToInlineInterpreter, mappedLocals);

            intermediateCode.LocalOperations.RemoveAt(indexCall);
            intermediateCode.LocalOperations.InsertRange(indexCall, localOperationsToInline);
        }

        private static void MergeVRegs(MetaMidRepresentation intermediateCode,
            MethodInterpreter methodToInlineInterpreter,
            Dictionary<int, int> mappedVregs)
        {
            var virtRegs = methodToInlineInterpreter.MidRepresentation.Vars.VirtRegs;
            var vregsToAdd = mappedVregs.Select(id => GetVRegMapped(virtRegs, id)).ToList();

            intermediateCode.Vars.VirtRegs.AddRange(vregsToAdd);
        }

        private static LocalVariable GetVRegMapped(List<LocalVariable> virtRegs, KeyValuePair<int, int> id)
        {
            var localVariable = virtRegs.First(item => id.Key == item.Id);
            return new LocalVariable
            {
                Kind = VariableKind.Vreg,
                Id = id.Value,
                FixedType = localVariable.FixedType
            };
        }

        private static void MergeLocalVariables(MetaMidRepresentation intermediateCode,
            MethodInterpreter methodToInlineInterpreter,
            Dictionary<int, LocalVariable> mappedLocals)
        {
            var localVars =
                methodToInlineInterpreter.MidRepresentation.Vars.LocalVars;
            var localVarsToAdd = mappedLocals.Select(id => new LocalVariable
            {
                Kind = VariableKind.Local,
                Id = id.Value.Id,
                FixedType =
                    localVars.First(
                        item =>
                            id.Key == item.Id).FixedType
            }).ToList();
            var vars = intermediateCode.Vars;
            foreach (var localVariable in localVarsToAdd)
            {
                vars.LocalVars[localVariable.Id] = localVariable;
            }
        }

        #region Instruction mapped

        private static List<LocalOperation> BuildLocalOperationsToInline(
            MethodInterpreter interpreter, Dictionary<LocalVariable, IdentifierValue> mappedNames, LocalVariable result)
        {
            var localOperationsToInline = new List<LocalOperation>();
            var localOperations = interpreter.MidRepresentation.LocalOperations.ToList();
            for (var index = 0; index < localOperations.Count; index++)
            {
                var localOperation = localOperations[index];
                if (localOperation.Kind == OperationKind.Return)
                {
                    HandleReturn(result, localOperationsToInline, localOperation);
                    break;
                }
                var clone = localOperation.Clone();
                SwitchUsageClones(mappedNames, clone);
                localOperationsToInline.Add(clone);
            }
            return localOperationsToInline;
        }

        private static void HandleReturn(LocalVariable result, List<LocalOperation> localOperationsToInline,
            LocalOperation localOperation)
        {
            var identifierValue = localOperation.Get<Return>();
            if (identifierValue == null)
                return;
            var assignOp = new Assignment()
            {
                AssignedTo = result,
                Right = identifierValue.Returning
            };
            var assignmentReturn = assignOp;
            localOperationsToInline.Add(assignmentReturn);
        }

        private static void SwitchUsageClones(Dictionary<LocalVariable, IdentifierValue> mappedNames,
            LocalOperation clone)
        {
            foreach (var localVariable in mappedNames)
            {
                clone.SwitchUsageWithDefinition(localVariable.Key, localVariable.Value);
            }
        }

        #endregion

        #region MappingCreation

        private static Dictionary<int, int> BuildMappedVregs(
            MetaMidRepresentation intermediateCode,
            MethodInterpreter interpreter)
        {
            var mappedVregs = new Dictionary<int, int>();
            var virtRegs = interpreter.MidRepresentation.Vars.VirtRegs;

            List<LocalVariable> localVariables = intermediateCode.Vars.VirtRegs;
            var countSourceVregs = localVariables.Count==0
                ? 1
                : localVariables.Max(vreg => vreg.Id) + 1;
            for (var i = 0; i < virtRegs.Count; i++)
            {
                var vreg = virtRegs[i];
                mappedVregs[vreg.Id] = countSourceVregs + i;
            }
            return mappedVregs;
        }

        private static Dictionary<int, LocalVariable> BuildMappedLocals(MethodInterpreter methodToInlineInterpreter,
            int count)
        {
            var mappedNames = new Dictionary<int, LocalVariable>();
            var localVariables =
                methodToInlineInterpreter.MidRepresentation.Vars.LocalVars;
            for (var i = 0; i < localVariables.Count; i++)
            {
                var identifierValue = (LocalVariable) localVariables[i].Clone();
                mappedNames[identifierValue.Id] = identifierValue;
                identifierValue.Id += count;
            }
            return mappedNames;
        }

        private static Dictionary<LocalVariable, IdentifierValue> BuildMappedParameters(MethodInterpreter interpreter,
            MethodData methodData)
        {
            var mappedNames = new Dictionary<LocalVariable, IdentifierValue>();
            for (var i = 0; i < methodData.Parameters.Count; i++)
            {
                var identifierValue = methodData.Parameters[i];
                var argumentVariable = interpreter.MidRepresentation.Vars.Arguments[i];
                argumentVariable.Id = i;
                mappedNames[argumentVariable] = identifierValue;
            }
            return mappedNames;
        }

        #endregion
    }
}