using System.Reflection;

namespace CodeRefractor.CodeWriter.Types
{
    /**
     * Transforms a string from one format to the other.
     */
    public class CallParameterStringMarshaller : CallParameterMarshaller
    {
        public CallParameterStringMarshaller(ParameterInfo parameterInfo, string targetType) : base(parameterInfo, targetType)
        {
        }

        /**
         * The transformation statements needed to get it to the required function type.
         */
        public override string GetTransformationCode()
        {
            return _targetType + " _" + _parameterInfo.Name + " = " + _parameterInfo.Name + "->Text->Items;\n";
        }

        /**
         * The code that is needed to get the parameter as the method call.
         */
        public override string GetParameterString()
        {
            return "_" + this._parameterInfo.Name;
        }
    }
}