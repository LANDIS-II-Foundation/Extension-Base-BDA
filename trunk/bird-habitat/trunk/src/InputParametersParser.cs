//  Copyright 2005-2010 Portland State University, University of Wisconsin-Madison
//  Authors:  Robert M. Scheller, Jimm Domingo

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using System.Collections.Generic;
using System.Text;
using System;

namespace Landis.Extension.Output.BirdHabitat
{
    /// <summary>
    /// A parser that reads the plug-in's parameters from text input.
    /// </summary>
    public class InputParametersParser
        : TextParser<IInputParameters>
    {
        public static ISpeciesDataset SpeciesDataset = null;

        //---------------------------------------------------------------------
        public override string LandisDataValue
        {
            get
            {
                return PlugIn.PlugInName;
            }
        }

        //---------------------------------------------------------------------

        public InputParametersParser()
        {
        }

        //---------------------------------------------------------------------

        protected override IInputParameters Parse()
        {
            ReadLandisDataVar();

            InputParameters parameters = new InputParameters(SpeciesDataset.Count);

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            const string LocalVariables = "LocalVariables";
            const string DerivedLocalVariables = "DerivedLocalVariables";
            const string NeighborhoodVariables = "NeighborhoodVariables";
            const string ClimateVariables = "ClimateVariables";
            const string SpeciesModels = "SpeciesModels";
            const string LocalVarMapFileNames = "LocalVarMapFileNames";
            const string NeighborVarMapFileNames = "NeighborVarMapFileNames";
            const string SpeciesMapFileName = "SpeciesMapFileNames";

            if (ReadOptionalName(LocalVariables))
            {
                InputVar<string> speciesName = new InputVar<string>("Species");

                Dictionary<string, int> lineNumbers = new Dictionary<string, int>();

                InputVar<string> mapName = new InputVar<string>("Map Name");
                InputVar<string> forestType = new InputVar<string>("Forest Type");
                InputVar<string> ageKeyword = new InputVar<string>("Age Keyword");
                InputVar<int> minAge = new InputVar<int>("Min Age");
                InputVar<int> maxAge = new InputVar<int>("Max Age");

                lineNumbers.Clear();
                Dictionary<string, int> forestTypeLineNumbers = new Dictionary<string, int>();


                const string nameDelimiter = "->";  // delimiter that separates map name and forest type

                IMapDefinition mapDefn = null;

                while (!AtEndOfInput && (CurrentName != DerivedLocalVariables) && (CurrentName != NeighborhoodVariables) && (CurrentName != ClimateVariables) && (CurrentName != SpeciesModels))
                {
                    StringReader currentLine = new StringReader(CurrentLine);

                    //  If the current line has the delimiter, then read the map
                    //  name.
                    if (CurrentLine.Contains(nameDelimiter))
                    {
                        ReadValue(mapName, currentLine);
                        CheckForRepeatedName(mapName.Value, "map name", lineNumbers);

                        mapDefn = new MapDefinition();
                        mapDefn.Name = mapName.Value;
                        parameters.ReclassMaps.Add(mapDefn);

                        TextReader.SkipWhitespace(currentLine);
                        string word = TextReader.ReadWord(currentLine);
                        if (word != nameDelimiter)
                        {
                            throw NewParseException("Expected \"{0}\" after the map name {1}.",
                                                    nameDelimiter, mapName.Value.String);
                        }

                        forestTypeLineNumbers.Clear();
                    }
                    else
                    {
                        //  If there is no name delimiter and we don't have the
                        //  name for the first map yet, then it's an error.
                        if (mapDefn == null)
                            throw NewParseException("Expected a line with map name followed by \"{0}\"", nameDelimiter);
                    }

                    ReadValue(forestType, currentLine);
                    CheckForRepeatedName(forestType.Value, "forest type",
                                         forestTypeLineNumbers);

                    IForestType currentForestType = new ForestType(SpeciesDataset.Count);
                    currentForestType.Name = forestType.Value;
                    mapDefn.ForestTypes.Add(currentForestType);

                    // Read the age ranges for the species:
                    ReadValue(ageKeyword, currentLine);
                    if (ageKeyword.Value == "All")
                    {
                        currentForestType.MinAge = 0;
                        int maxAgeAllSpecies = 0;
                        foreach (ISpecies species in PlugIn.ModelCore.Species)
                        {
                            if (species.Longevity > maxAgeAllSpecies)
                                maxAgeAllSpecies = species.Longevity;
                        }
                        currentForestType.MaxAge = maxAgeAllSpecies;
                    }
                    else
                    {
                        //ReadValue(minAge, currentLine);
                        currentForestType.MinAge = Convert.ToInt32(ageKeyword.Value);

                        TextReader.SkipWhitespace(currentLine);
                        string currentWord = TextReader.ReadWord(currentLine);
                        if (currentWord != "to")
                        {
                            StringBuilder message = new StringBuilder();
                            message.AppendFormat("Expected \"to\" after the minimum age ({0})",
                                                 minAge.Value.String);
                            if (currentWord.Length > 0)
                                message.AppendFormat(", but found \"{0}\" instead", currentWord);
                            throw NewParseException(message.ToString());
                        }

                        ReadValue(maxAge, currentLine);
                        currentForestType.MaxAge = maxAge.Value;
                    }
                    //  Read species for forest types

                    List<string> speciesNames = new List<string>();

                    TextReader.SkipWhitespace(currentLine);
                    while (currentLine.Peek() != -1)
                    {
                        ReadValue(speciesName, currentLine);
                        string name = speciesName.Value.Actual;
                        bool negativeMultiplier = name.StartsWith("-");
                        if (negativeMultiplier)
                        {
                            name = name.Substring(1);
                            if (name.Length == 0)
                                throw new InputValueException(speciesName.Value.String,
                                    "No species name after \"-\"");
                        }
                        if (name == "All")
                        {
                            foreach (ISpecies species in PlugIn.ModelCore.Species)
                            {
                                speciesNames.Add(species.Name);
                                currentForestType[species.Index] =  1;
                            }
                        }
                        else if (name == "None")
                        {
                            foreach (ISpecies species in PlugIn.ModelCore.Species)
                            {
                                speciesNames.Add(species.Name);
                                currentForestType[species.Index] = 0;
                            }
                        }
                        else
                        {
                            ISpecies species = GetSpecies(new InputValue<string>(name, speciesName.Value.String));
                            if (speciesNames.Contains(species.Name))
                                throw NewParseException("The species {0} appears more than once.", species.Name);
                            speciesNames.Add(species.Name);
                            currentForestType[species.Index] = negativeMultiplier ? -1 : 1;
                        }

                        TextReader.SkipWhitespace(currentLine);
                    }
                    if (speciesNames.Count == 0)
                        throw NewParseException("At least one species is required.");

                    GetNextLine();
                }
            }
            if (ReadOptionalName(DerivedLocalVariables))
            {
                Dictionary<string, int> lineNumbers = new Dictionary<string, int>();
                lineNumbers.Clear();
                InputVar<string> derVarName = new InputVar<string>("Derived Variable Name");
                InputVar<string> derVarFormula = new InputVar<string>("Derived Variable Formula");
                const string nameDelimiter = "->";  // delimiter that separates map name and forest type
                List<string> formulaSymbol = new List<string>(new string[] { "+", "-", "*" });

                IVariableDefinition varDefn = null;
                while (!AtEndOfInput && (CurrentName != NeighborhoodVariables) && (CurrentName != ClimateVariables) && (CurrentName != SpeciesModels))
                {
                    StringReader currentLine = new StringReader(CurrentLine);

                    ReadValue(derVarName, currentLine);
                    CheckForRepeatedName(derVarName.Value, "var name", lineNumbers);

                    varDefn = new VariableDefinition();
                    varDefn.Name = derVarName.Value;

                    TextReader.SkipWhitespace(currentLine);
                    string word = TextReader.ReadWord(currentLine);
                    if (word != nameDelimiter)
                    {
                        throw NewParseException("Expected \"{0}\" after the map name {1}.",
                                                nameDelimiter, derVarName.Value.String);
                    }

                    TextReader.SkipWhitespace(currentLine);
                    string variable = TextReader.ReadWord(currentLine);
                    varDefn.Variables.Add(variable);
                    while (currentLine.Peek() != null)
                    {
                        TextReader.SkipWhitespace(currentLine);
                        string op = TextReader.ReadWord(currentLine);
                        if (op == "")
                            break;
                        varDefn.Operators.Add(op);

                        TextReader.SkipWhitespace(currentLine);
                        variable = TextReader.ReadWord(currentLine);
                        varDefn.Variables.Add(variable);
                    }
                    parameters.DerivedVars.Add(varDefn);

                    GetNextLine();
                }
            }
            if (ReadOptionalName(NeighborhoodVariables))
            {
                // Read Neighborhood Variables
                Dictionary<string, int> lineNumbers = new Dictionary<string, int>();
                lineNumbers.Clear();
                InputVar<string> neighborVarName = new InputVar<string>("Neighbor Variable Name");
                InputVar<string> localVarName = new InputVar<string>("Local Variable Name");
                InputVar<int> neighborRadius = new InputVar<int>("Neighbor Radius");
                InputVar<string> transform = new InputVar<string>("Transform");

                INeighborVariableDefinition neighborVarDefn = null;
                while (!AtEndOfInput && (CurrentName != ClimateVariables) && (CurrentName != SpeciesModels))
                {
                    StringReader currentLine = new StringReader(CurrentLine);

                    ReadValue(neighborVarName, currentLine);
                    CheckForRepeatedName(neighborVarName.Value, "var name", lineNumbers);

                    neighborVarDefn = new NeighborVariableDefinition();
                    neighborVarDefn.Name = neighborVarName.Value;

                    ReadValue(localVarName, currentLine);
                    neighborVarDefn.LocalVariable = localVarName.Value;

                    ReadValue(neighborRadius, currentLine);
                    neighborVarDefn.NeighborRadius = neighborRadius.Value;

                    ReadValue(transform, currentLine);
                    neighborVarDefn.Transform = transform.Value;

                    parameters.NeighborVars.Add(neighborVarDefn);
                    GetNextLine();
                }
            }
            if (ReadOptionalName(ClimateVariables))
            {
                // Read Climate Variables
                Dictionary<string, int> lineNumbers = new Dictionary<string, int>();
                lineNumbers.Clear();
                InputVar<string> climateVarName = new InputVar<string>("Climate Variable Name");
                InputVar<string> climateLibraryVarName = new InputVar<string>("Climate Library Variable Name");
                InputVar<string> sourceName = new InputVar<string>("Source Name");
                InputVar<string> varYear = new InputVar<string>("Variable Year");
                InputVar<int> minMonth = new InputVar<int>("Min Month");
                InputVar<int> maxMonth = new InputVar<int>("Max Month");

                IClimateVariableDefinition climateVarDefn = null;
                while (!AtEndOfInput  && (CurrentName != SpeciesModels))
                {
                    StringReader currentLine = new StringReader(CurrentLine);
                    ReadValue(climateVarName, currentLine);
                    CheckForRepeatedName(climateVarName.Value, "var name", lineNumbers);

                    climateVarDefn = new ClimateVariableDefinition();
                    climateVarDefn.Name = climateVarName.Value;

                    ReadValue(varYear, currentLine);
                    climateVarDefn.Year = varYear.Value;

                    ReadValue(minMonth, currentLine);
                    climateVarDefn.MinMonth = minMonth.Value;

                    TextReader.SkipWhitespace(currentLine);
                    string currentWord = TextReader.ReadWord(currentLine);
                    if (currentWord != "to")
                    {
                        StringBuilder message = new StringBuilder();
                        message.AppendFormat("Expected \"to\" after the minimum month ({0})",
                                             minMonth.Value.String);
                        if (currentWord.Length > 0)
                            message.AppendFormat(", but found \"{0}\" instead", currentWord);
                        throw NewParseException(message.ToString());
                    }

                    ReadValue(maxMonth, currentLine);
                    climateVarDefn.MaxMonth = maxMonth.Value;

                    ReadValue(sourceName, currentLine);
                    climateVarDefn.SourceName = sourceName.Value;

                    ReadValue(climateLibraryVarName, currentLine);
                    climateVarDefn.ClimateLibVariable = climateLibraryVarName.Value;

                    if(climateVarDefn.SourceName != "Library")
                    {

                    }

                    parameters.ClimateVars.Add(climateVarDefn);
                    GetNextLine();
                }
            }

            // Read species models
            ReadName(SpeciesModels);

            InputVar<string> birdName = new InputVar<string>("Bird Name");
            InputVar<string> parameter = new InputVar<string>("Parameter");
            InputVar<string> paramType = new InputVar<string>("Parameter Type");
            InputVar<double> paramValue = new InputVar<double>("Parameter Value");


            Dictionary<string, int> speciesLineNumbers = new Dictionary<string, int>();
            speciesLineNumbers.Clear();

            const string speciesNameDelimiter = "->";  // delimiter that separates map name and forest type

            IModelDefinition modelDefn = null;

            while (!AtEndOfInput && (CurrentName != LocalVarMapFileNames) && (CurrentName != SpeciesMapFileName))
            {
                StringReader currentLine = new StringReader(CurrentLine);

                //  If the current line has the delimiter, then read the map
                //  name.
                if (CurrentLine.Contains(speciesNameDelimiter))
                {
                    ReadValue(birdName, currentLine);
                    CheckForRepeatedName(birdName.Value, "bird name", speciesLineNumbers);

                    modelDefn = new ModelDefinition();
                    modelDefn.Name = birdName.Value;
                    parameters.Models.Add(modelDefn);

                    TextReader.SkipWhitespace(currentLine);
                    string word = TextReader.ReadWord(currentLine);
                    if (word != speciesNameDelimiter)
                    {
                        throw NewParseException("Expected \"{0}\" after the map name {1}.",
                                                speciesNameDelimiter, birdName.Value.String);
                    }

                    speciesLineNumbers.Clear();
                }
                else
                {
                    //  If there is no name delimiter and we don't have the
                    //  name for the first map yet, then it's an error.
                    if (modelDefn == null)
                        throw NewParseException("Expected a line with map name followed by \"{0}\"", speciesNameDelimiter);
                }

                // Read the parameter name:
                ReadValue(parameter, currentLine);
                modelDefn.Parameters.Add(parameter.Value);

                // Read the parameter types:
                ReadValue(paramType, currentLine);
                modelDefn.ParamTypes.Add(paramType.Value);

                // Read the parameter value:
                ReadValue(paramValue, currentLine);
                modelDefn.Values.Add(paramValue.Value);

                GetNextLine();

            }

            // Template for filenames of maps
            InputVar<string> localVarMapFileNames = new InputVar<string>(LocalVarMapFileNames);
            bool readLocalMaps = false;
            if (ReadOptionalVar(localVarMapFileNames))
            {
                parameters.LocalVarMapFileNames = localVarMapFileNames.Value;
                readLocalMaps = true;
            }

            InputVar<string> neighborMapFileNames = new InputVar<string>(NeighborVarMapFileNames);
            bool readNeighborMaps = false;
            if (ReadOptionalVar(neighborMapFileNames))
            {
                parameters.NeighborMapFileNames = neighborMapFileNames.Value;
                readNeighborMaps = true;
            }

            InputVar<string> speciesMapFileNames = new InputVar<string>(SpeciesMapFileName);
            bool readSpeciesMaps = false;
            if (ReadOptionalVar(speciesMapFileNames))
            {
                parameters.SpeciesMapFileNames = speciesMapFileNames.Value;
                readSpeciesMaps = true;
            }
            if (readSpeciesMaps)
            {
                CheckNoDataAfter(string.Format("the {0} parameter", SpeciesMapFileName));
            }
            else if (readNeighborMaps)
            {
                CheckNoDataAfter(string.Format("the {0} parameter", NeighborVarMapFileNames));
            }
            else if (readLocalMaps)
            {
                CheckNoDataAfter(string.Format("the {0} parameter", LocalVarMapFileNames));
            }
            else
            {
                CheckNoDataAfter(string.Format("the {0} parameter", SpeciesModels));
            }

            return parameters; //.GetComplete();
        }

        //---------------------------------------------------------------------

        protected ISpecies GetSpecies(InputValue<string> name)
        {
            ISpecies species = SpeciesDataset[name.Actual];
            if (species == null)
                throw new InputValueException(name.String,
                                              "{0} is not a species name.",
                                              name.String);
            return species;
        }

        //---------------------------------------------------------------------

        private void CheckForRepeatedName(InputValue<string>      name,
                                          string                  description,
                                          Dictionary<string, int> lineNumbers)
        {
            int lineNumber;
            if (lineNumbers.TryGetValue(name.Actual, out lineNumber))
                throw new InputValueException(name.String,
                                              "The {0} {1} was previously used on line {2}",
                                              description, name.String, lineNumber);
            lineNumbers[name.Actual] = LineNumber;
        }
    }
}
