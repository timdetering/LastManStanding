using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Commands
{
    public class CommandResult
    {
        public bool UsesTurn { get; set; }
        public bool Success { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("{0} : {1} : {2}", Name, Success, Message);
        }
    }
}
