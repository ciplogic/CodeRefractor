#region Usings

using CodeRefractor.RuntimeBase.DataBase.SerializeXml;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using NUnit.Framework;

#endregion

namespace MsilCodeCompiler.Tests.Serializing
{
    [XNode]
    internal class DataToSave
    {
        public int X;
    }

    [TestFixture]
    internal class SerializingTests
    {
        [Test]
        public void SimpleSerialize()
        {
            var data = new DataToSave() {X = 2};
            var result = data.Serialize();
            data.X = 3;
            data.Deserialize(result);
            Assert.AreEqual(2, data.X);
        }

    }
}