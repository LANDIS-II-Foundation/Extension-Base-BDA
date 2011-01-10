//  Copyright 2006 University of Wisconsin
//  Author:  James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

namespace Landis.Fire
{
    /// <summary>
    /// The parameters for an ecoregion.
    /// </summary>
    public interface IEcoregionParameters
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Description.
        /// </summary>
        string Description
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Code that identifies the ecoregion on a map.
        /// </summary>
        ushort MapCode
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Fire event parameters.
        /// </summary>
        IFireParameters FireParameters
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Fire curve.
        /// </summary>
        IFireCurve FireCurve
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Wind curve.
        /// </summary>
        IWindCurve WindCurve
        {
            get;
            set;
        }
    }
}
