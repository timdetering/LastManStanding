using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.AI;
using LastManStanding.Domain.Commands;
using LastManStanding.Domain.FieldOfView;
using LastManStanding.Domain.Lighting;
using LastManStanding.Domain.Races;

namespace LastManStanding.Domain.Actors
{
    public interface IActor : ILocatable
    {
        Point? GetClosestExploredLocation(Point origin);
        Point? GetClosestUnexploredLocation();

        IList<IActor> Kills { get; }
        void AddKill(IActor victim);

        string Name { get; set; }
        /// <summary>
        /// The amount of life force 
        /// </summary>
        int Health { get; set; }
        int MaxHealth { get; set; }
        /// <summary>
        /// The amount of life force an attack does
        /// </summary>
        int Damage { get; set; }
        /// <summary>
        /// The rate at which the actor does stuff
        /// </summary>
        int Speed { get; set; }
        /// <summary>
        /// Is true while health is greater than zero
        /// </summary>
        bool IsAlive { get; }
        /// <summary>
        /// The intelligence controlling this actor (AI or Human)
        /// </summary>
        IIntellect Intellect { get; }

        IRace Race { get; set; }

        VisibilityMap VisibilityMap { get; set; }

        void SetIntellect(IIntellect intellect);

        void AddHealth(int healthToAdd);

        Game GameInstance { get; set; }

        LightSource LightSource { get; set; }

        void SetLocation(Point coordinate);
    }
}