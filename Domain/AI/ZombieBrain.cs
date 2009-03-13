using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Commands;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Movement.AStar;

namespace LastManStanding.Domain.AI
{
    public class ZombieBrain : IIntellect
    {
        private readonly IPathfindingAlgorithm pathFinder = new AStarPathFinding();
        private MovementPath movementPath = null;

        public ICommand GetNextAction()
        {
            IActor closestThreat = null; 
            var threats = IdentifyThreats(Host.GameInstance.Actors);
            if (threats.Count() > 0)
            {
                closestThreat = threats.OrderBy(x => x.Location.DistanceTo(Host.Location)).First();

                movementPath = pathFinder.FindPath(Host.GameInstance.Terrain, threats.ToList(), Host.Race.MovementProfile,
                                                       Host.Location.Coordinate, closestThreat.Location.Coordinate);
            }

            if ((movementPath == null) || (movementPath.Count == 0))
                return new SkipTurnCommand(Host);

            if ((movementPath.Count == 1) && (closestThreat != null))
                return new AttackCommand(Host, closestThreat);

            return new MoveCommand(Host, movementPath.Dequeue().Direction, Host.GameInstance.Terrain, Host.GameInstance.Actors);
        }

        public IEnumerable<IActor> IdentifyThreats(IEnumerable<IActor> actors)
        {
            return actors.Where(x => (x == x.GameInstance.Player) && (Host.VisibilityMap[x.Location.Coordinate].IsVisible));
        }

        public IActor Host
        {
            get; set;
        }

        public ICommand GetDefaultBumpAction(MoveCommand moveCommand)
        {
            return new SkipTurnCommand(Host);
        }
    }
}
