//  Copyright 2005-2010 Portland State University, University of Wisconsin-Madison
//  Authors:  Robert M. Scheller, Jimm Domingo

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Extension.Output.BirdHabitat
{
    /// <summary>
    /// The definition of a reclass map.
    /// </summary>
    public interface IVariableDefinition
    {
        /// <summary>
        /// Var name
        /// </summary>
        string Name
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Variables
        /// </summary>
        List<string> Variables
        {
            get;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Operators
        /// </summary>
        List<string> Operators
        {
            get;
        }
        //---------------------------------------------------------------------
    }

    /// <summary>
    /// The definition of a reclass map.
    /// </summary>
    public class VariableDefinition
        : IVariableDefinition
    {
        private string name;
        private List<string> variables;
        private List<string> operators;

        //---------------------------------------------------------------------

        /// <summary>
        /// Var name
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
        /// Variables
        /// </summary>
        public List<string> Variables
        {
            get {
                return variables;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Operators
        /// </summary>
        public List<string> Operators
        {
            get
            {
                return operators;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public VariableDefinition()
        {
            variables = new List<string>();
            operators = new List<string>();
        }
        //---------------------------------------------------------------------

    }
}
