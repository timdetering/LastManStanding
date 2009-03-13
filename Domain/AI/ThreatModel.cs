using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;

namespace LastManStanding.Domain.AI
{
    public class ThreatModel
    {
        private List<IActor> threats = new List<IActor>();
        private readonly IActor actor;

        public ThreatModel(IActor actor)
        {
            this.actor = actor;
        }

        public bool HasThreats { get { return threats.Count > 0; } }

        public List<IActor> Threats
        {
            get { return threats; }
        }

        public virtual void FindThreats(Game gameInstance)
        {
            threats = (from a in gameInstance.Actors
                       where IsThreat(a) && actor.VisibilityMap[a.Location.Coordinate].IsVisible
                       select a).ToList();
        }

        protected virtual bool IsThreat(IActor potentialThreat)
        {
            // TODO: Determine if an actor is a threat
            return actor != potentialThreat;
        }

        public IActor GetStrongestThreat()
        {
            // TODO: Implement logic to identify the strongest threat
            return threats.FirstOrDefault();
        }

        public IActor GetWeakestThreat()
        {
            // TODO: Implement logic to identify the weakest threat
            return threats.FirstOrDefault();
        }

        public IActor GetClosestThreat()
        {
            int closestDistance = int.MaxValue;
            IActor closestThreat = threats.FirstOrDefault();

            foreach (var threat in threats)
            {
                var currentDistance = actor.Location.DistanceTo(threat.Location);
                if (currentDistance >= closestDistance) continue;

                closestDistance = currentDistance;
                closestThreat = actor;
            }

            return closestThreat;
        }

        public IActor GetFurthestThreat()
        {
            int furthestDistance = int.MinValue;
            IActor furthestThreat = threats.First();

            foreach (var threat in threats)
            {
                var currentDistance = actor.Location.DistanceTo(threat.Location);
                if (currentDistance <= furthestDistance) continue;

                furthestDistance = currentDistance;
                furthestThreat = actor;
            }

            return furthestThreat;
        }
    }
}
