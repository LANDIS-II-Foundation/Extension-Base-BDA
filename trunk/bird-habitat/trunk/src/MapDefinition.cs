//  Copyright 2005-2010 Portland State University, University of Wisconsin-Madison
//  Authors:  Robert M. Scheller, Jimm Domingo

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Extension.Output.BirdHabitat
{
    /// <summary>
    /// The definition of a reclass map.
    /// </summary>
    public interface IMapDefinition
    {
        /// <summary>
        /// Map name
        /// </summary>
        string Name
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Forest types
        /// </summary>
        List<IForestType> ForestTypes
        {
            get;
        }
    }

    /// <summary>
    /// The definition of a reclass map.
    /// </summary>
    public class MapDefinition
        : IMapDefinition
    {
        private string name;
        private List<IForestType> forestTypes;

        //---------------------------------------------------------------------

        /// <summary>
        /// Map name
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
        /// Forest types
        /// </summary>
        public List<IForestType> ForestTypes
        {
            get {
                return forestTypes;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public MapDefinition()
        {
            forestTypes = new List<IForestType>();
        }
        //---------------------------------------------------------------------

/*        public MapDefinition(string        name,
                             IForestType[] forestTypes)
        {
            this.name = name;
            this.forestTypes = forestTypes;
        }*/
    }
}
