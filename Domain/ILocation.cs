using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain
{
    public interface ILocation
    {
     //   Map Map { get; set; }
        Point Coordinate { get; set; }
        int DistanceTo(ILocation location);
        int DistanceTo(Point location);
    }
}