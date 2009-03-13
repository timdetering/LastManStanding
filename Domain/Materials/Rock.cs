using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Materials
{
    public class Rock : IMaterial
    {
        public string Name { get { return "Rock"; } }

        public override string ToString()
        {
            return Name;
        }
    }
}
