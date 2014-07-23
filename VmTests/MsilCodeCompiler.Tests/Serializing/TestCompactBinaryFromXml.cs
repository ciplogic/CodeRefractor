using CodeRefractor.DataNode;
using CodeRefractor.RuntimeBase.DataBase;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests.Serializing
{
    [TestFixture]
    class TestCompactBinaryFromXml
    {
        [Test]
        public void TestCompactingXmlWithAttribute()
        {
            var dynNode = new DynNode("Type")
            {
                {"Name", "Int32"},
                {"Namespace", "System"}
            };
            var fieldsNode = dynNode.Add("Fields");
            fieldsNode.Add("X", "1"); //TypeId of int
            var offsets = dynNode.Add("OffsetFields");
            offsets.Add("X", "0");
            var minimizer = new MinimizeTransformer();
            var compressedReflectionData = minimizer.Minimize(dynNode, false);

            var expander = new ExpanderTransformer();
            var restoredNode = expander.Expand(compressedReflectionData, false);

            Assert.AreEqual(dynNode.Children.Count, restoredNode.Children.Count);
            Assert.AreEqual(dynNode.Attributes.Count, restoredNode.Attributes.Count);
        }
    }
}