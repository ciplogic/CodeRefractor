#region Uses

using System.Reflection;
using System.Runtime.InteropServices;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;

#endregion

namespace CodeRefractor.MiddleEnd.Interpreters.NonCil
{
    public class PlatformInvokeMethod : MethodInterpreter
    {
        public PlatformInvokeMethod(MethodBase method) : base(method)
        {
            Kind = MethodKind.PlatformInvoke;
            IsStatic = true;
            HandlePlatformInvokeMethod(method);
        }

        public bool IsStatic { get; set; }
        public CallingConvention CallingConvention { get; set; }
        public string MethodName { get; set; }
        public string EntryPoint { get; set; }
        public string LibraryName { get; set; }

        void HandlePlatformInvokeMethod(MethodBase method)
        {
            var pinvokeAttribute = method.GetCustomAttribute<DllImportAttribute>();

            if (pinvokeAttribute == null)
                return;
            LibraryName = pinvokeAttribute.Value;
            MethodName = method.Name;
            CallingConvention = pinvokeAttribute.CallingConvention;
            EntryPoint = pinvokeAttribute.EntryPoint;
            Kind = MethodKind.PlatformInvoke;
        }

        public static bool IsPlatformInvoke(MethodBase method)
        {
            var pinvokeAttribute = method.GetCustomAttribute<DllImportAttribute>();

            if (pinvokeAttribute == null)
                return false;
            return true;
        }
    }
}