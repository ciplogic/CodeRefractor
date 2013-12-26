using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.RuntimeBase.MiddleEnd;
using Tao.OpenGl;

namespace SimpleAdditions
{
    class CodeRefractorMapper : EntityMapper
    {
        public override MethodInterpreter MapMethod(MethodBase method)
        {
            if (method.DeclaringType == typeof (Gl))
            {
                var methodInterpreter = new MethodInterpreter(method);
                return methodInterpreter;
            }
            return null;
        }
    }
}
