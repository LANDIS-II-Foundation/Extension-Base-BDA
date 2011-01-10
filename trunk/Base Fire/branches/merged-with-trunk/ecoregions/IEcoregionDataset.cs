//  Copyright 2006 University of Wisconsin
//  Author:  James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using System.Collections.Generic;

namespace Landis.Fire
{
    /// <summary>
    /// A dataset of fire ecoregion parameters.
    /// </summary>
    public interface IEcoregionDataset
        : IEnumerable<IEcoregion>
    {
        /// <summary>
        /// The number of ecoregions in the dataset.
        /// </summary>
        int Count
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets an ecoregion by its name.
        /// </summary>
        /// <returns>
        /// null if there is no ecoregion with the specified name.
        /// </returns>
        IEcoregion this[string name]
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Finds an ecoregion by its map code.
        /// </summary>
        /// <returns>
        /// null if there is no ecoregion with the specified map code.
        /// </returns>
        IEcoregion Find(ushort mapCode);
    }
}
