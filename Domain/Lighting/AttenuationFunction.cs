using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LastManStanding.Domain.Lighting
{
    [Serializable]
    public class AttenuationFunction : IEquatable<AttenuationFunction>
    {
        [XmlAttribute]
        public float Constant { get; set; }
        [XmlAttribute]
        public float Linear { get; set; }
        [XmlAttribute]
        public float Quadratic { get; set; }

        #region IEquatable<AttenuationFunction?> Members

        bool IEquatable<AttenuationFunction>.Equals(AttenuationFunction other)
        {
            if (Constant != other.Constant) return false;
            if (Linear != other.Linear) return false;
            return Quadratic == other.Quadratic;
        }

        public override bool Equals(object obj)
        {
            var attenuationFunction = obj as AttenuationFunction;
            return obj != null && ((IEquatable<AttenuationFunction>)this).Equals(attenuationFunction);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
