
using System.Collections.Generic;
using System.Diagnostics;
using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;


namespace Landis.Extension.DroughtDisturbance
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public interface IInputParameters
    {
        int Timestep { get; set; }
        double MinDroughtYears { get; set; }
        string MapNamesTemplate { get; set; }
        string LogFileName { get; set; }
        Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_Y { get; }
        Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_YSE { get; }
        Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_B { get; }
        Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_BSE { get; }
        Landis.Extension.Succession.Biomass.Species.AuxParm<int> Drought_Sens { get; }
        
    }

    class InputParameters
        : IInputParameters
    {
        private int timestep;
        private double minDroughtYears;
        private string mapNamesTemplate;
        private string logFileName;
        private Landis.Extension.Succession.Biomass.Species.AuxParm<double> drought_Y;
        private Landis.Extension.Succession.Biomass.Species.AuxParm<double> drought_YSE;
        private Landis.Extension.Succession.Biomass.Species.AuxParm<double> drought_B;
        private Landis.Extension.Succession.Biomass.Species.AuxParm<double> drought_BSE;
        private Landis.Extension.Succession.Biomass.Species.AuxParm<int> drought_Sens;
        //---------------------------------------------------------------------
        /// <summary>
        /// Timestep (years)
        /// </summary>
        public int Timestep
        {
            get
            {
                return timestep;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                                                  "Value must be = or > 0.");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------
        public double MinDroughtYears
        {
            get
            {
                return minDroughtYears;
            }
            set
            {
                minDroughtYears = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        public string MapNamesTemplate
        {
            get
            {
                return mapNamesTemplate;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                mapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Name of log file.
        /// </summary>
        public string LogFileName
        {
            get
            {
                return logFileName;
            }
            set
            {
                // FIXME: check for null or empty path (value);
                logFileName = value;
            }
        }

        //---------------------------------------------------------------------
        public Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_Y
        {
            get
            {
                return drought_Y;
            }
        }

        //---------------------------------------------------------------------
        public Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_YSE
        {
            get
            {
                return drought_YSE;
            }
        }

        //---------------------------------------------------------------------
        public Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_B
        {
            get
            {
                return drought_B;
            }
        }

        //---------------------------------------------------------------------
        public Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_BSE
        {
            get
            {
                return drought_BSE;
            }
        }

        //---------------------------------------------------------------------
        public Landis.Extension.Succession.Biomass.Species.AuxParm<int> Drought_Sens
        {
            get
            {
                return drought_Sens;
            }
        }
        
        //---------------------------------------------------------------------
        public void SetDrought_Y(ISpecies species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            drought_Y[species] = Util.CheckBiomassParm(newValue, -100.0, 100.0);
        }

        //---------------------------------------------------------------------
        public void SetDrought_YSE(ISpecies species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            drought_YSE[species] = Util.CheckBiomassParm(newValue, -100.0, 100.0);
        }

        //---------------------------------------------------------------------
        public void SetDrought_B(ISpecies species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            drought_B[species] = Util.CheckBiomassParm(newValue, -100.0, 100.0);
        }

        //---------------------------------------------------------------------
        public void SetDrought_BSE(ISpecies species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            drought_BSE[species] = Util.CheckBiomassParm(newValue, -100.0, 100.0);
        }

        //---------------------------------------------------------------------
        public void SetDrought_Sens(ISpecies species,
                                     InputValue<double> newValue)
        {
            Debug.Assert(species != null);
            drought_Sens[species] = (int)Util.CheckBiomassParm(newValue, 1, 3);
        }
        //---------------------------------------------------------------------
        public InputParameters()
        {
            drought_Y = new Landis.Extension.Succession.Biomass.Species.AuxParm<double>(PlugIn.ModelCore.Species);
            drought_YSE = new Landis.Extension.Succession.Biomass.Species.AuxParm<double>(PlugIn.ModelCore.Species);
            drought_B = new Landis.Extension.Succession.Biomass.Species.AuxParm<double>(PlugIn.ModelCore.Species);
            drought_BSE = new Landis.Extension.Succession.Biomass.Species.AuxParm<double>(PlugIn.ModelCore.Species);
            drought_Sens = new Landis.Extension.Succession.Biomass.Species.AuxParm<int>(PlugIn.ModelCore.Species);
        }
    }
}
