using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Movement;

namespace LastManStanding.Domain.Commands
{
    public class CommandFactory
    {
        private readonly Game gameInstance;

        public CommandFactory(Game gameInstance)
        {
            this.gameInstance = gameInstance;
        }

        public ICommand BuildMoveCommand(IActor mover, Direction direction)
        {
            return new MoveCommand(mover, direction, gameInstance.Terrain, gameInstance.Actors);
        }

        public ICommand BuildAttackCommand(IActor attacker, IActor defender)
        {
            return new AttackCommand(attacker, defender);
        }
    }
}
