//  Authors:  Robert M. Scheller

using Landis.Utilities;

namespace Landis.Extension.BiomassBDA
{
    /// <summary>
    /// Extra Ecoregion Paramaters
    /// </summary>
    public interface IEcoParameters
    {
        double EcoModifier{get;set;}
    }
}


namespace Landis.Extension.BiomassBDA
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
