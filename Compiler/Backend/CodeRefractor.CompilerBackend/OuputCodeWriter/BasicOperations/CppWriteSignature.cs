using System.Reflection;
using System.Text;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.CompilerBackend.OuputCodeWriter.BasicOperations
{
    public static class CppWriteSignature
    {
        internal static StringBuilder WriteSignature(MethodInterpreter interpreter, bool writeEndColon = false)
        {
            var sb = new StringBuilder();
            if (interpreter == null)
                return sb;
            var text = interpreter.Method.WriteHeaderMethodWithEscaping(writeEndColon);
            sb.Append(text);
            return sb;
        }

    }
}