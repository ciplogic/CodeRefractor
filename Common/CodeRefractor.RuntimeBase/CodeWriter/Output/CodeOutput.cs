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

    public class CodeOutput
    {
        readonly IndentCode _indentCode;
        public StringBuilder StringBuilderOutput { get; }
        // keeps in mind if the outputting is now at a line beginning or not.
        bool _atLineBeginning = true;

        public CodeOutput()
        {
            StringBuilderOutput = new StringBuilder("");
            _indentCode = new IndentCode(StringBuilderOutput);
        }

        /**
         * Append the text applying the format as well.
         */

        public CodeOutput AppendFormat(string text, params object[] arguments)
        {
            var formattedText = string.Format(text, arguments);

            return Append(formattedText);
        }

        /**
         * Append the given code. This will take care of enters inside the code,
         * and eventual indenting that is required to be done.
         */

        public CodeOutput Append(string text)
        {
            // we only indent if we actually have new text, not enters or empty strings.
            if (_atLineBeginning && text.Trim() != "")
            {
                IndentCode();
            }

            // in case the text doesn't have enters, we simply add it to the buffer and are done
            if (!text.Contains("\n"))
            {
                StringBuilderOutput.Append(text);
                return this;
            }

            // if the text does contain enters it's a bit more tricky now, since we need to indent
            // between the lines, but also only when we don't have consecutive empty lines.
            var lines = text.Split('\n');

            // the lines of the current code we're supposed to append.
            // the first line shouldn't be indented, the last line shouldn't have an ending
            // newline.
            StringBuilderOutput.Append(lines[0]);
            lines.ButFirst(line =>
            {
                StringBuilderOutput.Append("\n");

                if (line != "")
                {
                    IndentCode();
                    StringBuilderOutput.Append(line);
                }
                else
                {
                    _atLineBeginning = true;
                }
            });

            return this;
        }

        /**
         * Opens a new bracket.
         * TODO: this should be moved into a bracket strategy, to allow changing the bracket style
         */

        public CodeOutput BracketOpen()
        {
            StringBuilderOutput.Append(" {\n");
            _indentCode.ChangeIndent(+1);
            _atLineBeginning = true;

            return this;
        }

        /**
         * Closes a bracket
         * TODO: this should be moved into a bracket strategy, to allow changing the bracket style
         */

        public CodeOutput BracketClose(bool assignedStatement = false)
        {
            _indentCode.ChangeIndent(-1);

            if (_atLineBeginning)
            {
                Append("}");
            }
            else
            {
                Append("\n}");
            }

            if (!assignedStatement)
            {
                Append("\n");
            }

            return this;
        }

        /**
         * Adds a blank line into the code.
         */

        public CodeOutput BlankLine()
        {
            // if we are already at the beginning of a line,
            // there's no need to close the current line
            if (_atLineBeginning)
            {
                StringBuilderOutput.Append("\n");
            }
            else
            {
                StringBuilderOutput.Append("\n\n");
            }

            _atLineBeginning = true;

            return this;
        }

        /**
         * Indents the code.
         */

        void IndentCode()
        {
            _atLineBeginning = false;
            _indentCode.indent();
        }

        public override string ToString()
        {
            return StringBuilderOutput.ToString();
        }
    }
}