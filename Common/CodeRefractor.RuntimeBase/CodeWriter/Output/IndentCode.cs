using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRefractor.CodeWriter.Output
{
    class IndentCode
    {
        private readonly StringBuilder _stringBuilderOutput;
        private int _indentLevel = 0;
        private const string IndentString = "    "; // TODO: this should be configurable.

        public IndentCode(StringBuilder stringBuilderOutput)
        {
            this._stringBuilderOutput = stringBuilderOutput;
        }

        /**
         * Change the current indentation level, and indent the code.
         */
        public IndentCode indent()
        {
            for (var i = 0; i < _indentLevel; i++)
            {
                _stringBuilderOutput.Append(IndentString);
            }

            return this;
        }

        public IndentCode ChangeIndent(int indent)
        {
            this._indentLevel += indent;

            return this;
        }
    }
}
