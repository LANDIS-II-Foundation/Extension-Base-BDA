//  Copyright 2005 University of Wisconsin
//  Authors:  
//      Robert M. Scheller
//      James B. Domingo
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.
//  Version 1.0
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.Cohorts;
using Landis.Ecoregions;
using Landis.Landscape;
using Landis.Species;
using Landis.Util;
using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Grids;

namespace Landis.BDA
{
    public class Epicenters
    {

        //---- NewEpicenters ------------------------------------------
        ///<summary>
        ///First, determine if new epicenter list required.
        ///If so, create the list.
        ///If not, create epicenter list from OUTSIDE of previous outbreak zones (only
        ///if User specified SeedEpicenter)
        ///If not, create epicenter list from INSIDE previous outbreak zones.
        ///The final epicenter list then spreads to neighboring areas of the landscape -
        ///see SpreadEpicenters()
        ///</summary>
        //---- NewEpicenters ------------------------------------------
        public static void NewEpicenters(IAgent agent, int BDAtimestep)
        {
            
            UI.WriteLine("Creating New Epicenters.");
            
            int numRows = (int) Model.Core.Landscape.Rows;
            int numCols = (int) Model.Core.Landscape.Columns;
            int epicenterNum = 0;
            int numInside = 0;
            int numOutside = 0;

            bool firstIteration = true;
            int oldEpicenterNum = agent.EpicenterNum;
            List<Location> newSiteList = new List<Location>(0);
            
            //Is the landscape empty of previous outbreak events?
            //If so, then generate a new list of epicenters.
            foreach(ActiveSite asite in Model.Core.Landscape) 
            {
                if (agent.OutbreakZone[asite] == Zone.Lastzone)
                {
                    firstIteration = false;  
                    continue;
                }
            }

            //Generate New Epicenters based on the location of past outbreaks
            //and the vulnerability of the current landscape:
            if(!firstIteration)
            {
                List<Location> oldZoneSiteList = new List<Location>(0);

                //---------------------------------------------------------
                //Count the number of potential new inside and outside epicenters:
                foreach(ActiveSite site in Model.Core.Landscape) 
                {
                    if (SiteVars.Vulnerability[site] > agent.EpidemicThresh) //potential new epicenter
                    {
                        if (agent.Severity[site]>0)
                        {
                            numInside ++;//potential new epicenter inside last OutbreakZone
                            oldZoneSiteList.Add(site.Location);
                            //UI.WriteLine("  Severity = {0}.  Zone = {1}.", agent.Severity[site], agent.OutbreakZone[site]);
                            
                        }
                        if(agent.OutbreakZone[site] != Zone.Lastzone)
                            numOutside++;//potential new epicenter outside last OutbreakZone
                    }
                }
                
                UI.WriteLine("  Potential Number of Epicenters, Inside = {0}; Outside={1}.", numInside, numOutside);
                
                //---------------------------------------------------------
                //Calculate number of Epicenters that will occur 
                //INSIDE the last epidemic outbreak area.  
                //This always occurs after the first iteration.
                UI.WriteLine("Adding epicenters INSIDE last outbreak zone.");
                Random.Shuffle(oldZoneSiteList);
                //UI.WriteLine("   Potential Number Inside = {0}.", oldZoneSiteList.Count);

                numInside = (int)((double) numInside * 
                            System.Math.Exp(-1.0 * agent.OutbreakEpicenterCoeff * oldEpicenterNum));

                UI.WriteLine("   Actual Number Inside = {0}.", numInside);

                int listIndex = 0;
                if(oldZoneSiteList.Count > 0)
                {
                    while (numInside > 0) 
                    {
                        newSiteList.Add(oldZoneSiteList[listIndex]);
                        //UI.WriteLine("   New Site Added INSIDE outbreak area.");
                        epicenterNum ++;
                        numInside--;
                        listIndex++;    
                    } //endwhile
                }
                
                //---------------------------------------------------------
                //SeedEpicenter determines if new epicenters will seed new outbreaks 
                //OUTSIDE of previous outbreak zones.  
                if (agent.SeedEpicenter)
                {
                    UI.WriteLine("Adding epicenters OUTSIDE last outbreak zone.");

                    numOutside = (int)((double) numOutside * 
                                    System.Math.Exp(-1.0 * (double) agent.SeedEpicenterCoeff * (double) oldEpicenterNum));
                    UI.WriteLine("   Actual Number Outside = {0}.  SeedCoef = {1}.  OldEpiNum = {2}.", numOutside, agent.SeedEpicenterCoeff, oldEpicenterNum);
                    //UI.WriteLine("   Actual Number Outside = {0}.", numOutside);

                    while (numOutside > 0)
                    {
                        uint i, j;
                        i = (uint) (Random.GenerateUniform() * numRows) + 1;
                        j = (uint) (Random.GenerateUniform() * numCols) + 1;
                        
                        Site rsite = Model.Core.Landscape[i,j];
                        if(rsite != null && rsite.IsActive)
                        {
                            if (SiteVars.Vulnerability[rsite]  > agent.EpidemicThresh &&
                                agent.OutbreakZone[rsite] == Zone.Nozone)  
                            {
                                newSiteList.Add(rsite.Location);
                                //UI.WriteLine("   New Site Added OUTSIDE outbreak area.");
                                epicenterNum ++;
                                //numOutside--;
                                numOutside = (int)((double) numOutside * 
                                    System.Math.Exp(-1.0 * (double) agent.SeedEpicenterCoeff * (double) epicenterNum));
                            }
                        }
                    }
                }
                
            } //end !firstIteration

            //If necessary, create list from scratch without
            //consideration of previous outbreaks.
            if (firstIteration || epicenterNum == 0) 
            {
                uint i, j;
                //int cnt = 0;
                
                while (epicenterNum < oldEpicenterNum)
                {
                    i = (uint) (Random.GenerateUniform() * numRows) + 1;
                    j = (uint) (Random.GenerateUniform() * numCols) + 1;

                    ActiveSite site = Model.Core.Landscape[i,j];
                    if(site != null && site.IsActive)
                    {
                        newSiteList.Add(site.Location);
                        epicenterNum++;
                    }
                }
                UI.WriteLine("No Prior Outbreaks OR No available sites within prior outbreaks.  EpicenterNum = {0}.", newSiteList.Count);
            } 
            
            agent.EpicenterNum = epicenterNum;

            //Generate NEW outbreak zones
            SpreadEpicenters(agent, newSiteList, BDAtimestep);
            
            return;
        }

        //---------------------------------------------------------------------
        ///<summary>
        ///Spread from Epicenters to outbreak zone using either a fixed radius method 
        ///or a percolation method with variable neighborhood sizes.
        ///</summary>
        //---------------------------------------------------------------------
        private static void SpreadEpicenters(IAgent agent, 
                                            List<Location> iSites,
                                            int BDAtimestep)
        {
            UI.WriteLine("Spreading to New Epicenters.  There are {0} initiation sites.", iSites.Count);
        
            if(iSites == null)
                UI.WriteLine("ERROR:  The newSiteList is empty.");    
            int dispersalDistance = agent.DispersalRate * BDAtimestep;
            //UI.WriteLine("Dispersal Distance = {0}.", dispersalDistance);
//            foreach(Site initiationSite in iSites)
//                UI.WriteLine("Zone = {0}.", agent.OutbreakZone[initiationSite]);

        
            foreach(Location siteLocation in iSites)
            {
                Site initiationSite = Model.Core.Landscape.GetSite(siteLocation);
                
                if(agent.DispersalTemp == DispersalTemplate.MaxRadius)
                {

                    foreach (RelativeLocation relativeLoc in agent.DispersalNeighbors) 
                    {
                        Site neighbor = initiationSite.GetNeighbor(relativeLoc);
                        if (neighbor != null && neighbor.IsActive)
                            agent.OutbreakZone[neighbor] = Zone.Newzone;
                    }
                }
                if(agent.DispersalTemp != DispersalTemplate.MaxRadius)
                {
                    //UI.WriteLine("   Begin Percolation Spread to Neighbors.");
                    //Queue<Site> sitesToConsider = new Queue<Site>();
                    System.Collections.Queue sitesToConsider = new System.Collections.Queue();
                    sitesToConsider.Enqueue(initiationSite);

                    while (sitesToConsider.Count > 0 ) 
                    {
                        //UI.WriteLine("   There are {0} neighbors being processed.", sitesToConsider.Count);

                        Site site = (Site)sitesToConsider.Dequeue();
                        agent.OutbreakZone[site] = Zone.Newzone;
                
                        foreach (RelativeLocation relativeLoc in agent.DispersalNeighbors) 
                        {
                            Site neighbor = site.GetNeighbor(relativeLoc);
                            
                            //Do not spread to inactive neighbors:
                            if(neighbor == null || !neighbor.IsActive)
                                continue;
                            //Do NOT spread to neighbors that have already been targeted for 
                            //disturbance:
                            if (agent.OutbreakZone[neighbor] == Zone.Newzone)
                                continue;
                            //Check for duplicates:
                            if (sitesToConsider.Contains(neighbor))
                                continue;
                                
                            //UI.WriteLine("Distance between neighbor and center = {0}.", DistanceBetweenSites(neighbor, initiationSite));
                            //UI.WriteLine("SV={0:0.0}.", SiteVars.Vulnerability[neighbor]);
                            //UI.WriteLine("Threshold={0:0.0}.", agent.EpidemicThresh);
                            if(DistanceBetweenSites(neighbor, initiationSite) <= dispersalDistance
                                && SiteVars.Vulnerability[neighbor] > agent.EpidemicThresh)
                            {
                                sitesToConsider.Enqueue(neighbor);
                            }
                        }
                    }
                }
            }
                
        }

        //-------------------------------------------------------
        ///<summary>
        ///Calculate the distance between two Sites
        ///</summary>
        public static double DistanceBetweenSites(Site a, Site b)
        {
        
            int Col = (int) a.Location.Column - (int) b.Location.Column;
            int Row = (int) a.Location.Row - (int) b.Location.Row;
            
            double aSq = System.Math.Pow(Col,2);
            double bSq = System.Math.Pow(Row,2);
            //UI.WriteLine("Col={0}, Row={1}.", Col, Row);
            //UI.WriteLine("aSq={0}, bSq={1}.", aSq, bSq);
            //UI.WriteLine("Distance in Grid Units = {0}.", System.Math.Sqrt(aSq + bSq));
            return (System.Math.Sqrt(aSq + bSq) * (double) Model.Core.CellLength);
        
        }

    }
}
