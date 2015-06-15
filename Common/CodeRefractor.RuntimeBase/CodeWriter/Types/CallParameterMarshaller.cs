#region Uses

using System.Reflection;

#endregion

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
            _parameterInfo = parameterInfo;
            _targetType = targetType;
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
            return _parameterInfo.Name;
        }
    }
}