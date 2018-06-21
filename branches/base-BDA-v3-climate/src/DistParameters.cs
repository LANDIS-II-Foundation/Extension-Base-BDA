//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.BaseBDA
{
    public enum DisturbanceType {Wind, Fire, Harvest, Null};

    /// <summary>
    /// Extra Dist Paramaters
    /// </summary>
    public interface IDistParameters
    {
        int Duration{get;set;}
        double DistModifier{get;set;}
    }
}

namespace Landis.Extension.BaseBDA
{
    public class DistParameters
        : IDistParameters
    {
        private int duration;
        private double distModifier;

        //---------------------------------------------------------------------
        public int Duration
        {
            get {
                return  duration;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or > 0.");
                duration = value;
            }
        }
        public double DistModifier
        {
            get {
                return distModifier;
            }
            set {
                if (value < -1.0 || value > 1.0)
                        throw new InputValueException(value.ToString(),
                            "Value must be > -1 and < 1.");
                distModifier = value;
            }
        }

        //---------------------------------------------------------------------
        public DistParameters()
        {
        }
        //---------------------------------------------------------------------
        /*public DistParameters(int duration,
                             double distModifier
                             )
        {
            this.duration = duration;
            this.distModifier = distModifier;
        }

        //---------------------------------------------------------------------

        public DistParameters()
        {
            this.duration = 0;
            this.distModifier = 0.0;
        }*/
    }
}
