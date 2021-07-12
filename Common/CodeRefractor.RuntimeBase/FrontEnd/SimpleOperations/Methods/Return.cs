#region Uses

using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Methods
{
    public class Return : LocalOperation
    {
        public Return()
            : base(OperationKind.Return)
        {
        }

        public IdentifierValue Returning { get; set; }

        public void WriteCodeToOutput(CodeOutput bodySb, MethodInterpreter interpreter)
        {
            bodySb.Append("\n");

            if (Returning == null)
            {
                bodySb.Append("return;");
            }
            else
            {
                //Need to expand this for more cases
                if (Returning is ConstValue)
                {
                    var retType = interpreter.Method.GetReturnType();
                    if (retType == typeof (string))
                    {
                        bodySb.AppendFormat("return {0};", Returning.ComputedValue());
                    }
                    else
                    {
                        bodySb.AppendFormat("return {0};", Returning.Name);
                    }
                }
                else
                {
                    bodySb.AppendFormat("return {0};", Returning.Name);
                }
            }
        }
    }
}