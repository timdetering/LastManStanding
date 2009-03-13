using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain
{
    [Serializable]

    public class Location : ILocation
    {
      //  public Map<ILocatable> Map { get; set; }
        public Point Coordinate { get; set; }

        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        /// <param name="destination">The destination point</param>
        /// <returns>The distance between the two point</returns>
        public int DistanceTo(ILocation destination)
        {
            return DistanceTo(destination.Coordinate);
        }

        public int DistanceTo(Point destination)
        {
            return Math.Max(Math.Abs(Coordinate.X - destination.X), Math.Abs(Coordinate.Y - destination.Y));
        }
    }
}
