#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Inliner
{
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

                methodData = (MethodData) localOperation.Value;
                var methodBase = methodData.Info;
                var typeData = (ClassTypeData) ProgramData.UpdateType(methodBase.DeclaringType);
                interpreter = ClassTypeData.GetInterpreterStatic(methodBase);
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
            InlineMethod(methodInterpreter.MidRepresentation, interpreter, methodData, pos);
        }

        public static void InlineMethod(
            MetaMidRepresentation intermediateCode,
            MethodInterpreter methodToInlineInterpreter,
            MethodData methodData,
            int pos)
        {
            var mappedParameters = BuildMappedParameters(methodToInlineInterpreter,
                                                         methodData);

            var mappedLocals = BuildMappedLocals(methodToInlineInterpreter,
                                                 intermediateCode.Vars.LocalVars.Count);

            var mappedVregs = BuildMappedVregs(intermediateCode, methodToInlineInterpreter);

            var indexCall = pos;
            var assignment = (MethodData) intermediateCode.LocalOperations[indexCall].Value;

            var localOperationsToInline = BuildLocalOperationsToInline(methodToInlineInterpreter,
                                                                       mappedParameters,
                                                                       mappedVregs,
                                                                       assignment.Result,
                                                                       mappedLocals);

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
            MethodInterpreter interpreter, Dictionary<string, IdentifierValue> mappedNames,
            Dictionary<int, int> mappedVregs, LocalVariable result,
            Dictionary<int, LocalVariable> mappedLocals)
        {
            var localOperationsToInline = new List<LocalOperation>();
            var localOperations = interpreter.MidRepresentation.LocalOperations.ToList();
            foreach (var localOperation in localOperations)
            {
                if(localOperation.Kind==OperationKind.Return)
                {
                    HandleReturn(result, localOperationsToInline, localOperation);
                    break;
                }
                var clone = localOperation.Clone();
                var usages = clone.GetUsages();
                SwitchUsageClones(mappedNames, mappedVregs, mappedLocals, usages, clone);
                localOperationsToInline.Add(clone);
            }
            return localOperationsToInline;
        }

        private static void HandleReturn(LocalVariable result, List<LocalOperation> localOperationsToInline, LocalOperation localOperation)
        {
            var identifierValue = localOperation.Get<IdentifierValue>();
            if (identifierValue == null)
                return;
            var assignOp = new Assignment()
                               {
                                   AssignedTo = result,
                                   Right = identifierValue
                               };
            var assignmentReturn = new LocalOperation
                                       {
                                           Kind = OperationKind.Assignment,
                                           Value = assignOp
                                       };
            localOperationsToInline.Add(assignmentReturn);
        }

        private static void SwitchUsageClones(Dictionary<string, IdentifierValue> mappedNames, Dictionary<int, int> mappedVregs, Dictionary<int, LocalVariable> mappedLocals,
                                              List<LocalVariable> usages, LocalOperation clone)
        {
            foreach (var localVariable in usages)
            {
                switch (localVariable.Kind)
                {
                    case VariableKind.Vreg:
                        {
                            int newVregId;
                            if (mappedVregs.TryGetValue(localVariable.Id, out newVregId))
                            {
                                var newVreg = (LocalVariable) localVariable.Clone();
                                newVreg.Id = newVregId;
                                clone.SwitchUsageWithDefinition(localVariable, newVreg);
                            }
                        }
                        break;
                    case VariableKind.Local:
                        {
                            LocalVariable newLocalId;
                            if (mappedLocals.TryGetValue(localVariable.Id, out newLocalId))
                            {
                                clone.SwitchUsageWithDefinition(localVariable, newLocalId);
                            }
                        }
                        break;
                    case VariableKind.Argument:
                        {
                            var argumentName = localVariable.Name;
                            IdentifierValue argumentValue;
                            if (mappedNames.TryGetValue(argumentName, out argumentValue))
                            {
                                clone.SwitchUsageWithDefinition(localVariable, argumentValue);
                            }
                        }
                        break;
                }
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

            var countSourceVregs = intermediateCode.Vars.VirtRegs.Max(vreg => vreg.Id) + 1;
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
                var identifierValue = (LocalVariable)localVariables[i].Clone();
                mappedNames[identifierValue.Id] = identifierValue;
                identifierValue.Id += count;
            }
            return mappedNames;
        }

        private static Dictionary<string, IdentifierValue> BuildMappedParameters(MethodInterpreter interpreter,
                                                                               MethodData methodData)
        {
            var mappedNames = new Dictionary<string, IdentifierValue>();
            for (var i = 0; i < methodData.Parameters.Count; i++)
            {
                var identifierValue = methodData.Parameters[i];
                var argumentVariable = interpreter.MidRepresentation.Vars.Arguments[i];
                mappedNames[argumentVariable.Name] = identifierValue;
            }
            return mappedNames;
        }

        #endregion
    }
}