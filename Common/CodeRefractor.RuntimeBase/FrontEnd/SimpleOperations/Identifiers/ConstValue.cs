#region Uses

using System;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Identifiers
{
    public class ConstValue : IdentifierValue
    {
        object _value;

        public ConstValue()
        {
        }

        public ConstValue(object value)
        {
            Value = value;
            if (Value != null)
                FixedType = new TypeDescription(Value.GetType());
            else
            {
                FixedType = new TypeDescription(null);
            }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (value is ConstValue)
                {
                    throw new Exception(
                        "Invalid value, you try to set the constant description instead of constant itself");
                }
                _value = value;
            }
        }

        public string Description
        {
            get { return string.Format("c: '{0}'", Value); }
        }

        public override IdentifierValue Clone()
        {
            return new ConstValue(Value);
        }

        public override TypeDescription ComputedType()
        {
            if (FixedType != null)
                return FixedType;

            return new TypeDescription(Value == null ? typeof (void) : Value.GetType());
        }

        public bool ValueEquals(ConstValue otherValue)
        {
            if (Value == null)
                return otherValue.Value == null;
            return Value.Equals(otherValue.Value);
        }

        public override string FormatVar()
        {
            if (Value == null)
                return "nullptr";
            var typeCode = Type.GetTypeCode(Value.GetType());
            if (typeCode == TypeCode.String)
            {
                return "\"" + Value + "\""; //Why single qoutes ?
            }
            return Value.ToString();
        }
    }
}