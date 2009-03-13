using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Terrain;

namespace LastManStanding.Domain.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly IActor actor;
        private readonly Direction direction;
        private readonly IList<IActor> actors;
        private readonly TerrainMap terrain;

        public MoveCommand(IActor actor, Direction direction, TerrainMap terrain, IList<IActor> actors)
        {
            this.direction = direction;
            this.actor = actor;
            this.actors = actors;
            this.terrain = terrain;
        }

        public Direction Direction
        {
            get { return direction; }
        }

        #region ICommand Members

        public CommandResult Execute()
        {
            Point targetLocation = direction.ApplyTransform(actor.Location.Coordinate);

            if (!terrain.Bounds.Contains(targetLocation))
                return new CommandResult()
                {
                    Name = "MoveCommand",
                    UsesTurn = true,
                    Success = false,
                    Message =
                        string.Format("Unable to move {0} outside bounds of map", direction)
                };

            // Filter out all the actors that are alive and at the target location
            foreach (Actor monster in actors.Where(x => x.IsAlive && (x.Location.Coordinate == targetLocation)))
            {
                // Check for collision with monster
                if (!actor.Race.MovementProfile.MaterialIsTraversable(monster.Race.Material))
                    return new CommandResult()
                               {
                                   Name = "MoveCommand",
                                   UsesTurn = true,
                                   Success = false,
                                   Message = string.Format("Unable to move {0} blocked by {1}", direction, monster)
                               };
            }

            // Check for collision with terrain
            if (actor.Race.MovementProfile.TerrainBlocksMovement((terrain[targetLocation])))
                return new CommandResult()
                           {
                               Name = "MoveCommand",
                               UsesTurn = true,
                               Success = false,
                               Message =
                                   string.Format("Unable to move {0} blocked by {1}", direction,
                                                 terrain[targetLocation])
                           };

            // No collision - move the actor
            actor.SetLocation(targetLocation);
            return new CommandResult()
                       {
                           Name = "MoveCommand",
                           UsesTurn = true,
                           Success = true,
                           Message = string.Format("Moved {0} to {1}", actor, direction)
                       };

        }

        #endregion
    }
}
