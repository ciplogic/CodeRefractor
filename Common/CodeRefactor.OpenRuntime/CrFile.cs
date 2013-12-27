using System.IO;
using CodeRefractor.RuntimeBase;

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof (File))]
    public class CrFile
    {

        [CppMethodBody(Header = "stdio.h", 
            Code = 
"    FILE *fl = fopen(path, \"r\");" +
    @"
fseek(fl, 0, SEEK_END);  
long len = ftell(fl);  
char *ret = new char[len];  
fseek(fl, 0, SEEK_SET);  
fread(ret, 1, len, fl);  
fclose(fl);  
return ret;  
"
        )]
        public static byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}