using System.IO;
using CodeRefractor.RuntimeBase.DataBase;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests.Serializing
{
    [TestFixture]
    public class TestMinimizer
    {
   
        [Test]
        public void TestSvgTiger()
        {
            var fullPath = "awesome_tiger.xml";

            var tigerXml = "tiger.xml";
            var fullData = File.ReadAllBytes(fullPath);
            var dynNode = new DynNode("root");

            dynNode.FromFile(fullPath);
            var miniTransformer = new MinimizeTransformer();
            var data = miniTransformer.Minimize(dynNode, true);
            var expandTransformer = new ExpanderTransformer();

            var expandedNode = expandTransformer.Expand(data, true);

            Assert.AreEqual(expandedNode.Name, dynNode.Name);
        }
    }
}