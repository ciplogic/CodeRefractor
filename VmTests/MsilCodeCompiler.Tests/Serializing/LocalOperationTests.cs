using CodeRefractor.MiddleEnd.SimpleOperations;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests.Serializing
{
    [TestFixture]
class LocalOperationTests
    {
        [Test]
        public void CloneInstructions()
        {
            var label = new Label {JumpTo = 245};
            var clonedLabel = (Label)label.Clone();
            Assert.AreEqual(label.JumpTo, clonedLabel.JumpTo);
        }

        [Test]
        public void BuildToString()
        {
            var label = new Label { JumpTo = 245 };
            var aString = label.ToString();
            Assert.IsNotEmpty(aString);
        }
    }
}