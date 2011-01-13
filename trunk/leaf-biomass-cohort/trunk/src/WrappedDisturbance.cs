
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.AgeOnlyCohorts;

namespace Landis.Library.LeafBiomassCohorts
{
    /// <summary>
    /// A wrapped age-cohort disturbance so it works with biomass cohorts.
    /// </summary>
    public class WrappedDisturbance
        : IDisturbance
    {
        private AgeOnlyCohorts.ICohortDisturbance ageCohortDisturbance;

        //---------------------------------------------------------------------

        public WrappedDisturbance(AgeOnlyCohorts.ICohortDisturbance ageCohortDisturbance)
        {
            this.ageCohortDisturbance = ageCohortDisturbance;
        }

        //---------------------------------------------------------------------

        public ExtensionType Type
        {
            get {
                return ageCohortDisturbance.Type;
            }
        }

        //---------------------------------------------------------------------

        public ActiveSite CurrentSite
        {
            get {
                return ageCohortDisturbance.CurrentSite;
            }
        }

        //---------------------------------------------------------------------

        public float[] RemoveMarkedCohort(ICohort cohort)
        {
            float[] damage = new float[]{0,0};
            if (ageCohortDisturbance.MarkCohortForDeath(cohort)) {
                Cohort.KilledByAgeOnlyDisturbance(this, cohort,
                                                  ageCohortDisturbance.CurrentSite,
                                                  ageCohortDisturbance.Type);
                
                damage[0] = cohort.WoodBiomass;
                damage[1] = cohort.LeafBiomass;
                return damage; 
            }
            
            //else
            return damage;
        }
    }
}
