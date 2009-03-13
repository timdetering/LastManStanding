using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Materials;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Terrain.TerrainTypes
{
    [Serializable]
    public class Door : ITerrain
    {
        public ILocation Location { get; set; }
        public IMaterial Material { get; set; }
        public DoorStates State { get; set; }
        public Char Symbol
        {
            get
            {
                switch (State)
                {
                    case DoorStates.Closed:
                        return '+';
                    case DoorStates.Open:
                        return '\'';
                    case DoorStates.Locked:
                        return '+';
                    case DoorStates.Secret:
                        return '#';
                    case DoorStates.Broken:
                        return '\'';
                    case DoorStates.Stuck:
                        return '+';
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
        }
        public override string ToString()
        {
            switch (State)
            {
                case DoorStates.Closed:
                    return "Closed Door";
                case DoorStates.Open:
                    return "Open Door";
                case DoorStates.Locked:
                    return "Locked Door";
                case DoorStates.Secret:
                    return "Secret Door";
                case DoorStates.Broken:
                    return "Broken Door";
                case DoorStates.Stuck:
                    return "Stuck Door";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}