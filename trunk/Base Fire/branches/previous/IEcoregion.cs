//  Copyright 2006 University of Wisconsin
//  Author:  James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf


namespace Landis.Fire
{
    /// <summary>
    /// A fire ecoregion and its parameters.
    /// </summary>
    public interface IEcoregion
        : IEcoregionParameters
    {
        /// <summary>
        /// Index of the ecoregion in the dataset of ecoregion parameters.
        /// </summary>
        int Index
        {
            get;
        }
    }
}
