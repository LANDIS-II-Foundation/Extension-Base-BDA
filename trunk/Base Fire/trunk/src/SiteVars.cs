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

            //cohorts = PlugIn.ModelCore.GetSiteVar<SiteCohorts>("Succession.BaseCohorts");

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.Severity, "Fire.Severity");

            /*foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                ushort maxAge = GetMaxAge(site);
                PlugIn.ModelCore.Log.WriteLine("maxAge = {0}.", maxAge);

                timeOfLastFire[site] = PlugIn.ModelCore.StartTime - maxAge;
            }*/
        }

        //---------------------------------------------------------------------
        public static void InitializeCohort()
        {
            cohorts = PlugIn.ModelCore.GetSiteVar<SiteCohorts>("Succession.BaseCohorts");
        }

        public static void InitializeDisturbances(int timestep)
        {
            timeOfLastWind = PlugIn.ModelCore.GetSiteVar<int>("Wind.TimeOfLastEvent");
            if (PlugIn.ModelCore.CurrentTime == timestep)
                PlugIn.ModelCore.Log.WriteLine("   INITIALIZING time since last fire.");
                foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
                {
                    ushort maxAge = GetMaxAge(site);
                    if(maxAge > 0)
                        PlugIn.ModelCore.Log.WriteLine("maxAge = {0}.", maxAge);

                    timeOfLastFire[site] = PlugIn.ModelCore.StartTime - maxAge;
                }
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
        public static ushort GetMaxAge(ActiveSite site)
        {
            if (SiteVars.Cohorts[site] == null)
            {
                PlugIn.ModelCore.Log.WriteLine("Cohort are null.  Why?");
                return 0;
            }
            ushort max = 0;
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                PlugIn.ModelCore.Log.WriteLine("Cohort = {0}.", speciesCohorts.Species.Name);
                foreach (ICohort cohort in speciesCohorts)
                {
                    PlugIn.ModelCore.Log.WriteLine("Cohort = {0}.", cohort.Age);
                    //ushort maxSpeciesAge = cohort.Age;
                    if (cohort.Age > max)
                        max = cohort.Age;
                }
            }
            return max;
        }
    }
}
