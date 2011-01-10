//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;


namespace Landis.Extension.BaseFire
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public interface IInputParameters
    {
        /// <summary>
        /// Timestep (years)
        /// </summary>
        int Timestep
        {
            get;set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Definitions of Fire damages.
        /// </summary>
        List<IDamageTable> FireDamages
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        string MapNamesTemplate
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Name of log file.
        /// </summary>
        string LogFileName
        {
            get;set;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Name of Summary log file.
        /// </summary>
        string SummaryLogFileName
        {
            get;set;
        }
        
        List<IDynamicFireRegion> DynamicFireRegions
        {
            get;
        }
        
    }
}


namespace Landis.Extension.BaseFire
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public class InputParameters
        : IInputParameters
    {
        private int timestep;
        private List<IDamageTable> damages;
        private string mapNamesTemplate;
        private string logFileName;
        private string summaryLogFileName;
        private List<IDynamicFireRegion> dynamicFireRegions;

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
                //if (value != null)
                    if (value < 0)
                        throw new InputValueException(value.ToString(),
                                                      "Value must be = or > 0.");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Definitions of Fire severities.
        /// </summary>
        public List<IDamageTable> FireDamages
        {
            get {
                return damages;
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
                if (value != null) {
                    MapNames.CheckTemplateVars(value);
                }
                mapNamesTemplate = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Name of log file.
        /// </summary>
        public string LogFileName
        {
            get {
                return logFileName;
            }
            set {
                if (value != null) 
                {
                    // FIXME: check for null or empty path (value.Actual);
                    logFileName = value;
                }
            }
        }

        /// <summary>
        /// Name of log file.
        /// </summary>
        public string SummaryLogFileName
        {
            get {
                return summaryLogFileName;
            }
            set {
                if (value != null) {
                    // FIXME: check for null or empty path (value.Actual);
                    summaryLogFileName = value;
                }
            }
        }
        //---------------------------------------------------------------------
        
        public List<IDynamicFireRegion> DynamicFireRegions
        {
            get {
                return dynamicFireRegions;
            }
        }
        //---------------------------------------------------------------------

        public InputParameters()
        {
            dynamicFireRegions = new List<IDynamicFireRegion>(0);
            damages = new List<IDamageTable>(0);
        }
        //---------------------------------------------------------------------
/*
        public Parameters(int            timestep,
                          IDynamicFireRegion[]      dynamicFireRegions,
                          IDamageTable[] damages,
                          string         mapNameTemplate,
                          string         logFileName,
                          string         summaryLogFileName)
        {
            this.timestep = timestep;
            this.dynamicFireRegions = dynamicFireRegions;
            this.damages = damages;
            this.mapNamesTemplate = mapNameTemplate;
            this.logFileName = logFileName;
            this.summaryLogFileName = summaryLogFileName;
        }*/
    }
}
