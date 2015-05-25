using System.Xml.Serialization;

namespace Studio
{
    using System.IO;
    using System.Runtime.Serialization;

    public static class SerializationExtensions
    {
        public static void SerializeToFile(this object instance, string filePath)
        {
            var xs = new DataContractSerializer(instance.GetType());

            using (var textWriter = File.Create(filePath))
            {
                xs.WriteObject(textWriter, instance);
            }
        }
        public static void XmlSerializeToFile(this object instance, string filePath)
        {
            var xs = new XmlSerializer(instance.GetType());

            using (var textWriter = File.Create(filePath))
            {
                xs.Serialize(textWriter, instance);
            }
        }

        public static T Deserialize<T>(string file)
        {
            var xs = new DataContractSerializer(typeof(T));
            using (var fileStream = File.Open(file, FileMode.Open))
            {
                return (T) xs.ReadObject(fileStream);
            }
        }

        public static T XmlDeserialize<T>(string file)
        {
            var xs = new XmlSerializer(typeof(T));
            using (var fileStream = File.Open(file, FileMode.Open))
            {
                return (T)xs.Deserialize(fileStream);
            }
        }
    }
}