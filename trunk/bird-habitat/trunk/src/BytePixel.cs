//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Srinivas S., Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;

namespace Landis.Extension.Output.BirdHabitat
{
    public class BytePixel : Pixel
    {
        public Band<byte> MapCode  = "The numeric code for each ecoregion";

        public BytePixel() 
        {
            SetBands(MapCode);
        }
    }
}
