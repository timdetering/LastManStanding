using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Commands;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Movement.AStar;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.AI
{
    public class HunterBrain : IIntellect
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
            else if ((movementPath == null) || (movementPath.Count == 0))
            {
                var closestUnexploredLocation = Host.GetClosestUnexploredLocation();
                if (closestUnexploredLocation != null)
                {
                    // Find the path to the closest unexplored location
                    movementPath = pathFinder.FindPath(Host.GameInstance.Terrain, Host.GameInstance.Actors, Host.Race.MovementProfile,
                                                           Host.Location.Coordinate, closestUnexploredLocation.Value);

                    if (movementPath.Count == 0)
                         Host.VisibilityMap.ResetSeenLocations();
                }
                else
                {
                    Host.VisibilityMap.ResetSeenLocations();
                }
            }

            if ((movementPath == null) || (movementPath.Count == 0))
                return new SkipTurnCommand(Host);

            if ((movementPath.Count == 1) && (closestThreat != null))
                return new AttackCommand(Host, closestThreat);

            return new MoveCommand(Host, movementPath.Dequeue().Direction, Host.GameInstance.Terrain, Host.GameInstance.Actors);
        }

        public IEnumerable<IActor> IdentifyThreats(IEnumerable<IActor> actors)
        {
            // We return the last threat regardless of whether we can see it
            return actors.Count() == 2 ? actors.Where(x => (x != Host)) : actors.Where(x => (x != Host) && (Host.VisibilityMap[x.Location.Coordinate].IsVisible));
        }

        public IActor Host
        {
            get;
            set;
        }

        public ICommand GetDefaultBumpAction(MoveCommand moveCommand)
        {
            Point targetLocation = moveCommand.Direction.ApplyTransform(Host.Location.Coordinate);

            // Check for collision with actors
            foreach (var monster in IdentifyThreats(Host.GameInstance.Actors).Where(x => x.Location.Coordinate == targetLocation))
                // Check for collision with monster
                if (!Host.Race.MovementProfile.MaterialIsTraversable(monster.Race.Material))
                    return new AttackCommand(Host, monster);

            var door = Host.GameInstance.Terrain[targetLocation] as Door;
            if (door != null)
                return new OpenDoorCommand(Host, door);

            return null;
        }
    }
}
