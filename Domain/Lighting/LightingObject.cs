using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Lighting
{
    public class LightingObject : ILocatable
    {
        public ILocation Location { get; set; }
        public Color Colour { get; set; }
    }
}
