//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.BaseBDA
{
    /// <summary>
    /// Extra Ecoregion Paramaters
    /// </summary>
    public interface IEcoParameters
    {
        double EcoModifier{get;set;}
        double Latitude { get; set; }
        double FieldCapacity { get; set; }
        double WiltingPoint { get; set; }
    }
}


namespace Landis.Extension.BaseBDA
{
    public class EcoParameters
        : IEcoParameters
    {
        private double ecoModifier;
        private double latitude;
        private double fieldCapacity;
        private double wiltingPoint;

        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        public double EcoModifier{
            get{
                return ecoModifier;
            }
            set {
                if (value < -1 || value > 1)
                        throw new InputValueException(value.ToString(),
                            "Value must be >= -1.0 and <= 1.0.");
                ecoModifier = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                if (value < -90 || value > 90)
                    throw new InputValueException(value.ToString(),
                        "Value must be >= -90.0 and <= 90.0.");
                latitude = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        public double FieldCapacity
        {
            get
            {
                return fieldCapacity;
            }
            set
            {
                if (value < 0.1 || value > 50)
                    throw new InputValueException(value.ToString(),
                        "Value must be >= 0.1 and <= 50.0.");
                fieldCapacity = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        public double WiltingPoint
        {
            get
            {
                return wiltingPoint;
            }
            set
            {
                if (value < 0.1 || value > 50)
                    throw new InputValueException(value.ToString(),
                        "Value must be >= 0.1 and <= 50.0.");
                wiltingPoint = value;
            }
        }
        
        //---------------------------------------------------------------------
        public EcoParameters()
        {
        }
    }
}
