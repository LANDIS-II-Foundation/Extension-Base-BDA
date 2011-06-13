using Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;
using Landis.Core;

using System.Collections.Generic;

namespace Landis.Extension.DroughtDisturbance
{
    /// <summary>
    /// A biomass disturbance that handles partial thinning of cohorts.
    /// </summary>
    public class PartialDisturbance
        : IDisturbance
    {
        private static PartialDisturbance singleton;
        private static IDictionary<ushort, int>[] reductions;
        private static ActiveSite currentSite;

        //---------------------------------------------------------------------
        ActiveSite Landis.Library.BiomassCohorts.IDisturbance.CurrentSite
        {
            get
            {
                return currentSite;
            }
        }

        //---------------------------------------------------------------------
        ExtensionType IDisturbance.Type
        {
            get
            {
                return PlugIn.Type;
            }
        }

        //---------------------------------------------------------------------
        static PartialDisturbance()
        {
            singleton = new PartialDisturbance();
        }

        //---------------------------------------------------------------------
        public PartialDisturbance()
        {
        }

        //---------------------------------------------------------------------
        int IDisturbance.ReduceOrKillMarkedCohort(ICohort cohort)
        {
            int reduction;
            if (reductions[cohort.Species.Index].TryGetValue(cohort.Age, out reduction))
            {

                //SiteVars.BiomassRemoved[currentSite] += reduction;
                //SiteVars.CohortsPartiallyDamaged[currentSite]++;

                return reduction;
            }
            else
            return 0;
        }
        //---------------------------------------------------------------------

        public static void Initialize()
        {
            reductions = new IDictionary<ushort, int>[PlugIn.ModelCore.Species.Count];
            for (int i = 0; i < reductions.Length; i++)
                reductions[i] = new Dictionary<ushort, int>();
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Reduces the biomass of cohorts that have been marked for partial
        /// reduction.
        /// </summary>
        public static void ReduceCohortBiomass(ActiveSite site)
        {
            currentSite = site;

            //PlugIn.ModelCore.Log.WriteLine("ReducingCohortBiomass NOW!");

            SiteVars.Cohorts[site].ReduceOrKillBiomassCohorts(singleton);

        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Records the biomass reduction for a particular cohort.
        /// </summary>
        public static void RecordBiomassReduction(ICohort cohort,
                                                  int reduction)
        {
            //PlugIn.ModelCore.Log.WriteLine("Recording reduction:  {0:0.0}/{1:0.0}/{2}.", cohort.Species.Name, cohort.Age, reduction);
            reductions[cohort.Species.Index][cohort.Age] = reduction;
        }
    }
}
