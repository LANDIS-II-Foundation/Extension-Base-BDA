//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Srinivas S., Robert M. Scheller, James B. Domingo

using Landis.SpatialModeling;

namespace Landis.Extension.Output.BirdHabitat
{
    public class ShortPixel : Pixel
    {
        public Band<short> MapCode  = "The numeric value for map code";

        public ShortPixel() 
        {
            SetBands(MapCode);
        }
    }
}
