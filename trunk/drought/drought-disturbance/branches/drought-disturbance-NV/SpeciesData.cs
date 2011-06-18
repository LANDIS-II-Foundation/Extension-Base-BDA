using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using Landis.Core;
using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.Succession;
using Landis.Extension.Succession.Biomass;

namespace Landis.Extension.DroughtDisturbance
{
    public class SpeciesData
    {
        public static Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_Y;
        public static Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_YSE;
        public static Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_B;
        public static Landis.Extension.Succession.Biomass.Species.AuxParm<double> Drought_BSE;
        public static Landis.Extension.Succession.Biomass.Species.AuxParm<int> Drought_Sens;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            //ChangeParameters(parameters);
            Drought_Y = parameters.Drought_Y;
            Drought_YSE = parameters.Drought_YSE;
            Drought_B = parameters.Drought_B;
            Drought_BSE = parameters.Drought_BSE;
            Drought_Sens = parameters.Drought_Sens;
        }
    }
}
