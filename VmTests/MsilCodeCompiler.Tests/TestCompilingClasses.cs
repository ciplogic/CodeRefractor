using System.IO;
using CodeRefractor.Compiler.Util;
using MsilCodeCompiler.Tests.Shared;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests
{
    [TestFixture]
    public class TestCompilingClasses : CompilingProgramBase
    {
        [Test]
        public void TestCompilation()
        {
            var bodyOfProgram = @"
using System;

class Point {
    public int X;
    public int Y;
}

class Program {
    public static void Main() {
        var p = new Point() {
            X = 2,
            Y = 4
        };
        Console.Write(p.X);
    }
}
";

            Assert.IsTrue(TryCompileCSharp(bodyOfProgram));
        }
        [Test]
        public void TestCallCompilation()
        {
            var bodyOfProgram = @"
using System;
class Program {
    public static void Main() {
      var dx = 1.0;
        var squareRoot = Math.Sqrt(dx);
        Console.Write(squareRoot);
    }
}
";

            Assert.IsTrue(TryCompileCSharp(bodyOfProgram));
        }
    }
}