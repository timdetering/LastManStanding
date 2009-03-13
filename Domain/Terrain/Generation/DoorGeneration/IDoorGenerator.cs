using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Terrain.Generation.DoorGeneration
{
    public interface IDoorGenerator
    {
        void GenerateDoors(TerrainMap terrainMap);
        void RemoveInvalidDoors(TerrainMap terrainMap);
        void ResetDoors(TerrainMap terrainMap);
    }
}
