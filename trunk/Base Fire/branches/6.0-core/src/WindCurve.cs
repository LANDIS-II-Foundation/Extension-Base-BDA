//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;


namespace Landis.Extension.BaseFire
{
    /// <summary>
    /// Wind Curve parameters for an ecoregion.
    /// </summary>
    public interface IWindCurve
    {
        /// <summary>
        /// Severity 1 (5 point scale).
        /// </summary>
        int Severity1
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Severity 2 (5 point scale).
        /// </summary>
        int Severity2
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Severity 3 (5 point scale).
        /// </summary>
        int Severity3
        {
            get;set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Severity 4 (5 point scale).
        /// </summary>
        int Severity4
        {
            get;set;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Severity 5 (5 point scale).
        /// </summary>
        int Severity5
        {
            get;set;
        }
    }
}


namespace Landis.Extension.BaseFire
{
    public class WindCurve
        : IWindCurve
    {
        private int severity1;
        private int severity2;
        private int severity3;
        private int severity4;
        private int severity5;

        //---------------------------------------------------------------------
        /// <summary>
        /// Severity 1 (time in years).
        /// </summary>
        public int Severity1
        {
            get {
                return severity1;
            }
            set {
                if (value < -1)
                    throw new InputValueException(value.ToString(),"Value must be = or > -1.");
                        
                //if (severity1 != null && value.Actual != -1 && value.Actual > severity2.Actual)
                //    throw new InputValueException(value.ToString(),"Value must be = -1 or < next highest severity.");
                        
                severity1 = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Severity 2 (time in years).
        /// </summary>
        public int Severity2
        {
            get {
                return severity2;
            }
            set {
                if (value < -1)
                    throw new InputValueException(value.ToString(),"Value must be = or > -1.");
                        
                if (value != -1 && value < severity1)
                    throw new InputValueException(value.ToString(),"Value must be = -1 or > next lowest severity.");
                        
                severity2 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Severity 3 (time in years).
        /// </summary>
        public int Severity3
        {
            get {
                return severity3;
            }
            set {
                if (value < -1)
                    throw new InputValueException(value.ToString(),"Value must be = or > -1.");
                        
                if (value != -1 && value < severity2)
                    throw new InputValueException(value.ToString(),"Value must be = -1 or > next lowest severity.");
                        
                severity3 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Severity 4 (time in years).
        /// </summary>
        public int Severity4
        {
            get {
                return severity4;
            }
            set {
                if (value < -1)
                    throw new InputValueException(value.ToString(),"Value must be = or > -1.");
                        
                if (value != -1 && value < severity3)
                    throw new InputValueException(value.ToString(),"Value must be = -1 or > next lowest severity.");
                        
                severity4 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Severity 5 (time in years).
        /// </summary>
        public int Severity5
        {
            get {
                return severity5;
            }
            set {
                if (value < -1)
                    throw new InputValueException(value.ToString(),"Value must be = or > -1.");
                        
                if (value != -1 && value < severity4)
                    throw new InputValueException(value.ToString(),"Value must be = -1 or > next lowest severity.");
                        
                severity5 = value;
            }
        }
        //---------------------------------------------------------------------

        public WindCurve()
        {
        }
        //---------------------------------------------------------------------
/*
        public WindCurve(
        int severity1,
        int severity2,
        int severity3,
        int severity4,
        int severity5)
        {
        this.severity1 = severity1;
        this.severity2 = severity2;
        this.severity3 = severity3;
        this.severity4 = severity4;
        this.severity5 = severity5;
        }

        //---------------------------------------------------------------------

        public WindCurve()
        {
        this.severity1 = 0;
        this.severity2 = 0;
        this.severity3 = 0;
        this.severity4 = 0;
        this.severity5 = 0;
        }*/
    }
}
