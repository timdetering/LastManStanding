using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Materials;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Terrain.TerrainTypes
{
    [Serializable]
    public class Floor : ITerrain
    {
        public ILocation Location { get; set; }
        public IMaterial Material { get; set; }
        public Char Symbol { get { return '.'; } }
        public override string ToString()
        {
            return "Floor";
        }
    }
}