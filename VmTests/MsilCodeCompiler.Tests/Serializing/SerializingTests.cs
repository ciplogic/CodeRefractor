using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.RuntimeBase.DataBase;
using CodeRefractor.RuntimeBase.DataBase.SerializeXml;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests.Serializing
{
    [XNode]
    class DataToSave
    {
        public int X;
    }

    [TestFixture]
    class SerializingTests
    {
        [Test]
        public void SimpleSerialize()
        {
            var data = new DataToSave(){ X = 2};
            var result = data.Serialize();
            data.X = 3;
            data.Deserialize(result);
            Assert.AreEqual(2, data.X);
        }
    }
}

