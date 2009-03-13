using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Movement;

namespace LastManStanding.Domain.Terrain.Generation.CorridorGeneration.HuntAndKill
{
    public class CellDungeon : CellMap
    {
        #region Fields

        private readonly List<Point> visitedCells = new List<Point>();

        #endregion

        #region Constructors

        public CellDungeon(int width, int height)
            : base(width, height)
        {
        }

        #endregion

        #region Methods

        public void SetAllCellsAsUnvisited()
        {
            foreach (Point location in CellLocations)
                this[location].Visited = false;
        }

        public Point PickRandomCellAndSetItAsVisited()
        {
            var randomLocation = new Point(Rng.Next(Width - 1), Rng.Next(Height - 1));
            SetCellAsVisited(randomLocation);
            return randomLocation;
        }

        public bool AdjacentCellIsVisited(Point location, Direction direction)
        {
            Point? target = GetTargetLocation(location, direction);

            return (target == null) ? false : this[target.Value].Visited;
        }

        public bool AdjacentCellIsCorridor(Point location, Direction direction)
        {
            Point? target = GetTargetLocation(location, direction);

            return (target == null) ? false : this[target.Value].IsCorridor;
        }

        public void SetCellAsVisited(Point location)
        {
            if (!Bounds.Contains(location)) throw new ArgumentException("Location is outside of Dungeon bounds", "location");
            if (this[location].Visited) throw new ArgumentException("Location is already visited", "location");

            this[location].Visited = true;
            visitedCells.Add(location);
        }

        public Point GetRandomVisitedCell(Point location)
        {
            if (visitedCells.Count == 0) throw new InvalidOperationException("There are no visited cells to return.");

            int index = Rng.Next(visitedCells.Count - 1);

            // Loop while the current cell is the visited cell
            while (visitedCells[index] == location)
                index = Rng.Next(visitedCells.Count - 1);

            return visitedCells[index];
        }

        public Point CreateCorridor(Point location, Direction direction)
        {
            Point targetLocation = Createside(location, direction, SideType.Empty);
            this[location].IsCorridor = true; // Set current location to corridor
            this[targetLocation].IsCorridor = true; // Set target location to corridor
            return targetLocation;
        }

        public Point CreateWall(Point location, Direction direction)
        {
            return Createside(location, direction, SideType.Wall);
        }

        public Point CreateDoor(Point location, Direction direction)
        {
            return Createside(location, direction, SideType.Door);
        }

        private Point Createside(Point location, Direction direction, SideType sideType)
        {
            Point? target = GetTargetLocation(location, direction);
            if (target == null) throw new ArgumentException("There is no adjacent cell in the given direction", "location");

            this[location].SetSideAt(direction, sideType);
            this[target.Value].SetSideAt(direction.Inverse, sideType);

            return target.Value;
        }

        #endregion

        #region Properties

        public IEnumerable<Point> DeadEndCellLocations
        {
            get { return (from Point point in CellLocations where this[point].IsDeadEnd select point).AsEnumerable(); }
        }

        public IEnumerable<Point> CorridorCellLocations
        {
            get { return (from Point point in CellLocations where this[point].IsCorridor select point).AsEnumerable(); }
        }

        public bool AllCellsAreVisited
        {
            get { return visitedCells.Count == (Width * Height); }
        }

        #endregion
    }
}
