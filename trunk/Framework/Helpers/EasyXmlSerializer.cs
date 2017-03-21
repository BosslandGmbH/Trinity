using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Trinity.Framework.Helpers
{
    public class EasyXmlSerializer
    {
        public static string Serialize<T>(T value)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[]
            {
                XmlQualifiedName.Empty
            });
            var serializer = new XmlSerializer(value.GetType());
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, value, emptyNamepsaces);
                return stream.ToString();
            }
        }

        public static T Deserialize<T>(string input)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(input))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}