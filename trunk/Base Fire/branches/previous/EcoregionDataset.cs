//  Copyright 2006 University of Wisconsin
//  Author:  James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.Landscape;
using System.Collections;
using System.Collections.Generic;

namespace Landis.Fire
{
    /// <summary>
    /// A dataset of fire ecoregions that are explicitly provided by the user.
    /// </summary>
    public class EcoregionDataset
        : IEcoregionDataset
    {
        private Ecoregion[] ecoregions;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance using the dataset of main ecoregions
        /// from the core framework.
        /// </summary>
        public EcoregionDataset()
        {
            ecoregions = new Ecoregion[Model.Core.Ecoregions.Count];
            foreach (Landis.Ecoregions.IEcoregion ecoregion in Model.Core.Ecoregions) {
                EcoregionParameters parameters = new EcoregionParameters(ecoregion.Name,
                                                                         ecoregion.Description,
                                                                         ecoregion.MapCode);
                ecoregions[ecoregion.Index] = new Ecoregion(ecoregion.Index,
                                                            parameters);
            }

            //  Initialize the ecoregion site variable.
            foreach (ActiveSite site in Model.Core.Landscape) {
                Landis.Ecoregions.IEcoregion mainEcoregion = Model.Core.Ecoregion[site];
                SiteVars.Ecoregion[site] = ecoregions[mainEcoregion.Index];
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance from a list of ecoregion parameters.
        /// </summary>
        public EcoregionDataset(IEcoregionParameters[] parameters)
        {
            if (parameters == null)
                ecoregions = new Ecoregion[0];
            else {
                ecoregions = new Ecoregion[parameters.Length];
                for (int index = 0; index < parameters.Length; ++index)
                    ecoregions[index] = new Ecoregion(index, parameters[index]);
            }
        }

        //---------------------------------------------------------------------

        public int Count
        {
            get {
                return ecoregions.Length;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets an ecoregion by its name.
        /// </summary>
        public IEcoregion this[string name]
        {
            get {
                foreach (Ecoregion ecoregion in ecoregions) {
                    if (ecoregion.Name == name)
                        return ecoregion;
                }
                return null;
            }
        }

        //---------------------------------------------------------------------
        
        public IEcoregion Find(ushort mapCode)
        {
            foreach (Ecoregion ecoregion in ecoregions) {
                if (ecoregion.MapCode == mapCode)
                    return ecoregion;
            }
            return null;
        }

        //---------------------------------------------------------------------

        IEnumerator<IEcoregion> IEnumerable<IEcoregion>.GetEnumerator()
        {
            foreach (Ecoregion ecoregion in ecoregions)
                yield return ecoregion;
        }
 
        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IEcoregion>) this).GetEnumerator();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a dataset from a text file.
        /// </summary>
        /// <param name="path">
        /// The text file with the fire ecoregion definitions.
        /// </param>
        public static IEcoregionDataset ReadFile(string path)
        {
            EcoregionsParser parser = new EcoregionsParser();
            return Data.Load<IEcoregionDataset>(path, parser);
        }
    }
}
