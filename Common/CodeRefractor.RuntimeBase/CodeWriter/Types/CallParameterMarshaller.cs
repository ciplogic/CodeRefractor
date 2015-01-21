using System.Reflection;

namespace CodeRefractor.CodeWriter.Types
{
    /**
     * A call parameter is a parameter that will be used in calling a function.
     */
    public class CallParameterMarshaller
    {
        protected ParameterInfo _parameterInfo;
        protected string _targetType;

        public CallParameterMarshaller(ParameterInfo parameterInfo, string targetType)
        {
            this._parameterInfo = parameterInfo;
            this._targetType = targetType;
        }

        /**
         * The transformation statements needed to get it to the required function type.
         */
        public virtual string GetTransformationCode()
        {
            return "";
        }

        /**
         * The code that is needed to get the parameter as the method call.
         */
        public virtual string GetParameterString()
        {
            return this._parameterInfo.Name;
        }
    }
}