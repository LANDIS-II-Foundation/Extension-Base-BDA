//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:   , Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;

namespace Landis.Extension.BaseBDA
{
    public class ShortPixel : Pixel
    {
        public Band<short> MapCode  = "The numeric code for each raster cell";

        public ShortPixel()
        {
            SetBands(MapCode);
        }
    }
}
