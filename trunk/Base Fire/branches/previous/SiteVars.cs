//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.AgeCohort;
using Landis.Landscape;

namespace Landis.Fire
{
    public static class SiteVars
    {
        private static ISiteVar<IEcoregion> ecoregions;
        private static ISiteVar<Event> eventVar;
        private static ISiteVar<int> timeOfLastFire;
        private static ISiteVar<int> timeOfLastWind;
        private static ISiteVar<byte> severity;
        private static ISiteVar<bool> disturbed;

        //---------------------------------------------------------------------

        public static void Initialize(ILandscapeCohorts cohorts)
        {
            ecoregions     = Model.Core.Landscape.NewSiteVar<IEcoregion>();
            eventVar       = Model.Core.Landscape.NewSiteVar<Event>(InactiveSiteMode.DistinctValues);
            timeOfLastFire = Model.Core.Landscape.NewSiteVar<int>();
            severity       = Model.Core.Landscape.NewSiteVar<byte>();
            disturbed      = Model.Core.Landscape.NewSiteVar<bool>();

            //Initialize TimeSinceLastFire to the maximum cohort age:
            foreach (ActiveSite site in Model.Core.Landscape) 
            {
                ushort maxAge = AgeCohort.Util.GetMaxAge(cohorts[site]);
                timeOfLastFire[site] = Model.Core.StartTime - maxAge;
            }
        }

        //---------------------------------------------------------------------

        public static void InitializeTimeOfLastWind()
        {
            timeOfLastWind = Model.Core.GetSiteVar<int>("Wind.TimeOfLastEvent");
        }

        //---------------------------------------------------------------------

        public static ISiteVar<IEcoregion> Ecoregion
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
    }
}
