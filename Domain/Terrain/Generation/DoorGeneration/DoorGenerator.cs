using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Terrain.Generation.DoorGeneration
{
    public class DoorGenerator : IDoorGenerator
    {
        public int MinDoorsPerRoom { get; set; }
        public int MaxDoorsPerRoom { get; set; }
        public int MinDoorsPerWall { get; set; }
        public int MaxDoorsPerWall { get; set; }

        public void GenerateDoors(TerrainMap terrainMap)
        {
            AddDoorsToDeadEnds(terrainMap);

            // Evaulate each prefab and see if doors need to be placed
            foreach (DungeonPrefab dungeonPrefab in terrainMap.Prefabs)
            {
                DungeonPrefab dungeonPrefab1 = dungeonPrefab;
                if (dungeonPrefab.Locations.Where(x => dungeonPrefab1[x] is Door).Count() == 0)
                {
                    // Need to place some doors

                    var noOfDoorsToPlace = Rng.Next(MinDoorsPerRoom, MaxDoorsPerRoom);
                    var doorsPlaced = 0;

                    doorsPlaced += PlaceDoorsOnWall(terrainMap, dungeonPrefab.Bounds, doorsPlaced, noOfDoorsToPlace, Direction.North);
                    doorsPlaced += PlaceDoorsOnWall(terrainMap, dungeonPrefab.Bounds, doorsPlaced, noOfDoorsToPlace, Direction.South);
                    doorsPlaced += PlaceDoorsOnWall(terrainMap, dungeonPrefab.Bounds, doorsPlaced, noOfDoorsToPlace, Direction.East);
                    doorsPlaced += PlaceDoorsOnWall(terrainMap, dungeonPrefab.Bounds, doorsPlaced, noOfDoorsToPlace, Direction.West);
                }
            }

            RemoveInvalidDoors(terrainMap);
        }

        private bool LocationIsInPrefab(TerrainMap terrainMap, Point location)
        {
            foreach (var dungeonPrefab in terrainMap.Prefabs)
                if ((dungeonPrefab.Connectors.Count > 0) && (dungeonPrefab.Bounds.Contains(location)) && (dungeonPrefab[location.X - dungeonPrefab.Bounds.X, location.Y - dungeonPrefab.Bounds.Y] != null))
                    return true;

            return false;
        }

        protected bool IsDeadEnd(TerrainMap terrainMap, Point location)
        {
            var wallCount = 0;
            if ((!terrainMap.HasAdjacentLocation(location, Direction.North)) || (terrainMap.TargetLocationIsOfType<Wall>(location, Direction.North))) wallCount++;
            if ((!terrainMap.HasAdjacentLocation(location, Direction.South)) || (terrainMap.TargetLocationIsOfType<Wall>(location, Direction.South))) wallCount++;
            if ((!terrainMap.HasAdjacentLocation(location, Direction.West)) || (terrainMap.TargetLocationIsOfType<Wall>(location, Direction.West))) wallCount++;
            if ((!terrainMap.HasAdjacentLocation(location, Direction.East)) || (terrainMap.TargetLocationIsOfType<Wall>(location, Direction.East))) wallCount++;

            return wallCount == 3;
        }

        private void AddDoorsToDeadEnds(TerrainMap terrainMap)
        {
            foreach (var location in terrainMap.Locations.Where(x => IsDeadEnd(terrainMap, x)))
            {
                if (LocationIsInPrefab(terrainMap, location))
                    continue;

                if (CreateDeadEndDoor(terrainMap, location, Direction.North)) continue;
                if (CreateDeadEndDoor(terrainMap, location, Direction.South)) continue;
                if (CreateDeadEndDoor(terrainMap, location, Direction.West)) continue;
                if (CreateDeadEndDoor(terrainMap, location, Direction.East)) continue;
            }
        }

        private bool TargetLocationIsObstacle(TerrainMap terrainMap, Point location, Direction direction)
        {
            var targetLocation = terrainMap.GetTargetLocation(location, direction).Value;

            if (terrainMap[targetLocation] is Floor)
                return false;

            var door = terrainMap[targetLocation] as Door;
            if ((door != null) && ((door.State == DoorStates.Open) || (door.State == DoorStates.Broken)))
                return false;

            return true;
        }

        private bool CreateDeadEndDoor(TerrainMap terrainMap, Point location, Direction direction)
        {
            if (((terrainMap.HasAdjacentLocation(location, direction) && (TargetLocationIsObstacle(terrainMap, location, direction)))) &&
                ((terrainMap.HasAdjacentLocation(direction.ApplyTransform(location), direction) && (!TargetLocationIsObstacle(terrainMap, direction.ApplyTransform(location), direction)))))
            {
                terrainMap[direction.ApplyTransform(location)] = new Door() {State = DoorStates.Closed};
                return true;
            }

            return false;
        }

        public void ResetDoors(TerrainMap terrainMap)
        {
            foreach (var location in terrainMap.Locations.Where(x => terrainMap[x] is Door))
            {
                if (((Door)terrainMap[location]).State == DoorStates.Open)
                    ((Door) terrainMap[location]).State = DoorStates.Closed;
            }
        }

        public void RemoveInvalidDoors(TerrainMap terrainMap)
        {
            foreach (var location in terrainMap.Locations.Where(x => terrainMap[x] is Door))
            {
                //if (LocationIsInPrefab(terrainMap, location))
                //    continue;

                if ((!IsValidDoorLocation(terrainMap, location, Direction.North) ||
                     (!IsValidDoorLocation(terrainMap, location, Direction.South))) &&
                    ((!IsValidDoorLocation(terrainMap, location, Direction.East) ||
                      (!IsValidDoorLocation(terrainMap, location, Direction.West)))))
                {
                    terrainMap[location] = new Wall();
                }
            }
        }

        private int PlaceDoorsOnWall(TerrainMap terrainMap, Rectangle roomBounds, int doorsPlaced, int noOfDoorsToPlace, Direction wallDirection)
        {
            var validDoorLocationsOnWall = GetValidDoorLocationsOnWall(terrainMap, roomBounds, wallDirection);
            int noOfDoorsForWall = Rng.Next(Math.Min(MinDoorsPerWall, validDoorLocationsOnWall.Count),
                                            Math.Min(MaxDoorsPerWall, validDoorLocationsOnWall.Count));
            noOfDoorsForWall -= GetNumberOfDoorsOnWall(terrainMap, roomBounds, wallDirection);
            doorsPlaced += PlaceDoorsOnWall(terrainMap, validDoorLocationsOnWall, noOfDoorsForWall, doorsPlaced,
                                            noOfDoorsToPlace);
            return doorsPlaced;
        }


        private int PlaceDoorsOnWall(TerrainMap terrainMap, List<Point> validDoorLocationsOnWall, int noOfDoorsForWall, int doorsPlaced, int noOfDoorsToPlace)
        {
            int doorsPlacedOnWall = 0;

            while ((doorsPlacedOnWall < noOfDoorsForWall) && (doorsPlaced <= noOfDoorsToPlace))
            {
                int index = Rng.Next(0, validDoorLocationsOnWall.Count - 1);
                terrainMap[validDoorLocationsOnWall[index]] = new Door() { State = DoorStates.Closed};
                validDoorLocationsOnWall.RemoveAt(index);
                doorsPlacedOnWall++;
            }

            return doorsPlacedOnWall;
        }

        private bool IsValidDoorLocation(TerrainMap terrainMap, Point location, Direction wallDirection)
        {
            int wallCount = 0;
            if (terrainMap.HasAdjacentLocation(location, Direction.North) && terrainMap.TargetLocationIsOfType<Wall>(location, Direction.North)) wallCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.South) && terrainMap.TargetLocationIsOfType<Wall>(location, Direction.South)) wallCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.West) && terrainMap.TargetLocationIsOfType<Wall>(location, Direction.West)) wallCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.East) && terrainMap.TargetLocationIsOfType<Wall>(location, Direction.East)) wallCount++;
            if (wallCount > 2) return false;

            int doorCount = 0;
            if (terrainMap.HasAdjacentLocation(location, Direction.North) && terrainMap.TargetLocationIsOfType<Door>(location, Direction.North)) doorCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.South) && terrainMap.TargetLocationIsOfType<Door>(location, Direction.South)) doorCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.West) && terrainMap.TargetLocationIsOfType<Door>(location, Direction.West)) doorCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.East) && terrainMap.TargetLocationIsOfType<Door>(location, Direction.East)) doorCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.Northwest) && terrainMap.TargetLocationIsOfType<Door>(location, Direction.Northwest)) doorCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.Northeast) && terrainMap.TargetLocationIsOfType<Door>(location, Direction.Northeast)) doorCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.Southwest) && terrainMap.TargetLocationIsOfType<Door>(location, Direction.Southwest)) doorCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.Southeast) && terrainMap.TargetLocationIsOfType<Door>(location, Direction.Southeast)) doorCount++;
            if (doorCount > 0) return false;

            int floorCount = 0;
            if (terrainMap.HasAdjacentLocation(location, Direction.North) && !TargetLocationIsObstacle(terrainMap, location, Direction.North)) floorCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.South) && !TargetLocationIsObstacle(terrainMap, location, Direction.South)) floorCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.West) && !TargetLocationIsObstacle(terrainMap, location, Direction.West)) floorCount++;
            if (terrainMap.HasAdjacentLocation(location, Direction.East) && !TargetLocationIsObstacle(terrainMap, location, Direction.East)) floorCount++;
            if (floorCount != 2) return false;

            return (terrainMap.HasAdjacentLocation(location, wallDirection) && !TargetLocationIsObstacle(terrainMap, location, wallDirection));
        }

        private int GetNumberOfDoorsOnWall(TerrainMap terrainMap, Rectangle roomBounds, Direction wallDirection)
        {
            int numberOfDoorsOnWall = 0;

            if (Direction.North == wallDirection)
                for (var x = roomBounds.X + 1; x < roomBounds.Right - 1; x++)
                    if (terrainMap[x, roomBounds.Y] is Door) numberOfDoorsOnWall++;


            if (Direction.South == wallDirection)
                for (var x = roomBounds.X + 1; x < roomBounds.Right - 1; x++)
                    if (terrainMap[x, roomBounds.Bottom - 1] is Door) numberOfDoorsOnWall++;


            if (Direction.West == wallDirection)
                for (var y = roomBounds.Y + 1; y < roomBounds.Bottom - 1; y++)
                    if (terrainMap[roomBounds.X, y] is Door) numberOfDoorsOnWall++;

            if (Direction.East == wallDirection)
                for (var y = roomBounds.Y + 1; y < roomBounds.Bottom - 1; y++)
                    if (terrainMap[roomBounds.Right - 1, y] is Door) numberOfDoorsOnWall++;

            return numberOfDoorsOnWall;
        }

        private List<Point> GetValidDoorLocationsOnWall(TerrainMap terrainMap, Rectangle roomBounds, Direction wallDirection)
        {
            var validDoorLocations = new List<Point>();

            if (Direction.North == wallDirection)
                for (var x = roomBounds.X + 1; x < roomBounds.Right - 1; x++)
                {
                    var location = new Point(x, roomBounds.Y);
                    if (IsValidDoorLocation(terrainMap, location, wallDirection)) validDoorLocations.Add(location);
                }


            if (Direction.South == wallDirection)
                for (var x = roomBounds.X + 1; x < roomBounds.Right - 1; x++)
                {
                    var location = new Point(x, roomBounds.Bottom - 1);
                    if (IsValidDoorLocation(terrainMap, location, wallDirection)) validDoorLocations.Add(location);
                }


            if (Direction.West == wallDirection)
                for (var y = roomBounds.Y + 1; y < roomBounds.Bottom - 1; y++)
                {
                    var location = new Point(roomBounds.X, y);
                    if (IsValidDoorLocation(terrainMap, location, wallDirection)) validDoorLocations.Add(location);
                }


            if (Direction.East == wallDirection)
                for (var y = roomBounds.Y + 1; y < roomBounds.Bottom - 1; y++)
                {
                    var location = new Point(roomBounds.Right - 1, y);
                    if (IsValidDoorLocation(terrainMap, location, wallDirection)) validDoorLocations.Add(location);
                }


            return validDoorLocations;
        }
    }
}
