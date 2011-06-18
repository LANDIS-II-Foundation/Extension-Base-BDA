using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;

namespace Landis.Extension.DroughtDisturbance
{
    class SiteVars
    {
        private static ISiteVar<ushort> droughtBioRemoved;
        private static ISiteVar<ushort> droughtYears;
        private static ISiteVar<ISiteCohorts> biomassCohorts;

        //---------------------------------------------------------------------
        public static void Initialize()
        {
            biomassCohorts = PlugIn.ModelCore.GetSiteVar<ISiteCohorts>("Succession.BiomassCohorts");
            droughtBioRemoved = PlugIn.ModelCore.Landscape.NewSiteVar<ushort>();
            droughtYears = PlugIn.ModelCore.GetSiteVar<ushort>( "Drought.Years");
        }

        //---------------------------------------------------------------------
        public static ISiteVar<ushort> DroughtBioRemoved
        {
            get
            {
                return droughtBioRemoved;
            }
        }

        //---------------------------------------------------------------------
        public static ISiteVar<ushort> DroughtYears
        {
            get
            {
                return droughtYears;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Biomass cohorts at each site.
        /// </summary>
        public static ISiteVar<ISiteCohorts> Cohorts
        {
            get
            {
                return biomassCohorts;
            }
            set
            {
                biomassCohorts = value;
            }
        }

        //---------------------------------------------------------------------

    }
}
