//  Copyright 2006 University of Wisconsin
//  Author:  James B. Domingo
//  License:  Available at
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Wisc.Flel.GeospatialModeling.Landscapes;
using Wisc.Flel.GeospatialModeling.RasterIO;
using System.IO;
using System.Collections.Generic;


namespace Landis.Extension.BaseFire
{
    public class FireRegions
    {
        public static List<IFireRegion> Dataset;

        //---------------------------------------------------------------------

        public static void ReadMap(string path)
        {
            IInputRaster<UShortPixel> map;

            try {
                map = PlugIn.ModelCore.OpenRaster<UShortPixel>(path);
            }
            catch (FileNotFoundException) {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }

            // TODO: This error checking is needed !!!
            
            /*
            if (map.Dimensions != Model.Core.Landscape.Dimensions)
            {
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }
            */

            using (map) {
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {

                    UShortPixel pixel = map.ReadPixel();
                    if (site.IsActive) {
                        ushort mapCode = pixel.Band0;
                        if (Dataset == null)
                            PlugIn.ModelCore.Log.WriteLine("FireRegion.Dataset not set correctly.");
                        IFireRegion ecoregion = Find(mapCode);

                        if (ecoregion == null)
                            throw new PixelException(site.Location, "Unknown map code: {0}", mapCode);

                        SiteVars.FireRegion[site] = ecoregion;
                    }
                }
            }
        }

        private static IFireRegion Find(int mapCode)
        {
            foreach(IFireRegion fireregion in Dataset)
                if(fireregion.MapCode == mapCode)
                    return fireregion;

            return null;
        }

        public static IFireRegion FindName(string name)
        {
            foreach(IFireRegion fireregion in Dataset)
                if(fireregion.Name == name)
                    return fireregion;

            return null;
        }

    }
}
