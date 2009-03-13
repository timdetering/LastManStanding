﻿using System;
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
using LastManStanding.Domain.Movement.MovementProfiles;
using LastManStanding.Domain.Races;
using LastManStanding.Domain.Terrain.Generation.CorridorGeneration;
using LastManStanding.Domain.Terrain.Generation.DoorGeneration;
using LastManStanding.Domain.Terrain.Generation.RoomGeneration;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Terrain
{
    public class DungeonGenerator
    {
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
            System.Console.WriteLine("Generating dungeon...");

            System.Console.WriteLine("Generating terrain...");
            gameInstance.Terrain = new TerrainMap(Rng.Next(MinSize.Width, MaxSize.Width),
                                         Rng.Next(MinSize.Height, MaxSize.Height), () => new Wall() {Material = new Rock()});


            if (CorridorGenerator != null)
            {
                System.Console.WriteLine("Generating corridors...");
                CorridorGenerator.GenerateCorridors(gameInstance.Terrain);
            }
            if (RoomGenerator != null)
            {
                System.Console.WriteLine("Generating rooms...");
                RoomGenerator.GenerateRooms(gameInstance.Terrain);
            }
            if (DoorGenerator != null)
            {
                System.Console.WriteLine("Generating doors...");
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

            System.Console.WriteLine("Dungeon generation complete.");
        }

        private static void WalkTheDungeon(Game gameInstance)
        {
            System.Console.WriteLine("Walking the dungeon...");
            var digger = new Actor()
                             {
                                 Race =
                                     new Race() { FovProfile = new NightVisionFov() {FovRadius = 100}, MovementProfile = new HumanoidMovement() }
                             }; 

            var walkableLocations = gameInstance.Terrain.WalkableLocations(digger.Race.MovementProfile).ToList();
            digger.SetLocation(walkableLocations[Rng.Next(walkableLocations.Count - 1)]);
            digger.SetIntellect(new DiggerBrain());

            gameInstance.AddActor(digger);

            digger.VisibilityMap.UpdateVisibilityMap(gameInstance.Terrain, gameInstance.LightMap, digger.Location.Coordinate);
            var command = digger.Intellect.GetNextAction();
            while (command is MoveCommand)
            {
                var result = command.Execute();
                if ((!result.Success) && (command is MoveCommand))
                {
                    // The move action failed
                    // Ask the actor for a default action on bump
                    var defaultBumpAction = digger.Intellect.GetDefaultBumpAction((MoveCommand)command);
                    if (defaultBumpAction != null)
                       defaultBumpAction.Execute();
                }

                digger.VisibilityMap.UpdateVisibilityMap(gameInstance.Terrain, gameInstance.LightMap, digger.Location.Coordinate);
                command = digger.Intellect.GetNextAction();
            }

            gameInstance.RemoveActor(digger);
            System.Console.WriteLine("Dungeon walk complete.");
        }
    }
}