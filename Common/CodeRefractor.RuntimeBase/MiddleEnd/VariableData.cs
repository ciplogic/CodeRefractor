using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class VariableData
    {
        public EscapingMode Escaping;

        public override string ToString()
        {
            return Escaping.ToString();
        }
    }
}