namespace CodeRefractor.RuntimeBase.Analyze
{
    public static class NamerUtils
    {
        public static string ValidName(this string name)
        {
            return FieldNameTable.Instance.GetFieldName(name);
        }
    }
}