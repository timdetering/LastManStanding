using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LastManStanding
{
    public static class Extensions
    {
        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        /// <param name="origin">The origin point</param>
        /// <param name="destination">The destination point</param>
        /// <returns>The distance between the two point</returns>
        public static int DistanceTo(this Point origin, Point destination)
        {
            return Math.Max(Math.Abs(origin.X - destination.X), Math.Abs(origin.Y - destination.Y));
        }
    }
}