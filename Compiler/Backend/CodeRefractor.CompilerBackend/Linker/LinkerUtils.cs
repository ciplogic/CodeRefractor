using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Linker
{
    public static class LinkerUtils
    {
        public static string ComputedValue(this IdentifierValue identifierValue)
        {
            var constValue = identifierValue as ConstValue;
            if(constValue==null)
            {
                return identifierValue.Name;
            }
            var computeType = identifierValue.ComputedType();
            if (computeType == typeof(string))
            {
                var stringTable = LinkingData.Instance.Strings;
                var stringId = stringTable.GetStringId((string) constValue.Value);
                
                return string.Format("_str({0})", stringId);
            }
            return constValue.Name;
        }
    }
}