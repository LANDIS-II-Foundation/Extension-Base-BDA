//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using System.Collections.Generic;

namespace Landis.Extension.BudwormBDA
{

    public class SiteResources
    {

        //---------------------------------------------------------------------
        ///<summary>
        ///Calculate the Site Resource Dominance (SRD) for all active sites.
        ///The SRD averages the resources for each species as defined in the
        ///BDA species table.
        ///SRD ranges from 0 - 1.
        ///</summary>
        //---------------------------------------------------------------------
        public static void SiteResourceDominance(IAgent agent, int ROS)
        {
            PlugIn.ModelCore.Log.WriteLine("   Calculating BDA Site Resource Dominance.");

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape) {

                double sumValue = 0.0;
                double maxValue = 0.0;
                int    ageOldestCohort= 0;
                int    numValidSpp = 0;
                double speciesHostValue = 0;

                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    ageOldestCohort = Util.GetMaxAge(SiteVars.Cohorts[site][species]);
                    ISppParameters sppParms = agent.SppParameters[species.Index];
                    if (sppParms == null)
                        continue;

                    bool negList = false;
                    foreach (ISpecies negSpp in agent.NegSppList)
                    {
                        if (species == negSpp)
                            negList = true;
                    }

                    if ((ageOldestCohort > 0) && (! negList))
                    {
                        numValidSpp++;
                        speciesHostValue = 0.0;

                        if (ageOldestCohort >= sppParms.MinorHostAge)
                            //speciesHostValue = 0.33;
                            speciesHostValue = sppParms.MinorHostSRD;

                        if (ageOldestCohort >= sppParms.SecondaryHostAge)
                            //speciesHostValue = 0.66;
                            speciesHostValue = sppParms.SecondaryHostSRD;

                        if (ageOldestCohort >= sppParms.PrimaryHostAge)
                            //speciesHostValue = 1.0;
                            speciesHostValue = sppParms.PrimaryHostSRD;


                        sumValue += speciesHostValue;
                        maxValue = System.Math.Max(maxValue, speciesHostValue);
                    }
                }

                if (agent.SRDmode == SRDmode.mean)
                    SiteVars.SiteResourceDom[site] = sumValue / (double) numValidSpp;

                if (agent.SRDmode == SRDmode.max)
                    SiteVars.SiteResourceDom[site] = maxValue;

            }

        }  //end siteResourceDom

        //---------------------------------------------------------------------
        ///<summary>
        ///Calculate the Site Resource Dominance MODIFIER for all active sites.
        ///Site Resource Dominance Modifier takes into account other disturbances and
        ///any ecoregion modifiers defined.
        ///SRDMods range from 0 - 1.
        ///</summary>
        //---------------------------------------------------------------------
        public static void SiteResourceDominanceModifier(IAgent agent)
        {

            PlugIn.ModelCore.Log.WriteLine("   Calculating BDA Modified Site Resource Dominance.");
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape) {

                if (SiteVars.SiteResourceDom[site] > 0.0)
                {
                    int     lastDisturb = 0;
                    int     duration = 0;
                    double  disturbMod = 0;
                    double  sumDisturbMods = 0.0;
                    double  SRDM = 0.0;

                    //---- FIRE -------------------------
                    if(SiteVars.TimeOfLastFire != null &&
                        agent.DistParameters[(int) DisturbanceType.Fire].Duration > 0)
                    {
                        PlugIn.ModelCore.Log.WriteLine("   Calculating effect of Fire.");
                        lastDisturb = SiteVars.TimeOfLastFire[site];
                        duration = agent.DistParameters[(int) DisturbanceType.Fire].Duration;

                        if (lastDisturb < duration)
                        {
                            disturbMod = agent.DistParameters[(int) DisturbanceType.Fire].DistModifier *
                                (double)(duration - lastDisturb) / duration;
                            sumDisturbMods += disturbMod;
                        }
                    }

                    //---- WIND -------------------------
                    if(SiteVars.TimeOfLastWind != null &&
                        agent.DistParameters[(int) DisturbanceType.Wind].Duration > 0)
                    {
                        PlugIn.ModelCore.Log.WriteLine("   Calculating effect of Wind.");
                        lastDisturb = SiteVars.TimeOfLastWind[site];
                        duration = agent.DistParameters[(int) DisturbanceType.Wind].Duration;

                        if (lastDisturb < duration)
                        {
                            disturbMod = agent.DistParameters[(int) DisturbanceType.Wind].DistModifier *
                                (double)(duration - lastDisturb) / duration;
                            sumDisturbMods += disturbMod;
                        }
                    }

                    //---- HARVEST -------------------------
                    if(SiteVars.TimeOfLastHarvest != null &&
                        agent.DistParameters[(int) DisturbanceType.Harvest].Duration > 0)
                    {
                        PlugIn.ModelCore.Log.WriteLine("   Calculating effect of Harvesting.");
                        lastDisturb = SiteVars.TimeOfLastHarvest[site];
                        duration = agent.DistParameters[(int) DisturbanceType.Harvest].Duration;

                        if (lastDisturb < duration)
                        {
                            disturbMod = agent.DistParameters[(int) DisturbanceType.Harvest].DistModifier *
                                (double)(duration - lastDisturb) / duration;
                            sumDisturbMods += disturbMod;
                        }

                    }

                    //PlugIn.ModelCore.Log.WriteLine("   Summation of Disturbance Modifiers = {0}.", sumMods);
                    //---- APPLY ECOREGION MODIFIERS --------
                    IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];


                    SRDM = SiteVars.SiteResourceDom[site] +
                           sumDisturbMods +
                           agent.EcoParameters[ecoregion.Index].EcoModifier;

                    SRDM = System.Math.Max(0.0, SRDM);
                    SRDM = System.Math.Min(1.0, SRDM);

                    SiteVars.SiteResourceDomMod[site] = SRDM;
                }//end of one site

                else SiteVars.SiteResourceDomMod[site] = 0.0;
            } //end Active sites
        } //end Function

        //---------------------------------------------------------------------
        ///<summary>
        ///Calculate SITE VULNERABILITY
        ///Following equations found in Sturtevant et al. 2004.
        ///Ecological Modeling 180: 153-174.
        ///</summary>
        //---------------------------------------------------------------------
        public static void SiteVulnerability(IAgent agent,
                                                int ROS,
                                                bool considerNeighbor)
        {
            double   SRD, SRDMod, NRD;
            double   CaliROS3 = ((double) ROS / 3) * agent.BDPCalibrator;

            //PlugIn.ModelCore.Log.WriteLine("   Calculating BDA SiteVulnerability.");

            if (considerNeighbor)      //take neigborhood into consideration
            {
                foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
                {

                    SRD = SiteVars.SiteResourceDom[site];

                    //If a site has been chosen for an outbreak and there are
                    //resources available for an outbreak:
                    if (SRD > 0)
                    {
                        SRDMod = SiteVars.SiteResourceDomMod[site];
                        NRD = SiteVars.NeighborResourceDom[site];
                        double tempSV = 0.0;

                        //Equation (8) in Sturtevant et al. 2004.
                        tempSV = SRDMod + (NRD * agent.NeighborWeight);
                        tempSV = tempSV / (1 + agent.NeighborWeight);
                        double vulnerable = (double)(CaliROS3 * tempSV);
                        //PlugIn.ModelCore.Log.WriteLine("tempSV={0}, SRDMod={1}, NRD={2}, neighborWeight={3}.", tempSV, SRDMod,NRD,agent.NeighborWeight);

                        SiteVars.Vulnerability[site] = System.Math.Max(0.0, vulnerable);
                        //PlugIn.ModelCore.Log.WriteLine("Site Vulnerability = {0}, CaliROS3={1}, tempSV={2}.", SiteVars.Vulnerability[site], CaliROS3, tempSV);
                    }
                    else
                        SiteVars.Vulnerability[site] = 0.0;
                }
            }
            else        //Do NOT take neigborhood into consideration
            {
                foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
                {
                    SRDMod = SiteVars.SiteResourceDomMod[site];

                    double vulnerable = (double)(CaliROS3 * SRDMod);
                    SiteVars.Vulnerability[site] = System.Math.Max(0, vulnerable);
                }
            }
        }

        //---------------------------------------------------------------------
        ///<summary>
        /// Calculate the The Resource Dominance of all active NEIGHBORS
        /// within the User defined NeighborRadius.
        ///
        /// The weight of neighbors is dependent upon distance and a
        /// weighting algorithm:  uniform, linear, or gaussian.
        ///
        /// Subsampling determined by User defined NeighborSpeedUp.
        /// Gaussian equation:  http://www.anc.ed.ac.uk/~mjo/intro/node7.html
        ///</summary>
        //---------------------------------------------------------------------
        public static void NeighborResourceDominance(IAgent agent)
        {
            PlugIn.ModelCore.Log.WriteLine("   Calculating BDA Neighborhood Resource Dominance.");

            double totalNeighborWeight = 0.0;
            double maxNeighborWeight = 0.0;
            int neighborCnt = 0;
            int speedUpFraction = (int) agent.NeighborSpeedUp + 1;

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape) {
                if (agent.OutbreakZone[site] == Zone.Newzone)
                {
                    //neighborWeight = 0.0;
                    totalNeighborWeight = 0.0;
                    maxNeighborWeight = 0.0;
                    neighborCnt = 0;

                    if (SiteVars.SiteResourceDom[site] > 0 )
                    {

                        List<RelativeLocationWeighted> neighborhood = new List<RelativeLocationWeighted>();
                        foreach (RelativeLocationWeighted relativeLoc in agent.ResourceNeighbors)
                        {

                            Site neighbor = site.GetNeighbor(relativeLoc.Location);
                            if (neighbor != null
                                && neighbor.IsActive)
                            {
                                neighborhood.Add(relativeLoc);
                            }
                        }

                        neighborhood = PlugIn.ModelCore.shuffle(neighborhood);
                        foreach(RelativeLocationWeighted neighbor in neighborhood)
                        {
                            //Do NOT subsample if there are too few neighbors
                            //i.e., <= subsample size.
                            if(neighborhood.Count <= speedUpFraction ||
                                neighborCnt%speedUpFraction == 0)
                            {
                                Site activeSite = site.GetNeighbor(neighbor.Location);

                                //Note:  SiteResourceDomMod ranges from 0 - 1.
                                if (SiteVars.SiteResourceDomMod[activeSite] > 0)
                                {
                                    totalNeighborWeight += SiteVars.SiteResourceDomMod[activeSite] * neighbor.Weight;
                                    maxNeighborWeight += neighbor.Weight;
                                }
                            }
                            neighborCnt++;
                        }

                        if (maxNeighborWeight > 0.0)
                            SiteVars.NeighborResourceDom[site] = totalNeighborWeight / maxNeighborWeight;
                        else
                            SiteVars.NeighborResourceDom[site] = 0.0;
                    } else
                        SiteVars.NeighborResourceDom[site] = 0.0;
                 }
             }
        }


//End of SiteResources
    }
}
