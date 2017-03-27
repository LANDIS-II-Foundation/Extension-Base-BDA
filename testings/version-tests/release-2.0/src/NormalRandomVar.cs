//  Copyright 2005, University of Wisconsin
//  Author: James Domingo, UW-Madison, Forest Landscape Ecology Lab

using System;

namespace Landis.BDA
{
    /// <summary>
    /// Random number generator with a normal distribution.
    /// </summary>
    /// <remarks>
    /// Uses outward Cartesian form of the Box-Muller method [1958] as
    /// described in "Anisotropic density & yet another Box Muller",
    /// Marijke van Gans, 15 Dec 2004, available on-line at:
    ///
    ///     http://web.mat.bham.ac.uk/marijke/bm/yabm.html
    ///
    /// Box, G. E. P., Muller, M. E., 1958.  Notes on the generation of random
    /// normal deviates.  Annals Math. Stat. 29 pp 610-11.
    ///
    /// All the instances of this class share the uniform random number
    /// generator in the Landis.Util.Random class.
    /// </remarks>
    public class NormalRandomVar
    {
        private const double EPS = 6.0E-8;
        private bool valueStored;
        private double store;

        private double mean;
        private double stdDev;

        //--------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with a specified mean and standard
        /// deviation.
        /// </summary>
        public NormalRandomVar(double mean,
                               double stdDev)
        {
            this.mean = mean;
            this.stdDev = stdDev;
            this.valueStored = false;
        }

        //--------------------------------------------------------------------

        /// <summary>
        /// Generates a number.
        /// </summary>
        public double GenerateNumber()
        {
            double x;
            double y;
            double r2;
            double stretch;

            if (valueStored) {
                valueStored = false;
                return mean + store * stdDev;
            }

            do {
                x = 2 * Util.Random.GenerateUniform() - (1-EPS);
                y = 2 * Util.Random.GenerateUniform() - (1-EPS);
                r2 = x*x + y*y;
            } while( r2 >= 1.0);
            // From http://web.mat.bham.ac.uk/marijke/bm/yabm.html:
            // stretch = sqrt(-2 * log(1 - v) / v);
            // WAS: stretch = Math.Sqrt( -Math.Log(1 - r2) / r2 );
            stretch = Math.Sqrt(-2 * Math.Log(1 - r2) / r2);
            x *= stretch;
            y *= stretch;
            store = x+y;
            valueStored = true;
            return mean + (x-y) * stdDev;
        }
    }
}
