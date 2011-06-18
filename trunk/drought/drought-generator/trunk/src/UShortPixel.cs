using Landis.SpatialModeling;

namespace Landis.Extension.DroughtGenerator
{
    public class UShortPixel : Pixel
    {
        public Band<ushort> MapCode = "The numeric code for each raster cell";

        public UShortPixel()
        {
            SetBands(MapCode);
        }
    }
}
