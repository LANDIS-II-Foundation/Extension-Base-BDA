//  Copyright 2005-2010 Portland State University, University of Wisconsin-Madison
//  Authors:  Robert M. Scheller, Jimm Domingo

using Landis.Core;
using Landis.Library.BiomassCohorts;
using Landis.SpatialModeling;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Landis.Library.Climate;
using System.Linq;
using System.Data;


namespace Landis.Extension.Output.BirdHabitat
{
    public class PlugIn
        : ExtensionMain
    {

        public static readonly ExtensionType extType = new ExtensionType("output");
        public static readonly string PlugInName = "Output Bird Habitat";

        private string localVarMapNameTemplate;
        private string speciesMapNameTemplate;

        private IEnumerable<IMapDefinition> mapDefs;
        private IEnumerable<IVariableDefinition> varDefs;
        private IEnumerable<INeighborVariableDefinition> neighborVarDefs;
        private IEnumerable<IClimateVariableDefinition> climateVarDefs;
        private IEnumerable<IModelDefinition> modelDefs;

        private static IInputParameters parameters;
        private static ICore modelCore;


        //---------------------------------------------------------------------

        public PlugIn()
            : base(PlugInName, extType)
        {
        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }

        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile, ICore mCore)
        {
            modelCore = mCore;
            InputParametersParser.SpeciesDataset = modelCore.Species;
            InputParametersParser parser = new InputParametersParser();
            parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);
            foreach(ClimateVariableDefinition climateVarDfn in parameters.ClimateVars)
            {
                if(climateVarDfn.SourceName != "Library")
                {
                    DataTable weatherTable = ClimateVariableDefinition.ReadWeatherFile(climateVarDfn.SourceName);
                    parameters.ClimateDataTable = weatherTable;
                }
            }

        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the component with a data file.
        /// </summary>
        public override void Initialize()
        {

            Timestep = parameters.Timestep;
            SiteVars.Initialize();
            this.localVarMapNameTemplate = parameters.LocalVarMapFileNames;
            this.speciesMapNameTemplate = parameters.SpeciesMapFileNames;
            this.mapDefs = parameters.ReclassMaps;
            this.varDefs = parameters.DerivedVars;
            this.neighborVarDefs = parameters.NeighborVars;
            this.climateVarDefs = parameters.ClimateVars;
            this.modelDefs = parameters.Models;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Runs the component for a particular timestep.
        /// </summary>
        /// <param name="currentTime">
        /// The current model timestep.
        /// </param>
        public override void Run()
        {

            // Calculate Local Variables
            foreach (IMapDefinition map in mapDefs)
            {
                List<IForestType> forestTypes = map.ForestTypes;

                foreach (Site site in modelCore.Landscape.AllSites)
                {
                    int mapCode = 0;
                    if (site.IsActive)
                        mapCode = CalcForestType(forestTypes, site);
                    else
                        mapCode = 0;
                    SiteVars.LocalVars[site][map.Name] = mapCode;
                }
            }

            // Calculate Derived Variables
            foreach (IVariableDefinition var in varDefs)
            {
                foreach (Site site in modelCore.Landscape.AllSites)
                {
                    SiteVars.DerivedVars[site][var.Name] = 0;
                }
                List<string> variables = var.Variables;
                List<string> operators = var.Operators;

                // Parse variable name into mapDef and fortype
                for (int i = 0; i < variables.Count; i++)
                {
                    string fullVar = variables[i];
                    // string[] varSplit = Regex.Split(fullVar, "\\[.*?\\]");
                    string[] varSplit = fullVar.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                    string mapName = varSplit[0];
                    string varName = varSplit[1];
                    int mapCode = 0;

                    foreach (IMapDefinition map in mapDefs)
                    {
                        if (map.Name == mapName)
                        {
                            int forTypeCnt = 1;
                            foreach (IForestType forestType in map.ForestTypes)
                            {
                                if (forestType.Name == varName)
                                {
                                    mapCode = forTypeCnt;
                                }
                                forTypeCnt++;
                            }
                        }
                    }
                    foreach (Site site in modelCore.Landscape.AllSites)
                    {
                        if (SiteVars.LocalVars[site][mapName] == mapCode)
                        {
                            SiteVars.DerivedVars[site][var.Name] = 1;
                        }
                    }
                }

            }

            // Calculate Neighborhood Variables
            foreach (INeighborVariableDefinition neighborVar in neighborVarDefs)
            {
                //Parse LocalVar
                string fullVar = neighborVar.LocalVariable;
                //string[] varSplit = Regex.Split(fullVar, "[]");
                string[] varSplit = fullVar.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                string varName = "";
                int mapCode = 0;
                string mapName = "";
                if (varSplit.Length > 1)
                {
                    mapName = varSplit[0];
                    varName = varSplit[1];

                    foreach (IMapDefinition map in mapDefs)
                    {
                        if (map.Name == mapName)
                        {
                            int forTypeCnt = 1;
                            foreach (IForestType forestType in map.ForestTypes)
                            {
                                if (forestType.Name == varName)
                                {
                                    mapCode = forTypeCnt;
                                }
                                forTypeCnt++;
                            }
                        }
                    }
                }
                else
                {
                    varName = fullVar;
                }

                // Calculate neighborhood 
                double CellLength = PlugIn.ModelCore.CellLength;
                PlugIn.ModelCore.UI.WriteLine("Creating Dispersal Neighborhood List.");

                List<RelativeLocation> neighborhood = new List<RelativeLocation>();
                int neighborRadius = neighborVar.NeighborRadius;
                int numCellRadius = (int)(neighborRadius / CellLength);
                PlugIn.ModelCore.UI.WriteLine("NeighborRadius={0}, CellLength={1}, numCellRadius={2}",
                        neighborRadius, CellLength, numCellRadius);
                double centroidDistance = 0;
                double cellLength = CellLength;

                for (int row = (numCellRadius * -1); row <= numCellRadius; row++)
                {
                    for (int col = (numCellRadius * -1); col <= numCellRadius; col++)
                    {
                        centroidDistance = DistanceFromCenter(row, col);

                        //PlugIn.ModelCore.Log.WriteLine("Centroid Distance = {0}.", centroidDistance);
                        if (centroidDistance <= neighborRadius)
                        {
                            neighborhood.Add(new RelativeLocation(row, col));
                        }
                    }
                }
                // Calculate neighborhood value (% area of forest types)
                foreach (Site site in modelCore.Landscape.AllSites)
                {
                    int totalNeighborCells = 0;
                    int targetNeighborCells = 0;
                    foreach (RelativeLocation relativeLoc in neighborhood)
                    {
                        Site neighbor = site.GetNeighbor(relativeLoc);
                        if (neighbor != null && neighbor.IsActive)
                        {
                            if (mapName == "")
                            {
                                if (SiteVars.DerivedVars[neighbor][varName] > 0)
                                    targetNeighborCells++;
                            }
                            else if (SiteVars.LocalVars[neighbor][mapName] == mapCode)
                            {
                                targetNeighborCells++;
                            }
                            totalNeighborCells++;
                        }
                    }
                    double pctValue = 100.0*(double)targetNeighborCells / (double)totalNeighborCells;

                    // Calculate transformation
                    double transformValue = pctValue;
                    if (neighborVar.Transform == "log10")
                    {

                        transformValue = Math.Log10(pctValue + 1);
                    }
                    else if (neighborVar.Transform == "ln")
                    {
                        transformValue = Math.Log(pctValue + 1);
                    }

                    // Write Site Variable
                    SiteVars.NeighborVars[site][neighborVar.Name] = (float)transformValue;
                }

            }

            // Calculate Climate Variables

            foreach (IClimateVariableDefinition climateVar in climateVarDefs)
            {
                string varName = climateVar.Name;
                string climateLibVar = climateVar.ClimateLibVariable;
                string climateYear = climateVar.Year;
                int minMonth = climateVar.MinMonth;
                int maxMonth = climateVar.MaxMonth;

                int actualYear = PlugIn.ModelCore.CurrentTime;
                if (Climate.Future_MonthlyData != null)
                {
                    actualYear = (PlugIn.ModelCore.CurrentTime - 1) + Climate.Future_MonthlyData.Keys.Min();
                }
                if (climateYear == "prev")
                    actualYear = actualYear - 1;


                if (climateVar.SourceName == "Library")
                {
                    Dictionary<IEcoregion, Dictionary<string, double>> ecoClimateVars = new Dictionary<IEcoregion, Dictionary<string, double>>();

                    foreach (IEcoregion ecoregion in modelCore.Ecoregions)
                    {
                        AnnualClimate_Monthly AnnualWeather = Climate.Future_MonthlyData[actualYear][ecoregion.Index];

                        double monthTotal = 0;
                        int monthCount = 0;
                        double varValue = 0;
                        foreach (int monthIndex in Enumerable.Range(minMonth, maxMonth))
                        {
                            if (climateVar.ClimateLibVariable == "PDSI")
                            {
                                double monthPDSI = PDSI_Calculator.PDSI_Monthly[monthIndex];
                                varValue = monthPDSI;
                            }
                            else if (climateVar.ClimateLibVariable == "Precip")
                            {
                                double monthPrecip = AnnualWeather.MonthlyPrecip[monthIndex];
                                varValue = monthPrecip;
                            }
                            else if (climateVar.ClimateLibVariable == "Temp")
                            {
                                double monthTemp = AnnualWeather.MonthlyTemp[monthIndex];
                                varValue = monthTemp;
                            }
                            monthTotal += varValue;
                            monthCount++;
                        }
                        double avgValue = monthTotal / (double)monthCount;

                        ecoClimateVars[ecoregion][varName] = avgValue;
                    }

                    foreach (Site site in modelCore.Landscape.AllSites)
                    {
                        IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

                        double climateValue = 0;

                        climateValue = ecoClimateVars[ecoregion][varName];
                        // Write Site Variable
                        SiteVars.ClimateVars[site][varName] = (float)climateValue;
                    }
                }
                else
                {
                    double monthTotal = 0;
                    int monthCount = 0;
                    double varValue = 0;
                    var monthRange = Enumerable.Range(minMonth, (maxMonth - minMonth) + 1);
                    foreach (int monthIndex in monthRange)
                    {
                        string selectString = "Year = '" + actualYear + "' AND Month = '" + monthIndex + "'";
                        DataRow[] rows = parameters.ClimateDataTable.Select(selectString);
                        foreach (DataRow row in rows)
                        {
                            varValue = Convert.ToDouble(row[climateVar.ClimateLibVariable]);
                        }
                        monthTotal += varValue;
                        monthCount++;
                    }
                    double avgValue = monthTotal / (double)monthCount;

                    foreach (Site site in modelCore.Landscape.AllSites)
                    {
                        SiteVars.ClimateVars[site][varName] = (float)avgValue;
                    }
                }
            }
            // Calculate Species Models
            foreach (IModelDefinition model in modelDefs)
            {
                foreach (Site site in modelCore.Landscape.AllSites)
                {
                    double modelPredict = 0;
                    int paramIndex = 0;
                    foreach (string parameter in model.Parameters)
                    {
                        string paramType = model.ParamTypes[paramIndex];
                        double paramValue = model.Values[paramIndex];
                        if (paramType == "int")
                        {
                            modelPredict += paramValue;
                        }
                        else if (paramType == "neighbor")
                        {
                            double modelValue = SiteVars.NeighborVars[site][parameter] * paramValue;
                            modelPredict += modelValue;
                        }
                        else if (paramType == "climate")
                        {
                            double modelValue = SiteVars.ClimateVars[site][parameter] * paramValue;
                            modelPredict += modelValue;
                        }

                        paramIndex++;
                    }
                    // Write Site Variable
                    SiteVars.SpeciesModels[site][model.Name] = (float)modelPredict;
                }

            }

            // Ouput Maps
            if (!(parameters.LocalVarMapFileNames == null))
            {
                //----- Write LocalVar maps --------
                foreach (MapDefinition localVar in parameters.ReclassMaps)
                {
                    string localVarPath = MapFileNames.ReplaceTemplateVars(parameters.LocalVarMapFileNames, localVar.Name, PlugIn.ModelCore.CurrentTime);
                    using (IOutputRaster<BytePixel> outputRaster = modelCore.CreateRaster<BytePixel>(localVarPath, modelCore.Landscape.Dimensions))
                    {
                        BytePixel pixel = outputRaster.BufferPixel;
                        foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                        {
                            if (site.IsActive)
                            {
                                pixel.MapCode.Value = (byte)(SiteVars.LocalVars[site][localVar.Name] + 1);
                            }
                            else
                            {
                                //  Inactive site
                                pixel.MapCode.Value = 0;
                            }
                            outputRaster.WriteBufferPixel();
                        }
                    }
                }
            }
            if (!(parameters.NeighborMapFileNames == null))
            {
                //----- Write LocalVar maps --------
                foreach (NeighborVariableDefinition neighborVar in parameters.NeighborVars)
                {
                    string neighborVarPath = NeighborMapFileNames.ReplaceTemplateVars(parameters.NeighborMapFileNames, neighborVar.Name, PlugIn.ModelCore.CurrentTime);
                    using (IOutputRaster<ShortPixel> outputRaster = modelCore.CreateRaster<ShortPixel>(neighborVarPath, modelCore.Landscape.Dimensions))
                    {
                        ShortPixel pixel = outputRaster.BufferPixel;
                        foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                        {
                            if (site.IsActive)
                            {
                                pixel.MapCode.Value = (short)(System.Math.Round(SiteVars.NeighborVars[site][neighborVar.Name] * 100.0));
                            }
                            else
                            {
                                //  Inactive site
                                pixel.MapCode.Value = 0;
                            }
                            outputRaster.WriteBufferPixel();
                        }
                    }
                }
            }
            if (!(parameters.SpeciesMapFileNames == null))
            {
                //----- Write Species Model maps --------
                foreach (ModelDefinition sppModel in parameters.Models)
                {
                    string sppModelPath = SpeciesMapFileNames.ReplaceTemplateVars(parameters.SpeciesMapFileNames, sppModel.Name, PlugIn.ModelCore.CurrentTime);
                    using (IOutputRaster<ShortPixel> outputRaster = modelCore.CreateRaster<ShortPixel>(sppModelPath, modelCore.Landscape.Dimensions))
                    {
                        ShortPixel pixel = outputRaster.BufferPixel;
                        foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                        {
                            if (site.IsActive)
                            {
                                pixel.MapCode.Value = (short)System.Math.Round(SiteVars.SpeciesModels[site][sppModel.Name] * 100.0);
                            }
                            else
                            {
                                //  Inactive site
                                pixel.MapCode.Value = 0;
                            }
                            outputRaster.WriteBufferPixel();
                        }
                    }
                }
            }

        }


        //---------------------------------------------------------------------

        private byte CalcForestType(List<IForestType> forestTypes,
                                    Site site)
        {
            int forTypeCnt = 0;

            double[] forTypValue = new double[forestTypes.Count];

            foreach(ISpecies species in modelCore.Species)
            {
                double sppValue = 0.0;

                if (SiteVars.Cohorts[site] == null)
                    break;

                sppValue = Util.ComputeBiomass(SiteVars.Cohorts[site][species]);

                forTypeCnt = 0;
                foreach(IForestType ftype in forestTypes)
                {
                    if(ftype[species.Index] != 0)
                    {
                        if(ftype[species.Index] == -1)
                            forTypValue[forTypeCnt] -= sppValue;
                        if (ftype[species.Index] == 1)
                        {
                            double cohortValue = 0;
                            if (sppValue > 0)
                            {
                                foreach (ICohort cohort in SiteVars.Cohorts[site][species])
                                {
                                    if (cohort.Age >= ftype.MinAge && cohort.Age <= ftype.MaxAge)
                                        cohortValue += cohort.Biomass;
                                }
                            }
                            forTypValue[forTypeCnt] += cohortValue;
                        }
                    }
                    forTypeCnt++;
                }
            }

            int finalForestType = 0;
            double maxValue = -0.001;
            forTypeCnt = 0;
            foreach(IForestType ftype in forestTypes)
            {
                if(forTypValue[forTypeCnt]>maxValue)
                {
                    maxValue = forTypValue[forTypeCnt];
                    finalForestType = forTypeCnt+1;
                }
                forTypeCnt++;
            }
            return (byte) finalForestType;
        }

        //-------------------------------------------------------
        //Calculate the distance from a location to a center
        //point (row and column = 0).
        private static double DistanceFromCenter(double row, double column)
        {
            double CellLength = PlugIn.ModelCore.CellLength;
            row = System.Math.Abs(row) * CellLength;
            column = System.Math.Abs(column) * CellLength;
            double aSq = System.Math.Pow(column, 2);
            double bSq = System.Math.Pow(row, 2);
            return System.Math.Sqrt(aSq + bSq);
        }
    }
}
