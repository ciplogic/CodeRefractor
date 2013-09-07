using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeRefactor.OpenRuntime
{
    public class CrStartLib
    {
        public static unsafe CrString[] GetArguments(int argc, byte**argv)
        {
            var result = new CrString[argc];
            for(var i = 0;i<argc;i++)
            {
                result[i] = new CrString(argv[i]);
            }
            return result;
        }
    }
}
