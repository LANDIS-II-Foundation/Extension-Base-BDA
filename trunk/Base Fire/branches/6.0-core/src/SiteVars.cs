//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.Core;
using Wisc.Flel.GeospatialModeling.Landscapes;
using Landis.Library.BaseCohorts;


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
        private static ICore core;

        //---------------------------------------------------------------------

        public static void Initialize(ICore modelCore)
        {
            core = modelCore;
            modelCore.Log.WriteLine("Inside Initialize method !!!");
            ecoregions     = modelCore.Landscape.NewSiteVar<IFireRegion>();
            eventVar       = modelCore.Landscape.NewSiteVar<Event>(InactiveSiteMode.DistinctValues);
            timeOfLastFire = modelCore.Landscape.NewSiteVar<int>();
            severity = modelCore.Landscape.NewSiteVar<byte>();
            disturbed = modelCore.Landscape.NewSiteVar<bool>();

            cohorts = modelCore.GetSiteVar<SiteCohorts>("Succession.Cohorts");
            
            // Enable interactions with (almost) any fire extension:
            modelCore.RegisterSiteVar(SiteVars.Severity, "Fire.Severity");
            

            //Initialize TimeSinceLastFire to the maximum cohort age:
            foreach (ActiveSite site in modelCore.Landscape) 
            {
                // Test to make sure the cohort type is correct for this extension
                if (site.Location.Row == 1 && site.Location.Column == 1 && !SiteVars.Cohorts[site].HasAge())
                {
                    throw new System.ApplicationException("Error in the Scenario file:  Incompatible extensions; Cohort age data required for this extension to operate.");
                }

                //UI.WriteLine("Inside foreach loop.  Site R/C = {0}/{1} !!!", site.Location.Row, site.Location.Column);
                ushort maxAge = Library.BaseCohorts.Util.GetMaxAge(cohorts[site]);
                //ushort maxAge = Library.Cohort.AgeOnly.Util.GetMaxAge(SiteVars.Cohorts[site]);
                //UI.WriteLine("Assigned maxAge");
                timeOfLastFire[site] = modelCore.StartTime - maxAge;
            }
        }

        //---------------------------------------------------------------------

        public static void InitializeTimeOfLastWind()
        {
            timeOfLastWind = core.GetSiteVar<int>("Wind.TimeOfLastEvent");
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
