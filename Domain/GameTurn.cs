using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.Commands;

namespace LastManStanding.Domain
{
    public class GameTurn
    {
        private readonly PriorityQueue<int, IActor> commandQueue = new PriorityQueue<int, IActor>();
        private readonly IList<IActor> actors;
        private readonly IList<GameEvent> completedActions = new List<GameEvent>();
        private int lastTimeSlice = -1;

        public GameTurn(IList<IActor> actors)
        {
            this.actors = actors;
            InitializeCommandQueue();
        }

        public bool IsFinished
        {
            get
            {
                if (commandQueue.Count == 0) return true;

                return ((AllActorsHaveActed(completedActions))
                        && (lastTimeSlice != commandQueue.Peek().Key));
            }
        }

        public IList<GameEvent> ProcessTurn()
        {
            // We process the command queue while there are actors to process
            while (commandQueue.Count > 0)
            {
                // Get the next actor to process
                var nextTimeInterval = commandQueue.Peek();

                // stop processing if everyone has had a turn and we're on a new time slice
                if (IsFinished)
                    break;

                // Update the last time slice to the current time slice
                lastTimeSlice = nextTimeInterval.Key;

                // Remove dead actors from the queue
                if (!nextTimeInterval.Value.IsAlive)
                {
                    commandQueue.Dequeue();
                    continue;
                }

                // Get the next action from the actor
                ICommand nextAction = nextTimeInterval.Value.Intellect.GetNextAction();
                if (nextAction == null) break;

                var nextResult = nextAction.Execute();

                if (nextResult.UsesTurn)
                {
                    foreach (var actor in actors)
                        actor.VisibilityMap.UpdateVisibilityMap(actor.GameInstance.Terrain, actor.GameInstance.LightMap, actor.Location.Coordinate);

                    // Remove the actor from the queue if the action was successful
                    commandQueue.Dequeue();
                    // Add the successful command to the list of completed actions
                    completedActions.Add(new GameEvent
                        {   Actor = nextTimeInterval.Value,
                            Command = nextAction,
                            TimeSlice = nextTimeInterval.Key,
                            Result = nextResult
                        });
                    // Add the actor to the queue incrementing the priority with the speed
                    commandQueue.Enqueue(
                        nextTimeInterval.Key + nextTimeInterval.Value.Speed,
                        nextTimeInterval.Value);

                    if ((!nextResult.Success) && (nextAction is MoveCommand))
                    {
                        // The move action failed
                        // Ask the actor for a default action on bump
                        var defaultBumpAction = nextTimeInterval.Value.Intellect.GetDefaultBumpAction((MoveCommand)nextAction);
                        if (defaultBumpAction != null)
                        {
                            var bumpResult = defaultBumpAction.Execute();
                            completedActions.Add(new GameEvent
                            {
                                Actor = nextTimeInterval.Value,
                                Command = defaultBumpAction,
                                TimeSlice = nextTimeInterval.Key,
                                Result = bumpResult
                            });
                        }
                    }
                }
                else
                {
                    // We stop processing actions for this game loop cycle as 
                    // the last action was did not use a turn
                    break;
                }

            }

            return completedActions;
        }

        private void InitializeCommandQueue()
        {
            // We check if we have any actors in our command queue to process
            if (commandQueue.Count == 0)
                // Add the actors to the command queue using the actor speed as priority
                foreach (var actor in actors)
                {
                    if (actor.IsAlive) commandQueue.Enqueue(actor.Speed, actor);
                }
        }

        private bool AllActorsHaveActed(IEnumerable<GameEvent> completedActions)
        {
            var result = true;
            foreach (var actor in actors)
            {
                var actor1 = actor;
                if ((actor.IsAlive) && (completedActions.Any(x => x.Actor == actor1)) == false)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}
