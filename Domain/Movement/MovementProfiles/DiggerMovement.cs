using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Materials;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Movement.MovementProfiles
{
    public class DiggerMovement : IMovementProfile
    {
        #region IMovementProfile Members

        public Direction[] AvailableDirections
        {
            get
            {
                return new[]
                           {
                               Direction.North, Direction.East, Direction.South, Direction.West
                               
                           };
            }
        }

        public bool MaterialIsTraversable(IMaterial material)
        {
            if (material != null)
            {
                if (material is Flesh) return true;
                if (material is Rock) return true;
                if (material is Wood) return true;
            }
            return true;
        }

        public bool TerrainBlocksMovement(ITerrain terrain)
        {
            if (terrain is Floor) return false;

            var door = terrain as Door;
            if ((door != null) && ((door.State == DoorStates.Open) || (door.State == DoorStates.Broken)))
                return false;

            return true;
        }

        public decimal CalculateMaterialMovementCost(IMaterial material)
        {
            if (material != null)
            {
                if (material is Flesh) return 1;
                if (material is Rock) return 1;
                if (material is Wood) return 1;
            }
            return 1;
        }

        public bool TerrainIsTraversable(ITerrain terrain)
        {
            return true;
        }

        public decimal CalculateTerrainMovementCost(ITerrain terrain)
        {
            if (terrain is Floor)
                return 1;

            var door = terrain as Door;
            if (door != null)
            {
                switch (door.State)
                {
                    case DoorStates.Closed:
                        return 2;
                    case DoorStates.Open:
                        return 1;
                    case DoorStates.Locked:
                        return 100;
                    case DoorStates.Secret:
                        return 100;
                    case DoorStates.Broken:
                        return 1;
                    case DoorStates.Stuck:
                        return 100;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return 1;
        }

        #endregion
    }
}
