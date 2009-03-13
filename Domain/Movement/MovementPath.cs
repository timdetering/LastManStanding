using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Movement
{
    public class MovementPath : Queue<PathFindingNode>
    {
        public bool LocationIsAlongPath(Point location)
        {
            return this.Any(node => node.Location == location);
        }
    }
}
