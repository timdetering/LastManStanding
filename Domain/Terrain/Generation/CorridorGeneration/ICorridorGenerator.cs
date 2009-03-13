using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Terrain.Generation.CorridorGeneration
{
    public interface ICorridorGenerator
    {
        void GenerateCorridors(TerrainMap terrainMap);
    }
}