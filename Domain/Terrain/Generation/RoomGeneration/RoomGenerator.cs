using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Lighting;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Terrain.Generation.RoomGeneration
{
    public class RoomGenerator : IRoomGenerator
    {
        public int MinNoOfRooms { get; set; }
        public int MaxNoOfRooms { get; set; }
        public Size MinRoomSize { get; set; }
        public Size MaxRoomSize { get; set; }

        public virtual void GenerateRooms(TerrainMap terrainMap)
        {
            var noOfRoomsToGenerate = Rng.Next(MinNoOfRooms, MaxNoOfRooms);

            for (var roomCounter = 0; roomCounter < noOfRoomsToGenerate; roomCounter++)
            {
                var room = CreateRandomRoom(MinRoomSize, MaxRoomSize);
                var bestRoomPlacementScore = int.MinValue;
                Point? bestRoomPlacementLocation = null;

                foreach (Point currentRoomPlacementLocation in terrainMap.Locations)
                {
                    int currentRoomPlacementScore = CalculateRoomPlacementScore(currentRoomPlacementLocation, room, terrainMap);

                    if (currentRoomPlacementScore > bestRoomPlacementScore)
                    {
                        bestRoomPlacementScore = currentRoomPlacementScore;
                        bestRoomPlacementLocation = currentRoomPlacementLocation;
                    }
                }

                // Create room at best room placement cell
                if ((bestRoomPlacementLocation != null) && (bestRoomPlacementScore != int.MinValue))
                    PlaceRoom(bestRoomPlacementLocation.Value, room, terrainMap);
            }
        }

        protected virtual int CalculateRoomPlacementScore(Point location, DungeonPrefab room, TerrainMap terrainMap)
        {
            if (!ContainingBoundsIsWithinDungeonScore(terrainMap, location, room.Bounds.Size)) return int.MinValue;

            if (RoomOverlapsOtherRooms(terrainMap, location, room.Bounds.Size)) return int.MinValue;

            return GetSurroundedByFloorScore(terrainMap, location, room.Bounds.Size);
        }

        protected void PlaceRoom(Point location, DungeonPrefab room, TerrainMap terrainMap)
        {
            // Create a copy of the room
            var roomToPlace = (DungeonPrefab)room.Clone();

            // Offset the room origin to the new location
            roomToPlace.SetLocation(location);

            // Loop for each cell in the room
            foreach (var roomLocation in room.Locations)
            {
                // Translate the room cell location to its location in the dungeon
                var dungeonLocation = new Point(location.X + roomLocation.X, location.Y + roomLocation.Y);

                if (room[roomLocation] != null)
                {
                    // Copy the tiles from the room to the dungeon
                    terrainMap[dungeonLocation] = room[roomLocation];
                }
            }

            // Add the lightsources to the dungeon
            foreach (var lightSource in room.LightSources)
            {
                var lightSourceToPlace = (LightSource)lightSource.Clone();
                lightSourceToPlace.Location.Coordinate = new Point(lightSourceToPlace.Location.Coordinate.X + location.X, lightSourceToPlace.Location.Coordinate.Y + location.Y);
                terrainMap.LightSources.Add(lightSourceToPlace);
            }


            terrainMap.Prefabs.Add(roomToPlace);
        }

        protected int GetSurroundedByDeadEndsScore(TerrainMap terrainMap, Point location, Size roomSize)
        {
            var containingBounds = GetContainingBounds(location, roomSize);
            if (!terrainMap.Bounds.Contains(containingBounds)) return int.MinValue;

            var score = 0;

            for (var y = containingBounds.Y; y < containingBounds.Bottom; y++)
                for (var x = containingBounds.X; x < containingBounds.Right; x++)
                {
                    if ((x > containingBounds.X + 1) && (x < containingBounds.Right - 2) && (y == containingBounds.Y) && (IsDeadEnd(terrainMap, x, y)))
                        score += 5;

                    if ((x > containingBounds.X + 1) && (x < containingBounds.Right - 2) && (y == containingBounds.Bottom - 1) && (IsDeadEnd(terrainMap, x, y)))
                        score += 5;

                    if ((y > containingBounds.Y + 1) && (y < containingBounds.Bottom - 2) && (x == containingBounds.X) && (IsDeadEnd(terrainMap, x, y)))
                        score += 5;

                    if ((y > containingBounds.Y + 1) && (y < containingBounds.Bottom - 2) && (x == containingBounds.Right - 1) && (IsDeadEnd(terrainMap, x, y)))
                        score += 5;
                }

            return score == 0 ? int.MinValue : score;
        }

        protected bool IsDeadEnd(TerrainMap terrainMap, int x, int y)
        {
            var wallCount = 0;
            var location = new Point(x, y);
            if ((!terrainMap.HasAdjacentLocation(location, Direction.North)) || (terrainMap.TargetLocationIsOfType<Wall>(location, Direction.North))) wallCount++;
            if ((!terrainMap.HasAdjacentLocation(location, Direction.South)) || (terrainMap.TargetLocationIsOfType<Wall>(location, Direction.South))) wallCount++;
            if ((!terrainMap.HasAdjacentLocation(location, Direction.West)) || (terrainMap.TargetLocationIsOfType<Wall>(location, Direction.West))) wallCount++;
            if ((!terrainMap.HasAdjacentLocation(location, Direction.East)) || (terrainMap.TargetLocationIsOfType<Wall>(location, Direction.East))) wallCount++;

            return wallCount == 3;
        }

        protected int GetSurroundedByFloorScore(TerrainMap dungeon, Point location, Size roomSize)
        {
            var containingBounds = GetContainingBounds(location, roomSize);
            if (!dungeon.Bounds.Contains(containingBounds)) return int.MinValue;

            var score = 0;

            for (var y = containingBounds.Y; y < containingBounds.Bottom; y++)
                for (var x = containingBounds.X; x < containingBounds.Right; x++)
                {
                    if ((x > containingBounds.X + 1) && (x < containingBounds.Right - 2) && (y == containingBounds.Y) && (dungeon[x, y] is Floor))
                        score++;

                    if ((x > containingBounds.X + 1) && (x < containingBounds.Right - 2) && (y == containingBounds.Bottom - 1) && (dungeon[x, y] is Floor))
                        score++;

                    if ((y > containingBounds.Y + 1) && (y < containingBounds.Bottom - 2) && (x == containingBounds.X) && (dungeon[x, y] is Floor))
                        score++;

                    if ((y > containingBounds.Y + 1) && (y < containingBounds.Bottom - 2) && (x == containingBounds.Right - 1) && (dungeon[x, y] is Floor))
                        score++;
                }

            return score == 0 ? int.MinValue : score;
        }

        protected static Rectangle GetContainingBounds(Point location, Size roomSize)
        {
            return new Rectangle(location.X - 1, location.Y - 1, roomSize.Width + 2, roomSize.Height + 2);
        }

        protected bool ContainingBoundsIsWithinDungeonScore(TerrainMap dungeon, Point location, Size roomSize)
        {
            var containingBounds = GetContainingBounds(location, roomSize);
            return !dungeon.Bounds.Contains(containingBounds) ? false : true;
        }

        protected bool RoomOverlapsOtherRooms(TerrainMap dungeon, Point location, Size roomSize)
        {
            var containingBounds = GetContainingBounds(location, roomSize);
            if (!dungeon.Bounds.Contains(containingBounds)) return true;

            foreach (var dungeonPrefab in dungeon.Prefabs)
            {
                var prefabContainingBounds = GetContainingBounds(dungeonPrefab.Bounds.Location,
                                                                 dungeonPrefab.Bounds.Size);

                if (prefabContainingBounds.IntersectsWith(containingBounds)) return true;
            }

            return false;
        }

        protected virtual DungeonPrefab CreateRandomRoom(Size minRoomSize, Size maxRoomSize)
        {
            var room = new DungeonPrefab(Rng.Next(minRoomSize.Width, maxRoomSize.Width),
                                              Rng.Next(minRoomSize.Height, maxRoomSize.Height), () => new Floor());

            // Surround room with walls
            foreach (var location in room.Locations)
            {
                if ((location.X >= room.Bounds.X) && (location.X < room.Bounds.Right) && (location.Y == room.Bounds.Y)) room[location] = new Wall();
                if ((location.X >= room.Bounds.X) && (location.X < room.Bounds.Right) && (location.Y == room.Bounds.Bottom - 1)) room[location] = new Wall();
                if ((location.Y >= room.Bounds.Y) && (location.Y < room.Bounds.Bottom) && (location.X == room.Bounds.X)) room[location] = new Wall();
                if ((location.Y >= room.Bounds.Y) && (location.Y < room.Bounds.Bottom) && (location.X == room.Bounds.Right - 1)) room[location] = new Wall();
            }

            return room;
        }
    }
}
