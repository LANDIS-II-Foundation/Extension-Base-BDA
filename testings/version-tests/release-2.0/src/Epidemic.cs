//  Copyright 2005 University of Wisconsin
//  Authors:  
//      Robert M. Scheller
//      James B. Domingo
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.
//  Version 1.0
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Grids;
using Landis.AgeCohort;
using Landis.Ecoregions;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;
using Landis.Util;
using System.Collections.Generic;

namespace Landis.BDA
{
    public class Epidemic
        : AgeCohort.ICohortDisturbance

    {
        private static Ecoregions.IDataset ecoregions;
        //private static ISuccession successionPlugIn;
        //private static ILandscapeCohorts cohorts;

        private IAgent epidemicParms;
        private int totalSitesDamaged;
        private int totalCohortsKilled;
        private double meanSeverity;
        private int siteSeverity;
        private int siteCohortsKilled;
        private int siteCFSconifersKilled;
        private int[] sitesInEvent;

        private ActiveSite currentSite; // current site where cohorts are being damaged

        private enum TempPattern        {random, cyclic};
        private enum NeighborShape      {uniform, linear, gaussian};
        private enum InitialCondition   {map, none};
        private enum SRDMode            {SRDmax, SRDmean};


        //---------------------------------------------------------------------

        static Epidemic()
        {
        }

        //---------------------------------------------------------------------

        public int[] SitesInEvent
        {
            get {
                return sitesInEvent;
            }
        }

        //---------------------------------------------------------------------

        public int CohortsKilled
        {
            get {
                return totalCohortsKilled;
            }
        }

        //---------------------------------------------------------------------

        public double MeanSeverity
        {
            get {
                return meanSeverity;
            }
        }

        //---------------------------------------------------------------------

        public int TotalSitesDamaged
        {
            get {
                return totalSitesDamaged;
            }
        }
        //---------------------------------------------------------------------

        PlugInType AgeCohort.IDisturbance.Type
        {
            get {
                return PlugIn.Type;
            }
        }

        //---------------------------------------------------------------------

        ActiveSite AgeCohort.IDisturbance.CurrentSite
        {
            get {
                return currentSite;
            }
        }

        //---------------------------------------------------------------------
        ///<summary>
        ///Initialize an Epidemic - defined as an agent outbreak for an entire landscape
        ///at a single BDA timestep.  One epidemic per agent per BDA timestep
        ///</summary>

        public static void Initialize(IAgent agent)
        {
            UI.WriteLine("Initializing agent {0}.", agent.AgentName);

            ecoregions = Model.Core.Ecoregions;
            //successionPlugIn = Model.Core.GetSuccession();

            //cohorts = Model.Core.SuccessionCohorts as ILandscapeCohorts;
            //if (cohorts == null)
            //    throw new System.ApplicationException("Error: Cohorts don't support age-cohort interface");

            //.ActiveSiteValues allows you to reset all active site at once.
            SiteVars.NeighborResourceDom.ActiveSiteValues = 0;
            SiteVars.Vulnerability.ActiveSiteValues = 0;
            SiteVars.SiteResourceDomMod.ActiveSiteValues = 0;
            SiteVars.SiteResourceDom.ActiveSiteValues = 0;

            foreach (ActiveSite site in Model.Core.Landscape) 
            {
                if(agent.OutbreakZone[site] == Zone.Newzone)
                    agent.OutbreakZone[site] = Zone.Lastzone;
                else 
                    agent.OutbreakZone[site] = Zone.Nozone;
            }

        }

        //---------------------------------------------------------------------
        ///<summary>
        ///Simulate an Epidemic - This is the controlling function that calls the 
        ///subsequent function.  The basic logic of an epidemic resides here.
        ///</summary>
        public static Epidemic Simulate(IAgent agent, 
                                        int currentTime, 
                                        int timestep,
                                        int ROS)
        {


            Epidemic CurrentEpidemic = new Epidemic(agent);
            UI.WriteLine("   New BDA Epidemic Activated:  {0}.", agent.AgentName);

            SiteResources.SiteResourceDominance(agent, ROS);
            SiteResources.SiteResourceDominanceModifier(agent);

            if(agent.Dispersal) {
                //Asynchronous - Simulate Agent Dispersal
                
                // Calculate Site Vulnerability without considering the Neighborhood
                // If neither disturbance modifiers nor ecoregion modifiers are active,
                //  Vulnerability will equal SiteReourceDominance.
                SiteResources.SiteVulnerability(agent, ROS, false);  

                Epicenters.NewEpicenters(agent, timestep);
                
            } else 
            {
                //Synchronous:  assume that all Active sites can potentially be
                //disturbed without regard to initial locations.  
                foreach (ActiveSite site in Model.Core.Landscape) 
                    agent.OutbreakZone[site] = Zone.Newzone;

            }
            
            //Consider the Neighborhood if requested:
            if (agent.NeighborFlag)
                SiteResources.NeighborResourceDominance(agent);
                    
            //Recalculate Site Vulnerability considering neighbors if necessary:
            SiteResources.SiteVulnerability(agent, ROS, agent.NeighborFlag);

            CurrentEpidemic.DisturbSites(agent);

            return CurrentEpidemic;
        }

        //---------------------------------------------------------------------
        // Epidemic Constructor
        private Epidemic(IAgent agent)
        {
            this.sitesInEvent = new int[ecoregions.Count];
            foreach(IEcoregion ecoregion in ecoregions)
                this.sitesInEvent[ecoregion.Index] = 0;
            this.epidemicParms = agent;
            this.totalCohortsKilled = 0;
            this.meanSeverity = 0.0;
            this.totalSitesDamaged = 0;

            UI.WriteLine("New Agent event");
        }

        //---------------------------------------------------------------------
        //Go through all active sites and damage them according to the 
        //Site Vulnerability.
        private void DisturbSites(IAgent agent)
        {
            int totalSiteSeverity = 0;
            int siteCohortsKilled = 0;
            int[] cohortsKilled = new int[2];

            foreach (ActiveSite site in Model.Core.Landscape) 
            {
                siteCohortsKilled = 0;
                this.siteSeverity = 0;

                if(agent.OutbreakZone[site] == Zone.Newzone 
                    && SiteVars.Vulnerability[site] > Random.GenerateUniform())
                {
                    double vulnerability = SiteVars.Vulnerability[site];
            
                    if(vulnerability >= 0) this.siteSeverity= 1;
            
                    if(vulnerability >= 0.33) this.siteSeverity= 2;
            
                    if(vulnerability >= 0.66) this.siteSeverity= 3;

                    if(this.siteSeverity > 0)
                        cohortsKilled = Damage(site);
                        
                    siteCohortsKilled = cohortsKilled[0];
                    
                    //int[] temp = SiteVars.NumberCFSconifersKilled[site];
                    //temp[Model.Core.CurrentTime] = cohortsKilled[1];
                    //SiteVars.NumberCFSconifersKilled[site] = temp;
                    SiteVars.NumberCFSconifersKilled[site].Add(Model.Core.CurrentTime, cohortsKilled[1]);
                    
                    //for(int i=0; i<=Model.Core.EndTime; i++)
                    //    UI.WriteLine("temp Cohorts Killed = {0}. i={1}", SiteVars.NumberCFSconifersKilled[site][i], i);
                        
                    
                    //UI.WriteLine("Number Site Cohorts Killed = {0}.  CFS Killed = {1}.", cohortsKilled[0], cohortsKilled[1]);
                    //UI.WriteLine("SiteVars.CFS Killed = {0}, Time = {1}.", SiteVars.NumberCFSconifersKilled[site][Model.Core.CurrentTime], Model.Core.CurrentTime);
                    
                    if (siteCohortsKilled > 0) 
                    {
                        this.totalCohortsKilled += siteCohortsKilled;
                        this.totalSitesDamaged++;
                        totalSiteSeverity += this.siteSeverity;
                        SiteVars.Disturbed[site] = true;
                        SiteVars.TimeOfLastEvent[site] = Model.Core.CurrentTime;
                    } else
                        this.siteSeverity = 0;
                }
                agent.Severity[site] = (byte) this.siteSeverity;
            }
            this.meanSeverity = (double) totalSiteSeverity / (double) this.totalSitesDamaged;
        }

        //---------------------------------------------------------------------
        //A small helper function for going through list of cohorts at a site
        //and checking them with the filter provided by DamageCohort(ICohort).
        //Remove(DamageCohort) is defined within the Landis-II Core.
        private int[] Damage(ActiveSite site)
        {
            this.siteCohortsKilled = 0;
            this.siteCFSconifersKilled = 0;
            
            currentSite = site;
            PlugIn.Cohorts[site].DamageBy(this);
            
            int[] cohortsKilled = new int[2];
            
            cohortsKilled[0] = this.siteCohortsKilled;
            cohortsKilled[1] = this.siteCFSconifersKilled;
            
            
            return cohortsKilled; //this.siteCohortsKilled;
        }

        //---------------------------------------------------------------------
        // DamageCohort is a filter to determine which cohorts are removed.
        // Each cohort is passed into the function and tested whether it should
        // be killed.
        bool AgeCohort.ICohortDisturbance.Damage(AgeCohort.ICohort cohort)
        {
            bool killCohort = false;

            ISppParameters sppParms = epidemicParms.SppParameters[cohort.Species.Index];

            if (sppParms == null)
                return killCohort;
            
            
            if(this.siteSeverity == 1)
                if(cohort.Age >= sppParms.VulnerableHostAge)
                    killCohort = true;
            if(this.siteSeverity == 2)
                if(cohort.Age >= sppParms.TolerantHostAge)
                    killCohort = true;
            if(this.siteSeverity == 3)
                if(cohort.Age >= sppParms.ResistantHostAge)
                    killCohort = true;
                    
            if (killCohort)
            {
                this.siteCohortsKilled++;
                if (sppParms.CFSConifer)
                    this.siteCFSconifersKilled++;
            }

            return killCohort;
        }

    }
    
}

