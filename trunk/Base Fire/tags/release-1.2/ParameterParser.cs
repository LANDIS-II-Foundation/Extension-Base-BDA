//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;
using Landis.Util;
using System.Collections.Generic;
using System.Text;

namespace Landis.Fire
{
    /// <summary>
    /// A parser that reads the plug-in's parameters from text input.
    /// </summary>
    public class ParameterParser
        : Landis.TextParser<IParameters>
    {
        public override string LandisDataValue
        {
            get {
                return "Fire";
            }
        }

        //---------------------------------------------------------------------

        public ParameterParser()
        {
            // FIXME: Hack to ensure that Percentage is registered with InputValues
            Edu.Wisc.Forest.Flel.Util.Percentage p = new Edu.Wisc.Forest.Flel.Util.Percentage();
        }

        //---------------------------------------------------------------------

        protected override IParameters Parse()
        {
            ReadLandisDataVar();

            EditableParameters parameters = new EditableParameters();

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            //  Read optional variable FireEcoregions : text file with fire ecoregions
            //  Format much like main ecoregion file:
            //  
            //     >> Map
            //     >> Code  Name  Description
            //     >> ----  ----  -----------
            
            //  If FireEcoregions variable is present, then the variable FireEcoregionMap
            //  must be present.
            InputVar<string> ecoregionsFile = new InputVar<string>("FireEcoregions");
            if (ReadOptionalVar(ecoregionsFile)) {
                ValidatePath(ecoregionsFile.Value);
                Ecoregions.Dataset = EcoregionDataset.ReadFile(ecoregionsFile.Value);

                InputVar<string> ecoregionsMap = new InputVar<string>("FireEcoregionsMap");
                ReadVar(ecoregionsMap);
                Ecoregions.ReadMap(ecoregionsMap.Value);
            }
            else {
                //  Use the main ecoregions as the default fire ecoregions
                Ecoregions.Dataset = new EcoregionDataset();
            }

            //----------------------------------------------------------
            // First, read table of Fire event parameters for ecoregions
            
            InputVar<string> ecoregionName = new InputVar<string>("Ecoregion");
            InputVar<int> maxSize = new InputVar<int>("Max Size");
            InputVar<int> meanSize = new InputVar<int>("Mean Size");
            InputVar<int> minSize = new InputVar<int>("Min Size");
            InputVar<float> ignitionProb = new InputVar<float>("Ignition Probability");
            InputVar<int> fireSpreadAge = new InputVar<int>("Fire Spread Age");

            Dictionary <string, int> lineNumbers = new Dictionary<string, int>();
            const string FireCurves = "FireCurveTable";

            while (! AtEndOfInput && CurrentName != FireCurves) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(ecoregionName, currentLine);
                IEcoregion ecoregion = GetEcoregion(ecoregionName.Value,
                                                    lineNumbers);

                IEditableFireParameters eventParms = new EditableFireParameters();

                ReadValue(maxSize, currentLine);
                eventParms.MaxSize = maxSize.Value;

                ReadValue(meanSize, currentLine);
                eventParms.MeanSize = meanSize.Value;

                ReadValue(minSize, currentLine);
                eventParms.MinSize = minSize.Value;

                ReadValue(ignitionProb, currentLine);
                eventParms.IgnitionProb = ignitionProb.Value;

                ReadValue(fireSpreadAge, currentLine);
                eventParms.FireSpreadAge = fireSpreadAge.Value;

                ecoregion.FireParameters = eventParms.GetComplete();

                CheckNoDataAfter("the " + fireSpreadAge.Name + " column",
                                 currentLine);
                GetNextLine();
            }

            //-------------------------------------------------------------
            // Second, read table of Fire curve parameters for ecoregions
            ReadName(FireCurves);

            InputVar<int> severity1 = new InputVar<int>("Fire Severity1 Age");
            InputVar<int> severity2 = new InputVar<int>("Fire Severity2 Age");
            InputVar<int> severity3 = new InputVar<int>("Fire Severity3 Age");
            InputVar<int> severity4 = new InputVar<int>("Fire Severity4 Age");
            InputVar<int> severity5 = new InputVar<int>("Fire Severity5 Age");

            lineNumbers.Clear();
            const string WindCurves = "WindCurveTable";

            while (! AtEndOfInput && CurrentName != WindCurves) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(ecoregionName, currentLine);
                IEcoregion ecoregion = GetEcoregion(ecoregionName.Value,
                                                    lineNumbers);

                IEditableFireCurve fireCurve = new EditableFireCurve();

                ReadValue(severity1, currentLine);
                fireCurve.Severity1 = severity1.Value;

                ReadValue(severity2, currentLine);
                fireCurve.Severity2 = severity2.Value;

                ReadValue(severity3, currentLine);
                fireCurve.Severity3 = severity3.Value;

                ReadValue(severity4, currentLine);
                fireCurve.Severity4 = severity4.Value;

                ReadValue(severity5, currentLine);
                fireCurve.Severity5 = severity5.Value;

                ecoregion.FireCurve = fireCurve.GetComplete();

                CheckNoDataAfter("the " + severity1.Name + " column",
                                 currentLine);
                GetNextLine();
            }
            
            //----------------------------------------------------------
            // Third, read table of wind curve parameters for ecoregions
            ReadName(WindCurves);

            InputVar<int> wseverity1 = new InputVar<int>("Wind Severity1 Age");
            InputVar<int> wseverity2 = new InputVar<int>("Wind Severity2 Age");
            InputVar<int> wseverity3 = new InputVar<int>("Wind Severity3 Age");
            InputVar<int> wseverity4 = new InputVar<int>("Wind Severity4 Age");
            InputVar<int> wseverity5 = new InputVar<int>("Wind Severity5 Age");

            lineNumbers.Clear();
            const string FireDamage = "FireDamageTable";

            while (! AtEndOfInput && CurrentName != FireDamage) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(ecoregionName, currentLine);
                IEcoregion ecoregion = GetEcoregion(ecoregionName.Value,
                                                    lineNumbers);

                IEditableWindCurve windCurve = new EditableWindCurve();

                ReadValue(wseverity5, currentLine);
                windCurve.Severity5 = wseverity5.Value;

                ReadValue(wseverity4, currentLine);
                windCurve.Severity4 = wseverity4.Value;

                ReadValue(wseverity3, currentLine);
                windCurve.Severity3 = wseverity3.Value;

                ReadValue(wseverity2, currentLine);
                windCurve.Severity2 = wseverity2.Value;

                ReadValue(wseverity1, currentLine);
                windCurve.Severity1 = wseverity1.Value;

                ecoregion.WindCurve = windCurve.GetComplete();

                CheckNoDataAfter("the " + wseverity1.Name + " column",
                                 currentLine);
                GetNextLine();
            }
            
            //-------------------------------------------------------------------
            //  Read table of Fire Damage classes.
            //  Damages are in increasing order.
            ReadName(FireDamage);

            InputVar<Percentage> maxAge = new InputVar<Percentage>("Max Survival Age");
            InputVar<int> severTolerDifference = new InputVar<int>("Severity Tolerance Diff");

            const string MapNames = "MapNames";
            int previousNumber = -4;
            double previousMaxAge = 0.0;

            while (! AtEndOfInput && CurrentName != MapNames
                                  && previousNumber != 4) {
                StringReader currentLine = new StringReader(CurrentLine);

                IEditableDamageTable damage = new EditableDamageTable();
                parameters.FireDamages.Add(damage);

                ReadValue(maxAge, currentLine);
                damage.MaxAge = maxAge.Value;
                if (maxAge.Value.Actual <= 0)
                {
                //  Maximum age for damage must be > 0%
                    throw new InputValueException(maxAge.Value.String,
                                      "Must be > 0% for the all damage classes");
                }
                if (maxAge.Value.Actual > 1)
                {
                //  Maximum age for damage must be <= 100%
                    throw new InputValueException(maxAge.Value.String,
                                      "Must be <= 100% for the all damage classes");
                }
                //  Maximum age for every damage must be > 
                //  maximum age of previous damage.
                if (maxAge.Value.Actual <= previousMaxAge)
                {
                    throw new InputValueException(maxAge.Value.String,
                        "MaxAge must > the maximum age ({0}) of the preceeding damage class",
                        previousMaxAge);
                }

                previousMaxAge = (double) maxAge.Value.Actual;

                ReadValue(severTolerDifference, currentLine);
                damage.SeverTolerDifference = severTolerDifference.Value;

                //Check that the current damage number is > than
                //the previous number (numbers are must be in increasing
                //order).
                if (severTolerDifference.Value.Actual <= previousNumber)
                    throw new InputValueException(severTolerDifference.Value.String,
                                                  "Expected the damage number {0} to be greater than previous {1}",
                                                  damage.SeverTolerDifference, previousNumber);
                if (severTolerDifference.Value.Actual > 4)
                    throw new InputValueException(severTolerDifference.Value.String,
                                                  "Expected the damage number {0} to be less than 5",
                                                  damage.SeverTolerDifference);
                                                  
                previousNumber = severTolerDifference.Value.Actual;

                CheckNoDataAfter("the " + severTolerDifference.Name + " column",
                                 currentLine);
                GetNextLine();
            }
            
            if (parameters.FireDamages.Count == 0)
                throw NewParseException("No damage classes defined.");

            InputVar<string> mapNames = new InputVar<string>(MapNames);
            ReadVar(mapNames);
            parameters.MapNamesTemplate = mapNames.Value;

            InputVar<string> logFile = new InputVar<string>("LogFile");
            ReadVar(logFile);
            parameters.LogFileName = logFile.Value;

            InputVar<string> summaryLogFile = new InputVar<string>("SummaryLogFile");
            ReadVar(summaryLogFile);
            parameters.SummaryLogFileName = summaryLogFile.Value;

            CheckNoDataAfter(string.Format("the {0} parameter", summaryLogFile.Name));

            return parameters.GetComplete();
        }

        //---------------------------------------------------------------------

        private void ValidatePath(InputValue<string> path)
        {
            if (path.Actual.Trim(null) == "")
                throw new InputValueException(path.String,
                                              "Invalid file path: {0}",
                                              path.String);
        }

        //---------------------------------------------------------------------

        private IEcoregion GetEcoregion(InputValue<string>      ecoregionName,
                                        Dictionary<string, int> lineNumbers)
        {
            IEcoregion ecoregion = Ecoregions.Dataset[ecoregionName.Actual];
            if (ecoregion == null)
                throw new InputValueException(ecoregionName.String,
                                              "{0} is not an ecoregion name.",
                                              ecoregionName.String);
            int lineNumber;
            if (lineNumbers.TryGetValue(ecoregion.Name, out lineNumber))
                throw new InputValueException(ecoregionName.String,
                                              "The ecoregion {0} was previously used on line {1}",
                                              ecoregionName.String, lineNumber);
            else
                lineNumbers[ecoregion.Name] = LineNumber;

            return ecoregion;
        }
    }
}
