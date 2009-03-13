using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Materials
{
    public class Flesh : IMaterial
    {
        public string Name { get { return "Flesh"; } }

        public override string ToString()
        {
            return Name;
        }
    }
}
