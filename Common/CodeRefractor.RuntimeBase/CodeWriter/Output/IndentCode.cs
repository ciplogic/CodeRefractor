#region Uses

using System.Text;

#endregion

namespace CodeRefractor.CodeWriter.Output
{
    internal class IndentCode
    {
        private const string IndentString = "    "; // TODO: this should be configurable.
        private readonly StringBuilder _stringBuilderOutput;
        private int _indentLevel;

        public IndentCode(StringBuilder stringBuilderOutput)
        {
            _stringBuilderOutput = stringBuilderOutput;
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
            _indentLevel += indent;

            return this;
        }
    }
}