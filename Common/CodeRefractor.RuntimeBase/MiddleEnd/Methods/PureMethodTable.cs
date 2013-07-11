using System.Collections.Generic;
using System.Reflection;

namespace CodeRefractor.RuntimeBase.MiddleEnd.Methods
{
    public class PureMethodTable
    {
        public HashSet<string> Descriptions= new HashSet<string>();
        private static PureMethodTable StaticInstance = new PureMethodTable();

        public static PureMethodTable Instance{get
            {
                return StaticInstance;
            }}

        public bool ComputeMethodPurity(string description)
        {
            return Instance.Descriptions.Contains(description);
        }

        public static void AddPureFunction(MemberInfo description)
        {
            Instance.Descriptions.Add(description.ToString());
        }
    }
}
