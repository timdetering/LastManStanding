using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Commands;
using LastManStanding.Domain.FieldOfView;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Movement.AStar;
using LastManStanding.Domain.Movement.MovementProfiles;
using LastManStanding.Domain.Terrain;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.AI
{
    public class DiggerBrain : IIntellect
    {
        private readonly IPathfindingAlgorithm pathFinder = new AStarPathFinding();
        private MovementPath movementPath = null;

        public ICommand GetNextAction()
        {
            if ((movementPath != null) && (movementPath.Count > 0))
                return new MoveCommand(Host, movementPath.Dequeue().Direction, Host.GameInstance.Terrain, Host.GameInstance.Actors);

            Host.Race.MovementProfile = new HumanoidMovement();

            var closestUnexploredLocation = Host.GetClosestUnexploredLocation();
            if (closestUnexploredLocation != null)
            {
                // Find the path to the closest unexplored location
                movementPath = pathFinder.FindPath(Host.GameInstance.Terrain, Host.GameInstance.Actors, Host.Race.MovementProfile,
                                                       Host.Location.Coordinate, closestUnexploredLocation.Value);

                if (movementPath.Count == 0)
                {
                    // We can't find a path so we need to start digging
                    var closestExploredLocation = Host.GetClosestExploredLocation(closestUnexploredLocation.Value);
                    if (closestExploredLocation == null) return new SkipTurnCommand(Host);

                    if (closestExploredLocation == Host.Location.Coordinate)
                    {
                        // We are at the closest seen destination
                        Host.Race.MovementProfile = new DiggerMovement();
                        movementPath = pathFinder.FindPath(Host.GameInstance.Terrain, Host.GameInstance.Actors, Host.Race.MovementProfile, closestExploredLocation.Value, closestUnexploredLocation.Value);
                    }
                    else
                    {
                        movementPath = pathFinder.FindPath(Host.GameInstance.Terrain, Host.GameInstance.Actors, Host.Race.MovementProfile, Host.Location.Coordinate, closestExploredLocation.Value);
                    }

                    if (movementPath.Count == 0) return new SkipTurnCommand(Host);
                }

                return new MoveCommand(Host, movementPath.Dequeue().Direction, Host.GameInstance.Terrain, Host.GameInstance.Actors);
            }

            return new SkipTurnCommand(Host);
        }

        public IEnumerable<IActor> IdentifyThreats(IEnumerable<IActor> actors)
        {
            return actors.Where(x => (x == x.GameInstance.Player) && (Host.VisibilityMap[x.Location.Coordinate].IsVisible));
        }

        public IActor Host
        {
            get;
            set;
        }

        public ICommand GetDefaultBumpAction(MoveCommand moveCommand)
        {
            Point targetLocation = moveCommand.Direction.ApplyTransform(Host.Location.Coordinate);

            var door = Host.GameInstance.Terrain[targetLocation] as Door;
            if (door != null)
                return new OpenDoorCommand(Host, door);

            if ((Host.Race.MovementProfile.TerrainIsTraversable(Host.GameInstance.Terrain[targetLocation])) && (Host.GameInstance.Terrain[targetLocation] is Wall))
                Host.GameInstance.Terrain[targetLocation] = new Floor();

            return null;
        }


    }
}
