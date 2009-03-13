using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Commands
{
    public class NullCommand : ICommand
    {
        public CommandResult Execute()
        {
            return new CommandResult() {Success = true, UsesTurn = false, Name = "NullCommand", Message = "NullCommand"};
        }
    }
}
