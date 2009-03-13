using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Commands;

namespace LastManStanding.Domain.AI
{
    public class NullBrain : IIntellect
    {
        public IActor Host { get; set; }

        public ICommand GetDefaultBumpAction(MoveCommand moveCommand)
        {
            return new SkipTurnCommand(Host);
        }

        //public ThreatModel ThreatModel { get; private set; }

        //public NullBrain()
        //{
        //    Host = host;
        //    ThreatModel = new ThreatModel(Host);
        //}

        public ICommand GetNextAction()
        {
            return new SkipTurnCommand(Host);
        }

        public IEnumerable<IActor> IdentifyThreats(IEnumerable<IActor> actors)
        {
            return new List<IActor>();
        }
    }
}
