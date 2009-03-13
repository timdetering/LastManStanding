using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Terrain;

namespace LastManStanding.Domain.FieldOfView
{
    public interface IFovAlgorithm
    {
        FovResultset CalculateFov(TerrainMap terrain, Point origin, IFovProfile fovProfile);
    }
}
