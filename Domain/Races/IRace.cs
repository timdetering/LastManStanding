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
    public interface IRace : IComposite
    {
        IMovementProfile MovementProfile { get; set; }
        IFovProfile FovProfile { get; set; }
        Char Symbol { get; }
    }
}
