using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using libtcodWrapper;
using LastManStanding.Domain;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.AI;
using LastManStanding.Domain.Commands;
using LastManStanding.Domain.FieldOfView.FovProfiles;
using LastManStanding.Domain.Lighting;
using LastManStanding.Domain.Materials;
using LastManStanding.Domain.Movement.MovementProfiles;
using LastManStanding.Domain.Races;
using LastManStanding.Domain.Terrain;
using LastManStanding.Domain.Terrain.Generation.CorridorGeneration.HuntAndKill;
using LastManStanding.Domain.Terrain.Generation.DoorGeneration;
using LastManStanding.Domain.Terrain.Generation.RoomGeneration;
using log4net;
using Color=libtcodWrapper.Color;
using Console=libtcodWrapper.Console;

namespace LastManStanding
{
    internal class Program
    {
        private static ILog logger;
        private static ILog Logger
        {
            get
            {
                if (logger == null)
                {
                    log4net.Config.XmlConfigurator.Configure();
                    logger = LogManager.GetLogger(typeof(Program));
                }
                return logger;
            }
        }

        private static Game CreateGame()
        {
            Logger.Info("Creating new game...");
            var game = new Game();
            var humanoidMovement = new HumanoidMovement();

            Logger.Info("Creating races...");
            var human = new Race
            {
                Symbol = 'H',
                Material = new Flesh(),
                MovementProfile = humanoidMovement,
                FovProfile = new HumanoidFov()
            };
            var dps = new Race
            {
                Symbol = 'D',
                Material = new Flesh(),
                MovementProfile = humanoidMovement,
                FovProfile = new NightVisionFov()
            };
            var tank = new Race
            {
                Symbol = 'T',
                Material = new Flesh(),
                MovementProfile = humanoidMovement,
                FovProfile = new NightVisionFov()
            };

            var dungeonGenerator = new DungeonGenerator
            {
                MaxSize = new Size(78, 78),
                MinSize = new Size(78, 78),
                CorridorGenerator =
                    new CorridorGenerator
                    {
                        ChangeDirectionModifier = 30,
                        DeadEndRemovalModifier = 50,
                        SparsenessModifier = 70
                    },
                RoomGenerator = new RoomGenerator
                {
                    MaxNoOfRooms = 15,
                    MinNoOfRooms = 10,
                    MaxRoomSize = new Size(15, 15),
                    MinRoomSize = new Size(10, 10)
                },
                DoorGenerator =
                    new DoorGenerator
                    {
                        MinDoorsPerRoom = 1,
                        MaxDoorsPerRoom = 4,
                        MaxDoorsPerWall = 1,
                        MinDoorsPerWall = 1
                    }
            };

            dungeonGenerator.Generate(game);

            List<Point> walkableLocations = game.Terrain.WalkableLocations(humanoidMovement).ToList();

            Logger.Info("Creating player...");
            var player = new Actor
            {
                Name = "Player",
                Damage = 5,
                Health = 10,
                Speed = 10,
                Race = human,
                LightSource = new LightSource
                {
                    Attenuation =
                        new AttenuationFunction { Constant = 1, Linear = 0.3f, Quadratic = 0 },
                    Colour = System.Drawing.Color.Gold,
                    FovProfile = new TorchFov(),
                    Location = new Location()
                }
            };
            player.SetLocation(walkableLocations[Rng.Next(walkableLocations.Count - 1)]);
            var commandFactory = new CommandFactory(game);
            player.SetIntellect(new PlayerAI(commandFactory));

            game.AddActor(player, true);

            Logger.Info("Creating monsters...");
            var monsters = new List<IActor>
                               {
                                   new Actor
                                       {Name = "Gary", Damage = 1, Health = 10, MaxHealth = 10, Speed = 10, Race = tank},
                                   new Actor
                                       {
                                           Name = "William",
                                           Damage = 10,
                                           Health = 1,
                                           MaxHealth = 1,
                                           Speed = 10,
                                           Race = dps
                                       },
                                   new Actor
                                       {
                                           Name = "Byron",
                                           Damage = 1,
                                           Health = 10,
                                           MaxHealth = 10,
                                           Speed = 10,
                                           Race = tank
                                       },
                                   new Actor
                                       {Name = "Calvin", Damage = 10, Health = 1, MaxHealth = 1, Speed = 10, Race = dps},
                                   new Actor
                                       {Name = "John", Damage = 1, Health = 10, MaxHealth = 10, Speed = 10, Race = tank},
                                   new Actor
                                       {Name = "Eugene", Damage = 10, Health = 1, MaxHealth = 1, Speed = 10, Race = dps},
                                   new Actor
                                       {
                                           Name = "Andreas",
                                           Damage = 1,
                                           Health = 10,
                                           MaxHealth = 10,
                                           Speed = 10,
                                           Race = tank
                                       },
                                   new Actor
                                       {Name = "Fred", Damage = 10, Health = 1, MaxHealth = 1, Speed = 10, Race = dps}
                               };

            foreach (IActor monster in monsters)
            {
                monster.SetLocation(walkableLocations[Rng.Next(walkableLocations.Count - 1)]);
                monster.SetIntellect(new HunterBrain());
                game.AddActor(monster);
            }

            Logger.Info("Game created.");
            return game;
        }

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (CurrentDomain_UnhandledException);
            Color fogOfWarColour = Color.FromRGB(80, 80, 80);

            
            var game = CreateGame();

            Logger.Info("Initializing RootConsole...");
            RootConsole.Width = 124;
            RootConsole.Height = 80;
            RootConsole.Fullscreen = false;
            RootConsole.Font = new CustomFontRequest("celtic_garamond_10x10_gs_tc.png", 10, 10,
                                                     CustomFontRequestFontTypes.Grayscale |
                                                     CustomFontRequestFontTypes.LayoutTCOD);
            RootConsole.WindowTitle = "Last Man Standing v1.0";
            RootConsole rootConsole = RootConsole.GetInstance();

            Logger.Info("Initializing playConsole...");
            Console playConsole = RootConsole.GetNewConsole(78, 78);
            Logger.Info("Initializing threatConsole...");
            Console threatConsole = RootConsole.GetNewConsole(42, 10);
            Logger.Info("Initializing playerConsole...");
            Console playerConsole = RootConsole.GetNewConsole(42, 6);
            Logger.Info("Initializing competitorConsole...");
            Console competitorConsole = RootConsole.GetNewConsole(42, 17);
            Logger.Info("Initializing eventsConsole...");
            Console eventsConsole = RootConsole.GetNewConsole(42, 38);

            Logger.Info("Starting Game Loop...");
            do
            {
                KeyPress key = Keyboard.CheckForKeypress(KeyPressType.Pressed);
                
                if (game.IsActive)
                {
                    game.ProcessTurn();
                    
                    if (key.KeyCode != KeyCode.TCODK_NONE)
                        ((PlayerAI)game.Player.Intellect).EvaluateKeyPress(key);
                }

                RenderAllConsoles(game, rootConsole, playConsole, fogOfWarColour, playerConsole, threatConsole,
                                  competitorConsole, eventsConsole);

                if (!game.IsActive)
                {
                    rootConsole.PrintLineRect("Press SPACE to start a new game. Press ESC to quit.", 30, 40, 30, 10, LineAlignment.Left);
                    if (key.KeyCode == KeyCode.TCODK_SPACE)
                    {
                        rootConsole.PrintLine("Creating new game...", 1, 1, LineAlignment.Left);
                        rootConsole.Flush();
                        game = CreateGame();
                    }
                        
                }

                rootConsole.Flush();

                if (key.KeyCode == KeyCode.TCODK_ESCAPE)
                    return;

            } while (!rootConsole.IsWindowClosed());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                Logger.Error("An exception occurred", exception);
            }
            
        }

        private static void RenderAllConsoles(Game game, Console rootConsole, Console playConsole, Color fogOfWarColour,
                                              Console playerConsole, Console threatConsole, Console competitorConsole,
                                              Console eventsConsole)
        {
            rootConsole.Clear();
            rootConsole.ForegroundColor = ColorPresets.White;
            rootConsole.BackgroundColor = ColorPresets.Black;

            rootConsole.DrawFrame(0, 0, 80, 80, true);
            rootConsole.DrawFrame(80, 0, 44, 9, true, "Player");
            rootConsole.DrawFrame(80, 9, 44, 12, true, "Threats");
            rootConsole.DrawFrame(80, 21, 44, 19, true, "Competitors");
            rootConsole.DrawFrame(80, 40, 44, 40, true, "Events");

            RenderPlayConsole(game, playConsole, fogOfWarColour);
            RenderPlayerConsole(game.Player, playerConsole);
            RenderThreatConsole(game.Player, game.Actors, threatConsole);
            RenderCompetitorConsole(game, competitorConsole);
            RenderEventsConsole(game, eventsConsole);

            playConsole.Blit(0, 0, 78, 78, rootConsole, 1, 1);
            playerConsole.Blit(0, 0, 42, 6, rootConsole, 81, 1);
            threatConsole.Blit(0, 0, 42, 10, rootConsole, 81, 10);
            competitorConsole.Blit(0, 0, 42, 17, rootConsole, 81, 22);
            eventsConsole.Blit(0, 0, 42, 38, rootConsole, 81, 41);
        }

        private static void RenderEventsConsole(Game gameInstance, Console eventsConsole)
        {
            eventsConsole.Clear();
            int j = gameInstance.GameEvents.Count - 1;
            int i = 0;

            if (!gameInstance.IsActive)
            {
                eventsConsole.ForegroundColor = gameInstance.AllMonstersAreDead() ? ColorPresets.Gold : ColorPresets.Red;
                eventsConsole.PrintLine(gameInstance.AllMonstersAreDead() ? "You are the last man standing!" : "You have been eliminated", 0, i, LineAlignment.Left);
                i++;
            }

            for (; i < 38; i++)
            {
                eventsConsole.ForegroundColor = ColorPresets.White;

                if ((j < 0) || (j >= gameInstance.GameEvents.Count)) break;

                while ((j >= 0) &&
                       !(gameInstance.GameEvents[j].Command is AttackCommand ||
                         gameInstance.GameEvents[j].Command is OpenDoorCommand))
                    j--;

                if ((j < 0) || (j >= gameInstance.GameEvents.Count)) break;
                eventsConsole.PrintLine(gameInstance.GameEvents[j].Result.Message, 0, i, LineAlignment.Left);
                j--;
            }
        }

        private static void RenderCompetitorConsole(Game gameInstance, Console competitorConsole)
        {
            competitorConsole.Clear();
            List<IActor> competitors = gameInstance.AllActors.Where(x => x != gameInstance.Player).ToList();

            for (int i = 0; i < competitors.Count; i++)
            {
                competitorConsole.ForegroundColor = competitors[i].IsAlive ? ColorPresets.White : ColorPresets.Red;
                competitorConsole.PrintLine(
                    string.Format("{0} is {1} with {2} kills", competitors[i], competitors[i].IsAlive ? "Alive" : "Dead",
                                  competitors[i].Kills.Count), 0, (i*2) + 1, LineAlignment.Left);
            }
        }

        private static void RenderPlayerConsole(IActor player, Console playerConsole)
        {
            playerConsole.Clear();
            playerConsole.ForegroundColor = ColorPresets.White;

            playerConsole.PrintLine(string.Format("Health : {0}", player.Health), 0, 1, LineAlignment.Left);
            playerConsole.PrintLine(string.Format("Damage : {0}", player.Damage), 0, 3, LineAlignment.Left);
            playerConsole.PrintLine(string.Format("Kills : {0}", player.Kills.Count), 0, 5, LineAlignment.Left);
        }

        private static void RenderThreatConsole(IActor player, IList<IActor> monsters, Console threatConsole)
        {
            threatConsole.Clear();
            threatConsole.ForegroundColor = ColorPresets.White;

            List<IActor> threats = player.Intellect.IdentifyThreats(monsters).ToList();

            for (int i = 0; i < threats.Count; i++)
                threatConsole.PrintLine(string.Format("{0} : {1}", threats[i].Race.Symbol, threats[i]), 0, (i*2) + 1,
                                        LineAlignment.Left);
        }

        private static void RenderPlayConsole(Game gameInstance, Console playConsole, Color fogOfWarColour)
        {
            playConsole.Clear();

            for (int y = 0; y < gameInstance.Terrain.Height; y++)
                for (int x = 0; x < gameInstance.Terrain.Width; x++)
                    if (gameInstance.Player.VisibilityMap[x, y].WasSeen)
                    {
                        Color lightColour = Color.FromRGB(gameInstance.LightMap[x, y].Colour.R,
                                                          gameInstance.LightMap[x, y].Colour.G,
                                                          gameInstance.LightMap[x, y].Colour.B);

                        if (lightColour.Value < fogOfWarColour.Value) lightColour = fogOfWarColour;

                        playConsole.ForegroundColor = gameInstance.Player.VisibilityMap[x, y].IsVisible
                                                          ? lightColour
                                                          : fogOfWarColour;
                        playConsole.PutChar(x, y, gameInstance.Terrain[x, y].Symbol);
                    }

            playConsole.ForegroundColor =
                Color.FromRGB(gameInstance.LightMap[gameInstance.Player.Location.Coordinate].Colour.R,
                              gameInstance.LightMap[gameInstance.Player.Location.Coordinate].Colour.G,
                              gameInstance.LightMap[gameInstance.Player.Location.Coordinate].Colour.B);
            playConsole.PutChar(gameInstance.Player.Location.Coordinate.X, gameInstance.Player.Location.Coordinate.Y,
                                '@');

            foreach (IActor actor in gameInstance.Actors.Where(x => x != gameInstance.Player))
            {
                if (gameInstance.Player.VisibilityMap[actor.Location.Coordinate].IsVisible)
                {
                    Color lightColour = Color.FromRGB(gameInstance.LightMap[actor.Location.Coordinate].Colour.R,
                                                      gameInstance.LightMap[actor.Location.Coordinate].Colour.G,
                                                      gameInstance.LightMap[actor.Location.Coordinate].Colour.B);

                    if (lightColour.Value < fogOfWarColour.Value) lightColour = fogOfWarColour;

                    playConsole.ForegroundColor = lightColour;
                    playConsole.PutChar(actor.Location.Coordinate.X, actor.Location.Coordinate.Y, actor.Race.Symbol);
                }
            }
        }
    }
}
