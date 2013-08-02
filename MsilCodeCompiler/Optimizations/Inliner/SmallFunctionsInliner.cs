#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.Compiler.Optimizations.Inliner
{
    internal class SmallFunctionsInliner : ResultingOptimizationPass
    {
        public static int MaxLengthInliner = 200;
        public static int MaxLengthChildFunction = 40;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            if (intermediateCode.LocalOperations.Count > MaxLengthInliner) return;
            MethodInterpreter interpreter = null;
            MethodData methodData = null;
            var pos = 0;
            foreach (var localOperation in intermediateCode.LocalOperations)
            {
                pos++;
                if (localOperation.Kind != LocalOperation.Kinds.Call) continue;

                methodData = (MethodData) localOperation.Value;
                var methodBase = methodData.Info;
                var typeData = (ClassTypeData) ProgramData.UpdateType(methodBase.DeclaringType);
                interpreter = typeData.GetInterpreter(methodBase.ToString());
                if (interpreter == null)
                    continue;
                if (intermediateCode.GetMethodBody == null)
                    continue;
                if (intermediateCode.LocalOperations.Count > MaxLengthChildFunction)
                    continue;
                break;
            }
            if (interpreter == null)
                return;
            InlineMethod(intermediateCode, interpreter, methodData, pos);
        }

        private void InlineMethod(
            MetaMidRepresentation intermediateCode,
            MethodInterpreter methodToInlineInterpreter,
            MethodData methodData,
            int pos)
        {
            var mappedParameters = BuildMappedParameters(methodToInlineInterpreter,
                                                         methodData);

            var mappedLocals = BuildMappedLocals(methodToInlineInterpreter,
                                                 intermediateCode.Vars.LocalVariables.Count);

            var mappedVregs = BuildMappedVregs(intermediateCode, methodToInlineInterpreter);

            var indexCall = pos - 1;
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
                methodToInlineInterpreter.MidRepresentation.Vars.LocalVariables;
            var localVarsToAdd = mappedLocals.Select(id => new LocalVariable
                                                               {
                                                                   Kind = VariableKind.Local,
                                                                   Id = id.Value.Id,
                                                                   FixedType =
                                                                       localVars.First(
                                                                           item =>
                                                                           id.Key == item.Value.Id).
                                                                       Value.
                                                                       FixedType
                                                               }).ToList();
            var vars = intermediateCode.Vars;
            foreach (var localVariable in localVarsToAdd)
            {
                vars.LocalVariables[localVariable.Id] = localVariable;
            }
            vars.LocalVars.Clear();
            vars.LocalVars.AddRange(vars.LocalVariables.Values.ToList());
        }

        #region Instruction mapped

        private static List<LocalOperation> BuildLocalOperationsToInline(
            MethodInterpreter interpreter, Dictionary<string, LocalVariable> mappedNames,
            Dictionary<int, int> mappedVregs, LocalVariable result,
            Dictionary<int, LocalVariable> mappedLocals)
        {
            var localOperationsToInline = new List<LocalOperation>();
            var localOperations = interpreter.MidRepresentation.LocalOperations.ToList();
            foreach (var localOperation in localOperations)
            {
                var clone = localOperation.Clone();
                switch (clone.Kind)
                {
                    case LocalOperation.Kinds.Assignment:
                        InlineOperation((Assignment) clone.Value, mappedNames, mappedVregs, mappedLocals);
                        break;
                    case LocalOperation.Kinds.Call:
                        InlineCallInstruction((MethodData) clone.Value, mappedNames, mappedVregs, mappedLocals);
                        break;
                    case LocalOperation.Kinds.Return:
                        if (InlineFinalReturn(localOperationsToInline, result, clone)) continue;
                        break;
                }
                localOperationsToInline.Add(clone);
            }
            return localOperationsToInline;
        }

        private static void InlineCallInstruction(MethodData value, Dictionary<string, LocalVariable> mappedNames,
                                                  Dictionary<int, int> mappedVregs,
                                                  Dictionary<int, LocalVariable> mappedLocals)
        {
            for (var i = 0; i < value.Parameters.Count; i++)
            {
                var parameter = value.Parameters[i];
                var identifier = parameter as LocalVariable;
                if (identifier == null)
                    continue;
                value.Parameters[i] = UpdateVregId(mappedVregs, identifier.Clone(), mappedNames, mappedLocals);
            }
        }

        private static bool InlineFinalReturn(List<LocalOperation> localOperationsToInline, LocalVariable result,
                                              LocalOperation clone)
        {
            if (result == null)
                return true;
            var assign = new Assignment
                             {
                                 Left = result,
                                 Right = (IdentifierValue) clone.Value
                             };
            localOperationsToInline.Add(new LocalOperation
                                            {
                                                Kind = LocalOperation.Kinds.Assignment,
                                                Value = assign
                                            }
                );
            return false;
        }

        private static void InlineOperation(Assignment value,
                                            Dictionary<string, LocalVariable> mappedNames,
                                            Dictionary<int, int> mappedVregs,
                                            Dictionary<int, LocalVariable> mappedLocals)
        {
            var leftVar = value.Left;
            value.Left = UpdateVregId(mappedVregs, leftVar, mappedNames, mappedLocals);

            var rightVar = value.Right as LocalVariable;
            if (rightVar == null)
                return;
            value.Right = UpdateVregId(mappedVregs, rightVar, mappedNames, mappedLocals);
        }

        private static LocalVariable UpdateVregId(Dictionary<int, int> mappedVregs, LocalVariable leftVar,
                                                  Dictionary<string, LocalVariable> argumentNames,
                                                  Dictionary<int, LocalVariable> mappedLocals)
        {
            switch (leftVar.Kind)
            {
                case VariableKind.Vreg:
                    leftVar = leftVar.Clone();
                    leftVar.Id = mappedVregs[leftVar.Id];
                    break;
                case VariableKind.Argument:
                    leftVar = argumentNames[leftVar.Name].Clone();
                    leftVar.Id = mappedVregs[leftVar.Id];
                    break;
                case VariableKind.Local:
                    leftVar = mappedLocals[leftVar.Id].Clone();
                    break;
            }
            return leftVar.Clone();
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
                methodToInlineInterpreter.MidRepresentation.Vars.LocalVariables;
            for (var i = 0; i < localVariables.Count; i++)
            {
                var identifierValue = localVariables[i].Clone();
                mappedNames[identifierValue.Id] = identifierValue;
                identifierValue.Id += count;
            }
            return mappedNames;
        }

        private static Dictionary<string, LocalVariable> BuildMappedParameters(MethodInterpreter interpreter,
                                                                               MethodData methodData)
        {
            var mappedNames = new Dictionary<string, LocalVariable>();
            for (var i = 0; i < methodData.Parameters.Count; i++)
            {
                var identifierValue = methodData.Parameters[i];
                var argumentVariable = interpreter.MidRepresentation.Vars.Arguments[i];
                mappedNames[argumentVariable.Name] = identifierValue as LocalVariable;
            }
            return mappedNames;
        }

        #endregion
    }
}