//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.AgeCohort;
using Landis.Landscape;
using Landis.RasterIO;
using Landis.Util;
using Landis.Ecoregions;
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
        private ILandscapeCohorts cohorts;
        private string mapNameTemplate;
        private StreamWriter log;

        //---------------------------------------------------------------------

        public PlugIn()
            : base("Base Fire", new PlugIns.PlugInType("disturbance:fire"))
        {
        }


        //---------------------------------------------------------------------

        public override void Initialize(string        dataFile,
                                        PlugIns.ICore modelCore)
        {
            Model.Core = modelCore;

            ParameterParser.EcoregionsDataset = Model.Core.Ecoregions;
            ParameterParser parser = new ParameterParser();
            IParameters parameters = Data.Load<IParameters>(dataFile, parser);

            Timestep = parameters.Timestep;
            mapNameTemplate = parameters.MapNamesTemplate;

            cohorts = Model.Core.SuccessionCohorts as ILandscapeCohorts;
            if (cohorts == null)
                throw new ApplicationException("Error: Cohorts don't support age-cohort interface");

            SiteVars.Initialize(cohorts);

            Event.Initialize(parameters.FireParameters,
                             parameters.FireCurves,
                             parameters.WindCurves,
                             parameters.FireDamages);

            UI.WriteLine("Opening Fire log file \"{0}\" ...", parameters.LogFileName);
            log = Data.CreateTextFile(parameters.LogFileName);
            log.AutoFlush = true;
            log.Write("Time,Initiation Site,Sites Checked,Cohorts Killed,Mean Severity,");
            foreach (IEcoregion ecoregion in Model.Core.Ecoregions)
            {
                if (ecoregion.Active)
                    log.Write("{0},", ecoregion.Name);
            }
            log.WriteLine("");
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

            int eventCount = 0;
            foreach (ActiveSite site in Model.Core.Landscape) {
            
                Event FireEvent = Event.Initiate(site, Model.Core.CurrentTime, Timestep);
                if (FireEvent != null) {
                    LogEvent(Model.Core.CurrentTime, FireEvent);
                    eventCount++;
                }
            }
            UI.WriteLine("  Fire events: {0}", eventCount);

            //  Write Fire severity map
            string path = MapNames.ReplaceTemplateVars(mapNameTemplate, Model.Core.CurrentTime);
            IOutputRaster<SeverityPixel> map = CreateMap(path);
            using (map) {
                SeverityPixel pixel = new SeverityPixel();
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
        }

        //---------------------------------------------------------------------

        private void LogEvent(int   currentTime,
                              Event FireEvent)
        {
            if (FireEvent.Severity > 0) 
            {
                log.Write("{0},\"{1}\",{2},{3},{4:0.0}",
                          currentTime,
                          FireEvent.StartLocation,
                          FireEvent.NumSiteChecked,
                          FireEvent.CohortsKilled,
                          FireEvent.Severity);
                foreach (IEcoregion ecoregion in Model.Core.Ecoregions)
                {
                    if (ecoregion.Active)
                        log.Write(",{0}", FireEvent.SitesInEvent[ecoregion.Index]);
                }
                log.WriteLine("");
            }
        }

        //---------------------------------------------------------------------

        private IOutputRaster<SeverityPixel> CreateMap(string path)
        {
            UI.WriteLine("Writing Fire severity map to {0} ...", path);
            return Model.Core.CreateRaster<SeverityPixel>(path,
                                                          Model.Core.Landscape.Dimensions,
                                                          Model.Core.LandscapeMapMetadata);
        }
    }
}
