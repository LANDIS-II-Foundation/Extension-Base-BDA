//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.BaseBDA
{
    /// <summary>
    /// Extra Ecoregion Paramaters
    /// </summary>
    public interface IEcoParameters
    {
        double EcoModifier{get;set;}
    }
}


namespace Landis.Extension.BaseBDA
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
