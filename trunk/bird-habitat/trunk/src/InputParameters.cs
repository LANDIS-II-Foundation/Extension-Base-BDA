//  Copyright 2005-2010 Portland State University, University of Wisconsin-Madison
//  Authors:  Robert M. Scheller, Jimm Domingo

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;
using System.Data;

namespace Landis.Extension.Output.BirdHabitat
{
    /// <summary>
    /// The parameters for the plug-in.
    /// </summary>
    public class InputParameters
        : IInputParameters
    {
        private int timestep;
        private List<IMapDefinition> mapDefns;
        private string localVarMapFileNames;
        private string neighborMapFileNames;
        private string speciesMapFileNames;
        private List<IVariableDefinition> varDefn;
        private List<INeighborVariableDefinition> neighborVarDefn;
        private List<IClimateVariableDefinition> climateVarDefn;
        private List<IModelDefinition> modelDefn;
        private DataTable climateDataTable;

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
                    throw new InputValueException(value.ToString(),"Value must be = or > 0.");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------


        /// <summary>
        /// Reclass maps
        /// </summary>
        public List<IMapDefinition> ReclassMaps
        {
            get {
                return mapDefns;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Derived Variables
        /// </summary>
        public List<IVariableDefinition> DerivedVars
        {
            get
            {
                return varDefn;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Neighborhood Variables
        /// </summary>
        public List<INeighborVariableDefinition> NeighborVars
        {
            get
            {
                return neighborVarDefn;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Climate Variables
        /// </summary>
        public List<IClimateVariableDefinition> ClimateVars
        {
            get
            {
                return climateVarDefn;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Species Models
        /// </summary>
        public List<IModelDefinition> Models
        {
            get
            {
                return modelDefn;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for the filenames for reclass maps.
        /// </summary>
        public string LocalVarMapFileNames
        {
            get {
                return localVarMapFileNames;
            }
            set {
                BirdHabitat.MapFileNames.CheckTemplateVars(value);
                localVarMapFileNames = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for neighborhood maps.
        /// </summary>
        public string NeighborMapFileNames
        {
            get
            {
                return neighborMapFileNames;
            }
            set
            {
                BirdHabitat.NeighborMapFileNames.CheckTemplateVars(value);
                neighborMapFileNames = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Template for the filenames for species model results.
        /// </summary>
        public string SpeciesMapFileNames
        {
            get
            {
                return speciesMapFileNames;
            }
            set
            {
                BirdHabitat.SpeciesMapFileNames.CheckTemplateVars(value);
                speciesMapFileNames = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Climate Data Table.
        /// </summary>
        public DataTable ClimateDataTable
        {
            get
            {
                return climateDataTable;
            }
            set
            {
                climateDataTable = value;
            }
        }

        //---------------------------------------------------------------------

        public InputParameters(int speciesCount)
        {
            mapDefns = new List<IMapDefinition>();
            varDefn = new List<IVariableDefinition>();
            neighborVarDefn = new List<INeighborVariableDefinition>();
            climateVarDefn = new List<IClimateVariableDefinition>();
            modelDefn = new List<IModelDefinition>();
            climateDataTable = new DataTable();
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="timestep"></param>
        /// <param name="mapDefns"></param>
        /// <param name="mapFileNames"></param>
/*        public Parameters(int              timestep,
                          IMapDefinition[] mapDefns,
                          string           mapFileNames)
        {
            this.timestep = timestep;
            this.mapDefns = mapDefns;
            this.mapFileNames = mapFileNames;
        }*/
    }
}
