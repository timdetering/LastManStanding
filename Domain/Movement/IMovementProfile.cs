using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LastManStanding.Domain.Commands;
using LastManStanding.Domain.Materials;
using LastManStanding.Domain.Races;
using LastManStanding.Domain.Terrain.TerrainTypes;

namespace LastManStanding.Domain.Movement
{
    /// <summary>
    /// The movement behaviour interface defines a set of functionality used by path finding and other movement based operations.
    /// </summary>
    public interface IMovementProfile
    {
        // TODO: Not sure about this either
    //    Race Race { get; set; }

        /// <summary>
        /// This property describes the directions that are available for use by the movement behaviour
        /// </summary>
        Direction[] AvailableDirections { get; }

        /// <summary>
        /// This method evaluates the terrain based on its properties and determines if the
        /// movement behaviour can traverse this type of terrain.
        /// </summary>
        /// <param name="terrainComposition">The properties of the terrain to evaluate</param>
        /// <returns>Returns true if the terrain is traversable</returns>
        bool TerrainIsTraversable(ITerrain terrain);

        bool TerrainBlocksMovement(ITerrain terrain);

        bool MaterialIsTraversable(IMaterial material);

        /// <summary>
        /// This method calculates the movement cost for the given terrain properties. This movement cost is typically used by pathfinding
        /// algorithms such as A*
        /// </summary>
        /// <param name="terrainComposition">The properties of the terrain to evaulate</param>
        /// <returns>Returns the movement cost for the terrain</returns>
        decimal CalculateTerrainMovementCost(ITerrain terrain);

        decimal CalculateMaterialMovementCost(IMaterial material);

        // TODO: Should not be here
        ///// <summary>
        ///// Evaluates the GameObject and returns the default action to perform when a collision occurs with the game object
        ///// </summary>
        ///// <param name="gameObject">The GameObject to evaluate</param>
        ///// <returns>Returns the default command object</returns>
        //ICommand GetDefaultBumpAction(IGameObject gameObject);
    }
}
