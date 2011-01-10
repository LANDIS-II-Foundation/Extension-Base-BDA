//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;


namespace Landis.Fire
{

    public interface IDynamicFireRegion
    {
        int Year {get;set;}
        string MapName{get;set;}
    }
}

namespace Landis.Fire
{
    public class DynamicFireRegion
    : IDynamicFireRegion
    {
        private string mapName;
        private int year;
        
        //---------------------------------------------------------------------
        public string MapName
        {
            get {
                return mapName;
            }

            set {
                mapName = value;
            }
        }

        //---------------------------------------------------------------------
        public int Year
        {
            get {
                return year;
            }

            set {
                //if (value != null) {
                    if (value < 0 )
                        throw new InputValueException(value.ToString(),
                            "Value must be > 0 ");
                //}
                year = value;
            }
        }
        //---------------------------------------------------------------------

        public DynamicFireRegion()
        {
        }
        //---------------------------------------------------------------------
/*
        public DynamicFireRegion(
            string mapName,
            int year
            )
        {
            this.mapName = mapName;
            this.year = year;
        }

        //---------------------------------------------------------------------

        public DynamicFireRegion()
        {
            this.mapName = "";
            this.year = 0;
        }*/


    }
}
