#region Usings

using System.Text;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.CodeWriter.BasicOperations
{
    internal static class CppHandleCompareBranches
    {
        public static void WriteCompareBranch(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb,
            int jumpAddress,
            string comparisonOperator)
        {
            var local = localVar.Name;
            var second = secondVar.Name;
            sb.AppendFormat("if({0}{3}{1}) goto label_{2};", local, second, jumpAddress.ToHex(), comparisonOperator);
        }

        public static void HandleBne(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb,
            int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, "!=");
        }

        public static void HandleBlt(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb,
            int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, "<");
        }

        public static void HandleBle(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb,
            int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, "<=");
        }

        public static void HandleBgt(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb,
            int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, ">");
        }

        public static void HandleBge(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb,
            int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, ">=");
        }

        public static void HandleBeq(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb,
            int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, "==");
        }
    }
}