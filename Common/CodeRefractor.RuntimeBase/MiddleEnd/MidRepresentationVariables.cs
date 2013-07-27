#region Usings

using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class MidRepresentationVariables
    {
        public readonly SortedDictionary<int, LocalVariable> LocalVariables = new SortedDictionary<int, LocalVariable>();
        public readonly List<LocalVariableInfo> Variables = new List<LocalVariableInfo>();
        public readonly List<ArgumentVariable> Arguments = new List<ArgumentVariable>();
        public List<LocalVariable> VirtRegs = new List<LocalVariable>();
        public readonly List<LocalVariable> LocalVars = new List<LocalVariable>();
    }
}