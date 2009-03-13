﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Materials
{
    public class Wood : IMaterial
    {
        public string Name { get { return "Wood"; } }

        public override string ToString()
        {
            return Name;
        }
    }
}
