using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain.Movement
{
    public enum DirectionsAvailableType
    {
        AllCompassDirections,
        MainCompassDirections
    }

    public class DirectionPicker
    {
        #region Fields

        private readonly int changeDirectionModifier;
        private readonly List<Direction> directionsAvailable;
        private readonly Direction previousDirection;
        private bool directionWasPicked;

        #endregion

        #region Constructors

        public DirectionPicker(Direction previousDirection, int changeDirectionModifier,
                               DirectionsAvailableType directionsAvailableType)
            : this(previousDirection, changeDirectionModifier)
        {
            switch (directionsAvailableType)
            {
                case DirectionsAvailableType.AllCompassDirections:
                    directionsAvailable = new List<Direction>
                                              {
                                                  Direction.North,
                                                  Direction.Northeast,
                                                  Direction.East,
                                                  Direction.Southeast,
                                                  Direction.South,
                                                  Direction.Southwest,
                                                  Direction.West,
                                                  Direction.Northwest
                                              };
                    break;
                case DirectionsAvailableType.MainCompassDirections:
                    directionsAvailable = new List<Direction> { Direction.North, Direction.East, Direction.South, Direction.West };
                    break;
                default:
                    throw new InvalidOperationException("Invalid DirectionsAvailableType passed");
            }
        }

        public DirectionPicker(Direction previousDirection, int changeDirectionModifier,
                               params Direction[] directionsAvailable)
            : this(previousDirection, changeDirectionModifier)
        {
            this.directionsAvailable = new List<Direction>(directionsAvailable);
        }

        public DirectionPicker(Direction previousDirection, int changeDirectionModifier)
        {
            directionsAvailable = new List<Direction> { Direction.North, Direction.East, Direction.South, Direction.West };
            this.previousDirection = previousDirection;
            this.changeDirectionModifier = changeDirectionModifier;
        }

        #endregion

        #region Properties

        public bool HasDirectionToPick
        {
            get { return directionsAvailable.Count > 0; }
        }

        private bool MustChangeDirection
        {
            get
            {
                // changeDirectionModifier of 100 will always change direction
                // value of 0 will never change direction
                return ((directionWasPicked) || (changeDirectionModifier > Rng.Next(0, 99)));
            }
        }

        #endregion

        #region Methods

        private Direction PickDifferentRandomDirection()
        {
            Direction directionPicked;
            do
            {
                directionPicked = directionsAvailable[Rng.Next(directionsAvailable.Count - 1)];
            } while ((directionPicked == previousDirection) && (directionsAvailable.Count > 1));

            return directionPicked;
        }

        public Direction PickRandomDirection()
        {
            if (!HasDirectionToPick) throw new InvalidOperationException("No directions available");

            Direction directionPicked;

            do
            {
                directionPicked = MustChangeDirection ? PickDifferentRandomDirection() : previousDirection;
            } while (!directionsAvailable.Contains(directionPicked));

            directionsAvailable.Remove(directionPicked);

            directionWasPicked = true;
            return directionPicked;
        }

        #endregion
    }
}
