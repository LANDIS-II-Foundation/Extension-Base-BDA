//  Authors:  Robert M. Scheller

using Landis.Utilities;

namespace Landis.Extension.ClimateBDA
{
    /// <summary>
    /// Extra Ecoregion Paramaters
    /// </summary>
    public interface IEcoParameters
    {
        double EcoModifier{get;set;}
    }
}


namespace Landis.Extension.ClimateBDA
{
    public class EcoParameters
        : IEcoParameters
    {
        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        public double EcoModifier { get; set; }

        //---------------------------------------------------------------------
        public EcoParameters()
        {
        }
    }
}
