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

        [Test]
        public void TestStringTrimStart()
        {
            const string stringValue = " \n\t Abcd";

            Assert.AreEqual(stringValue.TrimStart(), 
                StringImpl.TrimStart(stringValue), 
                "Default trim should remove '\\n', '\\t' and ' '");
            
            Assert.AreEqual(stringValue.TrimStart('\n', ' '), 
                StringImpl.TrimStart(stringValue, '\n', ' '), 
                "Custom trims should remove only some characters.");

            Assert.AreEqual(stringValue.TrimStart('\n', '\t', ' ', 'A', 'b', 'c', 'd'), 
                StringImpl.TrimStart(stringValue, '\n', '\t', ' ', 'A', 'b', 'c', 'd'),
                "Trims should be able to make strings empty.");

            Assert.AreEqual("".TrimStart(), 
                StringImpl.TrimStart(""),
                "Trims on empty strings should be allowed.");
        }
    }
}
