using Edu.Wisc.Forest.Flel.Util;
using Landis.SpatialModeling;

namespace Landis.Library.LeafBiomassCohorts
{
    public class CohortDefoliation
    {
        /// <summary>
        /// Various delegates associated with defoliation.
        /// </summary>
        public static class Delegates
        {
            /// <summary>
            /// A method to compute how much a cohort is defoliated at a
            /// site.
            /// </summary>
            public delegate double Compute(ICohort    cohort,
                                           ActiveSite site,
                                           int        siteBiomass);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Defaults for defoliation.
        /// </summary>
        public static class Defaults
        {
            /// <summary>
            /// Default method for computing how much a cohort is defoliated at
            /// a site.
            /// </summary>
            /// <returns>
            /// 0%
            /// </returns>
            public static double Compute(ICohort    cohort,
                                         ActiveSite site,
                                         int        siteBiomass)
            {
                return 0.0;
            }
        }

        //---------------------------------------------------------------------

        private static Delegates.Compute computeMethod = Defaults.Compute;

        //---------------------------------------------------------------------

        /// <summary>
        /// The method to compute how much a cohort is defoliated at a site.
        /// </summary>
        public static Delegates.Compute Compute
        {
            get {
                return computeMethod;
            }

            set {
                Require.ArgumentNotNull(value);
                computeMethod = value;
            }
        }
    }
}
