using System.Xml;

namespace XML.Tests
{
    public class CreateXmlFile
    {
        public void CreateFile()
        {
            using (var writer = XmlWriter.Create(@"xml-test.xml"))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("ElementOne");

                writer.WriteStartAttribute("Class");
                writer.WriteValue("Test123");
                writer.WriteEndAttribute();

                writer.WriteStartElement("Two");
                writer.WriteValue("Hello Universe!");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}