//  Copyright 2005 University of Wisconsin
//  Authors:  
//      Robert M. Scheller
//      James B. Domingo
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.
//  Version 1.0
//  License:  Available at  
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.AgeCohort;
using Landis.Landscape;
using System.Collections.Generic;

namespace Landis.BDA
{
    ///<summary>
    /// Site Variables for a disturbance plug-in that simulates Biological Agents.
    /// </summary>
    public static class SiteVars
    {
        //private static ISiteVar<Epidemic> eventVar;
        private static ISiteVar<int> timeOfLastBDA;
        private static ISiteVar<int> timeOfLastFire;
        private static ISiteVar<int> timeOfLastWind;
        private static ISiteVar<int> timeOfLastHarvest;
        private static ISiteVar<double> neighborResourceDom;
        private static ISiteVar<double> siteResourceDomMod;
        private static ISiteVar<double> siteResourceDom;
        private static ISiteVar<double> vulnerability;
        private static ISiteVar<bool> disturbed;
        //private static ISiteVar<int[]> numberCFSconifersKilled;
        private static ISiteVar<Dictionary<int,int>> numberCFSconifersKilled;

        //---------------------------------------------------------------------

        public static void Initialize(ILandscapeCohorts cohorts)
        {
            //eventVar       = Model.Core.Landscape.NewSiteVar<Epidemic>(InactiveSiteMode.DistinctValues);
            timeOfLastBDA  = Model.Core.Landscape.NewSiteVar<int>();
            neighborResourceDom = Model.Core.Landscape.NewSiteVar<double>();
            siteResourceDomMod  = Model.Core.Landscape.NewSiteVar<double>();
            siteResourceDom     = Model.Core.Landscape.NewSiteVar<double>();
            vulnerability       = Model.Core.Landscape.NewSiteVar<double>();
            disturbed           = Model.Core.Landscape.NewSiteVar<bool>();
            //numberCFSconifersKilled = Model.Core.Landscape.NewSiteVar<int[]>();
            numberCFSconifersKilled = Model.Core.Landscape.NewSiteVar<Dictionary<int,int>>();
            
            SiteVars.TimeOfLastEvent.ActiveSiteValues = -10000;
            SiteVars.NeighborResourceDom.ActiveSiteValues = 0.0;
            SiteVars.SiteResourceDomMod.ActiveSiteValues = 0.0;
            SiteVars.SiteResourceDom.ActiveSiteValues = 0.0;
            SiteVars.Vulnerability.ActiveSiteValues = 0.0;
            
            foreach(ActiveSite site in Model.Core.Landscape)
                //SiteVars.NumberCFSconifersKilled[site] = new int[Model.Core.EndTime - Model.Core.StartTime + 1];
                SiteVars.NumberCFSconifersKilled[site] = new Dictionary<int, int>();
            
            // Added for v1.1 to enable interactions with CFS fuels extension.
            Model.Core.RegisterSiteVar(SiteVars.NumberCFSconifersKilled, "BDA.NumCFSConifers");
            Model.Core.RegisterSiteVar(SiteVars.TimeOfLastEvent, "BDA.TimeOfLastEvent");

        }

        //---------------------------------------------------------------------

        public static void InitializeTimeOfLastDisturbances()
        {
            timeOfLastWind  = Model.Core.GetSiteVar<int>("Wind.TimeOfLastEvent");
            timeOfLastFire  = Model.Core.GetSiteVar<int>("Fire.TimeOfLastEvent");
            timeOfLastHarvest  = Model.Core.GetSiteVar<int>("Harvest.TimeOfLastEvent");

        }
        //---------------------------------------------------------------------

        /*public static ISiteVar<Epidemic> Epidemic
        {
            get {
                return eventVar;
            }
        }*/

        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastEvent
        {
            get {
                return timeOfLastBDA;
            }
        }

        public static ISiteVar<int> TimeOfLastFire
        {
            get {
                return timeOfLastFire;
            }
        }

        public static ISiteVar<int> TimeOfLastWind
        {
            get {
                return timeOfLastWind;
            }
        }
        public static ISiteVar<int> TimeOfLastHarvest
        {
            get {
                return timeOfLastHarvest;
            }
        }
        //---------------------------------------------------------------------

        
        public static ISiteVar<double> SiteResourceDom
        {
            get {
                return siteResourceDom;
            }
        }
        public static ISiteVar<double> NeighborResourceDom
        {
            get {
                return neighborResourceDom;
            }
        }
        public static ISiteVar<double> SiteResourceDomMod
        {
            get {
                return siteResourceDomMod;
            }
        }
        
        public static ISiteVar<double> Vulnerability
        {
            get {
                return vulnerability;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<bool> Disturbed
        {
            get {
                return disturbed;
            }
        }
        
        //public static ISiteVar<int[]> NumberCFSconifersKilled
        public static ISiteVar<Dictionary<int,int>> NumberCFSconifersKilled
        {
            get {
                return numberCFSconifersKilled;
            }
            set {
                numberCFSconifersKilled = value;
            }
        }
    }
}
