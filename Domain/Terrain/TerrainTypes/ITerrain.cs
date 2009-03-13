using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Terrain.TerrainTypes
{
    public interface ITerrain : ILocatable, IComposite
    {
        char Symbol { get; }
    }
}