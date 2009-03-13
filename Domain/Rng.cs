using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LastManStanding.Domain
{
    public class Rng
    {
        #region Singleton

        private Rng()
        {
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit

            internal static readonly Rng instance = new Rng();
        }

        #endregion

        #region Fields

        private readonly MersenneTwister mersenneTwister = new MersenneTwister();

        #endregion

        #region Methods

        public static int Next(int maxValue)
        {
            return Nested.instance.mersenneTwister.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return Nested.instance.mersenneTwister.Next(minValue, maxValue);
        }

        #endregion
    }
}
