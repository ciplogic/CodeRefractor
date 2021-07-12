#region Uses

using System.Text;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.CodeWriter.Output
{
    /**
     * Class that manages code output generation, indenting the text nicely in the 
     * process.
     */

    public static class StringBuilderExtensions
    {
        /**
         * Opens a new bracket.
         * TODO: this should be moved into a bracket strategy, to allow changing the bracket style
         */

        public static StringBuilder BracketOpen(this StringBuilder sb)
        {
            sb.Append(" {\n");

            return sb;
        }

        /**
         * Closes a bracket
         * TODO: this should be moved into a bracket strategy, to allow changing the bracket style
         */

        public static StringBuilder BracketClose(this StringBuilder sb, bool assignedStatement = false)
        {
            sb.Append("}\n");
         
            return sb;
        }

        /**
         * Adds a blank line into the code.
         */

        public static StringBuilder BlankLine(this StringBuilder sb)
        {
            // if we are already at the beginning of a line,
            // there's no need to close the current line
         
                sb.Append("\n");
          

            return sb;
        }

    }
}