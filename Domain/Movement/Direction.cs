using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Movement
{
    public sealed class Direction : IEquatable<Direction>
    {
        public static readonly Direction East = new Direction { Index = 1, Name = "East", DX = 1, DY = 0 };
        public static readonly Direction Empty = new Direction { Index = 8, Name = "Empty", DX = 0, DY = 0 };
        public static readonly Direction North = new Direction { Index = 0, Name = "North", DX = 0, DY = -1 };
        public static readonly Direction Northeast = new Direction { Index = 4, Name = "Northeast", DX = 1, DY = -1 };
        public static readonly Direction Northwest = new Direction { Index = 7, Name = "Northwest", DX = -1, DY = -1 };
        public static readonly Direction South = new Direction { Index = 2, Name = "South", DX = 0, DY = 1 };
        public static readonly Direction Southeast = new Direction { Index = 5, Name = "Southeast", DX = 1, DY = 1 };
        public static readonly Direction Southwest = new Direction { Index = 6, Name = "Southwest", DX = -1, DY = 1 };
        public static readonly Direction West = new Direction { Index = 3, Name = "West", DX = -1, DY = 0 };

        private Direction()
        {
        }

        /// <summary>
        /// The name of the direction.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The horizontal change in position.
        /// </summary>
        public int DX { get; private set; }

        /// <summary>
        /// The vertical change in position.
        /// </summary>
        public int DY { get; private set; }

        /// <summary>
        /// The index of the direction as mapped to enum values
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The inverse direction of the current direction
        /// </summary>
        public Direction Inverse
        {
            get
            {
                switch (Name)
                {
                    case "North":
                        return South;
                    case "South":
                        return North;
                    case "West":
                        return East;
                    case "East":
                        return West;
                    case "Northwest":
                        return Southeast;
                    case "Northeast":
                        return Southwest;
                    case "Southwest":
                        return Northeast;
                    case "Southeast":
                        return Northwest;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// The next clockwise direction of the current direction
        /// </summary>
        public Direction Next
        {
            get
            {
                switch (Name)
                {
                    case "North":
                        return Northeast;
                    case "Northeast":
                        return East;
                    case "East":
                        return Southeast;
                    case "Southeast":
                        return South;
                    case "South":
                        return Southwest;
                    case "Southwest":
                        return West;
                    case "West":
                        return Northwest;
                    case "Northwest":
                        return North;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// The previous anti-clockwise direction of the current direction
        /// </summary>
        public Direction Previous
        {
            get
            {
                switch (Name)
                {
                    case "North":
                        return Northwest;
                    case "Northwest":
                        return West;
                    case "West":
                        return Southwest;
                    case "Southwest":
                        return South;
                    case "South":
                        return Southeast;
                    case "Southeast":
                        return East;
                    case "East":
                        return Northeast;
                    case "Northeast":
                        return North;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// The next clockwise perpendicular direction of the current direction
        /// </summary>
        public Direction NextPerpendicular
        {
            get
            {
                switch (Name)
                {
                    case "North":
                        return East;
                    case "Northeast":
                        return Southeast;
                    case "East":
                        return South;
                    case "Southeast":
                        return Southwest;
                    case "South":
                        return West;
                    case "Southwest":
                        return Northwest;
                    case "West":
                        return North;
                    case "Northwest":
                        return Northeast;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// The next anti-clockwise perpendicular direction of the current direction
        /// </summary>
        public Direction PreviousPerpendicular
        {
            get
            {
                switch (Name)
                {
                    case "North":
                        return West;
                    case "Northwest":
                        return Southwest;
                    case "West":
                        return South;
                    case "Southwest":
                        return Southeast;
                    case "South":
                        return East;
                    case "Southeast":
                        return Northeast;
                    case "East":
                        return North;
                    case "Northeast":
                        return Northwest;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        #region IEquatable<Direction> Members

        public bool Equals(Direction other)
        {
            if (Name != other.Name) return false;
            if (Index != other.Index) return false;
            if (DX != other.DX) return false;
            return (DY == other.DY);
        }

        #endregion

        /// <summary>
        /// Allows us to cast from an Int to Direction based on the index value
        /// </summary>
        /// <param name="value">The integer index value to cast</param>
        /// <returns>The Direction instance matching the provided index</returns>
        public static implicit operator Direction(int value)
        {
            switch (value)
            {
                case 0:
                    return North;
                case 1:
                    return East;
                case 2:
                    return South;
                case 3:
                    return West;
                case 4:
                    return Northeast;
                case 5:
                    return Southeast;
                case 6:
                    return Southwest;
                case 7:
                    return Northwest;
                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Applies the change in direction to the provided point
        /// </summary>
        /// <param name="location">The location in 2-dimensional space</param>
        /// <returns>Returns a point after the direction changes were applied to the provided location.</returns>
        public Point ApplyTransform(Point location)
        {
            return new Point(location.X + DX, location.Y + DY);
        }

        public override bool Equals(object obj)
        {
            var direction = obj as Direction;
            return obj != null && Equals(direction);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
