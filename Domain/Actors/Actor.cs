using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LastManStanding.Domain.AI;
using LastManStanding.Domain.FieldOfView;
using LastManStanding.Domain.Lighting;
using LastManStanding.Domain.Races;
using LastManStanding.Domain.Terrain;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Actors
{
    public class Actor : IActor
    {
        public override string ToString()
        {
            return Name;
        }

        public IList<IActor> Kills
        {
            get { return kills; }
        }

        private readonly IList<IActor> kills = new List<IActor>();

        public void AddKill(IActor victim)
        {
            if (!kills.Contains(victim))
            {
                // TODO: Refactor this behaviour into a passive ability
                AddHealth(victim.MaxHealth);
                Damage += victim.Damage;
                kills.Add(victim);
            }
        }

        public int MaxHealth
        {
            get; set;
        }

        public void AddHealth(int healthToAdd)
        {
            Health += healthToAdd;
            MaxHealth += healthToAdd;
        }

        public string Name { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
        public int Speed { get; set; }

        public bool IsAlive
        {
            get { return Health > 0; }
        }

        public IIntellect Intellect { get; private set; }

        private ILocation location = new Location();

        public ILocation Location
        {
            get { return location; }
            set { location = value; }
        }

        public IRace Race { get; set; }

        public void SetLocation(Point coordinate)
        {
            Location.Coordinate = coordinate;
            if (LightSource != null) LightSource.Location.Coordinate = coordinate;
        }

        public VisibilityMap VisibilityMap { get; set; }

        public void SetIntellect(IIntellect intellect)
        {
            intellect.Host = this;
            Intellect = intellect;
        }

        public Game GameInstance { get; set; }

        public LightSource LightSource { get; set; }

        public Point? GetClosestExploredLocation(Point origin)
        {
            int distance = int.MaxValue;
            Point? closestLocation = null;

            foreach (
                Point visibleLocation in
                    VisibilityMap.GetSeenLocations().Where(
                        x => Race.MovementProfile.TerrainIsTraversable(GameInstance.Terrain[x])))
            {
                int currentDistance = origin.DistanceTo(visibleLocation);
                if (currentDistance >= distance) continue;

                distance = currentDistance;
                closestLocation = visibleLocation;
            }

            return closestLocation;
        }

        public Point? GetClosestUnexploredLocation()
        {
            int closestDistance = int.MaxValue;
            Point? closestLocation = null;

            // We want to explore rooms before we explore corridors
            DungeonPrefab currentRoom = GameInstance.Terrain.GetPrefabAtLocation(Location.Coordinate);

            // If we are in a room then get the closest unseen location in the room
            foreach (
                Point unseenLocation in
                    currentRoom == null
                        ? GetWalkableUnseenLocations()
                        : GetWalkableUnseenLocationsWithinPrefab(currentRoom))
            {
                int currentDistance = Location.DistanceTo(unseenLocation);
                if (currentDistance >= closestDistance) continue;

                closestDistance = currentDistance;
                closestLocation = unseenLocation;
            }

            return closestLocation;
        }

        /// <summary>
        /// Enumerates all the locations within the dungeon prefab and returns the closest unseen location.
        /// Will return the closest unseen location within the whole map if none are found within the prefab
        /// </summary>
        /// <param name="prefab">The dungeon prefab to search</param>
        /// <param name="actor">The actor to base the search on</param>
        /// <returns>Returns the closest unseen location within a prefab or the whole map if none were found</returns>
        private IEnumerable<Point> GetWalkableUnseenLocationsWithinPrefab(Map<ITerrain> prefab)
        {
            IEnumerable<Point> unseenLocationsInPrefab = from location in prefab.Locations
                                                         where
                                                             Race.MovementProfile.TerrainIsTraversable(
                                                                 GameInstance.Terrain[location]) &&
                                                             !VisibilityMap[location].WasSeen
                                                         select location;

            // Return any unseen locations within the prefab else return the closest unseen location on the map
            return unseenLocationsInPrefab.Count() > 0 ? unseenLocationsInPrefab : GetWalkableUnseenLocations();
        }

        /// <summary>
        /// Enumerates all the unseen locations within the dungeon and returns the location closest to the actor.
        /// </summary>
        /// <param name="actor">The actor to evaluate the locations against</param>
        /// <returns>Returns the closest unseen location within the map</returns>
        private IEnumerable<Point> GetWalkableUnseenLocations()
        {
            return from location in VisibilityMap.GetUnseenLocations()
                   where Race.MovementProfile.TerrainIsTraversable(GameInstance.Terrain[location])
                   select location;
        }
    }
}