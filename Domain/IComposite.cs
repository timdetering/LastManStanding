using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Materials;

namespace LastManStanding.Domain
{
    public interface IComposite
    {
        IMaterial Material { get; set; }
    }
}
