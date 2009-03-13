using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.FieldOfView
{
    [Serializable]
    public class FovResultset : List<FovObject>
    {
        public bool ContainsLocation(Point location)
        {
            return (Find(visibleLocation => visibleLocation.Location.Coordinate == location) != null);
        }
    }
}
