//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;
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
            IInputRaster<BytePixel> map;

            try {
                map = PlugIn.ModelCore.OpenRaster<BytePixel>(path);
            }
            catch (FileNotFoundException) {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }

            if (map.Dimensions != PlugIn.ModelCore.Landscape.Dimensions)
            {
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

            using (map) {
                BytePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    ushort mapCode = (ushort)pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (Dataset == null)
                            PlugIn.ModelCore.Log.WriteLine("FireRegion.Dataset not set correctly.");
                        IFireRegion ecoregion = Find(mapCode);

                        if (ecoregion == null)
                        {
                            string mesg = string.Format("mapCode = {0}, dimensions.rows = {1}", mapCode, map.Dimensions.Rows);
                            throw new System.ApplicationException(mesg);
                        }

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
