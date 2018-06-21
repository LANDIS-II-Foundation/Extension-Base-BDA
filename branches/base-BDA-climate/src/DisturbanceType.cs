//  Copyright 2007-2010 USFS Portland State University, Northern Research Station, University of Wisconsin
//  Authors:  Robert M. Scheller, Brian R. Miranda

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Extension.BaseBDA
{
    //This slash type is used for all disturbance fuel types
    public interface IDisturbanceType
    {
        double SRDModifier {get;set;}
        int MaxAge {get;set;}
        List<string> PrescriptionNames{get;set;}
    }

    /// <summary>
    /// A forest type.
    /// </summary>
    public class DisturbanceType
        : IDisturbanceType
    {
        private double srdMod;
        private int maxAge;
        private List<string> prescriptionNames;

        //---------------------------------------------------------------------

        /// <summary>
        /// Index
        /// </summary>
        public double SRDModifier
        {
            get {
                return srdMod;
            }
            set {
                if (value < -1.0 || value > 1.0)
                    throw new InputValueException(value.ToString(),"Value must be >= -1.0 and <= 1.0.");
                srdMod = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum cohort age.
        /// </summary>
        public int MaxAge
        {
            get {
                return maxAge;
            }
            set {
                if (value <= 0)
                    throw new InputValueException(value.ToString(),"Value must be > 0.");
                maxAge = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A prescription name
        /// </summary>
        public List<string> PrescriptionNames
        {
            get {
                return prescriptionNames;
            }
            set {
                if (value != null)
                    prescriptionNames = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize a new instance.
        /// </summary>
        public DisturbanceType()
        {
            prescriptionNames = new List<string>();
        }

    }
}
