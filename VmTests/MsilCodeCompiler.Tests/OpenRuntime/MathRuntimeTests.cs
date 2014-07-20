using System;
using CodeRefactor.OpenRuntime;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests.OpenRuntime
{
    [TestFixture]
    public class MathRuntimeTests
    {
        [Test]
        public void MathAbsInt16Test()
        {
            var v1 = (Int16)(-3);
            var v2 = Math.Abs(v1);
            var v3 = MathImpl.Abs(v1);
            Assert.AreEqual(v2, v3);
        }
    }
}