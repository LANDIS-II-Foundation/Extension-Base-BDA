//  Copyright 2005-2010 Portland State University, University of Wisconsin-Madison
//  Authors:  Robert M. Scheller, Jimm Domingo

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;
using Landis.Library.Climate;
using System.Data;

namespace Landis.Extension.Output.BirdHabitat
{
    /// <summary>
    /// The definition of a reclass map.
    /// </summary>
    public interface IClimateVariableDefinition
    {
        /// <summary>
        /// Var name
        /// </summary>
        string Name
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Climate Library Variable
        /// </summary>
        string ClimateLibVariable
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Source Name
        /// </summary>
        string SourceName
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Year
        /// </summary>
        string Year
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Min Month
        /// </summary>
        int MinMonth
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Max Month
        /// </summary>
        int MaxMonth
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Climate Data
        /// </summary>
        AnnualClimate_Monthly ClimateData
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
    }

    /// <summary>
    /// The definition of a reclass map.
    /// </summary>
    public class ClimateVariableDefinition
        : IClimateVariableDefinition
    {
        private string name;
        private string climateLibVariable;
        private string sourceName;
        private string year;
        private int minMonth;
        private int maxMonth;
        private AnnualClimate_Monthly climateData;
        //---------------------------------------------------------------------

        /// <summary>
        /// Var name
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Climate Library Variable
        /// </summary>
        public string ClimateLibVariable
        {
            get
            {
                return climateLibVariable;
            }
            set
            {
                climateLibVariable = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Source Name
        /// </summary>
        public string SourceName
        {
            get
            {
                return sourceName;
            }
            set
            {
                sourceName = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Year
        /// </summary>
        public string Year
        {
            get
            {
                return year;
            }
            set
            {
                year = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Min Month
        /// </summary>
        public int MinMonth
        {
            get
            {
                return minMonth;
            }
            set
            {
                minMonth = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Max Month
        /// </summary>
        public int MaxMonth
        {
            get
            {
                return maxMonth;
            }
            set
            {
                maxMonth = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Climate Data
        /// </summary>
        public AnnualClimate_Monthly ClimateData
        {
            get
            {
                return climateData;
            }
            set
            {
                climateData = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public ClimateVariableDefinition()
        {
        }
        //---------------------------------------------------------------------
        
        public static DataTable ReadWeatherFile(string path)
        {
            PlugIn.ModelCore.UI.WriteLine("   Loading Climate Data...");

            CSVParser weatherParser = new CSVParser();

            DataTable weatherTable = weatherParser.ParseToDataTable(path);

            return weatherTable;
        }
        //---------------------------------------------------------------------
    }
}