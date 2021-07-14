#region Uses

using System;
using System.IO;

#endregion

namespace CodeRefractor.Util
{
    public static class NativeCompilationUtils
    {
        public static Options CompilerOptions = new GccOptions();

        public static void SetCompilerOptions(string compilerKind)
        {
            switch (compilerKind)
            {
                case "gcc":
                    CompilerOptions = new GccOptions();
                    break;
                case "clang":
                    CompilerOptions = new ClangOptions();
                    break;
                case "msvc":
                    CompilerOptions = new WindowsClOptions();
                    break;
            }
        }

        public static string GetFullFileName(this string fileName)
        {
            var fileInfo = new FileInfo(fileName);

            return fileInfo.FullName;
        }

        public static void CompileAppToNativeExe(string fileName, string applicationNativeExe)
        {
            var fileInfo = new FileInfo(fileName);

            fileName = fileInfo.FullName;
            if (!fileInfo.Exists)
            {
                throw new InvalidDataException($"Filename: {fileName} does not exist!");
            }
            var pathToGpp = CompilerOptions.PathOfCompilerTools + CompilerOptions.CompilerExe;

            var commandLineFormat = "{0} " + CompilerOptions.OptimizationFlags + " {2} -o {1}";

            fileName = GetSafeFileOrPath(fileName);
            applicationNativeExe = GetSafeFileOrPath(applicationNativeExe);

            var arguments = string.Format(commandLineFormat, fileName, applicationNativeExe,
                CompilerOptions.LinkerOptions);
            var standardOutput = pathToGpp.ExecuteCommand(arguments, CompilerOptions.PathOfCompilerTools);
            if (!string.IsNullOrWhiteSpace(standardOutput) && standardOutput.Contains("error"))
            {
                throw new InvalidOperationException($"Errors when compiling: {standardOutput}");
            }
            (CompilerOptions.PathOfCompilerTools + "strip").ExecuteCommand(applicationNativeExe,
                Path.GetDirectoryName(applicationNativeExe));
        }

        private static string GetSafeFileOrPath(string fileName)
        {
            if (fileName.Contains(" "))
                fileName = $"\"{fileName}\"";
            return fileName;
        }

        private class ClangOptions : Options
        {
            public ClangOptions()
            {
                PathOfCompilerTools = @"C:\Oss\Llvm\3_4_svn\bin\";
                CompilerExe = "clang++.exe";
                OptimizationFlags = "-O2 ";
            }
        }

        private class GccOptions : Options
        {
            public GccOptions()
            {
                PathOfCompilerTools = @"C:\MinGW\bin\";
                CompilerExe = "g++.exe";
                OptimizationFlags = "-Ofast -fomit-frame-pointer -ffast-math -std=c++11 -static-libgcc -fpermissive";
                LinkerOptions = "";
            }
        }

        private class WindowsClOptions : Options
        {
            public WindowsClOptions()
            {
                PathOfCompilerTools = @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\";
                CompilerExe = "cl.exe";
                OptimizationFlags = "";
                LinkerOptions = "";
            }
        }

        public class Options
        {
            public string CompilerExe;
            public string CompilerKind;
            public string LinkerOptions;
            public string OptimizationFlags;
            public string PathOfCompilerTools;
        }
    }
}