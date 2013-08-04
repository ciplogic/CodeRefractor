#region Usings

using System;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class ConstValue : IdentifierValue
    {
        private object _value;

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

        public ConstValue(object value)
        {
            Value = value;
        }

        public override Type ComputedType()
        {
            return Value == null ? typeof (void) : Value.GetType();
        }

        public string Description
        {
            get { return string.Format("c: '{0}'", Value); }
        }

        public override string FormatVar()
        {
            if (Value is string)
            {
            }
            return Value.ToString();
        }
    }
}