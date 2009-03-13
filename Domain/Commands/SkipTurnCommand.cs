using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;

namespace LastManStanding.Domain.Commands
{
    public class SkipTurnCommand : ICommand
    {
        private readonly IActor actor;

        public SkipTurnCommand(IActor actor)
        {
            this.actor = actor;
        }

        public CommandResult Execute()
        {
            return new CommandResult() {Success = true, UsesTurn = true, Name = "SkipTurnCommand", Message = string.Format("{0} Skipped a turn", actor)};
        }
    }
}
