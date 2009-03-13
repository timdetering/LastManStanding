using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using libtcodWrapper;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Commands;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.AI
{
    public class PlayerAI : IIntellect
    {
        private readonly CommandFactory commandFactory;

        public PlayerAI(CommandFactory commandFactory)
        {
            this.commandFactory = commandFactory;
        }

        public IEnumerable<IActor> IdentifyThreats(IEnumerable<IActor> actors)
        {
            return (from a in actors
                       where IsThreat(a)
                       select a).Distinct();
        }

        protected virtual bool IsThreat(IActor potentialThreat)
        {
            return potentialThreat.IsAlive && (Host != potentialThreat) && (Host.VisibilityMap[potentialThreat.Location.Coordinate].IsVisible);
        }

        public IActor Host { get; set; }

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

        private readonly Queue<ICommand> commandBuffer = new Queue<ICommand>();
        private readonly Queue<KeyPress> keyBuffer = new Queue<KeyPress>();

        private void AddKeyPressToBuffer(KeyPress keyPress)
        {
            switch (keyPress.KeyCode)
            {
                case KeyCode.TCODK_UP:
                case KeyCode.TCODK_LEFT:
                case KeyCode.TCODK_RIGHT:
                case KeyCode.TCODK_DOWN:
                case KeyCode.TCODK_ENTER:
                case KeyCode.TCODK_KP1:
                case KeyCode.TCODK_KP2:
                case KeyCode.TCODK_KP3:
                case KeyCode.TCODK_KP4:
                case KeyCode.TCODK_KP5:
                case KeyCode.TCODK_KP6:
                case KeyCode.TCODK_KP7:
                case KeyCode.TCODK_KP8:
                case KeyCode.TCODK_KP9:
                    keyBuffer.Enqueue(keyPress);
                    break;
                case KeyCode.TCODK_CHAR:

                    switch (keyPress.Character)
                    {
                        case 101:
                        case 113:
                        case 119:
                        case 100:
                        case 97:
                        case 115:
                        case 99:
                        case 120:
                        case 122:
                            keyBuffer.Enqueue(keyPress);
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }
        }

        public void EvaluateKeyPress(KeyPress keyPress)
        {
            AddKeyPressToBuffer(keyPress);
            ProcessKeyPressBuffer();
        }

        private void ProcessKeyPressBuffer()
        {
            if (keyBuffer.Count == 0) return;

            var firstKeyPress = keyBuffer.Peek();
            switch(firstKeyPress.KeyCode)
            {
                case KeyCode.TCODK_KP9:
                    commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.Northeast));
                    keyBuffer.Dequeue();
                    break;
                case KeyCode.TCODK_KP7:
                    commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.Northwest));
                    keyBuffer.Dequeue();
                    break;
                case KeyCode.TCODK_UP:
                case KeyCode.TCODK_KP8:
                    commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.North));
                    keyBuffer.Dequeue();
                    break;
                case KeyCode.TCODK_LEFT:
                case KeyCode.TCODK_KP4:
                    commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.West));
                    keyBuffer.Dequeue();
                    break;
                case KeyCode.TCODK_RIGHT:
                case KeyCode.TCODK_KP6:
                    commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.East));
                    keyBuffer.Dequeue();
                    break;
                case KeyCode.TCODK_DOWN:
                case KeyCode.TCODK_KP2:
                    commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.South));
                    keyBuffer.Dequeue();
                    break;
                case KeyCode.TCODK_KP5:
                    commandBuffer.Enqueue(new SkipTurnCommand(Host));
                    keyBuffer.Dequeue();
                    break;
                case KeyCode.TCODK_KP3:
                    commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.Southeast));
                    keyBuffer.Dequeue();
                    break;
                case KeyCode.TCODK_KP1:
                    commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.Southwest));
                    keyBuffer.Dequeue();
                    break;
                case KeyCode.TCODK_ENTER:
                    if (keyBuffer.Count == 1) return;

                    // Remove the first keypress
                    keyBuffer.Dequeue();

                    Direction direction = Direction.Empty;
                    var secondKeyPress = keyBuffer.Dequeue();
                    switch (secondKeyPress.KeyCode)
                    {
                        case KeyCode.TCODK_KP7:
                            direction = Direction.Northwest;
                            break;
                        case KeyCode.TCODK_KP9:
                            direction = Direction.Northeast;
                            break;
                        case KeyCode.TCODK_KP1:
                            direction = Direction.Southwest;
                            break;
                        case KeyCode.TCODK_KP3:
                            direction = Direction.Southeast;
                            break;
                        case KeyCode.TCODK_UP:
                        case KeyCode.TCODK_KP8:
                            direction = Direction.North;
                            break;
                        case KeyCode.TCODK_LEFT:
                        case KeyCode.TCODK_KP4:
                            direction = Direction.West;
                            break;
                        case KeyCode.TCODK_RIGHT:
                        case KeyCode.TCODK_KP6:
                            direction = Direction.East;
                            break;
                        case KeyCode.TCODK_DOWN:
                        case KeyCode.TCODK_KP2:
                            direction = Direction.South;
                            break;
                        case KeyCode.TCODK_CHAR:

                            switch (secondKeyPress.Character)
                            {
                                case 101:
                                    direction = Direction.Northeast;
                                    break;
                                case 113:
                                    direction = Direction.Northwest;
                                    break;
                                case 119:
                                    direction = Direction.North;
                                    break;
                                case 100:
                                    direction = Direction.East;
                                    break;
                                case 97:
                                    direction = Direction.West;
                                    break;
                                case 99:
                                    direction = Direction.Southeast;
                                    break;
                                case 120:
                                    direction = Direction.South;
                                    break;
                                case 122:
                                    direction = Direction.Southwest;
                                    break;

                                default:
                                    break;
                            }

                            break;
                        default:
                            return;
                    }

                    // We assume a melee attack therefor the targetLocation is one tile away

                    // Determine the target using the actors and direction
                    Point targetLocation = direction.ApplyTransform(Host.Location.Coordinate);
                    var target = IdentifyThreats(Host.GameInstance.Actors).FirstOrDefault(x => x.Location.Coordinate == targetLocation);

                    if (target != null) commandBuffer.Enqueue(commandFactory.BuildAttackCommand(Host, target));
                    
                    break;
                case KeyCode.TCODK_CHAR:

                    switch(firstKeyPress.Character)
                    {
                        case 101:
                            commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.Northeast));
                            keyBuffer.Dequeue();
                            break;
                        case 113:
                            commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.Northwest));
                            keyBuffer.Dequeue();
                            break;
                        case 119:
                            commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.North));
                            keyBuffer.Dequeue();
                            break;
                        case 100:
                            commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.East));
                            keyBuffer.Dequeue();
                            break;
                        case 97:
                            commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.West));
                            keyBuffer.Dequeue();
                            break;
                        case 115:
                            commandBuffer.Enqueue(new SkipTurnCommand(Host));
                            keyBuffer.Dequeue();
                            break;
                        case 99:
                            commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.Southeast));
                            keyBuffer.Dequeue();
                            break;
                        case 120:
                            commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.South));
                            keyBuffer.Dequeue();
                            break;
                        case 122:
                            commandBuffer.Enqueue(commandFactory.BuildMoveCommand(Host, Direction.Southwest));
                            keyBuffer.Dequeue();
                            break;

                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }
        }

        public ICommand GetNextAction()
        {
            if (commandBuffer.Count == 0) return new NullCommand();

            return commandBuffer.Dequeue();
        }
    }
}