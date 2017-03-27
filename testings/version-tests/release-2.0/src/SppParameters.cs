//  Copyright 2005 University of Wisconsin
//  Authors:
//      Robert M. Scheller
//      James B. Domingo
//  Version 1.0
//  License:  Available at
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.BDA
{
    /// <summary>
    /// Extra Spp Paramaters
    /// </summary>
    public interface ISppParameters
    {
        /// <summary>
        /// </summary>
        int MinorHostAge{get;set;}
        int SecondaryHostAge{get;set;}
        int PrimaryHostAge{get;set;}
        int ResistantHostAge{get;set;}
        int TolerantHostAge{get;set;}
        int VulnerableHostAge{get;set;}
        bool CFSConifer{get;set;}
    }
}


namespace Landis.BDA
{
    public class SppParameters
        : ISppParameters
    {
        private int minorHostAge     ;
        private int secondaryHostAge ;
        private int primaryHostAge   ;
        private int resistantHostAge ;
        private int tolerantHostAge  ;
        private int vulnerableHostAge;
        private bool cfsConifer;

        //---------------------------------------------------------------------

        /// <summary>
        /// </summary>
        public int MinorHostAge
        {
            get {
                return minorHostAge;
            }
            set {
               if (value < 0)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or > 0.");
               if (value > 999)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or < 999.");
                minorHostAge = value;
            }
        }
        public int SecondaryHostAge
        {
            get {
                return secondaryHostAge;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or > 0.");
                if (value > 999)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or < 999.");
                secondaryHostAge = value;
            }
        }
        public int PrimaryHostAge
        {
            get {
                return primaryHostAge;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or > 0.");
                if (value > 999)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or < 999.");
                primaryHostAge = value;
            }
        }
        public int ResistantHostAge
        {
            get {
                return resistantHostAge;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or > 0.");
                if (value > 999)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or < 999.");
                resistantHostAge = value;
            }
        }
        public int TolerantHostAge
        {
            get {
                return tolerantHostAge;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or > 0.");
                if (value > 999)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or < 999.");
                tolerantHostAge = value;
            }
        }
        public int VulnerableHostAge
        {
            get {
                return vulnerableHostAge;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or > 0.");
                if (value > 999)
                        throw new InputValueException(value.ToString(),
                            "Value must be = or < 999.");
                vulnerableHostAge = value;
            }
        }
        public bool CFSConifer
        {
            get {
                return cfsConifer;
            }

            set {
                cfsConifer = value;
            }
        }

        //---------------------------------------------------------------------
        public SppParameters()
        {
            this.minorHostAge = 999;
            this.secondaryHostAge = 999;
            this.primaryHostAge = 999;
            this.resistantHostAge = 999;
            this.tolerantHostAge = 999;
            this.vulnerableHostAge = 999;
            this.cfsConifer = false;
        }
        //---------------------------------------------------------------------
        /*public SppParameters(int minorHostAge,
                             int secondaryHostAge,
                                 int primaryHostAge,
                                 int resistantHostAge,
                                 int tolerantHostAge,
                                 int vulnerableHostAge,
                                 bool cfsConifer)
        {
            this.minorHostAge     = minorHostAge;
            this.secondaryHostAge = secondaryHostAge     ;
            this.primaryHostAge   = primaryHostAge       ;
            this.resistantHostAge = resistantHostAge     ;
            this.tolerantHostAge  = tolerantHostAge      ;
            this.vulnerableHostAge= vulnerableHostAge;
            this.cfsConifer = cfsConifer;
        }

        //---------------------------------------------------------------------

        public SppParameters()
        {
            this.minorHostAge       = 999;
            this.secondaryHostAge   = 999;
            this.primaryHostAge     = 999;
            this.resistantHostAge   = 999;
            this.tolerantHostAge    = 999;
            this.vulnerableHostAge  = 999;
            this.cfsConifer = false;
        }*/
    }
}
