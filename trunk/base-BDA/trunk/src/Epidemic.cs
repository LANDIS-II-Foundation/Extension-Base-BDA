//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using System.Collections.Generic;

namespace Landis.Extension.BaseBDA
{
    public class Epidemic
        : ICohortDisturbance

    {
        private static IEcoregionDataset ecoregions;

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

        ExtensionType IDisturbance.Type
        {
            get {
                return PlugIn.Type;
            }
        }

        //---------------------------------------------------------------------

        ActiveSite IDisturbance.CurrentSite
        {
            get {
                return currentSite;
            }
        }
        //---------------------------------------------------------------------

        IAgent EpidemicParameters
        {
            get
            {
                return epidemicParms;
            }
        }

        //---------------------------------------------------------------------
        ///<summary>
        ///Initialize an Epidemic - defined as an agent outbreak for an entire landscape
        ///at a single BDA timestep.  One epidemic per agent per BDA timestep
        ///</summary>

        public static void Initialize(IAgent agent)
        {
            PlugIn.ModelCore.UI.WriteLine("   Initializing agent {0}.", agent.AgentName);

            ecoregions = PlugIn.ModelCore.Ecoregions;


            //.ActiveSiteValues allows you to reset all active site at once.
            SiteVars.NeighborResourceDom.ActiveSiteValues = 0;
            SiteVars.Vulnerability.ActiveSiteValues = 0;
            SiteVars.SiteResourceDomMod.ActiveSiteValues = 0;
            SiteVars.SiteResourceDom.ActiveSiteValues = 0;

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
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
            PlugIn.ModelCore.UI.WriteLine("   New BDA Epidemic Activated.");

            //SiteResources.SiteResourceDominance(agent, ROS, SiteVars.Cohorts);
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
                foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
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

            //PlugIn.ModelCore.UI.WriteLine("New Agent event");
        }

        //---------------------------------------------------------------------
        //Go through all active sites and damage them according to the
        //Site Vulnerability.
        private void DisturbSites(IAgent agent)
        {
            int totalSiteSeverity = 0;
            int siteCohortsKilled = 0;
            int[] cohortsKilled = new int[2];

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                siteCohortsKilled = 0;
                this.siteSeverity = 0;

                if(agent.OutbreakZone[site] == Zone.Newzone
                    && SiteVars.Vulnerability[site] > PlugIn.ModelCore.GenerateUniform())
                {
                    //PlugIn.ModelCore.UI.WriteLine("Zone={0}, agent.OutbreakZone={1}", Zone.Newzone.ToString(), agent.OutbreakZone[site]);
                    //PlugIn.ModelCore.UI.WriteLine("Vulnerability={0}, Randnum={1}", SiteVars.Vulnerability[site], PlugIn.ModelCore.GenerateUniform());
                    double vulnerability = SiteVars.Vulnerability[site];

                    if(vulnerability >= 0) this.siteSeverity= 1;

                    if(vulnerability >= 0.33) this.siteSeverity= 2;

                    if(vulnerability >= 0.66) this.siteSeverity= 3;



                    if(this.siteSeverity > 0)
                        cohortsKilled = KillSiteCohorts(site);

                    siteCohortsKilled = cohortsKilled[0];

                    SiteVars.NumberCFSconifersKilled[site].Add(PlugIn.ModelCore.CurrentTime, cohortsKilled[1]);

                    if (siteCohortsKilled > 0)
                    {
                        this.totalCohortsKilled += siteCohortsKilled;
                        this.totalSitesDamaged++;
                        totalSiteSeverity += this.siteSeverity;
                        SiteVars.Disturbed[site] = true;
                        SiteVars.TimeOfLastEvent[site] = PlugIn.ModelCore.CurrentTime;
                    } else
                        this.siteSeverity = 0;
                }
                agent.Severity[site] = (byte) this.siteSeverity;
            }
            if (this.totalSitesDamaged > 0)
                this.meanSeverity = (double) totalSiteSeverity / (double) this.totalSitesDamaged;
        }

        //---------------------------------------------------------------------
        //A small helper function for going through list of cohorts at a site
        //and checking them with the filter provided by RemoveMarkedCohort(ICohort).
        private int[] KillSiteCohorts(ActiveSite site)
        {
            this.siteCohortsKilled = 0;
            this.siteCFSconifersKilled = 0;

            currentSite = site;

            SiteVars.Cohorts[site].RemoveMarkedCohorts(this); 

            int[] cohortsKilled = new int[2];

            cohortsKilled[0] = this.siteCohortsKilled;
            cohortsKilled[1] = this.siteCFSconifersKilled;


            return cohortsKilled; 
        }

        //---------------------------------------------------------------------
        // DamageCohort is a filter to determine which cohorts are removed.
        // Each cohort is passed into the function and tested whether it should
        // be killed.
        bool ICohortDisturbance.MarkCohortForDeath(ICohort cohort)
        {
            //PlugIn.ModelCore.UI.WriteLine("Cohort={0}, {1}, {2}.", cohort.Species.Name, cohort.Age, cohort.Species.Index);
            
            bool killCohort = false;
            
            ISppParameters sppParms = epidemicParms.SppParameters[cohort.Species.Index];

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

