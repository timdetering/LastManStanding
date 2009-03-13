using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using LastManStanding.Domain.Terrain;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Terrain
{
    [Serializable]
    public class DungeonPrefab : TerrainMap, ICloneable
    {
        private List<Connector> connectors = new List<Connector>();

        public List<Connector> Connectors
        {
            get { return connectors; }
            set { connectors = value; }
        }


        public DungeonPrefab()
        {
        }

        public DungeonPrefab(int width, int height, Func<ITerrain> cellFactory)
            : base(width, height, cellFactory)
        {
        }

        //public override void WriteXml(XmlWriter writer)
        //{
        //    base.WriteXml(writer);

        //    var connectorSerializer = new XmlSerializer(typeof(Connector));
        //    var xsn = new XmlSerializerNamespaces();
        //    xsn.Add(string.Empty, string.Empty);

        //    writer.WriteStartElement("Connectors");

        //    foreach (var connector in Connectors)
        //        connectorSerializer.Serialize(writer, connector, xsn);

        //    writer.WriteEndElement();
        //}

        //public override void ReadXml(XmlReader reader)
        //{
        //    base.ReadXml(reader);

        //    if (reader.IsEmptyElement)
        //        reader.Read();
        //    else
        //    {
        //        reader.ReadStartElement("Connectors");

        //        var connectorSerializer = new XmlSerializer(typeof(Connector));
        //        Connectors = new List<Connector>();

        //        while (reader.NodeType != XmlNodeType.EndElement)
        //            Connectors.Add((Connector)connectorSerializer.Deserialize(reader));

        //        reader.ReadEndElement();
        //    }
        //}

        #region ICloneable Members

        public object Clone()
        {
            return SerializationHelper.FromByteArray<DungeonPrefab>(SerializationHelper.ToByteArray(this));
        }

        #endregion
    }
}