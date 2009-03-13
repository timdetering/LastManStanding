using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.FieldOfView
{
    public class FovObject : ILocatable
    {
        private ILocation location = new Location();
        public ILocation Location
        {
            get { return location; }
            set { location = value; }
        }

        public int DistanceFromOrigin { get; set; }
    }
}
