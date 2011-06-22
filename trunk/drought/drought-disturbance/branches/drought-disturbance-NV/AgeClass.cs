//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Library.BiomassCohorts;
using Edu.Wisc.Forest.Flel.Util;

using System;
using System.Collections.Generic;


namespace Landis.Extension.DroughtDisturbance
{
    public class AgeClass
    {
        private ushort bin_type = 4;//Enumerated 1 = <X, 2 = X-Y, 3 = >X, 4 = X
        private ushort lwr_age = 0;
        private ushort upr_age = 0;
        private double mortalityFraction = 0.0;

        public AgeClass()
        {
            bin_type = 4;
            lwr_age = 0;
            upr_age = 0;
            mortalityFraction = 0.0;
        }

        public AgeClass(ushort bin_type, ushort lwr_age, ushort upr_age, double mortalityFraction)
        {
            //Note - make sure to set upr_age for the < and = cases, lwr_age for the > case.
            this.bin_type = bin_type;
            this.lwr_age = lwr_age;
            this.upr_age = upr_age;
            this.mortalityFraction = mortalityFraction;
        }

        public ushort BinType
        {
            get { return bin_type; }
        }
        public ushort LwrAge
        {
            get {return lwr_age;}
        }
        public ushort UprAge
        {
            get {return upr_age; }
        }
        public double MortalityFraction
        {
            get { return mortalityFraction; }
        }

        public bool Parse(string word)
        {
            //consume ageclass text and parse into the appropriate member values
            //set this or return false if fail
            try
            {
                //must contain ( and last character must be ) 
                if (!(word.Contains("(") && word.EndsWith(")")))
                    return false;
                word = word.Replace(" ", "");
                word = word.Replace("\t", "");
                word = word.TrimEnd(")".ToCharArray());
                string[] vals = word.Split("(".ToCharArray());
                //if vals is valid, [0] will be ageclass name and [1] will be range expression
                if (vals == null)
                    return false;
                //string name = vals[0];
                double mortfrac = Convert.ToDouble(vals[0]);
                string range_expr = vals[1];
                if (mortfrac > 1.0 || mortfrac < 0.0 || range_expr == null || range_expr == "")
                    return false;

                this.mortalityFraction = mortfrac;
                if (range_expr.StartsWith("<"))
                {
                    this.bin_type = 1;
                    this.lwr_age = 0;
                    range_expr = range_expr.Replace("<", "");
                    this.upr_age = (ushort)Convert.ToUInt16(range_expr);
                }
                else if (range_expr.StartsWith(">"))
                {
                    this.bin_type = 3;
                    this.lwr_age = 0;
                    range_expr = range_expr.Replace(">", "");
                    this.lwr_age = (ushort)Convert.ToUInt16(range_expr);
                }
                else if (range_expr.Contains("-"))
                {
                    this.bin_type = 2;
                    string[] range_vals = range_expr.Split("-".ToCharArray());
                    this.lwr_age = (ushort)Convert.ToUInt16(range_vals[0]);
                    this.upr_age = (ushort)Convert.ToUInt16(range_vals[1]);
                }
                else
                {
                    this.bin_type = 4;
                    this.lwr_age = (ushort)Convert.ToUInt16(range_expr);
                    this.upr_age = this.lwr_age;
                }
                return true;
            }
            catch
            {
                return false;
            }            
        }
    }

}
