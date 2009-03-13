using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Lighting;
using LastManStanding.Domain.Movement;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Terrain
{
    [Serializable]
    public class TerrainMap : Map<ITerrain>
    {
        public TerrainMap()
        {
        }

        public TerrainMap(int width, int height, Func<ITerrain> cellFactory)
            : base(width, height, cellFactory)
        {
            
        }

        public TerrainMap(Size size, Func<ITerrain> cellFactory)
            : base(size, cellFactory)
        {
        }

        public TerrainMap(Rectangle rectangle, Func<ITerrain> cellFactory)
            : base(rectangle, cellFactory)
        {
        }

        private IList<LightSource> lightSources = new List<LightSource>();
        public IList<LightSource> LightSources
        {
            get { return lightSources; }
            set { lightSources = value; }
        }

        private List<DungeonPrefab> prefabs = new List<DungeonPrefab>();

        public List<DungeonPrefab> Prefabs
        {
            get { return prefabs; }
            set { prefabs = value; }
        }

        public DungeonPrefab GetPrefabAtLocation(Point location)
        {
            foreach (var prefab in Prefabs)
                if (prefab.Bounds.Contains(location)) return prefab;

            return null;
        }

        public IEnumerable<Point> WalkableLocations(IMovementProfile movementProfile)
        {
            return Locations.Where(x => movementProfile.TerrainIsTraversable(this[x]));
        }
    }
}