#region Usings

using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

namespace CodeRefractor.Compiler.FrontEnd
{
    public class GlobalMethodPool
    {
        public readonly Dictionary<string, MethodBase> MethodInfos = new Dictionary<string, MethodBase>();
        public readonly Dictionary<string, MethodInterpreter> Interpreters = new Dictionary<string, MethodInterpreter>();
        public readonly Dictionary<string, ClassInterpreter> Classes = new Dictionary<string, ClassInterpreter>();

        static GlobalMethodPool()
        {
            Instance = new GlobalMethodPool();
        }

        public static GlobalMethodPool Instance { get; private set; }
    }
}