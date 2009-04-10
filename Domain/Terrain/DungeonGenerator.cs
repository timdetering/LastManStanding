using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.AI;
using LastManStanding.Domain.Commands;
using LastManStanding.Domain.FieldOfView;
using LastManStanding.Domain.FieldOfView.FovProfiles;
using LastManStanding.Domain.Materials;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Movement.AStar;
using LastManStanding.Domain.Movement.MovementProfiles;
using LastManStanding.Domain.Races;
using LastManStanding.Domain.Terrain.Generation.CorridorGeneration;
using LastManStanding.Domain.Terrain.Generation.DoorGeneration;
using LastManStanding.Domain.Terrain.Generation.RoomGeneration;
using LastManStanding.Domain.Terrain.TerrainTypes;
using log4net;

namespace LastManStanding.Domain.Terrain
{
    public class DungeonGenerator
    {
        private static ILog logger;
        private static ILog Logger
        {
            get
            {
                if (logger == null)
                {
                    log4net.Config.XmlConfigurator.Configure();
                    logger = LogManager.GetLogger(typeof(DungeonGenerator));
                }
                return logger;
            }
        }

        public Size MinSize { get; set; }
        public Size MaxSize { get; set; }
        public ICorridorGenerator CorridorGenerator { get; set; }
        public IRoomGenerator RoomGenerator { get; set; }
        public IDoorGenerator DoorGenerator { get; set; }
        //public IStairGenerator StairGenerator { get; set; }
        //public ITreasureGenerator TreasureGenerator { get; set; }
        //public IMonsterGenerator MonsterGenerator { get; set; }

        public void Generate(Game gameInstance)
        {
            try
            {
                Logger.Info("Generating dungeon...");

                Logger.Info("Generating terrain...");
                gameInstance.Terrain = new TerrainMap(Rng.Next(MinSize.Width, MaxSize.Width),
                                             Rng.Next(MinSize.Height, MaxSize.Height), () => new Wall() { Material = new Rock() });


                if (CorridorGenerator != null)
                {
                    Logger.Info("Generating corridors...");
                    CorridorGenerator.GenerateCorridors(gameInstance.Terrain);
                }

                if (RoomGenerator != null)
                {
                    Logger.Info("Generating rooms...");
                    RoomGenerator.GenerateRooms(gameInstance.Terrain);
                }
                if (DoorGenerator != null)
                {
                    Logger.Info("Generating doors...");
                    DoorGenerator.GenerateDoors(gameInstance.Terrain);
                }

                WalkTheDungeon(gameInstance);

                if (DoorGenerator != null)
                {
                    DoorGenerator.RemoveInvalidDoors(gameInstance.Terrain);
                    DoorGenerator.ResetDoors(gameInstance.Terrain);
                }

                //if (StairGenerator != null) StairGenerator.GenerateStairs(dungeon);
                //if (TreasureGenerator != null) TreasureGenerator.GenerateTreasure(dungeon);
                //if (MonsterGenerator != null) MonsterGenerator.GenerateMonsters(dungeon);

                Logger.Info("Dungeon generation complete.");
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred during dungeon generation", ex);
                throw;
            }
        }

        private static void WalkTheDungeon(Game gameInstance)
        {
            Logger.Info("Walking the dungeon...");

            var pathFinder = new AStarPathFinding();
            var movementProfile = new HumanoidMovement();
            var visibilityMap = new VisibilityMap(gameInstance.Terrain.Width, gameInstance.Terrain.Height, null, null);

            var walkableLocations = gameInstance.Terrain.WalkableLocations(movementProfile).ToList();

            FloodFill.Fill(gameInstance.Terrain, visibilityMap, movementProfile, walkableLocations[Rng.Next(walkableLocations.Count - 1)]);

            var walkableUnseenLocations = GetWalkableUnseenLocations(gameInstance.Terrain, visibilityMap, movementProfile).ToList();
            while(walkableUnseenLocations.Count > 0)
            {
                var closestExploredLocation = GetClosestExploredLocation(gameInstance.Terrain, visibilityMap, movementProfile, walkableUnseenLocations[Rng.Next(walkableUnseenLocations.Count - 1)]);
                var closestUnexploredLocation = GetClosestUnexploredLocation(gameInstance.Terrain, visibilityMap, movementProfile, closestExploredLocation.Value);

                var movementPath = pathFinder.FindPath(gameInstance.Terrain, new List<IActor>(), new DiggerMovement(), closestExploredLocation.Value, closestUnexploredLocation.Value);
                foreach (var node in movementPath)
                {
                    gameInstance.Terrain[node.Location] = new Floor();
                    visibilityMap[node.Location].WasSeen = true;
                }

                visibilityMap[closestUnexploredLocation.Value].WasSeen = false;
                FloodFill.Fill(gameInstance.Terrain, visibilityMap, movementProfile, closestUnexploredLocation.Value);

                walkableUnseenLocations = GetWalkableUnseenLocations(gameInstance.Terrain, visibilityMap, movementProfile).ToList();
            }


            //digger.VisibilityMap.UpdateVisibilityMap(gameInstance.Terrain, gameInstance.LightMap, digger.Location.Coordinate);
            //var command = digger.Intellect.GetNextAction();
            //while (command is MoveCommand)
            //{
            //    var result = command.Execute();
            //    if ((!result.Success) && (command is MoveCommand))
            //    {
            //        // The move action failed
            //        // Ask the actor for a default action on bump
            //        var defaultBumpAction = digger.Intellect.GetDefaultBumpAction((MoveCommand)command);
            //        if (defaultBumpAction != null)
            //           defaultBumpAction.Execute();
            //    }

            //    digger.VisibilityMap.UpdateVisibilityMap(gameInstance.Terrain, gameInstance.LightMap, digger.Location.Coordinate);
            //    command = digger.Intellect.GetNextAction();
            //}

            //gameInstance.RemoveActor(digger);
            Logger.Info("Dungeon walk complete.");
        }

        private static Point? GetClosestExploredLocation(TerrainMap terrainMap, VisibilityMap visibilityMap, IMovementProfile movementProfile, Point location)
        {
            int distance = int.MaxValue;
            Point? closestLocation = null;

            foreach (var visibleLocation in GetWalkableSeenLocations(terrainMap, visibilityMap, movementProfile))
            {
                int currentDistance = location.DistanceTo(visibleLocation);
                if (currentDistance >= distance) continue;

                distance = currentDistance;
                closestLocation = visibleLocation;
            }

            return closestLocation;
        }

        private static Point? GetClosestUnexploredLocation(TerrainMap terrainMap, VisibilityMap visibilityMap, IMovementProfile movementProfile, Point location)
        {
            int closestDistance = int.MaxValue;
            Point? closestLocation = null;

            // We want to explore rooms before we explore corridors
            DungeonPrefab currentRoom = terrainMap.GetPrefabAtLocation(location);

            // If we are in a room then get the closest unseen location in the room
            foreach (
                Point unseenLocation in
                    currentRoom == null
                        ? GetWalkableUnseenLocations(terrainMap, visibilityMap, movementProfile)
                        : GetWalkableUnseenLocationsWithinPrefab(currentRoom, terrainMap, visibilityMap, movementProfile))
            {
                int currentDistance = location.DistanceTo(unseenLocation);
                if (currentDistance >= closestDistance) continue;

                closestDistance = currentDistance;
                closestLocation = unseenLocation;
            }

            return closestLocation;
        }


        private static IEnumerable<Point> GetWalkableUnseenLocationsWithinPrefab(Map<ITerrain> prefab, TerrainMap terrainMap, VisibilityMap visibilityMap, IMovementProfile movementProfile)
        {
            IEnumerable<Point> unseenLocationsInPrefab = from location in prefab.Locations
                                                         where
                                                             movementProfile.TerrainIsTraversable(
                                                                 terrainMap[location]) &&
                                                             !visibilityMap[location].WasSeen
                                                         select location;

            // Return any unseen locations within the prefab else return the closest unseen location on the map
            return unseenLocationsInPrefab.Count() > 0 ? unseenLocationsInPrefab : GetWalkableUnseenLocations(terrainMap, visibilityMap, movementProfile);
        }

        private static IEnumerable<Point> GetWalkableUnseenLocations(TerrainMap terrainMap, VisibilityMap visibilityMap, IMovementProfile movementProfile)
        {
            return from location in visibilityMap.GetUnseenLocations()
                   where movementProfile.TerrainIsTraversable(terrainMap[location])
                   select location;
        }

        private static IEnumerable<Point> GetWalkableSeenLocations(TerrainMap terrainMap, VisibilityMap visibilityMap, IMovementProfile movementProfile)
        {
            return from location in visibilityMap.GetSeenLocations()
                   where movementProfile.TerrainIsTraversable(terrainMap[location])
                   select location;
        }
 
    }
}