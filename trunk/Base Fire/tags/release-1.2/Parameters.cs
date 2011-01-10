//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

namespace Landis.Fire
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public interface IParameters
    {
        /// <summary>
        /// Timestep (years)
        /// </summary>
        int Timestep
        {
            get;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Definitions of Fire damages.
        /// </summary>
        IDamageTable[] FireDamages
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for output maps.
        /// </summary>
        string MapNamesTemplate
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Name of log file.
        /// </summary>
        string LogFileName
        {
            get;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Name of Summary log file.
        /// </summary>
        string SummaryLogFileName
        {
            get;
        }
    }
}


namespace Landis.Fire
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public class Parameters
        : IParameters
    {
        private int timestep;
        private IDamageTable[] damages;
        private string mapNamesTemplate;
        private string logFileName;
        private string summaryLogFileName;

        //---------------------------------------------------------------------

        /// <summary>
        /// Timestep (years)
        /// </summary>
        public int Timestep
        {
            get {
                return timestep;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Definitions of Fire severities.
        /// </summary>
        public IDamageTable[] FireDamages
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
        }

        /// <summary>
        /// Name of log file.
        /// </summary>
        public string SummaryLogFileName
        {
            get {
                return summaryLogFileName;
            }
        }
        //---------------------------------------------------------------------

        public Parameters(int            timestep,
                          IDamageTable[] damages,
                          string         mapNameTemplate,
                          string         logFileName,
                          string         summaryLogFileName)
        {
            this.timestep = timestep;
            this.damages = damages;
            this.mapNamesTemplate = mapNameTemplate;
            this.logFileName = logFileName;
            this.summaryLogFileName = summaryLogFileName;
        }
    }
}
