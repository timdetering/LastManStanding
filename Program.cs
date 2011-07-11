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
using NLog;
using libtcod;

namespace LastManStanding
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                                                               {Constant = 1, Linear = 0.15f, Quadratic = 0},
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
                                           Race = tank
                                       },
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
                                       {
                                           Name = "Calvin",
                                           Damage = 10,
                                           Health = 1,
                                           MaxHealth = 1,
                                           Speed = 10,
                                           Race = dps
                                       },
                                   new Actor
                                       {
                                           Name = "John",
                                           Damage = 1,
                                           Health = 10,
                                           MaxHealth = 10,
                                           Speed = 10,
                                           Race = tank
                                       },
                                   new Actor
                                       {
                                           Name = "Eugene",
                                           Damage = 10,
                                           Health = 1,
                                           MaxHealth = 1,
                                           Speed = 10,
                                           Race = dps
                                       },
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
                                       {
                                           Name = "Fred",
                                           Damage = 10,
                                           Health = 1,
                                           MaxHealth = 1,
                                           Speed = 10,
                                           Race = dps
                                       }
                                       ,
                                   new Actor
                                       {
                                           Name = "Katrina",
                                           Damage = 5,
                                           Health = 5,
                                           MaxHealth = 5,
                                           Speed = 10,
                                           Race = dps
                                       },
                                   new Actor
                                       {
                                           Name = "Melissa",
                                           Damage = 5,
                                           Health = 5,
                                           MaxHealth = 5,
                                           Speed = 10,
                                           Race = tank
                                       },
                                   new Actor
                                       {
                                           Name = "Isabel",
                                           Damage = 5,
                                           Health = 5,
                                           MaxHealth = 5,
                                           Speed = 10,
                                           Race = dps
                                       },
                                   new Actor
                                       {
                                           Name = "Angela",
                                           Damage = 5,
                                           Health = 5,
                                           MaxHealth = 5,
                                           Speed = 10,
                                           Race = tank
                                       },
                                   new Actor
                                       {
                                           Name = "Janet",
                                           Damage = 5,
                                           Health = 5,
                                           MaxHealth = 5,
                                           Speed = 10,
                                           Race = dps
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
            TCODColor fogOfWarColour = new TCODColor(80, 80, 80);

            var screenBounds = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width/10,
                                             Screen.PrimaryScreen.Bounds.Height/10);

            int infoPanelWidth = 42;
            var playBounds = new Rectangle(0, 0, screenBounds.Width - infoPanelWidth, screenBounds.Height);
            var playerBounds = new Rectangle(playBounds.Right, 0, infoPanelWidth, 6);
            //var threatBounds = new Rectangle(playBounds.Right, playerBounds.Bottom, infoPanelWidth, 10);
            var competitorBounds = new Rectangle(playBounds.Right, playerBounds.Bottom, infoPanelWidth, 27);
            var eventBounds = new Rectangle(playBounds.Right, competitorBounds.Bottom, infoPanelWidth,
                                            screenBounds.Height -
                                            (playerBounds.Height + competitorBounds.Height));

            Game game = CreateGame();

            Logger.Info("Initializing RootConsole...");

            TCODConsole.setCustomFont(
                "dwd.png",
                (int)(TCODFontFlags.LayoutAsciiInRow),
                16,
                16);
            TCODConsole.initRoot(screenBounds.Width, screenBounds.Height, "Last Man Standing v1.0", true, TCODRendererType.SDL);

            TCODSystem.setFps(30);
            var rootConsole = TCODConsole.root;
            rootConsole.setForegroundColor(ColorPresets.White);
            rootConsole.setAlignment(TCODAlignment.LeftAlignment);
            rootConsole.setBackgroundFlag(TCODBackgroundFlag.Set);

            Logger.Info("Initializing playConsole...");
            TCODConsole playConsole = new TCODConsole(playBounds.Width, playBounds.Height);
            //Logger.Info("Initializing threatConsole...");
            //Console threatConsole = RootConsole.GetNewConsole(threatBounds.Width, threatBounds.Height);
            Logger.Info("Initializing playerConsole...");
            TCODConsole playerConsole = new TCODConsole(playerBounds.Width, playerBounds.Height);
            Logger.Info("Initializing competitorConsole...");
            TCODConsole competitorConsole = new TCODConsole(competitorBounds.Width, competitorBounds.Height);
            Logger.Info("Initializing eventsConsole...");
            TCODConsole eventsConsole = new TCODConsole(eventBounds.Width, eventBounds.Height);

            Logger.Info("Starting Game Loop...");
            do
            {
                TCODKey keyStroke = TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed);
                
                if (game.IsActive)
                {
                    game.ProcessTurn();

                    if (keyStroke.KeyCode != TCODKeyCode.NoKey)
                        ((PlayerAI)game.Player.Intellect).EvaluateKeyPress(keyStroke);
                }

                RenderAllConsoles(game, rootConsole, playConsole, fogOfWarColour, playerConsole,
                                  competitorConsole, eventsConsole, playBounds, playerBounds,
                                  competitorBounds, eventBounds);

                if (!game.IsActive)
                {
                    rootConsole.printEx((screenBounds.Width - 30)/2, (screenBounds.Height - 10)/2, TCODBackgroundFlag.Set,
                                              TCODAlignment.LeftAlignment, "Press SPACE to start a new game. Press ESC to quit.");
                    if (keyStroke.KeyCode == TCODKeyCode.Space)
                    {
                        rootConsole.print(1, 1, "Creating new game...");
                        TCODConsole.flush();
                        game = CreateGame();
                    }
                }

                TCODConsole.flush();

                if (keyStroke.KeyCode == TCODKeyCode.Escape)
                    return;
            } while (!TCODConsole.isWindowClosed());
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                Logger.Error("An exception occurred", exception);
            }
        }

        private static void RenderAllConsoles(Game game, TCODConsole rootConsole, TCODConsole playConsole, TCODColor fogOfWarColour,
                                              TCODConsole playerConsole, TCODConsole competitorConsole,
                                              TCODConsole eventsConsole, Rectangle playBounds, Rectangle playerBounds,
                                              Rectangle competitorBounds, Rectangle eventBounds)
        {
            rootConsole.clear();
            rootConsole.setForegroundColor(ColorPresets.White);
            rootConsole.setBackgroundColor(ColorPresets.Black);

            RenderPlayConsole(game, playConsole, fogOfWarColour, playBounds);
            RenderPlayerConsole(game.Player, playerConsole, playerBounds);
            //RenderThreatConsole(game.Player, game.Actors, threatConsole, threatBounds);
            RenderCompetitorConsole(game, competitorConsole, competitorBounds);
            RenderEventsConsole(game, eventsConsole, eventBounds);

            TCODConsole.blit(playConsole, 0, 0, playBounds.Width, playBounds.Height, rootConsole, playBounds.X, playBounds.Y);
            TCODConsole.blit(playerConsole, 0, 0, playerBounds.Width, playerBounds.Height, rootConsole, playerBounds.X, playerBounds.Y);
            TCODConsole.blit(competitorConsole, 0, 0, competitorBounds.Width, competitorBounds.Height, rootConsole, competitorBounds.X, competitorBounds.Y);
            TCODConsole.blit(eventsConsole, 0, 0, eventBounds.Width, eventBounds.Height, rootConsole, eventBounds.X, eventBounds.Y);

            //playConsole.Blit(0, 0, playBounds.Width, playBounds.Height, rootConsole, playBounds.X, playBounds.Y);
            //playerConsole.Blit(0, 0, playerBounds.Width, playerBounds.Height, rootConsole, playerBounds.X,
            //                   playerBounds.Y);
           
            //competitorConsole.Blit(0, 0, competitorBounds.Width, competitorBounds.Height, rootConsole,
            //                       competitorBounds.X, competitorBounds.Y);
            //eventsConsole.Blit(0, 0, eventBounds.Width, eventBounds.Height, rootConsole, eventBounds.X, eventBounds.Y);
        }

        private static void RenderEventsConsole(Game gameInstance, TCODConsole console, Rectangle bounds)
        {
            console.clear();
            console.setForegroundColor(ColorPresets.White);
            console.printFrame(0, 0, bounds.Width, bounds.Height, true, TCODBackgroundFlag.Set, "Events");

            int j = gameInstance.GameEvents.Count - 1;
            int i = 1;

            if (!gameInstance.IsActive)
            {
                console.setForegroundColor(gameInstance.AllMonstersAreDead() ? ColorPresets.Gold : ColorPresets.Red);
                console.printEx(
                    1,
                    i, TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, gameInstance.AllMonstersAreDead() ? "You are the last man standing!" : "You have been eliminated");
                i++;
            }

            for (; i < bounds.Height; i++)
            {
                console.setForegroundColor(ColorPresets.White);

                if ((j < 0) || (j >= gameInstance.GameEvents.Count)) break;

                while ((j >= 0) &&
                       !(gameInstance.GameEvents[j].Command is AttackCommand ||
                         gameInstance.GameEvents[j].Command is OpenDoorCommand))
                    j--;

                if ((j < 0) || (j >= gameInstance.GameEvents.Count)) break;
                console.printEx(1, i, TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, gameInstance.GameEvents[j].Result.Message);
                j--;
            }
        }

        private static void RenderCompetitorConsole(Game gameInstance, TCODConsole console, Rectangle bounds)
        {
            console.clear();
            console.setForegroundColor(ColorPresets.White);
            console.printFrame(0, 0, bounds.Width, bounds.Height, true, TCODBackgroundFlag.Set, "Competitors");


            List<IActor> competitors = gameInstance.AllActors.Where(x => x != gameInstance.Player).ToList();

            for (int i = 0; i < competitors.Count; i++)
            {
                console.setForegroundColor(competitors[i].IsAlive ? ColorPresets.White : ColorPresets.Red);
                console.printEx(1, (i * 2) + 1, TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, string.Format("{0} is {1} with {2} kills", competitors[i], competitors[i].IsAlive ? "Alive" : "Dead",
                                  competitors[i].Kills.Count));
            }
        }

        private static void RenderPlayerConsole(IActor player, TCODConsole console, Rectangle bounds)
        {
            console.clear();
            console.setForegroundColor(ColorPresets.White);
            console.printFrame(0, 0, bounds.Width, bounds.Height, true, TCODBackgroundFlag.Set, "Player");


            console.printEx(1, 2, TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, string.Format("Health : {0}", player.Health));
            console.printEx(1, 4, TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, string.Format("Damage : {0}", player.Damage));
            console.printEx(1, 6, TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, string.Format("Kills : {0}", player.Kills.Count));
        }

        private static void RenderThreatConsole(IActor player, IList<IActor> monsters, TCODConsole console, Rectangle bounds)
        {
            console.clear();
            console.setForegroundColor(ColorPresets.White);
            console.printFrame(0, 0, bounds.Width, bounds.Height, true, TCODBackgroundFlag.Set, "Threats");

            List<IActor> threats = player.Intellect.IdentifyThreats(monsters).ToList();

            for (int i = 1; i < threats.Count; i++)
                console.printEx(1, (i * 2) + 1, TCODBackgroundFlag.Set, TCODAlignment.LeftAlignment, string.Format("{0} : {1}", threats[i].Race.Symbol, threats[i]));
        }

        private static void RenderPlayConsole(Game gameInstance, TCODConsole console, TCODColor fogOfWarColour, Rectangle bounds)
        {
            console.clear();
            console.setForegroundColor(ColorPresets.White);
            console.printFrame(0, 0, bounds.Width, bounds.Height, true);


            for (int y = 0; y < gameInstance.Terrain.Height; y++)
                for (int x = 0; x < gameInstance.Terrain.Width; x++)
                    if (gameInstance.Player.VisibilityMap[x, y].WasSeen)
                    {
                        TCODColor lightColour = new TCODColor(gameInstance.LightMap[x, y].Colour.R,
                                                          gameInstance.LightMap[x, y].Colour.G,
                                                          gameInstance.LightMap[x, y].Colour.B);

                        if (lightColour.getValue() < fogOfWarColour.getValue()) lightColour = fogOfWarColour;

                        console.setForegroundColor(gameInstance.Player.VisibilityMap[x, y].IsVisible
                                                      ? lightColour
                                                      : fogOfWarColour);
                        console.putChar(x + 1, y + 1, gameInstance.Terrain[x, y].Symbol);
                    }

            console.setForegroundColor(
                new TCODColor(gameInstance.LightMap[gameInstance.Player.Location.Coordinate].Colour.R,
                              gameInstance.LightMap[gameInstance.Player.Location.Coordinate].Colour.G,
                              gameInstance.LightMap[gameInstance.Player.Location.Coordinate].Colour.B));
            console.putChar(gameInstance.Player.Location.Coordinate.X + 1, gameInstance.Player.Location.Coordinate.Y + 1,
                            '@');

            foreach (IActor actor in gameInstance.Actors.Where(x => x != gameInstance.Player))
            {
                if (gameInstance.Player.VisibilityMap[actor.Location.Coordinate].IsVisible)
                {
                    TCODColor lightColour = new TCODColor(gameInstance.LightMap[actor.Location.Coordinate].Colour.R,
                                                      gameInstance.LightMap[actor.Location.Coordinate].Colour.G,
                                                      gameInstance.LightMap[actor.Location.Coordinate].Colour.B);

                    if (lightColour.getValue() < fogOfWarColour.getValue()) lightColour = fogOfWarColour;

                    console.setForegroundColor(lightColour);
                    console.putChar(actor.Location.Coordinate.X + 1, actor.Location.Coordinate.Y + 1, actor.Race.Symbol);
                }
            }
        }
    }
}
