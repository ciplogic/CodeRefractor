
using System.Text;
using CodeRefractor.Util;


namespace CodeRefractor.CodeWriter.Output
{
    /**
     * Class that manages code output generation, indenting the text nicely in the 
     * process.
     */

    public static class StringBuilderExtensions
    {
        public static StringBuilder BracketOpen(this StringBuilder sb)
        {
            sb.Append(" {\n");

            return sb;
        }

        public static StringBuilder BracketClose(this StringBuilder sb, bool assignedStatement = false)
        {
            sb.Append("}\n");

            return sb;
        }

        public static StringBuilder BlankLine(this StringBuilder sb)
        {
            sb.Append("\n");


            return sb;
        }

    }
}