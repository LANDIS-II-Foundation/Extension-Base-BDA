
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

            //InputVar<double> minDroughtYears = new InputVar<double>("MinDroughtYears");
            //ReadVar(minDroughtYears);
            //parameters.MinDroughtYears = minDroughtYears.Value;

            //-------------------------
            //  Species Mortality table

            ReadName("SpeciesMortalityTable");
            //speciesLineNums.Clear();  //  If parser re-used (i.e., for testing purposes)

            InputVar<string> speciesNameVar = new InputVar<string>("Species");
            AgeClass ageClass = new AgeClass();
           
            //InputVar<double> mortTab = new InputVar<double>("Drought Sens");

            while (!AtEndOfInput && CurrentName != Names.MapName)
            {
                StringReader currentLine = new StringReader(CurrentLine);
                ISpecies species = ReadSpecies(currentLine);

                //AgeClass ageClass = new AgeClass();
                List<AgeClass> ageClasses = new List<AgeClass>();
                string word = "";
                bool success  = false;
                //int lineNumber = 0;

                //ageClasses.Add(species.Name, new List<AgeClass>());

                Dictionary<string, int> lineNumbers = new Dictionary<string, int>();
                lineNumbers[species.Name] = LineNumber;

                if (currentLine.Peek() == -1)
                    throw new InputVariableException(speciesNameVar, "No age classes were defined for species: {0}", species.Name);
                while (currentLine.Peek() != -1)
                {
                    TextReader.SkipWhitespace(currentLine);
                    word = TextReader.ReadWord(currentLine);
                    if (word == "")
                    {
                        if (!success)
                            throw new InputVariableException(speciesNameVar, "No age classes were defined for species: {0}", species.Name);
                        else
                            break;
                    }
                    ageClass = new AgeClass();
                    success = ageClass.Parse(word);
                    if (!success)
                        throw new InputVariableException(speciesNameVar, "Entry is not a valid age class: {0}", word);
                    ageClasses.Add(ageClass);
                }
                GetNextLine();
                success = false;

                /*while (!AtEndOfInput)
                {
                    currentLine = new StringReader(CurrentLine);
                    TextReader.SkipWhitespace(currentLine);
                    word = TextReader.ReadWord(currentLine);

                    species = GetSpecies(word);
                    if (lineNumbers.TryGetValue(species.Name, out lineNumber))
                        throw new InputValueException(word,
                                                      "The species {0} was previously used on line {1}",
                                                      word, lineNumber);
                    lineNumbers[species.Name] = LineNumber;

                    selectedSpecies.Add(species);
                    //CheckNoDataAfter("the species name", currentLine);
                    ageClasses.Add(species.Name, new List<AgeClass>());
                    while (currentLine.Peek() != -1)
                    {
                        TextReader.SkipWhitespace(currentLine);
                        word = TextReader.ReadWord(currentLine);
                        if (word == "")
                        {
                            if (!success)
                                throw new InputVariableException(speciesNameVar, "No age classes were defined for species: {0}", species.Name);
                            else
                                break;
                        }
                        ageClass = new AgeClass();
                        success = ageClass.Parse(word);
                        if (!success)
                            throw new InputVariableException(speciesNameVar, "Entry is not a valid age class: {0}", word);
                        ageClasses[species.Name].Add(ageClass);
                    }
                    GetNextLine();*/


                /*ReadValue(drought_Y, currentLine);
                parameters.SetDrought_Y(species, drought_Y.Value);
                ReadValue(drought_YSE, currentLine);
                parameters.SetDrought_YSE(species, drought_YSE.Value);
                ReadValue(drought_B, currentLine);
                parameters.SetDrought_B(species, drought_B.Value);
                ReadValue(drought_BSE, currentLine);
                parameters.SetDrought_BSE(species, drought_BSE.Value);
                ReadValue(drought_Sens, currentLine);
                parameters.SetDrought_Sens(species, drought_Sens.Value);*/

                parameters.SetMortalityTable(species, ageClasses);

                //CheckNoDataAfter(drought_Sens.Name, currentLine);
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
