
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
        //double MinDroughtYears { get; set; }
        string MapNamesTemplate { get; set; }
        string LogFileName { get; set; }
        Landis.Extension.Succession.Biomass.Species.AuxParm<List<AgeClass>> MortalityTable { get; }
        
    }

    class InputParameters
        : IInputParameters
    {
        private int timestep;
        private string mapNamesTemplate;
        private string logFileName;
        private Landis.Extension.Succession.Biomass.Species.AuxParm<List<AgeClass>> mortalityTable;
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
/*        public double MinDroughtYears
        {
            get
            {
                return minDroughtYears;
            }
            set
            {
                minDroughtYears = value;
            }
        }*/

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
        public Landis.Extension.Succession.Biomass.Species.AuxParm<List<AgeClass>> MortalityTable
        {
            get
            {
                return mortalityTable;
            }
        }
        

        //---------------------------------------------------------------------
        public void SetMortalityTable(ISpecies species,
                                     List<AgeClass> newValue)
        {
            Debug.Assert(species != null);
            mortalityTable[species] = newValue;
        }
        //---------------------------------------------------------------------
        public InputParameters()
        {
            mortalityTable = new Landis.Extension.Succession.Biomass.Species.AuxParm<List<AgeClass>>(PlugIn.ModelCore.Species);
        }
    }
}
