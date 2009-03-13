using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Commands;

namespace LastManStanding.Domain
{
    public class GameEvent
    {
        public IActor Actor { get; set; }
        public int TimeSlice { get; set; }
        public ICommand Command { get; set; }
        public CommandResult Result { get; set; }
    }
}
