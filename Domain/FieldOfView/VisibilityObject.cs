using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.FieldOfView
{
    public class VisibilityObject : ILocatable
    {
        public ILocation Location { get; set; }
        public bool IsVisible { get; set; }
        public bool WasSeen { get; set; }
    }
}
