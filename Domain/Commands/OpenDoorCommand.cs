using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Commands
{
    public class OpenDoorCommand : ICommand
    {
        private readonly IActor actor;
        private readonly Door door;

        public OpenDoorCommand(IActor actor, Door door)
        {
            this.actor = actor;
            this.door = door;
        }

        public CommandResult Execute()
        {
            if (door.State == DoorStates.Closed)
            {
                door.State = DoorStates.Open;
                return new CommandResult()
                {
                    Name = "OpenDoorCommand",
                    UsesTurn = true,
                    Success = true,
                    Message = string.Format("{0} opened a door", actor)
                };
            }

            if ((door.State == DoorStates.Open) || (door.State == DoorStates.Broken))
            {
                return new CommandResult()
                {
                    Name = "OpenDoorCommand",
                    UsesTurn = false,
                    Success = false,
                    Message = string.Format("Unable to open door already open")
                };
            }

            return new CommandResult()
            {
                Name = "OpenDoorCommand",
                UsesTurn = true,
                Success = false,
                Message = string.Format("Unable to open door : {0}", door.State)
            };
        }
    }
}
