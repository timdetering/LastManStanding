using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Commands;
using LastManStanding.Domain.Materials;
using LastManStanding.Domain.Races;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Movement.MovementProfiles
{
    public class HumanoidMovement : IMovementProfile
    {
        #region IMovementProfile Members

        public Direction[] AvailableDirections
        {
            get
            {
                return new[]
                           {
                               Direction.North, Direction.East, Direction.South, Direction.West,
                               Direction.Northwest, Direction.Northeast, Direction.Southwest, Direction.Southeast
                           };
            }
        }

        public bool MaterialIsTraversable(IMaterial material)
        {
            if (material != null)
            {
                if (material is Flesh) return false;
                if (material is Rock) return false;
                if (material is Wood) return false;
            }
            return true;
        }

        public decimal CalculateMaterialMovementCost(IMaterial material)
        {
            if (material != null)
            {
                if (material is Flesh) return 100;
                if (material is Rock) return 100;
                if (material is Wood) return 100;
            }
            return 1;
        }

        public bool TerrainIsTraversable(ITerrain terrain)
        {
            if (terrain is Floor) return true;

            if (terrain is Door) return true;

            //var door = terrain as Door;
            //if ((door != null) && ((door.State == DoorStates.Open) || (door.State == DoorStates.Broken)))
            //    return true;

            return false;
        }

        public bool TerrainBlocksMovement(ITerrain terrain)
        {
            if (terrain is Floor) return false;

            var door = terrain as Door;
            if ((door != null) && ((door.State == DoorStates.Open) || (door.State == DoorStates.Broken)))
                return false;

            return true;
        }

        public decimal CalculateTerrainMovementCost(ITerrain terrain)
        {
            if (terrain is Floor) 
                return 1;

            var door = terrain as Door;
            if (door != null)
            {
                switch(door.State)
                {
                    case DoorStates.Closed:
                        return 5;
                    case DoorStates.Open:
                        return 1;
                    case DoorStates.Locked:
                        return 20;
                    case DoorStates.Secret:
                        return 80;
                    case DoorStates.Broken:
                        return 1;
                    case DoorStates.Stuck:
                        return 20;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return 100;
        }

        #endregion
    }
}