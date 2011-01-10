//  Copyright 2006 University of Wisconsin
//  Author:  James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Landis.Landscape;
using Landis.RasterIO;

namespace Landis.Fire
{
    internal static class Ecoregions
    {
        internal static IEcoregionDataset Dataset = null;

        //---------------------------------------------------------------------

        internal static void ReadMap(string path)
        {
            IInputRaster<EcoregionPixel> map = Model.Core.OpenRaster<EcoregionPixel>(path);
            // TODO: make sure its dimensions match landscape's dimensions
            using (map) {
                foreach (Site site in Model.Core.Landscape.AllSites) {
                    EcoregionPixel pixel = map.ReadPixel();
                    if (site.IsActive) {
                        ushort mapCode = pixel.Band0;
                        IEcoregion ecoregion = Dataset.Find(mapCode);
                        if (ecoregion == null)
                            throw new PixelException(site.Location,
                                                     "Unknown map code: {0}",
                                                     mapCode);
                        SiteVars.Ecoregion[site] = ecoregion;
                    }
                }
            }
        }
    }
}
