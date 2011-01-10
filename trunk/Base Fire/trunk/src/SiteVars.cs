//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:    Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.AgeOnlyCohorts;


namespace Landis.Extension.BaseFire
{
    public static class SiteVars
    {
        private static ISiteVar<IFireRegion> ecoregions;
        private static ISiteVar<Event> eventVar;
        private static ISiteVar<int> timeOfLastFire;
        private static ISiteVar<int> timeOfLastWind;
        private static ISiteVar<byte> severity;
        private static ISiteVar<bool> disturbed;
        private static ISiteVar<SiteCohorts> cohorts;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            ecoregions     = PlugIn.ModelCore.Landscape.NewSiteVar<IFireRegion>();
            eventVar        = PlugIn.ModelCore.Landscape.NewSiteVar<Event>(InactiveSiteMode.DistinctValues);
            timeOfLastFire = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            severity = PlugIn.ModelCore.Landscape.NewSiteVar<byte>();
            disturbed = PlugIn.ModelCore.Landscape.NewSiteVar<bool>();

            cohorts = PlugIn.ModelCore.GetSiteVar<SiteCohorts>("Succession.BaseCohorts");

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.Severity, "Fire.Severity");

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                ushort maxAge = Util.GetMaxAge(cohorts[site]);

                timeOfLastFire[site] = PlugIn.ModelCore.StartTime - maxAge;
            }
        }

        //---------------------------------------------------------------------

        public static void InitializeTimeOfLastWind()
        {
            timeOfLastWind = PlugIn.ModelCore.GetSiteVar<int>("Wind.TimeOfLastEvent");
        }

        //---------------------------------------------------------------------

        public static ISiteVar<IFireRegion> FireRegion
        {
            get {
                return ecoregions;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<Event> Event
        {
            get {
                return eventVar;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLastFire
        {
            get {
                return timeOfLastFire;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLastWind
        {
            get {
                return timeOfLastWind;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<byte> Severity
        {
            get {
                return severity;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<bool> Disturbed
        {
            get {
                return disturbed;
            }
        }

        public static ISiteVar<SiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
        }
    }
}
