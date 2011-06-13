using Landis.SpatialModeling;

namespace Landis.Extension.DroughtGenerator
{
    class SiteVars
    {
        private static ISiteVar<ushort> droughtYears;

        //---------------------------------------------------------------------
        public static void Initialize()
        {
            droughtYears = PlugIn.ModelCore.Landscape.NewSiteVar<ushort>();

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.DroughtYears, "Drought.Years");
        }

        //---------------------------------------------------------------------
        public static ISiteVar<ushort> DroughtYears
        {
            get
            {
                return droughtYears;
            }
        }

    }
}
