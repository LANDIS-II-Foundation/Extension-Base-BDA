using Edu.Wisc.Forest.Flel.Util;

using Landis.Biomass;
using Landis.Biomass.Dead;
using Landis.Landscape;
using Landis.RasterIO;
using Landis.Species;
using Landis.Ecoregions;

using System;
using System.IO;

using System.Collections.Generic;

namespace Landis.Output.Biomass
{
    public class PlugIn
        : Landis.PlugIns.PlugIn
    {
        //private PlugIns.ICore Model.Core;
        private IEnumerable<ISpecies> selectedSpecies;
        private string speciesMapNameTemplate;
        private bool makeMaps;
        private bool makeTable;
        private ILandscapeCohorts cohorts;
        private StreamWriter log;

        //---------------------------------------------------------------------

        public PlugIn()
            : base("Output Biomass", new PlugIns.PlugInType("output"))
        {
        }

        //---------------------------------------------------------------------

        public override void Initialize(string        dataFile,
                                        PlugIns.ICore modelCore)
        {
            Model.Core = modelCore;

            ParametersParser parser = new ParametersParser(Model.Core.Species);
            IParameters parameters = Data.Load<IParameters>(dataFile,
                                                            parser);

            Timestep = parameters.Timestep;
            this.selectedSpecies = parameters.SelectedSpecies;
            this.speciesMapNameTemplate = parameters.SpeciesMapNames;
            this.makeMaps = parameters.MakeMaps;
            this.makeTable = parameters.MakeTable;
            
            if(makeTable)
                InitializeLogFile();

            cohorts = Model.Core.SuccessionCohorts as ILandscapeCohorts;
            if (cohorts == null)
                throw new ApplicationException("Error: Cohorts don't support biomass interface");
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            WriteMapForAllSpecies();
            
            if (selectedSpecies != null)
            {
                if(makeMaps)
                    WriteSpeciesMaps();
                if(makeTable)
                    WriteLogFile();
            }
        }

        //---------------------------------------------------------------------

        private void WriteSpeciesMaps()
        {
            foreach (ISpecies species in selectedSpecies) {
                IOutputRaster<BiomassPixel> map = CreateMap(MakeSpeciesMapName(species.Name));
                using (map) {
                    BiomassPixel pixel = new BiomassPixel();
                    foreach (Site site in Model.Core.Landscape.AllSites) {
                        if (site.IsActive)
                            pixel.Band0 = (ushort) ((float) Util.ComputeBiomass(cohorts[site][species]));
                        else
                            pixel.Band0 = 0;
                        map.WritePixel(pixel);
                    }
                }
            }

        }

        //---------------------------------------------------------------------

        private void WriteMapForAllSpecies()
        {
            // Biomass map for all species
            IOutputRaster<BiomassPixel> map = CreateMap(MakeSpeciesMapName("TotalBiomass"));
            using (map) {
                BiomassPixel pixel = new BiomassPixel();
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    if (site.IsActive)
                        pixel.Band0 = (ushort) ((float) Util.ComputeBiomass(cohorts[site]) / 100.0);
                    else
                        pixel.Band0 = 0;
                    map.WritePixel(pixel);
                }
            }
        }

        //---------------------------------------------------------------------
        public void InitializeLogFile()
        {
        
            string logFileName   = "spp-biomass-log.csv"; 
            UI.WriteLine("   Opening species biomass log file \"{0}\" ...", logFileName);
            try {
                this.log = Data.CreateTextFile(logFileName);
            }
            catch (Exception err) {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }
            
            log.AutoFlush = true;
            log.Write("Time, Ecoregion, NumSites,");
            
            foreach (ISpecies species in selectedSpecies) 
                log.Write("{0},", species.Name);
            
            log.WriteLine("");


        }


        //---------------------------------------------------------------------

        private void WriteLogFile()
        {
            
            int numSpp = 0;
            foreach (ISpecies species in selectedSpecies) 
                numSpp++;
            
            double[,] allSppEcos = new double[Model.Core.Ecoregions.Count, numSpp];
            
            int[] activeSiteCount = new int[Model.Core.Ecoregions.Count];
            
            //UI.WriteLine("Next, reset all values to zero.");
            
            foreach (IEcoregion ecoregion in Model.Core.Ecoregions) 
            {
                int sppCnt = 0;
                foreach (ISpecies species in selectedSpecies) 
                {
                    allSppEcos[ecoregion.Index, sppCnt] = 0.0;
                    sppCnt++;
                }
                
                activeSiteCount[ecoregion.Index] = 0;
            }

            //UI.WriteLine("Next, accumulate data.");


            foreach (ActiveSite site in Model.Core.Landscape)
            {
                IEcoregion ecoregion = Model.Core.Ecoregion[site];
                
                int sppCnt = 0;
                foreach (ISpecies species in selectedSpecies) 
                {
                    allSppEcos[ecoregion.Index, sppCnt] += Util.ComputeBiomass(cohorts[site][species]);
                    sppCnt++;
                }
                
                activeSiteCount[ecoregion.Index]++;
            }
            
            foreach (IEcoregion ecoregion in Model.Core.Ecoregions)
            {
                log.Write("{0}, {1}, {2}, ", 
                    Model.Core.CurrentTime,                 // 0
                    ecoregion.Name,                         // 1
                    activeSiteCount[ecoregion.Index]       // 2
                    );
                int sppCnt = 0;
                foreach (ISpecies species in selectedSpecies) 
                {
                    log.Write("{0}, ",
                        (allSppEcos[ecoregion.Index, sppCnt] / (double) activeSiteCount[ecoregion.Index])
                        );
                    
                    sppCnt++;
                }

                log.WriteLine("");
            }
        }
        //---------------------------------------------------------------------

        private string MakeSpeciesMapName(string species)
        {
            return SpeciesMapNames.ReplaceTemplateVars(speciesMapNameTemplate,
                                                       species,
                                                       Model.Core.CurrentTime);
        }

        //---------------------------------------------------------------------

        private IOutputRaster<BiomassPixel> CreateMap(string path)
        {
            UI.WriteLine("Writing biomass map to {0} ...", path);
            return Model.Core.CreateRaster<BiomassPixel>(path,
                                                        Model.Core.Landscape.Dimensions,
                                                        Model.Core.LandscapeMapMetadata);
        }

        //---------------------------------------------------------------------
/*
        private void WritePoolMaps()
        {
            if ((selectedPools & SelectedDeadPools.Woody) != 0)
                WritePoolMap("woody", Pools.Woody);

            if ((selectedPools & SelectedDeadPools.NonWoody) != 0)
                WritePoolMap("non-woody", Pools.NonWoody);
        }*/

        //---------------------------------------------------------------------

/*        private void WritePoolMap(string         poolName,
                                  ISiteVar<Pool> poolSiteVar)
        {
            string path = PoolMapNames.ReplaceTemplateVars(poolMapNameTemplate,
                                                           poolName,
                                                           Model.Core.CurrentTime);
            if(poolSiteVar != null)
            {
                IOutputRaster<BiomassPixel> map = CreateMap(path);
                using (map) {
                    BiomassPixel pixel = new BiomassPixel();
                    foreach (Site site in Model.Core.Landscape.AllSites) {
                        if (site.IsActive)
                            pixel.Band0 = (ushort) ((float) poolSiteVar[site].Biomass / 100.0);
                        else
                            pixel.Band0 = 0;
                        map.WritePixel(pixel);
                    }
                }
            }
        }*/
    }
}
