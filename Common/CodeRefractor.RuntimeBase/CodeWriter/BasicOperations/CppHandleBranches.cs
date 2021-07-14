﻿#region Uses

using System;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Operators;
using CodeRefractor.Shared;

#endregion

namespace CodeRefractor.CodeWriter.BasicOperations
{
    internal static class CppHandleBranches
    {
        public static void HandleBranchOperator(object operation, StringBuilder sb)
        {
            var localBranch = (LocalOperation) operation;
            var objList = (BranchOperator) localBranch;
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
                case OpcodeBranchNames.BleUn: //Should we treat unsigned differently ?
                case OpcodeBranchNames.BleUnS:
                    CppHandleCompareBranches.HandleBle(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Blt:
                case OpcodeBranchNames.BltUn:
                case OpcodeBranchNames.BltUnS:
                    CppHandleCompareBranches.HandleBlt(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Bne:
                case OpcodeBranchNames.BneUn:
                    CppHandleCompareBranches.HandleBne(localVar, secondVar, sb, jumpAddress);
                    break;

                default:
                    throw new InvalidOperationException($"Operation '{operationName}' is not handled");
            }
        }

        private static void HandleBrFalse(IdentifierValue localVar, StringBuilder sb, int jumpAddress)
        {
            var local = localVar.Name;
            sb.AppendFormat("if(!({0})) goto label_{1};", local, jumpAddress.ToHex());
        }

        private static void HandleBrTrue(IdentifierValue localVar, StringBuilder sb, int jumpAddress)
        {
            var local = localVar.Name;
            sb.AppendFormat("if({0}) goto label_{1};", local, jumpAddress.ToHex());
        }
    }
}