#region Usings

using System;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
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