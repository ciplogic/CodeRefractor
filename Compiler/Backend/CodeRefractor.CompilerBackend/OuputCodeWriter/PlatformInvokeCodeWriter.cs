#region Usings

using System.Linq;
using System.Text;
using CodeRefractor.CompilerBackend.Linker;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    internal static class PlatformInvokeCodeWriter
    {
        public static string Import(string dll, string method)
        {
            LinkingData.LibraryMethodCount++;
            var id = LinkingData.LibraryMethodCount;
            var findItem = LinkingData.Libraries.FirstOrDefault(lib => lib.DllName == dll);
            if (findItem == null)
            {
                findItem = new PlatformInvokeDllImports(dll);
                LinkingData.Libraries.Add(findItem);
            }
            var dllId = string.Format("dll_method_{0}", id);
            findItem.Methods.Add(method, dllId);
            return dllId;
        }

        public static string LoadDllMethods()
        {
            var sb = new StringBuilder();

            sb.AppendLine("void mapLibs() {");
            var pos = 0;
            foreach (var library in LinkingData.Libraries)
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