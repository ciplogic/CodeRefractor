#region Uses

using System.Reflection;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public class MethodBaseKey
    {
        public MethodBaseKey(MethodBase method)
        {
            Ensure.IsTrue(method != null, "Method should not be null");
            Method = method;
        }

        public MethodBase Method { get; }

        public override int GetHashCode()
        {
            var declaringType = Method.DeclaringType;
            var name = Method.Name;
            var hash = Method.GetParameters().Length.GetHashCode();
            return declaringType.GetHashCode() ^ name.GetHashCode() ^ hash;
        }

        public override bool Equals(object obj)
        {
            var destObject = (MethodBaseKey) obj;
            return MethodBaseKeyComparer.CompareEquals(this, destObject);
        }

        public static bool ParameterListIsMatching(ParameterInfo[] srcParameters, ParameterInfo[] destParameters)
        {
            if (srcParameters.Length != destParameters.Length)
                return false;
            for (var index = 0; index < srcParameters.Length; index++)
            {
                var parameter = srcParameters[index];
                var destParameter = destParameters[index];
                if (parameter.ParameterType != destParameter.ParameterType)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return $"{Method.DeclaringType.Name}.{Method.Name}";
        }
    }
}