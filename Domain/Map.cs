using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Movement;

namespace LastManStanding.Domain
{
    [Serializable]
    public class Map<T> : IEquatable<Map<T>>, IEnumerable<T> where T : ILocatable 
    {
        #region Fields

        private Rectangle bounds;
        private T[][] cells;

        #endregion

        #region Constructors

        public Map()
        {
        }

        public Map(int width, int height, Func<T> cellFactory)
        {
            Resize(new Size(width, height), cellFactory);
        }

        public Map(Size size, Func<T> cellFactory)
        {
            Resize(size, cellFactory);
        }

        public Map(Rectangle rectangle, Func<T> cellFactory)
        {
            Resize(rectangle, cellFactory);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Fills the map with new instances provided by the cell factory
        /// </summary>
        /// <param name="cellFactory">A factory method providing the appropriate instance for each cell</param>
        public virtual void FillMap(Func<T> cellFactory)
        {
            // Set each tile to the instance of the default tile
            foreach (Point location in Locations)
                this[location] = cellFactory();
        }

        public void Resize(Size size, Func<T> cellFactory)
        {
            Resize(new Rectangle(bounds.Location, size), cellFactory);
        }

        /// <summary>
        /// Resizes the map to the new size.
        /// MAp structure is not preserved
        /// </summary>
        /// <param name="rectangle">The bounds of the new map</param>
        /// <param name="cellFactory">A factory method providing the appropriate instance for each cell</param>
        public void Resize(Rectangle rectangle, Func<T> cellFactory)
        {
            // Check if any of the dimensions changed
            if ((rectangle.Width == Width) && (rectangle.Height == Height)) return;

            // Update the map bounds
            bounds = rectangle;

            // Create a new tile array and fill with the default tile type
            cells = new T[bounds.Width][];
            for (int x = 0; x < bounds.Width; x++)
                cells[x] = new T[bounds.Height];

            FillMap(cellFactory);
        }

        /// <summary>
        /// Sets the top left corner of the map to the specified location
        /// </summary>
        /// <param name="location">The new map location</param>
        public virtual void SetLocation(Point location)
        {
            Bounds = new Rectangle(location, Bounds.Size);
        }

        public Point? GetTargetLocation(Point location, Direction direction)
        {
            if (!HasAdjacentLocation(location, direction)) return null;

            return direction.ApplyTransform(location);
        }

        public bool TargetLocationIsOfType<U>(Point location, Direction direction)
        {
            Point? target = GetTargetLocation(location, direction);
            return (target == null) ? false : this[target.Value] is U;
        }

        public bool HasAdjacentLocation(Point location, Direction direction)
        {
            return Bounds.Contains(direction.ApplyTransform(location));
        }

        #endregion

        #region Composition

        public virtual Rectangle Bounds
        {
            get { return bounds; }
            protected set { bounds = value; }
        }

        public virtual T this[Point point]
        {
            get { return this[point.X, point.Y]; }
            set { this[point.X, point.Y] = value; }
        }

        public virtual T this[int x, int y]
        {
            get
            {
                if (!bounds.Contains(x, y))
                    throw new ArgumentException("The location is outside the bounds of the map");
                return cells[x - bounds.X][y - bounds.Y];
            }
            set
            {
                //value.Location = new Location() { Map = this, Coordinate = new Point(x, y) };
                value.Location = new Location() { Coordinate = new Point(x, y) };
                cells[x - bounds.X][y - bounds.Y] = value;
            }
        }

        public virtual int Width
        {
            get { return bounds.Width; }
        }

        public virtual int Height
        {
            get { return bounds.Height; }
        }

        public virtual IEnumerable<Point> Locations
        {
            get
            {
                for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++)
                        yield return new Point(x + bounds.X, y + bounds.Y);
            }
        }



        #endregion
 

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    yield return this[x, y];
        }

        #region IEquatable<Map<T>> Members

        bool IEquatable<Map<T>>.Equals(Map<T> other)
        {
            if (Width != other.Width) return false;
            if (Height != other.Height) return false;

            foreach (Point location in Locations)
                if (this[location].GetType() != other[location].GetType()) return false;

            return true;
        }

        #endregion

        public override bool Equals(object obj)
        {
            var map = obj as Map<T>;
            return obj != null && ((IEquatable<Map<T>>)this).Equals(map);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
