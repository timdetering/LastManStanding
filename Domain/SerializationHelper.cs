using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace LastManStanding.Domain
{
    public sealed class SerializationHelper
    {
        public static string ToXml(object entity)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new XmlSerializer(entity.GetType());
                formatter.Serialize(ms, entity);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static T FromXml<T>(string xml)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(ms);
            }
        }

        public static byte[] ToByteArray(object entity)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, entity);
                return ms.GetBuffer();
            }
        }

        public static MemoryStream ToMemoryStream(object entity)
        {
            var ms = new MemoryStream();
            var formatter = new BinaryFormatter();

            formatter.Serialize(ms, entity);
            return ms;
        }

        public static T FromByteArray<T>(byte[] entityBytes)
        {
            using (var ms = new MemoryStream(entityBytes))
            {
                var formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(ms);
            }
        }

        public static T FromMemoryStream<T>(Stream stream)
        {
            var formatter = new BinaryFormatter();
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }
    }
}
