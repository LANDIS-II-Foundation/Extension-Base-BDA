//  Copyright 2005-2010 Portland State University, University of Wisconsin-Madison
//  Authors:  Robert M. Scheller, Jimm Domingo

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Extension.Output.BirdHabitat
{
    /// <summary>
    /// The definition of a species model.
    /// </summary>
    public interface IModelDefinition
    {
        /// <summary>
        /// Species name
        /// </summary>
        string Name
        {
            get;set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Parameters
        /// </summary>
        List<string> Parameters
        {
            get;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Parameter Types
        /// </summary>
        List<string> ParamTypes
        {
            get;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Values
        /// </summary>
        List<double> Values
        {
            get;
        }
        //---------------------------------------------------------------------
    }

    /// <summary>
    /// The definition of a species model.
    /// </summary>
    public class ModelDefinition
        : IModelDefinition
    {
        private string name;
        private List<string> parameters;
        private List<string> paramTypes;
        private List<double> values;

        //---------------------------------------------------------------------

        /// <summary>
        /// Species name
        /// </summary>
        public string Name
        {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Parameters
        /// </summary>
        public List<string> Parameters
        {
            get {
                return parameters;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Parameter Types
        /// </summary>
        public List<string> ParamTypes
        {
            get
            {
                return paramTypes;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Values
        /// </summary>
        public List<double> Values
        {
            get
            {
                return values;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public ModelDefinition()
        {
            parameters = new List<string>();
            paramTypes = new List<string>();
            values = new List<double>();
        }
        //---------------------------------------------------------------------

    }
}
