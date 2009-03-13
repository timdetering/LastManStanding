using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Movement;

namespace LastManStanding.Domain.Terrain.Generation.CorridorGeneration.HuntAndKill
{
    public class Cell
    {
        #region Fields

        private readonly Dictionary<Direction, SideType> sides = new Dictionary<Direction, SideType>
                                                                     {
                                                                         {Direction.North, SideType.Wall},
                                                                         {Direction.East, SideType.Wall},
                                                                         {Direction.South, SideType.Wall},
                                                                         {Direction.West, SideType.Wall}
                                                                     };

        #endregion

        #region Properties

        public bool Visited { get; set; }

        public SideType Northside
        {
            get { return GetSideAt(Direction.North); }
            set { SetSideAt(Direction.North, value); }
        }

        public SideType Eastside
        {
            get { return GetSideAt(Direction.East); }
            set { SetSideAt(Direction.East, value); }
        }

        public SideType Southside
        {
            get { return GetSideAt(Direction.South); }
            set { SetSideAt(Direction.South, value); }
        }

        public SideType Westside
        {
            get { return GetSideAt(Direction.West); }
            set { SetSideAt(Direction.West, value); }
        }

        public bool IsDeadEnd
        {
            get { return WallCount == 3; }
        }

        public bool IsCorridor { get; set; }

        public int WallCount
        {
            get
            {
                int wallCount = 0;
                if (Northside == SideType.Wall) wallCount++;
                if (Southside == SideType.Wall) wallCount++;
                if (Westside == SideType.Wall) wallCount++;
                if (Eastside == SideType.Wall) wallCount++;
                return wallCount;
            }
        }

        #endregion

        #region Methods

        public SideType GetSideAt(Direction direction)
        {
            if (!sides.ContainsKey(direction)) throw new ArgumentException("Invalid direction specified", "direction");
            return sides[direction];
        }

        public SideType SetSideAt(Direction direction, SideType sideType)
        {
            if (!sides.ContainsKey(direction)) throw new ArgumentException("Invalid direction specified", "direction");
            return sides[direction] = sideType;
        }

        public Direction CalculateDeadEndCorridorDirection()
        {
            if (!IsDeadEnd) throw new InvalidOperationException();

            if (Northside == SideType.Empty) return Direction.North;
            if (Southside == SideType.Empty) return Direction.South;
            if (Westside == SideType.Empty) return Direction.West;
            if (Eastside == SideType.Empty) return Direction.East;

            throw new InvalidOperationException();
        }

        #endregion
    }
}
