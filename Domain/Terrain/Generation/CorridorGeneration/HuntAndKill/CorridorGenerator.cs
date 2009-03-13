using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Terrain.Generation.CorridorGeneration.HuntAndKill
{
    public class CorridorGenerator : ICorridorGenerator
    {
        public int ChangeDirectionModifier { get; set; }
        public int DeadEndRemovalModifier { get; set; }
        public int SparsenessModifier { get; set; }

        #region ICorridorGenerator Members

        public void GenerateCorridors(TerrainMap terrainMap)
        {
            ExpandToTiles(GenerateCellDungeon(terrainMap), terrainMap);
        }

        #endregion

        private CellDungeon GenerateCellDungeon(TerrainMap terrainMap)
        {
            var cellDungeon = new CellDungeon(terrainMap.Width / 2 - 1, terrainMap.Height / 2 - 1);
            cellDungeon.SetAllCellsAsUnvisited();

            CreateDenseMaze(cellDungeon);
            SparsifyMaze(cellDungeon);
            RemoveDeadEnds(cellDungeon);

            return cellDungeon;
        }

        private void CreateDenseMaze(CellDungeon dungeon)
        {
            var currentLocation = dungeon.PickRandomCellAndSetItAsVisited();
            var previousDirection = Direction.North;

            while (!dungeon.AllCellsAreVisited)
            {
                var directionPicker = new DirectionPicker(previousDirection, ChangeDirectionModifier);
                var direction = directionPicker.PickRandomDirection();

                while (!dungeon.HasAdjacentCell(currentLocation, direction) ||
                       dungeon.AdjacentCellIsVisited(currentLocation, direction))
                {
                    if (directionPicker.HasDirectionToPick)
                        direction = directionPicker.PickRandomDirection();
                    else
                    {
                        currentLocation = dungeon.GetRandomVisitedCell(currentLocation); // Get a new previously visited location
                        directionPicker = new DirectionPicker(previousDirection, ChangeDirectionModifier); // Reset the direction picker
                        direction = directionPicker.PickRandomDirection(); // Get a new direction
                    }
                }

                currentLocation = dungeon.CreateCorridor(currentLocation, direction);
                dungeon.SetCellAsVisited(currentLocation);
                previousDirection = direction;
            }
        }

        private void SparsifyMaze(CellDungeon dungeon)
        {
            // Calculate the number of cells to remove as a percentage of the total number of cells in the dungeon
            var noOfDeadEndCellsToRemove =
                (int)Math.Ceiling(((decimal)SparsenessModifier / 100) * (dungeon.Width * dungeon.Height));

            var enumerator = dungeon.DeadEndCellLocations.GetEnumerator();

            for (var i = 0; i < noOfDeadEndCellsToRemove; i++)
            {
                if (!enumerator.MoveNext()) // Check if there is another item in our enumerator
                {
                    enumerator = dungeon.DeadEndCellLocations.GetEnumerator(); // Get a new enumerator
                    if (!enumerator.MoveNext()) break; // No new items exist so break out of loop
                }

                var point = enumerator.Current;
                dungeon.CreateWall(point, dungeon[point].CalculateDeadEndCorridorDirection());
                dungeon[point].IsCorridor = false;
            }
        }

        private void RemoveDeadEnds(CellDungeon dungeon)
        {
            foreach (var deadEndLocation in dungeon.DeadEndCellLocations)
            {
                if (ShouldRemoveDeadend())
                {
                    var currentLocation = deadEndLocation;

                    do
                    {
                        // Initialize the direction picker not to select the dead-end corridor direction
                        var directionPicker =
                            new DirectionPicker(dungeon[currentLocation].CalculateDeadEndCorridorDirection(), 100);
                        var direction = directionPicker.PickRandomDirection();

                        while (!dungeon.HasAdjacentCell(currentLocation, direction))
                        {
                            if (directionPicker.HasDirectionToPick)
                                direction = directionPicker.PickRandomDirection();
                            else
                                throw new InvalidOperationException("This should not happen");
                        }
                        // Create a corridor in the selected direction
                        currentLocation = dungeon.CreateCorridor(currentLocation, direction);
                    } while (dungeon[currentLocation].IsDeadEnd); // Stop when you intersect an existing corridor.
                }
            }
        }

        private bool ShouldRemoveDeadend()
        {
            return Rng.Next(1, 99) < DeadEndRemovalModifier;
        }

        private void ExpandToTiles(CellDungeon cellDungeon, TerrainMap terrainMap)
        {
            // Loop for each corridor cell and expand it
            foreach (var cellLocation in cellDungeon.CorridorCellLocations)
            {
                var tileLocation = new Point(cellLocation.X * 2 + 1, cellLocation.Y * 2 + 1);
                terrainMap[tileLocation] = new Floor();

                if (cellDungeon[cellLocation].Northside == SideType.Empty)
                    terrainMap[Direction.North.ApplyTransform(tileLocation)] = new Floor();
                if (cellDungeon[cellLocation].Southside == SideType.Empty)
                    terrainMap[Direction.South.ApplyTransform(tileLocation)] = new Floor();
                if (cellDungeon[cellLocation].Westside == SideType.Empty)
                    terrainMap[Direction.West.ApplyTransform(tileLocation)] = new Floor();
                if (cellDungeon[cellLocation].Eastside == SideType.Empty)
                    terrainMap[Direction.East.ApplyTransform(tileLocation)] = new Floor();
            }
        }
    }
}
