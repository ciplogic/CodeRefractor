using System;
using System.IO;

namespace VisualCompiler
{
    public static class CompilerUtils
    {
        public static void DeleteFilesByWildcards(string pattern, string path = "./")
        {
            var files = Directory.GetFiles(path, pattern);
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Exception: "+ex);
                }
            }
        }
    }
}