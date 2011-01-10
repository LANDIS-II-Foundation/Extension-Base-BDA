//  Copyright 2005 University of Wisconsin
//  Authors:
//      Robert M. Scheller
//      James B. Domingo
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.
//  Version 1.0
//  License:  Available at
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.BDA
{
    /// <summary>
    /// Extra Ecoregion Paramaters
    /// </summary>
    public interface IEcoParameters
    {
        double EcoModifier{get;set;}
    }
}


namespace Landis.BDA
{
    public class EcoParameters
        : IEcoParameters
    {
        private double ecoModifier;

        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        public double EcoModifier{
            get{
                return ecoModifier;
            }
            set {
                if (value < -1 || value > 1)
                        throw new InputValueException(value.ToString(),
                            "Value must be >= -1.0 and <= 1.0.");
                ecoModifier = value;
            }
        }

        //---------------------------------------------------------------------
        public EcoParameters()
        {
        }
/*        //---------------------------------------------------------------------
        public EcoParameters(double ecoModifier)
        {
            this.ecoModifier = ecoModifier;
        }

        //---------------------------------------------------------------------
        public EcoParameters()
        {
            this.ecoModifier        = 0.0;
        }*/
    }
}
