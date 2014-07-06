using System;
using System.Reflection;
using System.Runtime.InteropServices;
using CodeRefractor.MiddleEnd;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using Tao.OpenGl;
using Tao.Sdl;

namespace SimpleAdditions
{
    public class CrGlu
    {
    }

    public class CrGl
    {
    }
    public class CrSdl
    {
    }
    public class TypeResolver : CrTypeResolver
    {
        public TypeResolver()
        {
            MapType<CrGl>(typeof(Gl));
            MapType<CrGlu>(typeof(Glu));
            MapType<CrSdl>(typeof(Sdl));
        }

        public override MethodInterpreter Resolve(MethodBase methodInterpreter)
        {
            var method = methodInterpreter;
            var result = new PlatformInvokeMethod(method) ;
            if (method.DeclaringType == typeof(Glu))
            {
                ResolveAsPinvoke(result, "glu32.dll", CallingConvention.StdCall);
                return result;
            }
            if (method.DeclaringType == typeof(Gl))
            {
                ResolveAsPinvoke(result, "opengl32.dll", CallingConvention.StdCall);
                return result;
            }
            if (method.DeclaringType == typeof(Sdl))
            {
                ResolveAsPinvoke(result, "sdl.dll", CallingConvention.Cdecl);
                return result;
            }
            return null;
        }
    }
}