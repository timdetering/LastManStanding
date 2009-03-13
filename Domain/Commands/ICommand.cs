using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Commands
{
    public interface ICommand
    {
        CommandResult Execute();
    }
}
