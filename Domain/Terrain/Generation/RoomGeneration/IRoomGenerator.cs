using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Terrain.Generation.RoomGeneration
{
    public interface IRoomGenerator
    {
        void GenerateRooms(TerrainMap terrainMap);
    }
}
