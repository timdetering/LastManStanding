using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
using libtcodWrapper;
using log4net;
using log4net.Config;
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
                    XmlConfigurator.Configure();
                    logger = LogManager.GetLogger(typeof (Program));
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
                                                           new AttenuationFunction
                                                               {Constant = 1, Linear = 0.3f, Quadratic = 0},
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
                                       {
                                           Name = "Gary",
                                           Damage = 1,
                                           Health = 10,
                                           MaxHealth = 10,
                                           Speed = 10,
                                           Race = tank,
                                           LightSource = new LightSource
                                                             {
                                                                 Attenuation =
                                                                     new AttenuationFunction
                                                                         {Constant = 1, Linear = 0.3f, Quadratic = 0},
                                                                 Colour = System.Drawing.Color.Silver,
                                                                 FovProfile = new TorchFov(),
                                                                 Location = new Location()
                                                             }
                                       },
                                   new Actor
                                       {
                                           Name = "William",
                                           Damage = 10,
                                           Health = 1,
                                           MaxHealth = 1,
                                           Speed = 10,
                                           Race = dps,
                                           LightSource = new LightSource
                                                             {
                                                                 Attenuation =
                                                                     new AttenuationFunction
                                                                         {Constant = 1, Linear = 0.3f, Quadratic = 0},
                                                                 Colour = System.Drawing.Color.Orange,
                                                                 FovProfile = new TorchFov(),
                                                                 Location = new Location()
                                                             }
                                       },
                                   new Actor
                                       {
                                           Name = "Byron",
                                           Damage = 1,
                                           Health = 10,
                                           MaxHealth = 10,
                                           Speed = 10,
                                           Race = tank,
                                           LightSource = new LightSource
                                                             {
                                                                 Attenuation =
                                                                     new AttenuationFunction
                                                                         {Constant = 1, Linear = 0.3f, Quadratic = 0},
                                                                 Colour = System.Drawing.Color.Azure,
                                                                 FovProfile = new TorchFov(),
                                                                 Location = new Location()
                                                             }
                                       },
                                   new Actor
                                       {
                                           Name = "Calvin",
                                           Damage = 10,
                                           Health = 1,
                                           MaxHealth = 1,
                                           Speed = 10,
                                           Race = dps,
                                           LightSource = new LightSource
                                                             {
                                                                 Attenuation =
                                                                     new AttenuationFunction
                                                                         {Constant = 1, Linear = 0.3f, Quadratic = 0},
                                                                 Colour = System.Drawing.Color.HotPink,
                                                                 FovProfile = new TorchFov(),
                                                                 Location = new Location()
                                                             }
                                       },
                                   new Actor
                                       {
                                           Name = "John",
                                           Damage = 1,
                                           Health = 10,
                                           MaxHealth = 10,
                                           Speed = 10,
                                           Race = tank,
                                           LightSource = new LightSource
                                                             {
                                                                 Attenuation =
                                                                     new AttenuationFunction
                                                                         {Constant = 1, Linear = 0.3f, Quadratic = 0},
                                                                 Colour = System.Drawing.Color.Green,
                                                                 FovProfile = new TorchFov(),
                                                                 Location = new Location()
                                                             }
                                       },
                                   new Actor
                                       {
                                           Name = "Eugene",
                                           Damage = 10,
                                           Health = 1,
                                           MaxHealth = 1,
                                           Speed = 10,
                                           Race = dps,
                                           LightSource = new LightSource
                                                             {
                                                                 Attenuation =
                                                                     new AttenuationFunction
                                                                         {Constant = 1, Linear = 0.3f, Quadratic = 0},
                                                                 Colour = System.Drawing.Color.CornflowerBlue,
                                                                 FovProfile = new TorchFov(),
                                                                 Location = new Location()
                                                             }
                                       },
                                   new Actor
                                       {
                                           Name = "Andreas",
                                           Damage = 1,
                                           Health = 10,
                                           MaxHealth = 10,
                                           Speed = 10,
                                           Race = tank,
                                           LightSource = new LightSource
                                                             {
                                                                 Attenuation =
                                                                     new AttenuationFunction
                                                                         {Constant = 1, Linear = 0.3f, Quadratic = 0},
                                                                 Colour = System.Drawing.Color.Blue,
                                                                 FovProfile = new TorchFov(),
                                                                 Location = new Location()
                                                             }
                                       },
                                   new Actor
                                       {
                                           Name = "Fred",
                                           Damage = 10,
                                           Health = 1,
                                           MaxHealth = 1,
                                           Speed = 10,
                                           Race = dps,
                                           LightSource = new LightSource
                                                             {
                                                                 Attenuation =
                                                                     new AttenuationFunction
                                                                         {Constant = 1, Linear = 0.3f, Quadratic = 0},
                                                                 Colour = System.Drawing.Color.Red,
                                                                 FovProfile = new TorchFov(),
                                                                 Location = new Location()
                                                             }
                                       }
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

            var screenBounds = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width/10,
                                             Screen.PrimaryScreen.Bounds.Height/10);

            int infoPanelWidth = 42;
            var playBounds = new Rectangle(0, 0, screenBounds.Width - infoPanelWidth, screenBounds.Height);
            var playerBounds = new Rectangle(playBounds.Right, 0, infoPanelWidth, 6);
            var threatBounds = new Rectangle(playBounds.Right, playerBounds.Bottom, infoPanelWidth, 10);
            var competitorBounds = new Rectangle(playBounds.Right, threatBounds.Bottom, infoPanelWidth, 17);
            var eventBounds = new Rectangle(playBounds.Right, competitorBounds.Bottom, infoPanelWidth,
                                            screenBounds.Height -
                                            (playerBounds.Height + threatBounds.Height + competitorBounds.Height));

            Game game = CreateGame();

            Logger.Info("Initializing RootConsole...");

            RootConsole.Fullscreen = true;
            RootConsole.Font = new CustomFontRequest("celtic_garamond_10x10_gs_tc.png", 10, 10,
                                                     CustomFontRequestFontTypes.Grayscale |
                                                     CustomFontRequestFontTypes.LayoutTCOD);

            RootConsole.Width = screenBounds.Width;
            RootConsole.Height = screenBounds.Height;
            RootConsole.WindowTitle = "Last Man Standing v1.0";
            RootConsole rootConsole = RootConsole.GetInstance();

            Logger.Info("Initializing playConsole...");
            Console playConsole = RootConsole.GetNewConsole(playBounds.Width, playBounds.Height);
            Logger.Info("Initializing threatConsole...");
            Console threatConsole = RootConsole.GetNewConsole(threatBounds.Width, threatBounds.Height);
            Logger.Info("Initializing playerConsole...");
            Console playerConsole = RootConsole.GetNewConsole(playerBounds.Width, playerBounds.Height);
            Logger.Info("Initializing competitorConsole...");
            Console competitorConsole = RootConsole.GetNewConsole(competitorBounds.Width, competitorBounds.Height);
            Logger.Info("Initializing eventsConsole...");
            Console eventsConsole = RootConsole.GetNewConsole(eventBounds.Width, eventBounds.Height);

            Logger.Info("Starting Game Loop...");
            do
            {
                KeyPress key = Keyboard.CheckForKeypress(KeyPressType.Pressed);

                if (game.IsActive)
                {
                    game.ProcessTurn();

                    if (key.KeyCode != KeyCode.TCODK_NONE)
                        ((PlayerAI) game.Player.Intellect).EvaluateKeyPress(key);
                }

                RenderAllConsoles(game, rootConsole, playConsole, fogOfWarColour, playerConsole, threatConsole,
                                  competitorConsole, eventsConsole, playBounds, playerBounds, threatBounds,
                                  competitorBounds, eventBounds);

                if (!game.IsActive)
                {
                    rootConsole.PrintLineRect("Press SPACE to start a new game. Press ESC to quit.",
                                              (screenBounds.Width - 30)/2, (screenBounds.Height - 10)/2, 30, 10,
                                              LineAlignment.Left);
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

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                Logger.Error("An exception occurred", exception);
            }
        }

        private static void RenderAllConsoles(Game game, Console rootConsole, Console playConsole, Color fogOfWarColour,
                                              Console playerConsole, Console threatConsole, Console competitorConsole,
                                              Console eventsConsole, Rectangle playBounds, Rectangle playerBounds,
                                              Rectangle threatBounds, Rectangle competitorBounds, Rectangle eventBounds)
        {
            rootConsole.Clear();
            rootConsole.ForegroundColor = ColorPresets.White;
            rootConsole.BackgroundColor = ColorPresets.Black;

            RenderPlayConsole(game, playConsole, fogOfWarColour, playBounds);
            RenderPlayerConsole(game.Player, playerConsole, playerBounds);
            RenderThreatConsole(game.Player, game.Actors, threatConsole, threatBounds);
            RenderCompetitorConsole(game, competitorConsole, competitorBounds);
            RenderEventsConsole(game, eventsConsole, eventBounds);

            playConsole.Blit(0, 0, playBounds.Width, playBounds.Height, rootConsole, playBounds.X, playBounds.Y);
            playerConsole.Blit(0, 0, playerBounds.Width, playerBounds.Height, rootConsole, playerBounds.X,
                               playerBounds.Y);
            threatConsole.Blit(0, 0, threatBounds.Width, threatBounds.Height, rootConsole, threatBounds.X,
                               threatBounds.Y);
            competitorConsole.Blit(0, 0, competitorBounds.Width, competitorBounds.Height, rootConsole,
                                   competitorBounds.X, competitorBounds.Y);
            eventsConsole.Blit(0, 0, eventBounds.Width, eventBounds.Height, rootConsole, eventBounds.X, eventBounds.Y);
        }

        private static void RenderEventsConsole(Game gameInstance, Console console, Rectangle bounds)
        {
            console.Clear();
            console.ForegroundColor = ColorPresets.White;
            console.DrawFrame(0, 0, bounds.Width, bounds.Height, true, "Events");

            int j = gameInstance.GameEvents.Count - 1;
            int i = 1;

            if (!gameInstance.IsActive)
            {
                console.ForegroundColor = gameInstance.AllMonstersAreDead() ? ColorPresets.Gold : ColorPresets.Red;
                console.PrintLine(
                    gameInstance.AllMonstersAreDead() ? "You are the last man standing!" : "You have been eliminated", 0,
                    i, LineAlignment.Left);
                i++;
            }

            for (; i < bounds.Height; i++)
            {
                console.ForegroundColor = ColorPresets.White;

                if ((j < 0) || (j >= gameInstance.GameEvents.Count)) break;

                while ((j >= 0) &&
                       !(gameInstance.GameEvents[j].Command is AttackCommand ||
                         gameInstance.GameEvents[j].Command is OpenDoorCommand))
                    j--;

                if ((j < 0) || (j >= gameInstance.GameEvents.Count)) break;
                console.PrintLine(gameInstance.GameEvents[j].Result.Message, 1, i, LineAlignment.Left);
                j--;
            }
        }

        private static void RenderCompetitorConsole(Game gameInstance, Console console, Rectangle bounds)
        {
            console.Clear();
            console.ForegroundColor = ColorPresets.White;
            console.DrawFrame(0, 0, bounds.Width, bounds.Height, true, "Competitors");


            List<IActor> competitors = gameInstance.AllActors.Where(x => x != gameInstance.Player).ToList();

            for (int i = 0; i < competitors.Count; i++)
            {
                console.ForegroundColor = competitors[i].IsAlive ? ColorPresets.White : ColorPresets.Red;
                console.PrintLine(
                    string.Format("{0} is {1} with {2} kills", competitors[i], competitors[i].IsAlive ? "Alive" : "Dead",
                                  competitors[i].Kills.Count), 1, (i*2) + 1, LineAlignment.Left);
            }
        }

        private static void RenderPlayerConsole(IActor player, Console console, Rectangle bounds)
        {
            console.Clear();
            console.ForegroundColor = ColorPresets.White;
            console.DrawFrame(0, 0, bounds.Width, bounds.Height, true, "Player");


            console.PrintLine(string.Format("Health : {0}", player.Health), 1, 2, LineAlignment.Left);
            console.PrintLine(string.Format("Damage : {0}", player.Damage), 1, 4, LineAlignment.Left);
            console.PrintLine(string.Format("Kills : {0}", player.Kills.Count), 1, 6, LineAlignment.Left);
        }

        private static void RenderThreatConsole(IActor player, IList<IActor> monsters, Console console, Rectangle bounds)
        {
            console.Clear();
            console.ForegroundColor = ColorPresets.White;
            console.DrawFrame(0, 0, bounds.Width, bounds.Height, true, "Threats");


            List<IActor> threats = player.Intellect.IdentifyThreats(monsters).ToList();

            for (int i = 1; i < threats.Count; i++)
                console.PrintLine(string.Format("{0} : {1}", threats[i].Race.Symbol, threats[i]), 1, (i*2) + 1,
                                  LineAlignment.Left);
        }

        private static void RenderPlayConsole(Game gameInstance, Console console, Color fogOfWarColour, Rectangle bounds)
        {
            console.Clear();
            console.ForegroundColor = ColorPresets.White;
            console.DrawFrame(0, 0, bounds.Width, bounds.Height, true);


            for (int y = 0; y < gameInstance.Terrain.Height; y++)
                for (int x = 0; x < gameInstance.Terrain.Width; x++)
                    if (gameInstance.Player.VisibilityMap[x, y].WasSeen)
                    {
                        Color lightColour = Color.FromRGB(gameInstance.LightMap[x, y].Colour.R,
                                                          gameInstance.LightMap[x, y].Colour.G,
                                                          gameInstance.LightMap[x, y].Colour.B);

                        if (lightColour.Value < fogOfWarColour.Value) lightColour = fogOfWarColour;

                        console.ForegroundColor = gameInstance.Player.VisibilityMap[x, y].IsVisible
                                                      ? lightColour
                                                      : fogOfWarColour;
                        console.PutChar(x + 1, y + 1, gameInstance.Terrain[x, y].Symbol);
                    }

            console.ForegroundColor =
                Color.FromRGB(gameInstance.LightMap[gameInstance.Player.Location.Coordinate].Colour.R,
                              gameInstance.LightMap[gameInstance.Player.Location.Coordinate].Colour.G,
                              gameInstance.LightMap[gameInstance.Player.Location.Coordinate].Colour.B);
            console.PutChar(gameInstance.Player.Location.Coordinate.X + 1, gameInstance.Player.Location.Coordinate.Y + 1,
                            '@');

            foreach (IActor actor in gameInstance.Actors.Where(x => x != gameInstance.Player))
            {
                if (gameInstance.Player.VisibilityMap[actor.Location.Coordinate].IsVisible)
                {
                    Color lightColour = Color.FromRGB(gameInstance.LightMap[actor.Location.Coordinate].Colour.R,
                                                      gameInstance.LightMap[actor.Location.Coordinate].Colour.G,
                                                      gameInstance.LightMap[actor.Location.Coordinate].Colour.B);

                    if (lightColour.Value < fogOfWarColour.Value) lightColour = fogOfWarColour;

                    console.ForegroundColor = lightColour;
                    console.PutChar(actor.Location.Coordinate.X + 1, actor.Location.Coordinate.Y + 1, actor.Race.Symbol);
                }
            }
        }
    }
}
