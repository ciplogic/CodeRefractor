#region Usings

using System;
using System.Text;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.Compiler.Backend.HandleOperations
{
    internal static class CppHandleBranches
    {
        public static void HandleBranchOperator(object operation, StringBuilder sb)
        {
            var localBranch = (LocalOperation) operation;
            var objList = (BranchOperator) localBranch.Value;
            var operationName = objList.Name;
            var jumpAddress = objList.JumpTo;
            var localVar = objList.CompareValue;
            var secondVar = objList.SecondValue;
            switch (operationName)
            {
                case OpcodeBranchNames.BrTrue:
                    HandleBrTrue(localVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.BrFalse:
                    HandleBrFalse(localVar, sb, jumpAddress);
                    break;

                case OpcodeBranchNames.Beq:
                    CppHandleCompareBranches.HandleBeq(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Bge:
                    CppHandleCompareBranches.HandleBge(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Bgt:
                    CppHandleCompareBranches.HandleBgt(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Ble:
                    CppHandleCompareBranches.HandleBle(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Blt:
                    CppHandleCompareBranches.HandleBlt(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Bne:
                    CppHandleCompareBranches.HandleBne(localVar, secondVar, sb, jumpAddress);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Operation '{0}' is not handled", operationName));
            }
        }

        private static void HandleBrFalse(IdentifierValue localVar, StringBuilder sb, int jumpAddress)
        {
            var local = localVar.Name;
            sb.AppendFormat("if(!({0})) goto label_{1};", local, jumpAddress);
        }

        private static void HandleBrTrue(IdentifierValue localVar, StringBuilder sb, int jumpAddress)
        {
            var local = localVar.Name;
            sb.AppendFormat("if({0}) goto label_{1};", local, jumpAddress);
        }
    }
}