using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Movement;

namespace LastManStanding.Domain.Terrain.Generation.CorridorGeneration.HuntAndKill
{
    public abstract class CellMap
    {
        #region Fields

        private readonly Cell[,] cells;
        private Rectangle bounds;

        #endregion

        #region Constructors

        protected CellMap(int width, int height)
        {
            cells = new Cell[width, height];
            bounds = new Rectangle(0, 0, width, height);

            // Initialize the array of cells
            foreach (Point location in CellLocations)
                this[location] = new Cell();
        }

        #endregion

        #region Properties

        public Rectangle Bounds
        {
            get { return bounds; }
            protected set { bounds = value; }
        }

        public Cell this[Point point]
        {
            get { return this[point.X, point.Y]; }
            set { this[point.X, point.Y] = value; }
        }

        public Cell this[int x, int y]
        {
            get { return cells[x, y]; }
            set { cells[x, y] = value; }
        }

        public int Width
        {
            get { return bounds.Width; }
        }

        public int Height
        {
            get { return bounds.Height; }
        }

        public IEnumerable<Point> CellLocations
        {
            get
            {
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                        yield return new Point(x, y);
            }
        }

        #endregion

        #region Methods

        public bool HasAdjacentCell(Point location, Direction direction)
        {
            return Bounds.Contains(direction.ApplyTransform(location));
        }

        protected Point? GetTargetLocation(Point location, Direction direction)
        {
            if (!HasAdjacentCell(location, direction)) return null;

            return direction.ApplyTransform(location);
        }

        #endregion
    }
}
