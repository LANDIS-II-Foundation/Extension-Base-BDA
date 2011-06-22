using Landis.Core;
using Landis.SpatialModeling;

using System;
using System.Collections.Generic;
using System.IO;


namespace Landis.Extension.DroughtGenerator
{
    public class PlugIn
        :ExtensionMain

    {
        private static readonly bool isDebugEnabled = false; 
        public static readonly ExtensionType Type = new ExtensionType("disturbance:drought");
        public static readonly string ExtensionName = "Drought Generator";

        private string mapNameTemplate;
        private StreamWriter log;
        private double mu;
        private double sigma;
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
            mu = parameters.Mu;
            sigma = parameters.Sigma;
            mapNameTemplate = parameters.MapNamesTemplate;

            SiteVars.Initialize();

            modelCore.Log.WriteLine("   Opening and Initializing Drought log file \"{0}\"...", parameters.LogFileName);
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
            log.Write("Time,DroughtYears");
            log.WriteLine("");

            if (isDebugEnabled)
                modelCore.Log.WriteLine("Initialization done");
        }

        //---------------------------------------------------------------------
        ///<summary>
        /// Run the plug-in at a particular timestep.
        ///</summary>
        public override void Run()
        {
            modelCore.Log.WriteLine("   Processing Drought Generator ... ");
            SiteVars.DroughtYears.ActiveSiteValues = 0;
            PlugIn.ModelCore.LognormalDistribution.Mu = mu;
            PlugIn.ModelCore.LognormalDistribution.Sigma = sigma;
            double dY = PlugIn.ModelCore.LognormalDistribution.NextDouble();
            if (dY > 10)
                dY = 10;
            // Multiply by 100 and round to integer in order to map
            // Output represents # years drought per 100 years
            ushort dy100_round = (ushort)Math.Round(dY * 100);
            log.WriteLine("{0},{1}",
                modelCore.CurrentTime,
                dy100_round);

            //  Assign values to sites
            foreach (Site site in modelCore.Landscape.AllSites)
            {
                if (site.IsActive)
                {
                    SiteVars.DroughtYears[site] = dy100_round;
                }
                else
                {
                    SiteVars.DroughtYears[site] = 0;
                }
            }
            
            //  Write DroughtYears map
            string path = MapNames.ReplaceTemplateVars(mapNameTemplate, modelCore.CurrentTime);
            modelCore.Log.WriteLine("   Writing Drought Years map to {0} ...", path);
            using (IOutputRaster<UShortPixel> outputRaster = modelCore.CreateRaster<UShortPixel>(path, modelCore.Landscape.Dimensions))
            {
                UShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in modelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (SiteVars.DroughtYears[site]);
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
        }
    }
}
