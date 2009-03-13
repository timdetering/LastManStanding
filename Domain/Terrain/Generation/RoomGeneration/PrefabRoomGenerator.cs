using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Terrain.Generation.RoomGeneration
{
    public class PrefabRoomGenerator : RoomGenerator
    {
        public List<DungeonPrefab> DungeonPrefabs { get; set; }
        public int ChanceToPlacePrefab { get; set; }

        protected override DungeonPrefab CreateRandomRoom(Size minRoomSize, Size maxRoomSize)
        {
            return ShouldPlacePrefab() ? SelectRandomPrefab() : base.CreateRandomRoom(minRoomSize, maxRoomSize);
        }

        private bool ShouldPlacePrefab()
        {
            return Rng.Next(1, 99) < ChanceToPlacePrefab;
        }

        private DungeonPrefab SelectRandomPrefab()
        {
            return DungeonPrefabs[Rng.Next(DungeonPrefabs.Count - 1)];
        }

        private int GetConnectorPlacementScore(Point connectorLocation, Point dungeonLocation, DungeonPrefab room, TerrainMap terrainMap, Direction direction)
        {
            // Check if the room has an adjacent location in the given direction from the connector location
            // Check that the dungeon has a walkable location in the given direction 
            if ((!room.HasAdjacentLocation(connectorLocation, direction)) && (terrainMap.Bounds.Contains(direction.ApplyTransform(dungeonLocation)))
                )
            {
                var targetLocation = terrainMap.GetTargetLocation(dungeonLocation, direction).Value;

                if (terrainMap[targetLocation] is Floor)
                    return 1;

                var door = terrainMap[targetLocation] as Door;
                if ((door != null) && ((door.State == DoorStates.Open) || (door.State == DoorStates.Broken)))
                    return 1;

                return 0;
            }

            return 0;
        }

        protected override int CalculateRoomPlacementScore(Point location, DungeonPrefab room, TerrainMap terrainMap)
        {
            if (room.Connectors.Count <= 0)
                return base.CalculateRoomPlacementScore(location, room, terrainMap);

            if (!ContainingBoundsIsWithinDungeonScore(terrainMap, location, room.Bounds.Size)) return int.MinValue;
            if (RoomOverlapsOtherRooms(terrainMap, location, room.Bounds.Size)) return int.MinValue;

            var connectorPlacementScore = 0;

            foreach (var connector in room.Connectors)
            {
                // Translate the connector location to its location in the dungeon
                var dungeonLocation = new Point(location.X + connector.X, location.Y + connector.Y);
                connectorPlacementScore += GetConnectorPlacementScore(new Point(connector.X, connector.Y),
                                                                      dungeonLocation, room, terrainMap,
                                                                      Direction.North);
                connectorPlacementScore += GetConnectorPlacementScore(new Point(connector.X, connector.Y),
                                                                      dungeonLocation, room, terrainMap,
                                                                      Direction.South);
                connectorPlacementScore += GetConnectorPlacementScore(new Point(connector.X, connector.Y),
                                                                      dungeonLocation, room, terrainMap, Direction.West);
                connectorPlacementScore += GetConnectorPlacementScore(new Point(connector.X, connector.Y),
                                                                      dungeonLocation, room, terrainMap, Direction.East);
            }

            return (connectorPlacementScore == 0) ? int.MinValue : connectorPlacementScore;
        }
    }
}
