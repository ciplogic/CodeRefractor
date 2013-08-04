#region Usings

using System;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class FieldSetter : LocalVariable
    {
        public IdentifierValue Instance;
        public string FieldName;
    }

    public class StaticFieldSetter : LocalVariable
    {
        public IdentifierValue Instance;
        public Type DeclaringType;
        public string FieldName;
    }
}