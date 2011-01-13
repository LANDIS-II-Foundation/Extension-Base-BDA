

using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Library.LeafBiomassCohorts
{
    /// <summary>
    /// Information about a cohort's death.
    /// </summary>
    public class DeathEventArgs
        // : Landis.Cohorts.DeathEventArgs<ICohort, ExtensionType>
    {
        /*
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public DeathEventArgs(ICohort    cohort,
                              ActiveSite site,
                              ExtensionType disturbanceType)
            // :base(cohort, site, disturbanceType)
        {
        }
        */

        private ICohort cohort;
        private ActiveSite site;
        private ExtensionType disturbanceType;

        //---------------------------------------------------------------------

        /// <summary>
        /// The cohort that died.
        /// </summary>
        public ICohort Cohort
        {
            get
            {
                return cohort;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The site where the cohort died.
        /// </summary>
        public ActiveSite Site
        {
            get
            {
                return site;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The type of disturbance that killed the cohort.
        /// </summary>
        /// <remarks>
        /// null if the cohort died during the growth phase of succession.
        /// </remarks>
        public ExtensionType DisturbanceType
        {
            get
            {
                return disturbanceType;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public DeathEventArgs(ICohort cohort,
                              ActiveSite site,
                              ExtensionType disturbanceType)
        {
            this.cohort = cohort;
            this.site = site;
            this.disturbanceType = disturbanceType;
        }
    }
}
