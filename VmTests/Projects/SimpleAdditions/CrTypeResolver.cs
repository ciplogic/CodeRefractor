using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using Tao.OpenGl;
using Tao.Sdl;

namespace Game
{
    public class TypeResolver : CrTypeResolver
    {
        private List<string> _sdlMethods = new List<string> {"SDL_Init"};
        public override bool Resolve(MethodInterpreter methodInterpreter)
        {
            var method = methodInterpreter.Method;
            if (method.DeclaringType == typeof (Sdl))
            {
                if(_sdlMethods.Contains(method.Name))
                {
                    methodInterpreter.Kind = MethodKind.PlatformInvoke;
                    methodInterpreter.PlatformInvoke.LibraryName = "sdl.dll";
                    methodInterpreter.PlatformInvoke.EntryPoint = method.Name;
                    methodInterpreter.Description.MethodName = method.Name;
                    methodInterpreter.Description.CallingConvention = CallingConvention.StdCall;
                    return true;
                }
            }
            return false;
        }

        public override Type ResolveType(Type type)
        {
            if (type == typeof (Gl))
                return typeof(CrGl);
            if (type == typeof(Sdl))
                return typeof(CrSdl);
            return null;
        }
    }

    public class CrGl
    {
    }
    public class CrSdl
    {
    }
}