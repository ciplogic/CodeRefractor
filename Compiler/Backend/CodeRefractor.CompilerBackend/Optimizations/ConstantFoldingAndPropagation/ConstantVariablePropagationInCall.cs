#region Usings

using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation
{
    public class ConstantVariablePropagationInCall : ConstantVariablePropagationBase
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var operations = methodInterpreter.MidRepresentation.LocalOperations.ToArray();
            for (var i = 0; i < operations.Length - 1; i++)
            {
                Assignment srcVariableDefinition;
                var constValue = GetConstantFromOperation(operations[i], out srcVariableDefinition);
                if (constValue == null)
                    continue;

                for (var j = i + 1; j < operations.Length; j++)
                {
                    var destOperation = operations[j];
                    if (destOperation.Kind == OperationKind.Label)
                        break;
                    if (destOperation.Kind == OperationKind.BranchOperator)
                        break;
                    if (destOperation.Kind != OperationKind.Call) continue;
                    var callData = (MethodData) destOperation.Value;
                    if (SameVariable(callData.Result, srcVariableDefinition.AssignedTo))
                        break;
                    var pos = -1;
                    bool found;
                    do
                    {
                        found = false;
                        foreach (var identifierValue in callData.Parameters)
                        {
                            pos++;
                            if (!SameVariable(srcVariableDefinition.AssignedTo, identifierValue as LocalVariable))
                                continue;
                            found = true;
                            break;
                        }
                        if (!found) continue;
                        callData.Parameters[pos] = constValue;
                        Result = true;
                    } while (found);
                }
            }
        }
    }
}