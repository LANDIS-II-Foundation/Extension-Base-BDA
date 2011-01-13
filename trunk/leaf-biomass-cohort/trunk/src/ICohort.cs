
using Landis.SpatialModeling;
using Landis.Cohorts;

namespace Landis.Library.LeafBiomassCohorts
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public interface ICohort
        : Library.AgeOnlyCohorts.ICohort
    {
        /// <summary>
        /// The cohort's biomass (g / m^2).
        /// </summary>
        float WoodBiomass
        {
            get;
        }

        /// <summary>
        /// The cohort's leaf biomass (g / m^2).
        /// </summary>
        float LeafBiomass
        {
            get;
        }
        
        
        int Biomass {get;}
        //---------------------------------------------------------------------

        
    }
}
