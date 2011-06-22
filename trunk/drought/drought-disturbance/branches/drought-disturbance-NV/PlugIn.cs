using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Landis.Extension.Succession.Biomass;

namespace Landis.Extension.DroughtDisturbance
{
    public class PlugIn
        : ExtensionMain
    {
        //private static readonly bool isDebugEnabled = false;
        public static readonly ExtensionType Type = new ExtensionType("disturbance:drought");
        public static readonly string ExtensionName = "Drought Disturbance NV";

        private string mapNameTemplate;
        private StreamWriter log;
        private static IInputParameters parameters;
        private static ICore modelCore;

        //---------------------------------------------------------------------
        public PlugIn()
            : base(ExtensionName, Type)
        {
        }

        //---------------------------------------------------------------------
        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }
        //---------------------------------------------------------------------
        public override void LoadParameters(string dataFile,
                                            ICore mCore)
        {
            modelCore = mCore;
            InputParametersParser parser = new InputParametersParser();
            parameters = modelCore.Load<IInputParameters>(dataFile, parser);
        }

        //---------------------------------------------------------------------
        public override void Initialize()
        {

            Timestep = parameters.Timestep;
            mapNameTemplate = parameters.MapNamesTemplate;

            SiteVars.Initialize();
            PartialDisturbance.Initialize();

            modelCore.Log.WriteLine("   Opening and Initializing Drought Disturbance log file \"{0}\"...", parameters.LogFileName);
            try
            {
                log = modelCore.CreateTextFile(parameters.LogFileName);
            }
            catch (Exception err)
            {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }
            log.AutoFlush = true;

            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                log.Write("BioRemoved_{0},", species.Name);
            }
            log.Write("TotalBioRemoved,");
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                log.Write("CohortsKilled_{0},", species.Name);
            }
            log.Write("TotalCohortsKilled,");
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                log.Write("ExtraRem_{0},", species.Name);
            }
            log.WriteLine("");
        }

        //---------------------------------------------------------------------
        ///<summary>
        /// Run the plug-in at a particular timestep.
        ///</summary>
        public override void Run()
        {
            modelCore.Log.WriteLine("   Processing Drought Disturbance ...");


            //-------------------------------------------------------------------
            // HOW TO USE DATA STRUCTURE FOR AGE CLASSES

            foreach(ISpecies species in modelCore.Species)
                foreach(AgeClass ageclass in SpeciesData.MortalityTable[species])
                {
                    ushort lwr_age = ageclass.LwrAge;
                    ushort upr_age = ageclass.UprAge;
                    double mortality_fraction = ageclass.MortalityFraction;
                }
            //-------------------------------------------------------------------


            //-------------------------------------------------------------------
            //-------------------------------------------------------------------
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

                //-------------------------------------------------------------------
                foreach (ISpecies species in modelCore.Species)
                {
                        if (SpeciesData.IsOnsetYear(modelCore.CurrentTime, species, ecoregion))
                        {   
                            string tempLine;
                            tempLine = "Onset of drought for spp " + species.Name + " and ecoregion " + ecoregion.Name + " at year " + modelCore.CurrentTime.ToString();
                            modelCore.Log.WriteLine(tempLine);
                            doStuff(site);
                            
                        }
                }
            }
        }

        private void doStuff(ActiveSite site)
        {


            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach (ICohort cohort in speciesCohorts)
                {

                    foreach (AgeClass ageclass in SpeciesData.MortalityTable[speciesCohorts.Species])
                    {
                        ushort lwr_age = ageclass.LwrAge;
                        ushort upr_age = ageclass.UprAge;
                        double mortality_fraction = ageclass.MortalityFraction;
                        if (cohort.Age >= lwr_age && cohort.Age < upr_age)
                        {
                            //remove partial biomass (see below) 
                            log.Write("Mortality fraction for {0} with age {1} is {2}.", cohort.Species.Name, cohort.Age, mortality_fraction);
                        }
                    }
                }
            }
        }


    }
}