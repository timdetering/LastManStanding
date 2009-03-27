using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.FieldOfView;
using LastManStanding.Domain.Terrain;

namespace LastManStanding.Domain.Lighting
{
    [Serializable]
    public class LightMap : Map<LightingObject>
    {
        private readonly IFovAlgorithm fovAlgorithm;
        private readonly TerrainMap terrain;

        public LightMap(IFovAlgorithm fovAlgorithm, TerrainMap terrain)
        {
            this.fovAlgorithm = fovAlgorithm;
            this.terrain = terrain;

            // Resize the lightmap to the dimensions of the dungeon
            Resize(new Size(terrain.Width, terrain.Height), () => new LightingObject());
        }

        public void UpdateLightMap(List<LightSource> dynamicLightSources)
        {
            // Fill the lightmap with black tiles
            FillMap(() => new LightingObject());

            // Process static lightsources
            ProcessLightsources(terrain.LightSources);

            // Process dynamic lightsources
            ProcessLightsources(dynamicLightSources);
        }

        private void ProcessLightsources(IEnumerable<LightSource> lightSources)
        {
            foreach (var lightSource in lightSources)
            {
                var litTiles = fovAlgorithm.CalculateFov(terrain, lightSource.Location.Coordinate, lightSource.FovProfile);

                foreach (var litTile in litTiles)
                {
                    var intensity = (1f /
                                       (lightSource.Attenuation.Constant +
                                        (lightSource.Attenuation.Linear * litTile.DistanceFromOrigin) +
                                        (lightSource.Attenuation.Quadratic * litTile.DistanceFromOrigin *
                                         litTile.DistanceFromOrigin)));

                    this[litTile.Location.Coordinate].Colour = AlphaBlendColours(intensity, this[litTile.Location.Coordinate].Colour, lightSource.Colour);
                    //this[litTile.Location.Coordinate].Colour = FromLibtcodColor(
                    //    libtcodWrapper.Color.Interpolate(ToLibtcodColor(this[litTile.Location.Coordinate].Colour), ToLibtcodColor(lightSource.Colour),
                    //                                     intensity));

                    
                }
            }
        }

        private static libtcodWrapper.Color ToLibtcodColor(Color source)
        {
            return libtcodWrapper.Color.FromRGB(source.R, source.G, source.B);
        }

        private static Color FromLibtcodColor(libtcodWrapper.Color source)
        {
            return Color.FromArgb(1, source.Red, source.Green, source.Blue);
        }

        private static Color AlphaBlendColours(float alpha, Color source, Color destination)
        {
            var red = (destination.R - source.R) * alpha + source.R;
            var green = (destination.G - source.G) * alpha + source.G;
            var blue = (destination.B - source.B) * alpha + source.B;

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }
}
