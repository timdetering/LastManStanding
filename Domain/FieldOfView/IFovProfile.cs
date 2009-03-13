using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Materials;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.FieldOfView
{
    public interface IFovProfile
    {
        /// <summary>
        /// The shape of the FovProfile
        /// </summary>
        FovShapeType FovShape { get;  }

        /// <summary>
        /// The radius the Fov extends beyond the actor
        /// </summary>
        int FovRadius { get; set; }

        /// <summary>
        /// This method determines if the composition types provided block the line of sight
        /// </summary>
        /// <param name="compositionTypes">The composition types to evaluate</param>
        /// <returns>Returns true if the line of sight is blocked</returns>
        bool LosIsBlockedByMaterial(IMaterial material);

        bool LosIsBlockedByTerrain(ITerrain terrain);

        /// <summary>
        /// This method determines if something is visible based on a light value
        /// </summary>
        /// <param name="lightValue">The light value to evaulate</param>
        /// <returns>Returns true if there is enough light</returns>
        bool IsVisible(Color lightValue);

    }
}
