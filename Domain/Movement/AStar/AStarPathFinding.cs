using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Terrain;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Movement.AStar
{
    public class AStarPathFinding : IPathfindingAlgorithm
    {
        public MovementPath FindPath(TerrainMap terrain, IList<IActor> actors, IMovementProfile movementProfile, Point locationFrom,
                                     Point locationTo)
        {
            var closed = new HashSet<PathFindingNode>();
            var queue = new PriorityQueue<decimal, Path<PathFindingNode>>();
            queue.Enqueue(0, new Path<PathFindingNode>(new PathFindingNode {Location = locationFrom}));
            while (!queue.IsEmpty)
            {
                KeyValuePair<decimal, Path<PathFindingNode>> path = queue.Dequeue();
                if (closed.Contains(path.Value.LastStep))
                    continue;
                if (path.Value.LastStep.Location == locationTo)
                    return GetDirections(path.Value);
                closed.Add(path.Value.LastStep);
                foreach (PathFindingNode n in GetNodeNeighbours(terrain, actors, movementProfile, path.Value.LastStep, locationTo))
                {
                    Path<PathFindingNode> newPath = path.Value.AddStep(n, n.MovementCost);
                    queue.Enqueue(newPath.TotalCost + GetEstimatedDistance(n, locationFrom, locationTo), newPath);
                }
            }
            return new MovementPath();
        }

        protected virtual decimal GetEstimatedDistance(PathFindingNode currentNode, Point locationFrom, Point locationTo)
        {
            int dx1 = currentNode.Location.X - locationTo.X;
            int dy1 = currentNode.Location.Y - locationTo.Y;
            int dx2 = locationFrom.X - locationTo.X;
            int dy2 = locationFrom.Y - locationTo.Y;

            return ((currentNode.MovementCost*
                     Math.Max(Math.Abs(locationFrom.X - locationTo.X), Math.Abs(locationFrom.Y - locationTo.Y))) +
                    (Math.Abs(dx1*dy2 - dx2*dy1)*0.001M));
        }

        protected virtual IEnumerable<PathFindingNode> GetNodeNeighbours(TerrainMap terrain, IList<IActor> actors,
                                                                         IMovementProfile movementProfile,
                                                                         PathFindingNode node, Point locationTo)
        {
            var picker = new DirectionPicker(Direction.Northwest, 100, movementProfile.AvailableDirections);
            while (picker.HasDirectionToPick)
            {
                Direction direction = picker.PickRandomDirection();
                if (terrain.HasAdjacentLocation(node.Location, direction))
                {
                    Point targetLocation = terrain.GetTargetLocation(node.Location, direction).Value;

                    if (LocationIsTraversable(terrain, actors, movementProfile, targetLocation, locationTo))
                    yield return
                        new PathFindingNode
                            {
                                Location = direction.ApplyTransform(node.Location),
                                Direction = direction,
                                MovementCost = CalculateLocationMovementCost(terrain, actors, movementProfile, targetLocation, locationTo)
                            };
                }
            }
        }

        private bool LocationIsTraversable(TerrainMap terrainMap, IList<IActor> actors, IMovementProfile movementProfile, Point targetLocation, Point locationTo)
        {
            if (!movementProfile.TerrainIsTraversable(terrainMap[targetLocation]))
                return false;

            // We exclude actor movement cost if the target location is the end location
            // This way we will always find a path to an actor
            if (targetLocation == locationTo) return true;

            foreach (var actor in actors.Where(x => x.IsAlive && (x.Location.Coordinate == targetLocation)))
                if (!movementProfile.MaterialIsTraversable(actor.Race.Material))
                    return false;

            return true;
        }

        private decimal CalculateLocationMovementCost(TerrainMap terrainMap, IList<IActor> actors, IMovementProfile movementProfile, Point targetLocation, Point locationTo)
        {
            decimal movementCost = 0M;

            if (movementProfile.TerrainIsTraversable(terrainMap[targetLocation]))
                movementCost += movementProfile.CalculateTerrainMovementCost(terrainMap[targetLocation]);

            // We exclude actor movement cost if the target location is the end location
            // This way we will always find a path to an actor
            if (targetLocation == locationTo) return movementCost;

            foreach (var actor in actors.Where(x => x.IsAlive && (x.Location.Coordinate == targetLocation)))
                if (movementProfile.MaterialIsTraversable(actor.Race.Material))
                    movementCost += movementProfile.CalculateMaterialMovementCost(actor.Race.Material);

            return movementCost;
        }

        protected virtual MovementPath GetDirections(Path<PathFindingNode> path)
        {
            var theDirections = new MovementPath();

            if ((path != null) && (path.PreviousSteps != null))
            {
                foreach (PathFindingNode node in GetDirections(path.PreviousSteps))
                    theDirections.Enqueue(node);

                theDirections.Enqueue(path.LastStep);
            }

            return theDirections;
        }
    }
}
