//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.BaseBDA
{
    /// <summary>
    /// Extra Spp Paramaters
    /// </summary>
    public interface ISppParameters
    {
        /// <summary>
        /// </summary>
        int MinorHostAge { get; set; }
        double MinorHostSRD { get; set; }
        int SecondaryHostAge { get; set; }
        double SecondaryHostSRD { get; set; }
        int PrimaryHostAge { get; set; }
        double PrimaryHostSRD { get; set; }
        int ResistantHostAge { get; set; }
        double ResistantHostVuln { get; set; }
        int TolerantHostAge { get; set; }
        double TolerantHostVuln { get; set; }
        int VulnerableHostAge { get; set; }
        double VulnerableHostVuln { get; set; }
        bool CFSConifer{get;set;}
    }
}


namespace Landis.Extension.BaseBDA
{
    public class SppParameters
        : ISppParameters
    {
        private int minorHostAge     ;
        private double minorHostSRD;
        private int secondaryHostAge;
        private double secondaryHostSRD;
        private int primaryHostAge;
        private double primaryHostSRD;
        private int resistantHostAge;
        private double resistantHostVuln;
        private int tolerantHostAge;
        private double tolerantHostVuln;
        private int vulnerableHostAge;
        private double vulnerableHostVuln;
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
        public double MinorHostSRD
        {
            get
            {
                return minorHostSRD;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                minorHostSRD = value;
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
        public double SecondaryHostSRD
        {
            get
            {
                return secondaryHostSRD;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                secondaryHostSRD = value;
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
        public double PrimaryHostSRD
        {
            get
            {
                return primaryHostSRD;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                primaryHostSRD = value;
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
        public double ResistantHostVuln
        {
            get
            {
                return resistantHostVuln;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                resistantHostVuln = value;
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
        public double TolerantHostVuln
        {
            get
            {
                return tolerantHostVuln;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                tolerantHostVuln = value;
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
        public double VulnerableHostVuln
        {
            get
            {
                return vulnerableHostVuln;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or > 0.");
                if (value > 1)
                    throw new InputValueException(value.ToString(),
                        "Value must be = or < 1.");
                vulnerableHostVuln = value;
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
            this.minorHostSRD = 0;
            this.secondaryHostAge = 999;
            this.secondaryHostSRD = 0;
            this.primaryHostAge = 999;
            this.primaryHostSRD = 0;
            this.resistantHostAge = 999;
            this.resistantHostVuln = 0;
            this.tolerantHostAge = 999;
            this.tolerantHostVuln = 0;
            this.vulnerableHostAge = 999;
            this.vulnerableHostVuln = 0;
            this.cfsConifer = false;
        }
 
    }
}
