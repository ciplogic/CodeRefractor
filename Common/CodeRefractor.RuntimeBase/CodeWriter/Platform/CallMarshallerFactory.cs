using System.Reflection;
using CodeRefractor.Util;

namespace CodeRefractor.CodeWriter.Platform
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
            return _targetType + " _" + _parameterInfo.Name + " = " + _parameterInfo.Name + ".get()->Text.get()->Items;\n";
        }

        /**
         * The code that is needed to get the parameter as the method call.
         */
        public override string GetParameterString()
        {
            return "_" + this._parameterInfo.Name;
        }
    }

    /**
     * This class checks if marshalling between types should be done, and
     * dispatches to the right marshaller if that's the case.
     */
    public class CallMarshallerFactory
    {
        public static CallParameterMarshaller CreateMarshaller(ParameterInfo parameterInfo)
        {
            var cppType = parameterInfo.ParameterType.ToCppName(isPInvoke: true);

            if ("System_Char*" == cppType)
            {
                return new CallParameterStringMarshaller(parameterInfo, cppType);
            }

            return new CallParameterMarshaller(parameterInfo, cppType);
        }
    }
}