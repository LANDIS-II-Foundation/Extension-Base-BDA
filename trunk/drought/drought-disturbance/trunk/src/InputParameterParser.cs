
using System.Collections.Generic;
using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.DroughtDisturbance
{
    class InputParametersParser
        :TextParser<IInputParameters>
    {
        public static class Names
        {
            public const string Timestep = "Timestep";
            public const string MapName = "MapName";
        }

        //---------------------------------------------------------------------
        private Dictionary<string, int> speciesLineNums;
        private InputVar<string> speciesName;
        
        //---------------------------------------------------------------------
        public InputParametersParser()
        {
            this.speciesLineNums = new Dictionary<string, int>();
            this.speciesName = new InputVar<string>("Species");
        }

        //---------------------------------------------------------------------
        protected override IInputParameters Parse()
        {
            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != PlugIn.ExtensionName)
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", PlugIn.ExtensionName);

            InputParameters parameters = new InputParameters();

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<double> minDroughtYears = new InputVar<double>("MinDroughtYears");
            ReadVar(minDroughtYears);
            parameters.MinDroughtYears = minDroughtYears.Value;

            //-------------------------
            //  SpeciesParameters table

            ReadName("SpeciesParameters");
            speciesLineNums.Clear();  //  If parser re-used (i.e., for testing purposes)

            InputVar<double> drought_Y = new InputVar<double>("Drought Y");
            InputVar<double> drought_YSE = new InputVar<double>("Drought YSE");
            InputVar<double> drought_B = new InputVar<double>("Drought B");
            InputVar<double> drought_BSE = new InputVar<double>("Drought BSE");
            InputVar<double> drought_Sens = new InputVar<double>("Drought Sens");

            while (!AtEndOfInput && CurrentName != Names.MapName)
            {
                StringReader currentLine = new StringReader(CurrentLine);
                ISpecies species = ReadSpecies(currentLine);

                ReadValue(drought_Y, currentLine);
                parameters.SetDrought_Y(species, drought_Y.Value);
                ReadValue(drought_YSE, currentLine);
                parameters.SetDrought_YSE(species, drought_YSE.Value);
                ReadValue(drought_B, currentLine);
                parameters.SetDrought_B(species, drought_B.Value);
                ReadValue(drought_BSE, currentLine);
                parameters.SetDrought_BSE(species, drought_BSE.Value);
                ReadValue(drought_Sens, currentLine);
                parameters.SetDrought_Sens(species, drought_Sens.Value);

                CheckNoDataAfter(drought_Sens.Name, currentLine);
                GetNextLine();
            }
            
            InputVar<string> mapNames = new InputVar<string>("MapName");
            ReadVar(mapNames);
            parameters.MapNamesTemplate = mapNames.Value;

            InputVar<string> logFile = new InputVar<string>("LogFile");
            ReadVar(logFile);
            parameters.LogFileName = logFile.Value;

            return parameters;

        }
        /// <summary>
        /// Reads a species name from the current line, and verifies the name.
        /// </summary>
        private ISpecies ReadSpecies(StringReader currentLine)
        {
            ReadValue(speciesName, currentLine);
            ISpecies species = PlugIn.ModelCore.Species[speciesName.Value.Actual];
            if (species == null)
                throw new InputValueException(speciesName.Value.String,
                                              "{0} is not a species name.",
                                              speciesName.Value.String);
            int lineNumber;
            if (speciesLineNums.TryGetValue(species.Name, out lineNumber))
                throw new InputValueException(speciesName.Value.String,
                                              "The species {0} was previously used on line {1}",
                                              speciesName.Value.String, lineNumber);
            else
                speciesLineNums[species.Name] = LineNumber;
            return species;
        }
        //---------------------------------------------------------------------

    }
}
