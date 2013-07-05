using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Xml;

namespace CodeRefractor.RuntimeBase.DataBase
{
    public static class DynNodeUtils
    {
        public static void ToFile(this DynNode node, string fileName)
        {
            CreateFolderForFile(fileName);
            File.WriteAllText(fileName, node.ToString());
        }

        private static void CreateFolderForFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!string.IsNullOrEmpty(fileInfo.DirectoryName))
                Directory.CreateDirectory(fileInfo.DirectoryName);
        }

        public static bool FromFile(this DynNode node, string fileName)
        {
            Contract.Requires(node != null);
            if (!File.Exists(fileName))
                return false;
            var xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.Load(new StreamReader(fileName));
            }
            catch (XmlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            var result = node;
            foreach (XmlNode xmlNode in xmlDoc.ChildNodes)
                UpdateData(result, xmlNode);
            return true;
        }

        private static void UpdateData(DynNode result, XmlNode node)
        {
            result.Name = node.Name;
            if (node.Name == "#text")
                result.InnerText = node.InnerText;
            result.Children.Clear();
            foreach (XmlNode xmlNode in node.ChildNodes)
                UpdateData(result.Add(xmlNode.Name), xmlNode);
            var attributes = node.Attributes;
            result.Atrributes.Clear();
            if (attributes == null)
                return;
            foreach (XmlAttribute attribute in attributes)
                result[attribute.Name] = attribute.Value;
        }
    }
}