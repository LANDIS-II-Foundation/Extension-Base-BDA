//  Copyright 2005-2010 Portland State University, University of Wisconsin-Madison
//  Authors:  Robert M. Scheller, Jimm Domingo

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.Output.BirdHabitat
{
    /// <summary>
    /// A forest type.
    /// </summary>
    public interface IForestType
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
        int MinAge { get; set; }
        int MaxAge { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// Multiplier for a species
        /// </summary>
        int this[int speciesIndex]
        {
            get;
            set;
        }
    }

    /// <summary>
    /// A forest type.
    /// </summary>
    public class ForestType
        : IForestType
    {
        private string name;
        private int minAge;
        private int maxAge;
        private int[] multipliers;

        //---------------------------------------------------------------------
        /// <summary>
        /// Name
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
        /// Minimum cohort age.
        /// </summary>
        public int MinAge
        {
            get
            {
                return minAge;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                minAge = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum cohort age.
        /// </summary>
        public int MaxAge
        {
            get
            {
                return maxAge;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                maxAge = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Multiplier for a species
        /// </summary>
        public int this[int speciesIndex]
        {
            get {
                return multipliers[speciesIndex];
            }
            set {
                multipliers[speciesIndex] = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public ForestType(int speciesCount)
        {
            multipliers = new int[speciesCount];
        }
        //---------------------------------------------------------------------

/*        public ForestType(string name,
                          int[]  multipliers)
        {
            this.name = name;
            this.multipliers = multipliers;
        }*/
    }
}
