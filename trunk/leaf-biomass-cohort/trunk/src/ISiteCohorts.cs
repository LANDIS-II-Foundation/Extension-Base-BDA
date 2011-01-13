using Landis.Core;
using Landis.Cohorts;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Library.LeafBiomassCohorts
{
    /// <summary>
    /// All the biomass cohorts at a site.
    /// </summary>
    public interface ISiteCohorts
        : Landis.Cohorts.ISiteCohorts<ISpeciesCohorts>
    {
        void AddNewCohort(ISpecies species, float initialWood, float initialLeaf);

        void Grow(ActiveSite site, bool isSuccessionTimestep, bool annualTimestep);

        /*int TotalBiomass
        {
            get;
        }*/

        /*int PrevYearMortality
        {
            get;
        }*/
        /// <summary>
        /// The total biomass of all the cohorts.
        /// </summary>
        /*float TotalBiomass
        {
            get;
        }*/

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes who much a disturbance damages the cohorts by reducing
        /// their biomass.
        /// </summary>
        /// <returns>
        /// The total of all the cohorts' biomass reductions.
        /// </returns>
        //int RemoveCohorts(IDisturbance disturbance);
    }
}
