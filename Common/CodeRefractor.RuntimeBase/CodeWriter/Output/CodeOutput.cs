using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeRefractor.Util;

namespace CodeRefractor.CodeWriter.Output
{
    /**
     * Class that manages code output generation, indenting the text nicely in the 
     * process.
     */
    public class CodeOutput
    {
        private IndentCode _indentCode;
        private StringBuilder _stringBuilderOutput;

        // keeps in mind if the outputting is now at a line beginning or not.
        private bool _atLineBeginning = true;

        public CodeOutput()
        {
            this._stringBuilderOutput = new StringBuilder("");
            this._indentCode = new IndentCode(this._stringBuilderOutput);
        }

        /**
         * Append the text applying the format as well.
         */
        public CodeOutput AppendFormat(string text, params Object[] arguments)
        {
            var formattedText = String.Format(text, arguments);

            return this.Append(formattedText);
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
                _stringBuilderOutput.Append(text);
                return this;
            }

            // if the text does contain enters it's a bit more tricky now, since we need to indent
            // between the lines, but also only when we don't have consecutive empty lines.
            var lines = text.Split('\n');
            
            // the lines of the current code we're supposed to append.
            // the first line shouldn't be indented, the last line shouldn't have an ending
            // newline.
            _stringBuilderOutput.Append(lines[0]);
            lines.ButFirst(line =>
            {
                _stringBuilderOutput.Append("\n");

                if (line != "")
                {
                    IndentCode();
                    _stringBuilderOutput.Append(line);
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
            _stringBuilderOutput.Append(" {\n");
            _indentCode.ChangeIndent(+1);
            _atLineBeginning = true;

            return this;
        }

        /**
         * Closes a bracket
         * TODO: this should be moved into a bracket strategy, to allow changing the bracket style
         */
        public CodeOutput BracketClose()
        {
            _indentCode.ChangeIndent(-1);
            _stringBuilderOutput.Append("\n}");

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
                _stringBuilderOutput.Append("\n");
            }
            else
            {
                _stringBuilderOutput.Append("\n\n");
            }

            _atLineBeginning = true;

            return this;
        }

        /**
         * Indents the code.
         */
        private void IndentCode()
        {
            _atLineBeginning = false;
            _indentCode.indent();
        }

        public override string ToString()
        {
            return _stringBuilderOutput.ToString();
        }
    }
}
