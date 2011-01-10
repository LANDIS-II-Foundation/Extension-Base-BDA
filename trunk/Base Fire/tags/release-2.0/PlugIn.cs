//  Copyright 2005-2010 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.AgeCohort;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.RasterIO;
using Landis.Util;
using System.Collections.Generic;
using System.IO;
using System;

namespace Landis.Fire
{
    ///<summary>
    /// A disturbance plug-in that simulates Fire disturbance.
    /// </summary>
    public class PlugIn
        : PlugIns.PlugIn, PlugIns.I2PhaseInitialization
    {
        public static readonly PlugInType Type = new PlugInType("disturbance:fire");

        private ILandscapeCohorts cohorts;
        private string mapNameTemplate;
        private StreamWriter log;
        private StreamWriter summaryLog;
        private int[] summaryFireRegionEventCount;
        private int summaryTotalSites;
        private int summaryEventCount;
        private List<IDynamicFireRegion> dynamicEcos;

        //---------------------------------------------------------------------

        public PlugIn()
            : base("Base Fire", Type)
        {
        }


        //---------------------------------------------------------------------

        public override void Initialize(string        dataFile,
                                        PlugIns.ICore modelCore)
        {
            Model.Core = modelCore;

            cohorts = Model.Core.SuccessionCohorts as ILandscapeCohorts;
            if (cohorts == null)
                throw new ApplicationException("Error: Cohorts don't support age-cohort interface");

            SiteVars.Initialize(cohorts);

            ParameterParser parser = new ParameterParser();
            IInputParameters parameters = Data.Load<IInputParameters>(dataFile, parser);

            Timestep = parameters.Timestep;
            mapNameTemplate = parameters.MapNamesTemplate;
            dynamicEcos = parameters.DynamicFireRegions;

            summaryFireRegionEventCount = new int[FireRegions.Dataset.Count];

            Event.Initialize(parameters.FireDamages);

            UI.WriteLine("Opening Fire log file \"{0}\" ...", parameters.LogFileName);
            log = Data.CreateTextFile(parameters.LogFileName);
            log.AutoFlush = true;
            log.Write("Time,InitialSiteRow,InitialSiteColumn,SitesChecked,CohortsKilled,MeanSeverity,");
            foreach (IFireRegion ecoregion in FireRegions.Dataset)
                log.Write("{0},", ecoregion.Name);
            log.Write("TotalBurnedSites");
            log.WriteLine("");

            summaryLog = Data.CreateTextFile(parameters.SummaryLogFileName);
            summaryLog.AutoFlush = true;
            summaryLog.Write("Time,TotalSitesBurned,TotalNumberEvents");
            foreach (IFireRegion ecoregion in FireRegions.Dataset)
                summaryLog.Write(",{0}", ecoregion.Name);
            summaryLog.WriteLine("");


        }

        //---------------------------------------------------------------------

        void PlugIns.I2PhaseInitialization.InitializePhase2()
        {
            SiteVars.InitializeTimeOfLastWind();
        }

        //---------------------------------------------------------------------

        ///<summary>
        /// Run the plug-in at a particular timestep.
        ///</summary>
        public override void Run()
        {
            UI.WriteLine("Processing landscape for Fire events ...");

            SiteVars.Event.SiteValues = null;
            SiteVars.Severity.ActiveSiteValues = 0;
            SiteVars.Disturbed.ActiveSiteValues = false;

            // Update the FireRegions Map as necessary:
            foreach(IDynamicFireRegion dyneco in dynamicEcos)
            {
                 if(dyneco.Year == Model.Core.CurrentTime)
                 {
                    UI.WriteLine("   Reading in new Fire Regions Map {0}.", dyneco.MapName);
                    FireRegions.ReadMap(dyneco.MapName);
                 }
            }

            foreach (IFireRegion fireregion in FireRegions.Dataset)
            {
                summaryFireRegionEventCount[fireregion.Index] = 0;
            }

            summaryTotalSites = 0;
            summaryEventCount = 0;

            foreach (ActiveSite site in Model.Core.Landscape) {

                Event FireEvent = Event.Initiate(site, Model.Core.CurrentTime, Timestep);
                if (FireEvent != null) {
                    LogEvent(Model.Core.CurrentTime, FireEvent);
                    summaryEventCount++;
                }
            }
            //UI.WriteLine("  Fire events: {0}", summaryEventCount);

            //  Write Fire severity map
            string path = MapNames.ReplaceTemplateVars(mapNameTemplate, Model.Core.CurrentTime);
            IOutputRaster<UShortPixel> map = CreateMap(path);
            using (map) {
                UShortPixel pixel = new UShortPixel();
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    if (site.IsActive) {
                        if (SiteVars.Disturbed[site])
                            pixel.Band0 = (byte) (SiteVars.Severity[site] + 1);
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

            WriteSummaryLog(Model.Core.CurrentTime);

        }

        //---------------------------------------------------------------------

        private void LogEvent(int   currentTime,
                              Event FireEvent)
        {
            int totalSitesInEvent = 0;
            if (FireEvent.Severity > 0)
            {
                log.Write("{0},{1},{2},{3},{4},{5:0.0}",
                          currentTime,
                          FireEvent.StartLocation.Row,
                          FireEvent.StartLocation.Column,
                          FireEvent.NumSiteChecked,
                          FireEvent.CohortsKilled,
                          FireEvent.Severity);

                foreach (IFireRegion fireregion in FireRegions.Dataset)
                {
                    log.Write(",{0}", FireEvent.SitesInEvent[fireregion.Index]);
                    totalSitesInEvent += FireEvent.SitesInEvent[fireregion.Index];
                    summaryFireRegionEventCount[fireregion.Index] += FireEvent.SitesInEvent[fireregion.Index];
                }
                summaryTotalSites += totalSitesInEvent;
                log.Write(", {0}", totalSitesInEvent);
                log.WriteLine("");
            }
        }

        //---------------------------------------------------------------------

        private void WriteSummaryLog(int   currentTime)
        {
            //int totalSitesInEvent = 0;
            summaryLog.Write("{0},{1},{2}", currentTime, summaryTotalSites, summaryEventCount);
            foreach (IFireRegion ecoregion in FireRegions.Dataset)
            {
                summaryLog.Write(",{0}", summaryFireRegionEventCount[ecoregion.Index]);
            }
            summaryLog.WriteLine("");
        }
        //---------------------------------------------------------------------

        private IOutputRaster<UShortPixel> CreateMap(string path)
        {
            UI.WriteLine("Writing Fire severity map to {0} ...", path);
            return Model.Core.CreateRaster<UShortPixel>(path,
                                                          Model.Core.Landscape.Dimensions,
                                                          Model.Core.LandscapeMapMetadata);
        }
    }
}
