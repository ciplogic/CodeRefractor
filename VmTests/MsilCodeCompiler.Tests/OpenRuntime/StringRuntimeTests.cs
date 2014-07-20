using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefactor.OpenRuntime;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests.OpenRuntime
{
    [TestFixture]
    public class StringRuntimeTests
    {
        [Test]
        public void TestStringSubstring1()
        {
            var s1 = "Abcd";
            var s2 = s1.Substring(2);
            var s3 = StringImpl.Substring(s1, 2);
            Assert.AreEqual(s2, s3);
        }
        [Test]
        public void TestStringSubstring2()
        {
            var s1 = "Abcd";
            var s2 = s1.Substring(2,2);
            var s3 = StringImpl.Substring(s1, 2,2);
            Assert.AreEqual(s2, s3);
        }
    }
}
