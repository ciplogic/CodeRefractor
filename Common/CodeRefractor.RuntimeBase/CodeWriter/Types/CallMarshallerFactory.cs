#region Uses

using System.Reflection;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.CodeWriter.Types
{
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