using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Landis.Utilities;

namespace Landis.Extension.BaseBDA
{
    class ClimateData
    {
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
    public interface IClimateModifier
    {
        string ClimateVariableName { get; set; }
        string  ClimateSource { get; set; }
        string ClimateThreshold { get; set; }
        string Aggregation { get; set; }
        int LagYears { get; set; }
        string ClimateMonths { get; set; }
        float ModifierValue { get; set; }
    
    }

    public class ClimateModifier
    : IClimateModifier
    {
        private string climateVariableName;
        private string climateSource;
        private string climateThreshold;
        private string thresholdOperator;
        private float thresholdValue;
        private string aggregation;
        private int lagYears;
        private string climateMonths;
        private int startMonth;
        private int endMonth;
        private float modValue;
        //---------------------------------------------------------------------
        /// <summary>
        /// Climate variable name
        /// </summary>
        public string ClimateVariableName
        {
            get
            {
                return climateVariableName;
            }
            set
            {
                if (value != null)
                    climateVariableName = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Climate source
        /// </summary>
        public string ClimateSource
        {
            get
            {
                return climateSource;
            }
            set
            {
                if (value != null)
                    climateSource = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Climate threshold
        /// </summary>
        public string ClimateThreshold
        {
            get
            {
                return climateThreshold;
            }
            set
            {
                if (value != null)
                {
                    climateThreshold = value;
                    int thresholdLength = climateThreshold.Length;
                    string firstChar = climateThreshold.Substring(0, 1);
                    string secondChar = climateThreshold.Substring(1, 2);
                    if (firstChar == "=")
                    {
                        thresholdOperator = "equal";
                        thresholdValue = float.Parse(climateThreshold.Substring(1, (thresholdLength - 1)));
                    }
                    else if (firstChar == ">")
                    {
                        if (secondChar == "=")
                        {
                            thresholdOperator = "gt_equal";
                            thresholdValue = float.Parse(climateThreshold.Substring(2, (thresholdLength - 2)));
                        }
                        else
                        {
                            thresholdOperator = "gt";
                            thresholdValue = float.Parse(climateThreshold.Substring(1, (thresholdLength - 1)));
                        }
                    }
                    else if (firstChar == "<")
                    {
                        if (secondChar == "=")
                        {
                            thresholdOperator = "lt_equal";
                            thresholdValue = float.Parse(climateThreshold.Substring(2, (thresholdLength - 2)));
                        }
                        else
                        {
                            thresholdOperator = "lt";
                            thresholdValue = float.Parse(climateThreshold.Substring(1, (thresholdLength - 1)));
                        }
                    }

                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Aggregation
        /// </summary>
        public string Aggregation
        {
            get
            {
                return aggregation;
            }
            set
            {
                if (value != null)
                {
                    if (value == "Average" || value == "Sum")
                        aggregation = value;
                    else
                    {
                        throw new InputValueException(value.ToString(), "Value must be 'Average' or 'Sum'.");
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Climate lag years
        /// </summary>
        public int LagYears
        {
            get
            {
                return lagYears;
            }
            set
            {

                if (value >= 0)
                {
                    lagYears = value;
                }
                else
                {
                    throw new InputValueException(value.ToString(), "Value must be >= 0.");
                }

            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Climate Months
        /// </summary>
        public string ClimateMonths
        {
            get
            {
                return climateMonths;
            }
            set
            {
                if (value != null)
                {
                    climateMonths = value;

                    int delimiterIndex = climateMonths.IndexOf('-');
                    if (delimiterIndex == -1)
                    {
                        int startMonth = int.Parse(value);
                        if (startMonth == 0)
                            throw new FormatException("Month index must be > 0");
                        StartMonth = startMonth;
                        EndMonth = startMonth;
                    }
                    else
                    {
                        string startMonthStr = climateMonths.Substring(0, delimiterIndex);
                        string endMonthStr = climateMonths.Substring(delimiterIndex + 1);
                        if (endMonthStr.Contains("-"))
                            throw new FormatException("Valid format for month range: #-#");
                        if (startMonthStr == "")
                        {
                            if (endMonthStr == "")
                                throw new FormatException("The range has no start and end months");
                            else
                                throw new FormatException("The range has no start month");
                        }
                        int start = int.Parse(startMonthStr);
                        if (start == 0)
                            throw new FormatException("The start age in the range must be > 0");
                        if (endMonthStr == "")
                            throw new FormatException("The range has no end age");
                        int end = int.Parse(endMonthStr);
                        if (start > end)
                            throw new FormatException("The start month in the range must be <= the end month");
                        StartMonth = start;
                        EndMonth = end;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Climate start month
        /// </summary>
        public int StartMonth
        {
            get
            {
                return startMonth;
            }
            set
            {

                if (value >= 0)
                {
                    startMonth = value;
                }
                else
                {
                    throw new InputValueException(value.ToString(), "Value must be >= 0.");
                }

            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Climate end month
        /// </summary>
        public int EndMonth
        {
            get
            {
                return endMonth;
            }
            set
            {

                if (value >= 0)
                {
                    endMonth = value;
                }
                else
                {
                    throw new InputValueException(value.ToString(), "Value must be >= 0.");
                }

            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Climate modifier value - change to site vulnerability
        /// </summary>
        public float ModifierValue
        {
            get
            {
                return modValue;
            }
            set
            {
                modValue = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Threshold operator (=,>,<,>=,<=)
        /// </summary>
        public string ThresholdOperator
        {
            get
            {
                return thresholdOperator;
            }
            set
            {
                thresholdOperator = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Threshold value
        /// </summary>
        public float ThresholdValue
        {
            get
            {
                return thresholdValue;
            }
            set
            {
                thresholdValue = value;
            }
        }
        //---------------------------------------------------------------------
        public ClimateModifier()
        {
           
        }
    }
}