//  Copyright 2005 University of Wisconsin
//  Authors:
//      Robert M. Scheller
//      James B. Domingo
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.
//  Version 1.0
//  License:  Available at
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.AgeCohort;
using Landis.Ecoregions;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.RasterIO;
using Landis.Util;
using System.Collections.Generic;
using System.IO;
using System;
using Edu.Wisc.Forest.Flel.Grids;
using Troschuetz.Random;

namespace Landis.BDA
{
    ///<summary>
    /// A disturbance plug-in that simulates Biological Agents.
    /// </summary>

    public class PlugIn
        : PlugIns.PlugIn, PlugIns.I2PhaseInitialization
    {
        public static readonly PlugInType Type = new PlugInType("disturbance:bda");

        private static ILandscapeCohorts cohorts;
        private string mapNameTemplate;
        private string srdMapNames;
        private string nrdMapNames;
        private StreamWriter log;
        private IEnumerable<IAgent> manyAgentParameters;

        //---------------------------------------------------------------------

        public PlugIn()
            : base("Base BDA", Type)
        {
        }

        public static ILandscapeCohorts Cohorts
        {
            get
            {
                return cohorts;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Initializes the extension with a data file.
        /// </summary>
        public override void Initialize(string        dataFile,
                                        PlugIns.ICore modelCore)
        {

            Model.Core = modelCore;

            InputParameterParser.EcoregionsDataset = Model.Core.Ecoregions;
            InputParameterParser parser = new InputParameterParser();
            IInputParameters parameters = Data.Load<IInputParameters>(dataFile, parser);

            Timestep = parameters.Timestep;
            mapNameTemplate = parameters.MapNamesTemplate;
            srdMapNames = parameters.SRDMapNames;
            nrdMapNames = parameters.NRDMapNames;

            cohorts = Model.Core.SuccessionCohorts as ILandscapeCohorts;
            if (cohorts == null)
                throw new ApplicationException("Error: Cohorts don't support age-cohort interface");

            SiteVars.Initialize(cohorts);

            manyAgentParameters = parameters.ManyAgentParameters;
            foreach(IAgent activeAgent in manyAgentParameters)
            //   UI.WriteLine("Parameters did not load successfully.");

            {

                //foreach (ISpecies spp in Model.Species)
                //    UI.WriteLine("Spp={0}, MinorHostAge={1}.", spp.Name, activeAgent.SppParameters[spp.Index].SecondaryHostAge);

                if(activeAgent == null)
                    UI.WriteLine("Agent Parameters NOT loading correctly.");
                //UI.WriteLine("Name of Agent = {0}", activeAgent.AgentName);
                activeAgent.TimeToNextEpidemic = TimeToNext(activeAgent, Timestep);

                int i=0;

                activeAgent.DispersalNeighbors
                    = GetDispersalNeighborhood(activeAgent, Timestep);
                if(activeAgent.DispersalNeighbors != null)
                {
                    foreach (RelativeLocation reloc in activeAgent.DispersalNeighbors) i++;
                    UI.WriteLine("Dispersal Neighborhood = {0} neighbors.", i);
                }

                i=0;
                activeAgent.ResourceNeighbors = GetResourceNeighborhood(activeAgent);
                if(activeAgent.ResourceNeighbors != null)
                {
                    foreach (RelativeLocationWeighted reloc in activeAgent.ResourceNeighbors) i++;
                    UI.WriteLine("Resource Neighborhood = {0} neighbors.", i);
                }
            }

            string logFileName = parameters.LogFileName;
            UI.WriteLine("Opening BDA log file \"{0}\" ...", logFileName);
            log = Data.CreateTextFile(logFileName);
            log.AutoFlush = true;
            log.Write("CurrentTime, ROS, NumCohortsKilled, NumSitesDamaged, MeanSeverity");
            log.WriteLine("");

        }

        public void InitializePhase2() //InitializePhase2
        {
                SiteVars.InitializeTimeOfLastDisturbances();
        }

        //---------------------------------------------------------------------
        ///<summary>
        /// Run the BDA extension at a particular timestep.
        ///</summary>
        public override void Run()
        {
            UI.WriteLine("Processing landscape for BDA events ...");

            //SiteVars.Epidemic.SiteValues = null;

            int eventCount = 0;

            foreach(IAgent activeAgent in manyAgentParameters)
            {

                activeAgent.TimeSinceLastEpidemic += Timestep;
                //UI.WriteLine("TimeSince={0}, TimeToNext={1}", activeAgent.TimeSinceLastEpidemic, activeAgent.TimeToNextEpidemic);
                    //(activeAgent.TimeSinceLastEpidemic+ activeAgent.TimeToNextEpidemic));

                int ROS = RegionalOutbreakStatus(activeAgent, Timestep);

                if(ROS > 0)
                {
                    Epidemic.Initialize(activeAgent);
                    Epidemic currentEpic = Epidemic.Simulate(activeAgent,
                        Model.Core.CurrentTime,
                        Timestep,
                        ROS);
                    //activeAgent.TimeSinceLastEpidemic = activeAgent.TimeSinceLastEpidemic + Timestep;

                    if (currentEpic != null)
                    {
                        LogEvent(Model.Core.CurrentTime, currentEpic, ROS);

                        //----- Write BDA severity maps --------
                        IOutputRaster<SeverityPixel> map = CreateMap(Model.Core.CurrentTime, activeAgent.AgentName);
                        using (map) {
                            SeverityPixel pixel = new SeverityPixel();
                            foreach (Site site in Model.Core.Landscape.AllSites) {
                                if (site.IsActive) {
                                    if (SiteVars.Disturbed[site])
                                        pixel.Band0 = (byte) (activeAgent.Severity[site] + 1);
                                    else
                                        pixel.Band0 = 1;
                                }
                                else {
                                    //  Inactive site
                                    pixel.Band0 = 0;
                                }
                                map.WritePixel(pixel);
                            }
                        }
                        if (!(srdMapNames == ""))
                        {
                            //----- Write BDA SRD maps --------
                            IOutputRaster<UShortPixel> srdmap = CreateSRDMap(Model.Core.CurrentTime, activeAgent.AgentName);
                            using (srdmap)
                            {
                                UShortPixel pixel = new UShortPixel();
                                foreach (Site site in Model.Core.Landscape.AllSites)
                                {
                                    if (site.IsActive)
                                    {
                                        pixel.Band0 = (ushort)System.Math.Round(SiteVars.SiteResourceDom[site] * 100.00);
                                    }
                                    else
                                    {
                                        //  Inactive site
                                        pixel.Band0 = 0;
                                    }
                                    srdmap.WritePixel(pixel);
                                }
                            }
                        }
                        if (!(nrdMapNames == ""))
                        {
                            //----- Write BDA NRD maps --------
                            IOutputRaster<UShortPixel> nrdmap = CreateNRDMap(Model.Core.CurrentTime, activeAgent.AgentName);
                            using (nrdmap)
                            {
                                UShortPixel pixel = new UShortPixel();
                                foreach (Site site in Model.Core.Landscape.AllSites)
                                {
                                    if (site.IsActive)
                                    {
                                        pixel.Band0 = (ushort)System.Math.Round(SiteVars.NeighborResourceDom[site] * 100.00);
                                    }
                                    else
                                    {
                                        //  Inactive site
                                        pixel.Band0 = 0;
                                    }
                                    nrdmap.WritePixel(pixel);
                                }
                            }
                        }

                        //----- Write Site Vulnerability or OutbreakZone maps --------
                        //----- USED FOR LANDSCAPE TESTING -----------
                        /*IOutputRaster<UShortPixel> dmap = CreateUShortMap(Model.Core.CurrentTimestep, activeAgent.AgentName);
                        using (dmap) {
                            UShortPixel pixel = new UShortPixel();
                            foreach (Site site in Model.Core.Landscape.AllSites) {
                                if (site.IsActive) {
                                    //pixel.Band0 = (ushort) (activeAgent.OutbreakZone[site]);
                                    pixel.Band0 = (ushort) (SiteVars.Vulnerability[site] * 100);
                                }
                                else {
                                    //  Inactive site
                                    pixel.Band0 = 0;
                                }
                                dmap.WritePixel(pixel);
                            }
                        }*/

                        /*IOutputRaster<UShortPixel> nmap = CreateUShortMap2(Model.Core.CurrentTimestep, activeAgent.AgentName);
                        using (nmap) {
                            UShortPixel pixel = new UShortPixel();
                            foreach (Site site in Model.Landscape.AllSites) {
                                if (site.IsActive) {
                                    //pixel.Band0 = (ushort) (activeAgent.OutbreakZone[site]);
                                    pixel.Band0 = (ushort) System.Math.Round(SiteVars.NeighborResourceDom[site] * 100.00);
                                }
                                else {
                                    //  Inactive site
                                    pixel.Band0 = 0;
                                }
                                nmap.WritePixel(pixel);
                            }
                        }*/

                        eventCount++;
                    }
                }
            }
        }

        //---------------------------------------------------------------------
        private void LogEvent(int   currentTime,
                              Epidemic CurrentEvent,
                              int ROS)
        {
            log.Write("{0},{1},{2},{3},{4:0.0}",
                      currentTime,
                      ROS,
                      CurrentEvent.CohortsKilled,
                      CurrentEvent.TotalSitesDamaged,
                      CurrentEvent.MeanSeverity);
            //foreach (IEcoregion ecoregion in Model.Ecoregions)
            //{
                //if(ecoregion.Active)
                    //log.Write(",{0}", CurrentEvent.SitesInEvent[ecoregion.Index]);
            //}
            log.WriteLine("");
        }

        //---------------------------------------------------------------------
        private IOutputRaster<SeverityPixel> CreateMap(int currentTime, string agentName)
        {
            string path = MapNames.ReplaceTemplateVars(mapNameTemplate, agentName, currentTime);
            UI.WriteLine("Writing BDA severity map to {0} ...", path);
            return Model.Core.CreateRaster<SeverityPixel>(path,
                                                          Model.Core.Landscape.Dimensions,
                                                          Model.Core.LandscapeMapMetadata);
        }

        private IOutputRaster<UShortPixel> CreateSRDMap(int currentTime, string agentName)
        {
            string path = MapNames.ReplaceTemplateVars(srdMapNames, agentName, currentTime);
            UI.WriteLine("Writing BDA SRD map to {0} ...", path);
            return Model.Core.CreateRaster<UShortPixel>(path,
                                                          Model.Core.Landscape.Dimensions,
                                                          Model.Core.LandscapeMapMetadata);
        }

        private IOutputRaster<UShortPixel> CreateNRDMap(int currentTime, string agentName)
        {
            string path = MapNames.ReplaceTemplateVars(nrdMapNames, agentName, currentTime);
            UI.WriteLine("Writing BDA NRD map to {0} ...", path);
            return Model.Core.CreateRaster<UShortPixel>(path,
                                                          Model.Core.Landscape.Dimensions,
                                                          Model.Core.LandscapeMapMetadata);
        }
        // TWO FUNCTIONS BELOW USED FOR LANDSCAPE TESTING
        /*private IOutputRaster<UShortPixel> CreateUShortMap(int currentTime, string agentName)
        {
            string path = MapNames.ReplaceTemplateVars("./tests/BDA/{agentName}/vulnerability-{timestep}.gis", agentName, currentTime);
//            string path = MapNames.ReplaceTemplateVars("./tests/BDA/{agentName}/outbreakzones-{timestep}.gis", agentName, currentTime);
            UI.WriteLine("Writing BDA output (with ushort values) map to {0} ...", path);
            return Util.Raster.Create<UShortPixel>(path,
                                                     Model.LandscapeMapDims,
                                                     Model.LandscapeMapMetadata);
        }
        private IOutputRaster<UShortPixel> CreateUShortMap2(int currentTime, string agentName)
        {
            string path = MapNames.ReplaceTemplateVars("./tests/BDA/{agentName}/neighbor_resources-{timestep}.gis", agentName, currentTime);
            UI.WriteLine("Writing BDA output (with ushort values) map to {0} ...", path);
            return Util.Raster.Create<UShortPixel>(path,
                                                     Model.LandscapeMapDims,
                                                     Model.LandscapeMapMetadata);
        }*/
        //---------------------------------------------------------------------
        private static int TimeToNext(IAgent activeAgent, int Timestep)
        {
            int timeToNext = 0;
            if (activeAgent.RandFunc == RandomFunction.RFuniform){
                int MaxI = (int) Math.Round(activeAgent.RandomParameter1);
                int MinI = (int) Math.Round(activeAgent.RandomParameter2);
                timeToNext =
                   (MinI - activeAgent.TimeSinceLastEpidemic) +
                   (int) (Landis.Util.Random.GenerateUniform() * (MaxI - MinI));

                //UI.WriteLine("RFuniform: TimeSince={0}, TimeToNext={1}", activeAgent.TimeSinceLastEpidemic, timeToNext);
                //    (activeAgent.TimeSinceLastEpidemic + timeToNext));

            }
            else if (activeAgent.RandFunc == RandomFunction.RFnormal){
                /*NormalRandomVar randVar = new NormalRandomVar(activeAgent.RandomParameter1, activeAgent.RandomParameter2);
                int randNum = (int)randVar.GenerateNumber();
                timeToNext = randNum - activeAgent.TimeSinceLastEpidemic ;
                */

                NormalDistribution randVar = new NormalDistribution(RandomNumberGenerator.Singleton);
                randVar.Mu = activeAgent.RandomParameter1;
                randVar.Sigma = activeAgent.RandomParameter2;
                int randNum = (int)randVar.NextDouble();
                timeToNext = randNum - activeAgent.TimeSinceLastEpidemic;


                // Interval times are always rounded up to the next time step increment.
                // This bias can be removed by reducing times by half the time step.
                timeToNext = timeToNext - (Timestep / 2);

                //UI.WriteLine("RFnormal: TimeSince={0}, TimeToNext={1}", activeAgent.TimeSinceLastEpidemic, timeToNext);
                    //(activeAgent.TimeSinceLastEpidemic + timeToNext));
                if (timeToNext < 0) timeToNext = 0;
            }
            return timeToNext;
        }

        //---------------------------------------------------------------------
        //Calculate the Regional Outbreak Status (ROS) - the landscape scale intensity
        //of an outbreak or epidemic.
        //Units are from 0 (no outbreak) to 3 (most intense outbreak)

        private static int RegionalOutbreakStatus(IAgent activeAgent, int BDAtimestep)
        {
            int ROS = 0;

            if(activeAgent.TimeToNextEpidemic <= activeAgent.TimeSinceLastEpidemic)
            {

                activeAgent.TimeSinceLastEpidemic = 0;
                activeAgent.TimeToNextEpidemic = TimeToNext(activeAgent, BDAtimestep);

                //calculate ROS
                if (activeAgent.TempType == TemporalType.pulse)
                    ROS = activeAgent.MaxROS;
                else if (activeAgent.TempType == TemporalType.variablepulse)
                {
                    //randomly select an ROS netween ROSmin and ROSmax
                    //ROS = (int) (Landis.Util.Random.GenerateUniform() *
                    //      (double) (activeAgent.MaxROS - activeAgent.MinROS + 1)) +
                    //      activeAgent.MinROS;

                    // Correction suggested by Brian Miranda, March 2008
                    ROS = (int) (Landis.Util.Random.GenerateUniform() *
                          (double) (activeAgent.MaxROS - activeAgent.MinROS)) + 1 +
                          activeAgent.MinROS;

                }

            } else  {
                //activeAgent.TimeSinceLastEpidemic += BDAtimestep;
                ROS = activeAgent.MinROS;
            }
            return ROS;

        }

        //---------------------------------------------------------------------
        //Generate a Relative Location array (with WEIGHTS) of neighbors.
        //Check each cell within a block surrounding the center point.  This will
        //create a set of POTENTIAL neighbors.  These potential neighbors
        //will need to be later checked to ensure that they are within the landscape
        // and active.

        private static IEnumerable<RelativeLocationWeighted> GetResourceNeighborhood(IAgent agent)
        {
            float CellLength = Model.Core.CellLength;
            UI.WriteLine("Creating Neighborhood List.");
            int neighborRadius = agent.NeighborRadius;
            int numCellRadius = (int) ((double) neighborRadius / CellLength) ;
            UI.WriteLine("NeighborRadius={0}, CellLength={1}, numCellRadius={2}",
                        neighborRadius, CellLength, numCellRadius);

            double centroidDistance = 0;
            double cellLength = CellLength;
            double neighborWeight = 0;

            List<RelativeLocationWeighted> neighborhood = new List<RelativeLocationWeighted>();

            for(int row=(numCellRadius * -1); row<=numCellRadius; row++)
            {
                for(int col=(numCellRadius * -1); col<=numCellRadius; col++)
                {
                    neighborWeight = 0;
                    centroidDistance = DistanceFromCenter(row ,col);
                    //UI.WriteLine("Centroid Distance = {0}.", centroidDistance);
                    if(centroidDistance  <= neighborRadius && centroidDistance > 0)
                    {

                        if(agent.ShapeOfNeighbor == NeighborShape.uniform)
                            neighborWeight = 1.0;
                        if(agent.ShapeOfNeighbor == NeighborShape.linear)
                        {
                            //neighborWeight = (neighborRadius - centroidDistance + (cellLength/2)) / (double) neighborRadius;
                            neighborWeight = 1.0 - (centroidDistance / (double) neighborRadius);
                        }
                        if(agent.ShapeOfNeighbor == NeighborShape.gaussian)
                        {
                            double halfRadius = neighborRadius / 2;
                                neighborWeight = (float)
                                System.Math.Exp(-1 *
                                System.Math.Pow(centroidDistance, 2) /
                                System.Math.Pow(halfRadius, 2));
                        }

                        RelativeLocation reloc = new RelativeLocation(row, col);
                        neighborhood.Add(new RelativeLocationWeighted(reloc, neighborWeight));
                    }
                }
            }
            return neighborhood;
        }

        //---------------------------------------------------------------------
        // Generate a Relative Location array of neighbors.
        // Check each cell within a circle surrounding the center point.  This will
        // create a set of POTENTIAL neighbors.  These potential neighbors
        // will need to be later checked to ensure that they are within the landscape
        // and active.

        private static IEnumerable<RelativeLocation> GetDispersalNeighborhood(IAgent agent, int timestep)
        {
            double CellLength = Model.Core.CellLength;
            UI.WriteLine("Creating Dispersal Neighborhood List.");

            List<RelativeLocation> neighborhood = new List<RelativeLocation>();

            if(agent.DispersalTemp == DispersalTemplate.N4)
                neighborhood = GetNeighbors(4);

            else if(agent.DispersalTemp == DispersalTemplate.N8)
                neighborhood = GetNeighbors(8);

            else if(agent.DispersalTemp == DispersalTemplate.N12)
                neighborhood = GetNeighbors(12);

            else if(agent.DispersalTemp == DispersalTemplate.N24)
                neighborhood = GetNeighbors(24);

            else if(agent.DispersalTemp == DispersalTemplate.MaxRadius)
            {
                int neighborRadius = agent.DispersalRate * timestep;
                int numCellRadius = (int) (neighborRadius / CellLength);
                UI.WriteLine("NeighborRadius={0}, CellLength={1}, numCellRadius={2}",
                        neighborRadius, CellLength, numCellRadius);
                double centroidDistance = 0;
                double cellLength = CellLength;

                for(int row=(numCellRadius * -1); row<=numCellRadius; row++)
                {
                    for(int col=(numCellRadius * -1); col<=numCellRadius; col++)
                    {
                        centroidDistance = DistanceFromCenter(row, col);

                        //UI.WriteLine("Centroid Distance = {0}.", centroidDistance);
                        if(centroidDistance  <= neighborRadius)
                        {
                            neighborhood.Add(new RelativeLocation(row,  col));
                        }
                    }
                }
            }
            return neighborhood;
        }

        //-------------------------------------------------------
        //Calculate the distance from a location to a center
        //point (row and column = 0).
        private static double DistanceFromCenter(double row, double column)
        {
            double CellLength = Model.Core.CellLength;
            row = System.Math.Abs(row) * CellLength;
            column = System.Math.Abs(column) * CellLength;
            double aSq = System.Math.Pow(column,2);
            double bSq = System.Math.Pow(row,2);
            return System.Math.Sqrt(aSq + bSq);
        }


        //---------------------------------------------------------------------
        //Generate List of 4,8,12, or 24 nearest neighbors.
        //---------------------------------------------------------------------
        private static List<RelativeLocation> GetNeighbors(int numNeighbors)
        {

            RelativeLocation[] neighborhood4 = new RelativeLocation[] {
                new RelativeLocation( 0,  1),   // east
                new RelativeLocation( 1,  0),   // south
                new RelativeLocation( 0, -1),   // west
                new RelativeLocation(-1,  0),   // north
            };

            RelativeLocation[] neighborhood8 = new RelativeLocation[] {
                new RelativeLocation(-1,  1),   // northeast
                new RelativeLocation( 1,  1),   // southeast
                new RelativeLocation( 1,  -1),  // southwest
                new RelativeLocation( -1, -1),  // northwest
            };

            RelativeLocation[] neighborhood12 = new RelativeLocation[] {
                new RelativeLocation(-2,  0),   // north north
                new RelativeLocation( 0,  2),   // east east
                new RelativeLocation( 2,  0),   // south south
                new RelativeLocation( 0, -2),   // west west
            };

            RelativeLocation[] neighborhood24 = new RelativeLocation[] {
                new RelativeLocation(-2,  -2),  // northwest northwest
                new RelativeLocation( -2,  -1),  // northwest south
                new RelativeLocation( -1,  -2),   // northwest east
                new RelativeLocation( -2,  2),   // northeast northeast
                new RelativeLocation( -2,  1),  // northeast west
                new RelativeLocation( -1, 2),   // northeast south
                new RelativeLocation( 2, 2),  // southeast southeast
                new RelativeLocation(1,  2),   // southeast north
                new RelativeLocation(2,  1),   //southeast west
                new RelativeLocation( 2,  -2),   // southwest southwest
                new RelativeLocation( 2,  -1),   // southwest east
                new RelativeLocation( 1, -2),   // southwest north
            };

            //Progressively add neighbors as necessary:
            List<RelativeLocation> neighbors = new List<RelativeLocation>();
            foreach (RelativeLocation relativeLoc in neighborhood4)
                neighbors.Add(relativeLoc);
            if(numNeighbors <= 4)  return neighbors;

            foreach (RelativeLocation relativeLoc in neighborhood8)
                neighbors.Add(relativeLoc);
            if(numNeighbors <= 8)  return neighbors;

            foreach (RelativeLocation relativeLoc in neighborhood12)
                neighbors.Add(relativeLoc);
            if(numNeighbors <= 12)  return neighbors;

            foreach (RelativeLocation relativeLoc in neighborhood24)
                neighbors.Add(relativeLoc);

            return neighbors;

        }

    }

}
