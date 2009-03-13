using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Commands;

namespace LastManStanding.Domain.AI
{
    public interface IIntellect
    {
        ICommand GetNextAction();
        IEnumerable<IActor> IdentifyThreats(IEnumerable<IActor> actors);
        IActor Host { get; set; }
        //ThreatModel ThreatModel { get; }
        ICommand GetDefaultBumpAction(MoveCommand moveCommand);
    }
}