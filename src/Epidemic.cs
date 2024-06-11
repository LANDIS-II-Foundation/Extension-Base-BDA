//  Copyright The LANDIS-II Foundation
//  Authors:  Robert M. Scheller, Brian Miranda, James B. Domingo

using Landis.Core;
using Landis.Library.UniversalCohorts;
using Landis.SpatialModeling;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Landis.Extension.ClimateBDA
{
    public class Epidemic
        : IDisturbance

    {
        private static IEcoregionDataset ecoregions;

        private IAgent epidemicParms;
        private int totalSitesDamaged;
        private int totalCohortsKilled;
        private double meanSeverity;
        private int siteSeverity;
        private double random;
        private double siteVulnerability;
        private int siteCohortsKilled;
        private int siteCFSconifersKilled;
        private int siteDarkAgesKilled;
        private int siteLightAgesKilled;
        private int[] sitesInEvent;

        private ActiveSite currentSite; // current site where cohorts are being damaged

        //private enum TempPattern        {random, cyclic};
        //private enum NeighborShape      {uniform, linear, gaussian};
        //private enum InitialCondition   {map, none};
        //private enum SRDMode            {SRDmax, SRDmean};


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
                return PlugIn.type;
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

            //PlugIn.ModelCore.Log.WriteLine("New Agent event");
        }

        //---------------------------------------------------------------------
        //Go through all active sites and damage them according to the
        //Site Vulnerability.
        private void DisturbSites(IAgent agent)
        {
            int totalSiteSeverity = 0;
            int siteCohortsKilled = 0;
            int[] cohortsKilled = new int[4];

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                siteCohortsKilled = 0;
                this.siteSeverity = 0;
                this.random = 0;

                double myRand = PlugIn.ModelCore.GenerateUniform();

                if((agent.OutbreakZone[site] == Zone.Newzone || agent.OutbreakZone[site] == Zone.Lastzone)
                    && SiteVars.Vulnerability[site] > myRand)
                {
                    //PlugIn.ModelCore.Log.WriteLine("Zone={0}, agent.OutbreakZone={1}", Zone.Newzone.ToString(), agent.OutbreakZone[site]);
                    //PlugIn.ModelCore.Log.WriteLine("Vulnerability={0}, Randnum={1}", SiteVars.Vulnerability[site], PlugIn.ModelCore.GenerateUniform());
                    double vulnerability = SiteVars.Vulnerability[site];

                    if(vulnerability >= 0) this.siteSeverity= 1;

                    if(vulnerability >= agent.Class2_SV) this.siteSeverity= 2;

                    if(vulnerability >= agent.Class3_SV) this.siteSeverity= 3;

                    this.random = myRand;
                    this.siteVulnerability = SiteVars.Vulnerability[site];
                    Dictionary<string, int> vegTypeDict = new Dictionary<string, int>();

                    if(this.siteSeverity > 0)                        
                    {

                        vegTypeDict = CalculateDominantVeg(site);
                        cohortsKilled = KillSiteCohorts(site);  // [0] - all cohorts killed; [1] CFS conifer cohorts killed; [2] dark cohorts killed; [3] light cohorts killed
                        SiteVars.TimeOfLastEvent[site] = PlugIn.ModelCore.CurrentTime;
                        SiteVars.AgentName[site] = agent.AgentName;
                        SiteVars.BDASeverity[site] = this.siteSeverity;
                    }

                    siteCohortsKilled = cohortsKilled[0]; // All cohorts killed

                    if (SiteVars.NumberCFSconifersKilled[site].ContainsKey(PlugIn.ModelCore.CurrentTime))
                    {
                        int prevKilled = SiteVars.NumberCFSconifersKilled[site][PlugIn.ModelCore.CurrentTime];
                        SiteVars.NumberCFSconifersKilled[site][PlugIn.ModelCore.CurrentTime] = prevKilled + cohortsKilled[1];
                    }
                    else
                    {
                        SiteVars.NumberCFSconifersKilled[site].Add(PlugIn.ModelCore.CurrentTime, cohortsKilled[1]);
                    }

                    if (siteCohortsKilled > 0)
                    {
                        this.totalCohortsKilled += siteCohortsKilled;
                        this.totalSitesDamaged++;
                        totalSiteSeverity += this.siteSeverity;
                        SiteVars.Disturbed[site] = true;

                        float disturbanceEffect = 0;
                        if (vegTypeDict.ContainsKey("dark"))
                        {
                            int darkAgesKilled = cohortsKilled[2];
                            int darkAgesTotal = vegTypeDict["dark"];
                            disturbanceEffect = (float)darkAgesKilled / (float)darkAgesTotal;
                            if (disturbanceEffect >= 0.5)
                            {
                                SiteVars.DisturbedFuel[site] = "DarkDisturbed";
                            }else
                                SiteVars.DisturbedFuel[site] = "";
                        }
                        else if (vegTypeDict.ContainsKey("light"))
                        {
                            int lightAgesKilled = cohortsKilled[3];
                            int lightAgesTotal = vegTypeDict["light"];
                            disturbanceEffect = (float)lightAgesKilled / (float)lightAgesTotal;
                            if (disturbanceEffect >= 0.5)
                            {
                                SiteVars.DisturbedFuel[site] = "LightDisturbed";
                            }else
                                SiteVars.DisturbedFuel[site] = "";
                        }
                        else
                            SiteVars.DisturbedFuel[site] = "";
                    } else
                        this.siteSeverity = 0;
                }
                agent.Severity[site] = (byte) this.siteSeverity;
                SiteVars.BDASeverity[site] = this.siteSeverity;
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
            this.siteDarkAgesKilled = 0;
            this.siteLightAgesKilled = 0;

            currentSite = site;

            SiteVars.Cohorts[site].ReduceOrKillCohorts(this); 

            int[] cohortsKilled = new int[4];

            cohortsKilled[0] = this.siteCohortsKilled;
            cohortsKilled[1] = this.siteCFSconifersKilled;
            cohortsKilled[2] = this.siteDarkAgesKilled;
            cohortsKilled[3] = this.siteLightAgesKilled;


            return cohortsKilled; 
        }

        //---------------------------------------------------------------------
        // DamageCohort is a filter to determine which cohorts are removed.
        // Each cohort is passed into the function and tested whether it should
        // be killed.
        int IDisturbance.ReduceOrKillMarkedCohort(ICohort cohort)
        {
            //PlugIn.ModelCore.Log.WriteLine("Cohort={0}, {1}, {2}.", cohort.Species.Name, cohort.Age, cohort.Species.Index);
            
            bool killCohort = false;
           // bool advRegenSpp = false;

            ISppParameters sppParms = epidemicParms.SppParameters[cohort.Species.Index];

            if (cohort.Data.Age >= sppParms.ResistantHostAge)
            {
                if (this.random <= this.siteVulnerability * sppParms.ResistantHostVuln)
                {
                        killCohort = true;
                }
            }

            if (cohort.Data.Age >= sppParms.TolerantHostAge)
            {
                if (this.random <= this.siteVulnerability * sppParms.TolerantHostVuln)
                {
                        killCohort = true;
                }
            }

            if (cohort.Data.Age >= sppParms.VulnerableHostAge)
            {
                if (this.random <= this.siteVulnerability * sppParms.VulnerableHostVuln)
                {
                        killCohort = true;
                }
            }


            if (killCohort)
            {
                this.siteCohortsKilled++;
                if (sppParms.CFSConifer.Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    this.siteCFSconifersKilled++;
                }
                if (sppParms.CFSConifer.Equals("dark", StringComparison.OrdinalIgnoreCase))
                {
                    this.siteCFSconifersKilled++;
                    this.siteDarkAgesKilled += cohort.Data.Age;
                }
                if (sppParms.CFSConifer.Equals("light", StringComparison.OrdinalIgnoreCase))
                {
                    this.siteCFSconifersKilled++;
                    this.siteLightAgesKilled += cohort.Data.Age;                
                }
            }

            if (killCohort)
                return cohort.Data.Biomass;

            return 0;
        }

        Dictionary<string, int> CalculateDominantVeg(ActiveSite site)
        {
            Dictionary<string, int> vegTypeDict = new Dictionary<string, int>();
            ISiteCohorts siteCohorts = SiteVars.Cohorts[site];
            int darkSum = 0;
            int lightSum = 0;
            int otherSum = 0;
            int totalSum = 0;
            foreach (ISpeciesCohorts speciesCohorts in siteCohorts)
            {
                foreach (Cohort cohort in speciesCohorts)
                {
                    ISppParameters sppParms = epidemicParms.SppParameters[cohort.Species.Index];
                    if (sppParms.CFSConifer.Equals("dark", StringComparison.OrdinalIgnoreCase))
                    {
                        darkSum += cohort.Age;
                        totalSum += cohort.Age;
                    }
                    else if (sppParms.CFSConifer.Equals("light", StringComparison.OrdinalIgnoreCase))
                    {
                        lightSum += cohort.Age;
                        totalSum += cohort.Age;
                    }
                    else if (this.EpidemicParameters.NegSppList.Contains(cohort.Species))
                    {
                    }
                    else
                    {
                        otherSum += cohort.Age;
                        totalSum += cohort.Age;
                    }
                }
            }
            float darkProp = (float)darkSum / (float)totalSum;
            float lightProp = (float)lightSum / (float)totalSum;
            if (darkProp >= 0.5)
            {   
                if (lightProp >= 0.5)
                {
                    vegTypeDict.Add("mixed", totalSum);
                }
                else
                    vegTypeDict.Add("dark", darkSum);
            }
            else if (lightProp >= 0.5)
            {
                if (darkProp >= 0.5)
                    vegTypeDict.Add("mixed", totalSum);
                else
                    vegTypeDict.Add("light", lightSum);
            }
            return vegTypeDict;
        }

    }

}

