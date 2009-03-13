﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Materials;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.FieldOfView.FovProfiles
{
    public class HumanoidFov : IFovProfile
    {
        public FovShapeType FovShape { get { return FovShapeType.Square; } }
        private int fovRadius = 10;
        public int FovRadius { get { return fovRadius; } set { fovRadius = value; } }

        public bool LosIsBlockedByMaterial(IMaterial material)
        {
            if (material != null)
            {
                if (material is Flesh) return true;
                if (material is Rock) return true;
                if (material is Wood) return true;
            }
            return false;
        }

        public bool LosIsBlockedByTerrain(ITerrain terrain)
        {
            if (terrain is Floor) return false;

            var door = terrain as Door;
            if ((door != null) && ((door.State == DoorStates.Open) || (door.State == DoorStates.Broken)))
                return false;

            return true;
        }

        public bool IsVisible(System.Drawing.Color lightValue)
        {
            return lightValue != System.Drawing.Color.Black;
        }
    }
}
