using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.FieldOfView;
using LastManStanding.Domain.Materials;
using LastManStanding.Domain.Movement;

namespace LastManStanding.Domain.Races
{
    public class Race : IRace
    {
        public IMaterial Material { get; set; }
        public IMovementProfile MovementProfile { get; set; }
        public IFovProfile FovProfile { get; set; }
        public Char Symbol { get; set; }
    }
}
