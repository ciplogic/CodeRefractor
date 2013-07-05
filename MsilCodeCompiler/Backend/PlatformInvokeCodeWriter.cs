#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace CodeRefractor.Compiler.Backend
{
    internal class PlatformInvokeCodeWriter
    {
        public List<PlatformInvokeDllImports> Libraries = new List<PlatformInvokeDllImports>();
        public int MethodCount;

        public static string Import(string dll, string method)
        {
            Instance.MethodCount++;
            var id = Instance.MethodCount;
            var findItem = Instance.Libraries.FirstOrDefault(lib => lib.DllName == dll);
            if (findItem == null)
            {
                findItem = new PlatformInvokeDllImports(dll);
                Instance.Libraries.Add(findItem);
            }
            var dllId = string.Format("dll_method_{0}", id);
            findItem.Methods.Add(method, dllId);
            return dllId;
        }

        public static PlatformInvokeCodeWriter StaticInstance = new PlatformInvokeCodeWriter();

        public static PlatformInvokeCodeWriter Instance
        {
            get { return StaticInstance; }
        }

        public static string LoadDllMethods()
        {
            var sb = new StringBuilder();

            sb.AppendLine("void mapLibs() {");
            var pos = 0;
            foreach (var library in Instance.Libraries)
            {
                sb.AppendFormat("auto lib_{0} = LoadNativeLibrary(L\"{1}\");", pos, library.DllName);
                sb.AppendLine();
                foreach (var method in library.Methods)
                {
                    sb.AppendFormat("{0} = ({0}_type)LoadNativeMethod(lib_{2}, \"{1}\");", method.Value, method.Key, pos);
                    sb.AppendLine();
                }
                pos++;
            }
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}