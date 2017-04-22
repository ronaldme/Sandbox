using System;

namespace XML.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var createXml = new CreateXmlFile();
            createXml.CreateFile();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
