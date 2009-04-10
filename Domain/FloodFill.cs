using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.FieldOfView;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Terrain;

namespace LastManStanding.Domain
{
    public class FloodFill 
    {
        public static void Fill(TerrainMap terrainMap, VisibilityMap visibilityMap, IMovementProfile movementProfile, Point location)
        {
            if (!terrainMap.Bounds.Contains(location)) return;
            if (!visibilityMap.Bounds.Contains(location)) return;
            if (visibilityMap[location].WasSeen) return;
            if (!movementProfile.TerrainIsTraversable(terrainMap[location])) return;

            visibilityMap[location].WasSeen = true;

            Fill(terrainMap, visibilityMap, movementProfile, Direction.North.ApplyTransform(location));
            Fill(terrainMap, visibilityMap, movementProfile, Direction.Northeast.ApplyTransform(location));
            Fill(terrainMap, visibilityMap, movementProfile, Direction.East.ApplyTransform(location));
            Fill(terrainMap, visibilityMap, movementProfile, Direction.Southeast.ApplyTransform(location));
            Fill(terrainMap, visibilityMap, movementProfile, Direction.South.ApplyTransform(location));
            Fill(terrainMap, visibilityMap, movementProfile, Direction.Southwest.ApplyTransform(location));
            Fill(terrainMap, visibilityMap, movementProfile, Direction.West.ApplyTransform(location));
            Fill(terrainMap, visibilityMap, movementProfile, Direction.Northwest.ApplyTransform(location));

        }
    }
}
