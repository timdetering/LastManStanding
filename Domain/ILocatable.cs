using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain
{
    public interface ILocatable
    {
        ILocation Location { get; set; }
    }
}
