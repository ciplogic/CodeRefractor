#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    internal class PlatformInvokeCodeWriter
    {
        private readonly List<PlatformInvokeDllImports> _libraries = new List<PlatformInvokeDllImports>();
        private int _methodCount;

        public static string Import(string dll, string method)
        {
            StaticInstance._methodCount++;
            var id = StaticInstance._methodCount;
            var findItem = StaticInstance._libraries.FirstOrDefault(lib => lib.DllName == dll);
            if (findItem == null)
            {
                findItem = new PlatformInvokeDllImports(dll);
                StaticInstance._libraries.Add(findItem);
            }
            var dllId = string.Format("dll_method_{0}", id);
            findItem.Methods.Add(method, dllId);
            return dllId;
        }

        private static PlatformInvokeCodeWriter StaticInstance = new PlatformInvokeCodeWriter();


        public static string LoadDllMethods()
        {
            var sb = new StringBuilder();

            sb.AppendLine("void mapLibs() {");
            var pos = 0;
            foreach (var library in StaticInstance._libraries)
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