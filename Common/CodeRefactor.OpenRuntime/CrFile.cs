using System.IO;
using CodeRefractor.Runtime.Annotations;
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

        [CppMethodBody(
            Header = "QString;QFile"
            , Code = "" +
                     "QFile inputFile(\"path\");" +
                     @" if (inputFile.open(QIODevice::ReadOnly))
    {
       QTextStream in(&inputFile);
       QVector<QString> lines;
       while ( !in.atEnd() )
       {
          auto line = in.readLine();
//in.readAll()
          lines.append(line);

       }
       inputFile.close();
    }"
            )]
        public static string ReadAllText(string path)
        {
            return string.Empty;
        }
    }
}