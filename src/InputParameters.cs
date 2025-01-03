//  Authors:  Robert M. Scheller

using System.Collections.Generic;
using Landis.Utilities;

namespace Landis.Extension.ClimateBDA
{
    /// <summary>
    /// Parameters for the extension.
    /// </summary>
    public interface IInputParameters
    {
        /// <summary>
        /// Timestep (years)
        /// </summary>
        int Timestep {get;set;}
        //---------------------------------------------------------------------
        // string ClimateConfigFile { get; set; }
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        string MapNamesTemplate {get;set;}
        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output SRD maps.
        /// </summary>
        string SRDMapNames{get;set;}
        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output SRD maps.
        /// </summary>
        string NRDMapNames{get;set;}
        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output Vulnerabilty maps.
        /// </summary>
        string BDPMapNames { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// Name of log file.
        /// </summary>
        string LogFileName{get;set;}

        //---------------------------------------------------------------------
        /// <summary>
        /// List of Agent Files
        /// </summary>
        IEnumerable<IAgent> ManyAgentParameters{get;set;}
    }
}

namespace Landis.Extension.ClimateBDA
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public class InputParameters
        : IInputParameters
    {
        private int timestep;
        // private string climateConfigFile;
        private string mapNamesTemplate;
        private string srdMapNames;
        private string nrdMapNames;
        private string bdpMapNames;

        //---------------------------------------------------------------------
        /// <summary>
        /// Timestep (years)
        /// </summary>
        public int Timestep
        {
            get {
                return timestep;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(),
                                                      "Value must be = or > 0.");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        public string MapNamesTemplate
        {
            get {
                return mapNamesTemplate;
            }
            set {
                MapNames.CheckTemplateVars(value);
                mapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for SRD output maps.
        /// </summary>
        public string SRDMapNames
        {
            get
            {
                return srdMapNames;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                srdMapNames = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for NRD output maps.
        /// </summary>
        public string NRDMapNames
        {
            get
            {
                return nrdMapNames;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                nrdMapNames = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for Vulnerability output maps.
        /// </summary>
        public string BDPMapNames
        {
            get
            {
                return bdpMapNames;
            }
            set
            {
                MapNames.CheckTemplateVars(value);
                bdpMapNames = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Name of log file.
        /// </summary>
        public string LogFileName { get; set; }

        //---------------------------------------------------------------------
        /// <summary>
        /// List of Agent Files
        /// </summary>
        public IEnumerable<IAgent> ManyAgentParameters { get; set; }

        //---------------------------------------------------------------------
        public InputParameters()
        {
        }
       
    }
}
