using System.Collections.Generic;
using System.Linq;
using LastManStanding.Domain.Actors;
using LastManStanding.Domain.FieldOfView;
using LastManStanding.Domain.Lighting;
using LastManStanding.Domain.Terrain;

namespace LastManStanding.Domain
{
    public class Game
    {
        public List<GameEvent> GameEvents
        {
            get { return gameEvents; }
        }

        private readonly List<GameEvent> gameEvents = new List<GameEvent>();
        private IActor player;
        private GameTurn currentTurn;
        private LightMap lightMap;

        public LightMap LightMap
        {
            get { return lightMap; }
        }

        public IActor Player
        {
            get { return player; }
        }

        public Game()
        {
            IsActive = true;
        }

        public void AddActor(IActor actor, bool isPlayer)
        {
            if (!actors.Contains(actor))
            {
                actors.Add(actor);
                actor.GameInstance = this;
                if (terrain != null)
                {
                    actor.VisibilityMap = new VisibilityMap(terrain.Width, terrain.Height, actor.Race.FovProfile,
                                                            new ShadowCastingFov());
                    actor.VisibilityMap.UpdateVisibilityMap(terrain, lightMap, actor.Location.Coordinate);
                }
                if (isPlayer) player = actor;
            }
        }

        public void AddActor(IActor actor)
        {
            AddActor(actor, false);
        }

        public void RemoveActor(IActor actor)
        {
            if (actors.Contains(actor))
            {
                actors.Remove(actor);
                actor.GameInstance = null;
                actor.VisibilityMap = null;
            }
        }

        private TerrainMap terrain;

        public TerrainMap Terrain
        {
            get { return terrain; }
            set
            {
                terrain = value;
                lightMap = new LightMap(new ShadowCastingFov(), terrain);

                foreach (IActor actor in actors)
                    actor.VisibilityMap = new VisibilityMap(terrain.Width, terrain.Height, actor.Race.FovProfile,
                                                            new ShadowCastingFov());
            }
        }

        public void ProcessTurn()
        {
            if (AllMonstersAreDead()) IsActive = false;

            if (ThePlayerIsDead()) IsActive = false;

            if (IsActive)
            {
                if ((currentTurn == null) || (currentTurn.IsFinished))
                {
                    currentTurn = new GameTurn(Actors);
                    lightMap.UpdateLightMap((from a in Actors where a.LightSource != null select a.LightSource).ToList());
                }

                gameEvents.AddRange(currentTurn.ProcessTurn());
            }
        }

        public bool AllMonstersAreDead()
        {
            if (actors.Any(x => (x != player) && (x.IsAlive))) return false;

            return true;
        }

        public bool ThePlayerIsDead()
        {
            return !player.IsAlive;
        }

        public bool IsActive { get; private set; }

        private readonly IList<IActor> actors = new List<IActor>();

        public IList<IActor> Actors
        {
            get { return actors.Where(x => x.IsAlive).ToList(); }
        }

        public IList<IActor> AllActors
        {
            get { return actors; }
        }
    }
}
