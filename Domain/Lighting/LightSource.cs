using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LastManStanding.Domain.FieldOfView;

namespace LastManStanding.Domain.Lighting
{
    [Serializable]
    public class LightSource : ILocatable, IEquatable<LightSource>, ICloneable
    {
        private ILocation location = new Location();
        public ILocation Location
        {
            get { return location; }
            set { location = value; }
        }

        public Color Colour { get; set; }
        public AttenuationFunction Attenuation { get; set; }
        public IFovProfile FovProfile { get; set; }

        public int Red { get { return Colour.R; } }
        public int Green { get { return Colour.G; } }
        public int Blue { get { return Colour.B; } }
        public int Alpha { get { return Colour.A; } }

        //#region IXmlSerializable Members

        //XmlSchema IXmlSerializable.GetSchema()
        //{
        //    return null;
        //}

        //void IXmlSerializable.ReadXml(XmlReader reader)
        //{
        //    Radius = DataConverter.FieldValueToInt32(reader.GetAttribute("Radius"));
        //    FovShape = (FovShapeType)Enum.Parse(typeof(FovShapeType), reader.GetAttribute("FovShape"));

        //    reader.Read();
        //    Location = new Point(DataConverter.FieldValueToInt32(reader.GetAttribute("X")), DataConverter.FieldValueToInt32(reader.GetAttribute("Y")));
        //    reader.Read();
        //    Colour = Color.FromArgb(DataConverter.FieldValueToInt32(reader.GetAttribute("Alpha")), DataConverter.FieldValueToInt32(reader.GetAttribute("Red")), DataConverter.FieldValueToInt32(reader.GetAttribute("Green")), DataConverter.FieldValueToInt32(reader.GetAttribute("Blue")));
        //    reader.Read();

        //    Attenuation = (AttenuationFunction)new XmlSerializer(typeof(AttenuationFunction)).Deserialize(reader);

        //    reader.ReadEndElement();
        //}

        //void IXmlSerializable.WriteXml(XmlWriter writer)
        //{
        //    writer.WriteAttributeString("Radius", Radius.ToString());
        //    writer.WriteAttributeString("FovShape", FovShape.ToString());

        //    writer.WriteStartElement("Location");
        //    writer.WriteAttributeString("X", Location.X.ToString());
        //    writer.WriteAttributeString("Y", Location.Y.ToString());
        //    writer.WriteEndElement();

        //    writer.WriteStartElement("Colour");
        //    writer.WriteAttributeString("Alpha", Colour.A.ToString());
        //    writer.WriteAttributeString("Red", Colour.R.ToString());
        //    writer.WriteAttributeString("Green", Colour.G.ToString());
        //    writer.WriteAttributeString("Blue", Colour.B.ToString());
        //    writer.WriteEndElement();

        //    var attenuationSerializer = new XmlSerializer(typeof(AttenuationFunction));
        //    var xsn = new XmlSerializerNamespaces();
        //    xsn.Add(string.Empty, string.Empty);
        //    attenuationSerializer.Serialize(writer, Attenuation, xsn);
        //}

        //#endregion

        #region IEquatable<LightSource> Members

        bool IEquatable<LightSource>.Equals(LightSource other)
        {
            if (Location != other.Location) return false;
            if (FovProfile != other.FovProfile) return false;
            if (!Attenuation.Equals(other.Attenuation)) return false;
            if (Red != other.Red) return false;
            if (Green != other.Green) return false;
            if (Blue != other.Blue) return false;
            return Alpha == other.Alpha;
        }

        public override bool Equals(object obj)
        {
            var lightSource = obj as LightSource;
            return obj != null && ((IEquatable<LightSource>)this).Equals(lightSource);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new LightSource() { Location = this.Location, FovProfile = this.FovProfile, Colour = this.Colour, Attenuation = this.Attenuation };
        }

        #endregion
    }
}
