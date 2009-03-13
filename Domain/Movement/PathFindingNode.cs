using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Movement
{
    public class PathFindingNode : IEquatable<PathFindingNode>
    {
        public Point Location { get; set; }
        public Direction Direction { get; set; }
        public decimal MovementCost { get; set; }

        #region IEquatable<PathfindingNode> Members

        bool IEquatable<PathFindingNode>.Equals(PathFindingNode other)
        {
            return (Location == other.Location);
        }

        #endregion

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0} - {1} - {2}", Location, Direction, MovementCost);
        }

        public override bool Equals(object obj)
        {
            var typedObject = obj as PathFindingNode;
            return obj != null && ((IEquatable<PathFindingNode>)this).Equals(typedObject);
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode();
        }
    }
}
