using System.Collections.Generic;
using System.Reflection;

namespace CodeRefractor.RuntimeBase.MiddleEnd.Methods
{
    public enum MethodKind
    {
        Default,
        RuntimeLibrary,
        PlatformInvoke,
        Delegate,
        RuntimeCppMethod
    }
}