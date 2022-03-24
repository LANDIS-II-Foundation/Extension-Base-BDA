//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Landis.Extension.BaseBDA
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
            PlugIn.ModelCore.UI.WriteLine("   Calculating BDA Site Resource Dominance.");

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

            PlugIn.ModelCore.UI.WriteLine("   Calculating BDA Modified Site Resource Dominance.");
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape) {

                if (SiteVars.SiteResourceDom[site] > 0.0)
                {
                    int     lastDisturb = 0;
                    int     duration = 0;
                    double  disturbMod = 0;
                    double  sumDisturbMods = 0.0;
                    double  SRDM = 0.0;

                    // Next check the disturbance types.  This will provide the disturbance modifier
                    // Check for harvest effects on SRD
                    IEnumerable<IDisturbanceType> disturbanceTypes = agent.DisturbanceTypes;
                    foreach (DisturbanceType disturbance in disturbanceTypes)
                    {
                        if (SiteVars.HarvestCohortsKilled != null && SiteVars.HarvestCohortsKilled[site] > 0)
                        {
                            lastDisturb = SiteVars.TimeOfLastHarvest[site];
                            duration = disturbance.MaxAge;

                            if (SiteVars.TimeOfLastHarvest != null && (PlugIn.ModelCore.CurrentTime - lastDisturb <= duration))
                            {
                                foreach (string pName in disturbance.PrescriptionNames)
                                {
                                    if ((SiteVars.HarvestPrescriptionName != null && SiteVars.HarvestPrescriptionName[site].Trim() == pName.Trim()) || (pName.Trim() == "Harvest"))
                                    {
                                        disturbMod = disturbance.SRDModifier * System.Math.Max(0, (double)(PlugIn.ModelCore.CurrentTime - lastDisturb)) / duration;
                                        sumDisturbMods += disturbMod;
                                    }
                                }
                            }
                        }
                        //Check for fire severity effects
                        if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
                        {
                            lastDisturb = SiteVars.TimeOfLastFire[site];
                            duration = disturbance.MaxAge;

                            if (SiteVars.TimeOfLastFire != null && (PlugIn.ModelCore.CurrentTime - lastDisturb <= duration))
                            {
                                foreach (string pName in disturbance.PrescriptionNames)
                                {
                                    if (pName.StartsWith("FireSeverity"))
                                    {
                                        if ((pName.Substring((pName.Length - 1), 1)).ToString() == SiteVars.FireSeverity[site].ToString())
                                        {
                                            disturbMod = disturbance.SRDModifier * System.Math.Max(0, (double)(PlugIn.ModelCore.CurrentTime - lastDisturb)) / duration;
                                            sumDisturbMods += disturbMod;
                                        }
                                    }
                                    else if (pName.Trim() == "Fire") // Generic for all fire
                                    {
                                        disturbMod = disturbance.SRDModifier * System.Math.Max(0, (double)(PlugIn.ModelCore.CurrentTime - lastDisturb)) / duration;
                                        sumDisturbMods += disturbMod;
                                    }
                                }
                            }
                        }
                        //Check for wind severity effects
                        if (SiteVars.WindSeverity != null && SiteVars.WindSeverity[site] > 0)
                        {
                            lastDisturb = SiteVars.TimeOfLastWind[site];
                            duration = disturbance.MaxAge;

                            if (SiteVars.TimeOfLastWind != null &&
                                (PlugIn.ModelCore.CurrentTime - lastDisturb <= duration))
                            {
                                foreach (string pName in disturbance.PrescriptionNames)
                                {
                                    if (pName.StartsWith("WindSeverity"))
                                    {
                                        if ((pName.Substring((pName.Length - 1), 1)).ToString() == SiteVars.WindSeverity[site].ToString())
                                        {
                                            disturbMod = disturbance.SRDModifier * System.Math.Max(0, (double)(PlugIn.ModelCore.CurrentTime - lastDisturb)) / duration;
                                            sumDisturbMods += disturbMod;
                                        }
                                    }
                                    else if (pName.Trim() == "Wind") // Generic for all wind
                                    {
                                        disturbMod = disturbance.SRDModifier * System.Math.Max(0, (double)(PlugIn.ModelCore.CurrentTime - lastDisturb)) / duration;
                                        sumDisturbMods += disturbMod;
                                    }
                                }
                            }
                        }
                        //Check for Biomass Insects effects
                        if (SiteVars.TimeOfLastBiomassInsects != null && SiteVars.TimeOfLastBiomassInsects[site] > 0)
                        {
                            lastDisturb = SiteVars.TimeOfLastBiomassInsects[site];
                            duration = disturbance.MaxAge;

                            if (SiteVars.TimeOfLastBiomassInsects != null && (PlugIn.ModelCore.CurrentTime - lastDisturb <= duration))
                            {
                                foreach (string pName in disturbance.PrescriptionNames)
                                {
                                    if((SiteVars.BiomassInsectsAgent[site].Trim() == pName.Trim()) || (pName.Trim() == "BiomassInsects"))
                                    {
                                        disturbMod = disturbance.SRDModifier * System.Math.Max(0, (double)(PlugIn.ModelCore.CurrentTime - lastDisturb)) / duration;
                                        sumDisturbMods += disturbMod;
                                    }
                                    else if(pName.Contains("BiomassInsectsDefol"))
                                    {
                                        var numAlpha = new Regex("(?<Alpha>[a-zA-Z]+)(?<Numeric>[0-9]+)");
                                        var match = numAlpha.Match(pName.Trim());
                                        var name = match.Groups["Alpha"].Value;
                                        var pct = int.Parse(match.Groups["Numeric"].Value);
                                        if ((name != "BiomassInsectsDefol") || (pct < 0) || (pct > 100))
                                        {
                                            string mesg = string.Format("Disturbance modifier using BiomassInsectsDefol must use the format BiomassInsectsDefol##, where ## is the percent defoliation threshold between 0 and 100.");
                                            throw new System.ApplicationException(mesg);
                                        }
                                        
                                        if(SiteVars.BiomassInsectsDefol[site] >= pct)
                                        {
                                            disturbMod = disturbance.SRDModifier * System.Math.Max(0, (double)(PlugIn.ModelCore.CurrentTime - lastDisturb)) / duration;
                                            sumDisturbMods += disturbMod;
                                        }
                                    }
                                }
                            }
                        }
                        //Check for other BDA agent effects
                        if (SiteVars.TimeOfLastEvent[site] > 0)
                        {
                            lastDisturb = SiteVars.TimeOfLastEvent[site];
                            duration = disturbance.MaxAge;

                            if (PlugIn.ModelCore.CurrentTime - lastDisturb <= duration)
                            {
                                foreach (string pName in disturbance.PrescriptionNames)
                                {
                                    if ((SiteVars.AgentName[site].Trim() == pName.Trim()) || (pName.Trim() == "BDA"))
                                    {
                                        disturbMod = disturbance.SRDModifier * System.Math.Max(0, (double)(PlugIn.ModelCore.CurrentTime - lastDisturb)) / duration;
                                        sumDisturbMods += disturbMod;
                                    }
                                }
                            }
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
            PlugIn.ModelCore.UI.WriteLine("   Calculating BDA Neighborhood Resource Dominance.");

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
                                //if (SiteVars.SiteResourceDomMod[activeSite] > 0)  //BRM - 092315 - Turned off this restriction so that non-host in neighborhood reduces calculated value
                                //{
                                    totalNeighborWeight += SiteVars.SiteResourceDomMod[activeSite] * neighbor.Weight;
                                    maxNeighborWeight += neighbor.Weight;
                                //}
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
