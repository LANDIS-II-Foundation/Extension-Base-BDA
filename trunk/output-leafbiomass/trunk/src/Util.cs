using Landis.Biomass;

namespace Landis.Output.Biomass
{
    /// <summary>
    /// Methods for computing biomass for groups of cohorts.
    /// </summary>
    public static class Util
    {
        public static double ComputeBiomass(ISpeciesCohorts cohorts)
        {
            double total = 0.0;
            if (cohorts != null)
                foreach (ICohort cohort in cohorts)
                    total += (double) (cohort.LeafBiomass + cohort.WoodBiomass);
            return total;
        }

        //---------------------------------------------------------------------

        public static double ComputeBiomass(ISiteCohorts cohorts)
        {
            double total = 0.0;
            if (cohorts != null)
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                    total += ComputeBiomass(speciesCohorts);
            return total;
        }
    }
}
