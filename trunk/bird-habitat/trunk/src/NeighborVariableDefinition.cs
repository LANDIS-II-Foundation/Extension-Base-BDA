//  Copyright 2005-2010 Portland State University, University of Wisconsin-Madison
//  Authors:  Robert M. Scheller, Jimm Domingo

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Extension.Output.BirdHabitat
{
    /// <summary>
    /// The definition of a reclass map.
    /// </summary>
    public interface INeighborVariableDefinition
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
        /// Local Variable
        /// </summary>
        string LocalVariable
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// NeighborRadius
        /// </summary>
        int NeighborRadius
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Transform
        /// </summary>
        string Transform
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
    }

    /// <summary>
    /// The definition of a reclass map.
    /// </summary>
    public class NeighborVariableDefinition
        : INeighborVariableDefinition
    {
        private string name;
        private string localVariable;
        private int neighborRadius;
        private string transform;

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
        /// LocalVariable
        /// </summary>
        public string LocalVariable
        {
            get
            {
                return localVariable;
            }
            set
            {
                localVariable = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// NeighborRadius
        /// </summary>
        public int NeighborRadius
        {
            get
            {
                return neighborRadius;
            }
            set
            {
                neighborRadius = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Transform
        /// </summary>
        public string Transform
        {
            get
            {
                return transform;
            }
            set
            {
                transform = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public NeighborVariableDefinition()
        {
        }
        //---------------------------------------------------------------------

    }
}
