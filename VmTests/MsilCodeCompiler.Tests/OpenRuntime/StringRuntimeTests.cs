using System;
using System.Collections.Generic;
using System.Globalization;
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

        [Test]
        public void TestStringTrimEnd()
        {
            const string stringValue = "Abcd \n\t ";

            Assert.AreEqual(stringValue.TrimEnd(),
                StringImpl.TrimEnd(stringValue),
                "Default trim should remove '\\n', '\\t' and ' '");

            Assert.AreEqual(stringValue.TrimEnd('\n', ' '),
                StringImpl.TrimEnd(stringValue, '\n', ' '),
                "Custom trims should remove only some characters.");

            Assert.AreEqual(stringValue.TrimEnd('\n', '\t', ' ', 'A', 'b', 'c', 'd'),
                StringImpl.TrimEnd(stringValue, '\n', '\t', ' ', 'A', 'b', 'c', 'd'),
                "Trims should be able to make strings empty.");

            Assert.AreEqual("".TrimEnd(),
                StringImpl.TrimEnd(""),
                "Trims on empty strings should be allowed.");
        }

        [Test]
        public void TestStringTrim()
        {
            const string stringValue = " \t\n Abcd \n\t ";

            Assert.AreEqual(stringValue.Trim(),
                StringImpl.Trim(stringValue),
                "Default trim should remove '\\n', '\\t' and ' '");

            Assert.AreEqual(stringValue.Trim('\n', ' '),
                StringImpl.Trim(stringValue, '\n', ' '),
                "Custom trims should remove only some characters.");

            Assert.AreEqual(stringValue.Trim('\n', '\t', ' ', 'A', 'b', 'c', 'd'),
                StringImpl.Trim(stringValue, '\n', '\t', ' ', 'A', 'b', 'c', 'd'),
                "Trims should be able to make strings empty.");

            Assert.AreEqual("".Trim(),
                StringImpl.Trim(""),
                "Trims on empty strings should be allowed.");
        }

        [Test]
        public void TestIndexOf()
        {
            const string stringValue = "Some longer string, and some other data.";

            Assert.AreEqual(stringValue.IndexOf("om", 4, 3, StringComparison.Ordinal), // overload #1
                StringImpl.IndexOf(stringValue, "om", 4, 3, StringComparison.Ordinal));

            Assert.AreEqual(stringValue.IndexOf("om", 0, stringValue.Length, StringComparison.Ordinal),
                StringImpl.IndexOf(stringValue, "om", 0, stringValue.Length, StringComparison.Ordinal));

            Assert.AreEqual(stringValue.IndexOf("om", 3, stringValue.Length - 3, StringComparison.Ordinal),
                StringImpl.IndexOf(stringValue, "om", 3, stringValue.Length - 3, StringComparison.Ordinal));

            Assert.AreEqual(stringValue.IndexOf("some", 0, stringValue.Length, StringComparison.Ordinal),
                StringImpl.IndexOf(stringValue, "some", 0, stringValue.Length, StringComparison.Ordinal));

            Assert.AreEqual(stringValue.IndexOf("Some", 0, stringValue.Length, StringComparison.Ordinal),
                StringImpl.IndexOf(stringValue, "Some", 0, stringValue.Length, StringComparison.Ordinal));

            // overloaded versions
            Assert.AreEqual(stringValue.IndexOf("a", 0, stringValue.Length), // overload #2
                StringImpl.IndexOf(stringValue, "a", 0, stringValue.Length));

            Assert.AreEqual(stringValue.IndexOf("a", 30), // overload #3
                StringImpl.IndexOf(stringValue, "a", 30));

            Assert.AreEqual(stringValue.IndexOf("a", 30, StringComparison.Ordinal), // overload #4
                StringImpl.IndexOf(stringValue, "a", 30, StringComparison.Ordinal));

            Assert.AreEqual(stringValue.IndexOf("a"), // overload #5
                StringImpl.IndexOf(stringValue, "a"));

            Assert.AreEqual(stringValue.IndexOf("a", StringComparison.Ordinal), // overload #6
                StringImpl.IndexOf(stringValue, "a", StringComparison.Ordinal));

            // character overload versions of IndexOf
            Assert.AreEqual(stringValue.IndexOf('a', 30, stringValue.Length - 30), // overload #7
                StringImpl.IndexOf(stringValue, 'a', 30, stringValue.Length - 30));

            Assert.AreEqual(stringValue.IndexOf('a', 30), // overload #8
                StringImpl.IndexOf(stringValue, 'a', 30));

            Assert.AreEqual(stringValue.IndexOf('a'), // overload #9
                StringImpl.IndexOf(stringValue, 'a'));
        }

        [Test]
        public void TestStartsWith()
        {
            const string stringValue = "SomeRandomStringIsHere";

            Assert.AreEqual( stringValue.StartsWith("Some", StringComparison.Ordinal), // overload #1
                StringImpl.StartsWith(stringValue, "Some", StringComparison.Ordinal));

            Assert.AreEqual(stringValue.StartsWith("", StringComparison.Ordinal),
                StringImpl.StartsWith(stringValue, "", StringComparison.Ordinal));

            Assert.AreEqual(stringValue.StartsWith("some", StringComparison.Ordinal),
                StringImpl.StartsWith(stringValue, "some", StringComparison.Ordinal));

            Assert.AreEqual(stringValue.StartsWith("Some"), // overload #2
                StringImpl.StartsWith(stringValue, "Some"));

            Assert.AreEqual(stringValue.StartsWith(""),
                StringImpl.StartsWith(stringValue, ""));

            Assert.AreEqual(stringValue.StartsWith("some"),
                StringImpl.StartsWith(stringValue, "some"));

            Assert.AreEqual(stringValue.StartsWith("Some", false, null), //overload #3
                StringImpl.StartsWith(stringValue, "Some", false, null));

            Assert.AreEqual(stringValue.StartsWith("", false, null),
                StringImpl.StartsWith(stringValue, "", false, null));

            Assert.AreEqual(stringValue.StartsWith("some", false, null),
                StringImpl.StartsWith(stringValue, "some", false, null));

        }

    }
}
