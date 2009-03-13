using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Lighting;
using LastManStanding.Domain.Terrain;

namespace LastManStanding.Domain.FieldOfView
{
    public class VisibilityMap : Map<VisibilityObject>
    {
        private readonly IFovProfile fovProfile;
        private FovResultset visiblePoints;
        private readonly IFovAlgorithm fovAlgorithm;

        public VisibilityMap(int width, int height, IFovProfile fovProfile, IFovAlgorithm fovAlgorithm)
            : base(new Size(width, height), () => new VisibilityObject())
        {
            this.fovProfile = fovProfile;
            this.fovAlgorithm = fovAlgorithm;
        }

        public void UpdateVisibilityMap(TerrainMap terrain, LightMap lightmap, Point origin)
        {
            // Reset the visible flag of the previous visible points
            if (visiblePoints != null)
                foreach (var visiblePoint in visiblePoints)
                    this[visiblePoint.Location.Coordinate].IsVisible = false;

            // Calculate new visible Fov
            visiblePoints = fovAlgorithm.CalculateFov(terrain, origin, fovProfile);

            // Evaluate each visible point against the lightmap
            // Only lit tiles are visible
            foreach (var visiblePoint in visiblePoints)
            {
                if (!fovProfile.IsVisible(lightmap[visiblePoint.Location.Coordinate].Colour)) continue;

                this[visiblePoint.Location.Coordinate].IsVisible = true;
                this[visiblePoint.Location.Coordinate].WasSeen = true;
            }
        }

        public void ResetSeenLocations()
        {
            foreach (var location in Locations)
                this[location].WasSeen = false;
        }

        public IEnumerable<Point> GetVisibleLocations()
        {
            return from location in Locations where this[location].IsVisible select location;
        }

        public IEnumerable<Point> GetSeenLocations()
        {
            return from location in Locations where this[location].WasSeen select location;
        }

        public IEnumerable<Point> GetUnseenLocations()
        {
            return from location in Locations where !this[location].WasSeen select location;
        }

    }
}
