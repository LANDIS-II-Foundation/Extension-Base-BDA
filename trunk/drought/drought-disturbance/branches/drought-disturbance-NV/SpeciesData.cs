using Landis.SpatialModeling;
using Landis.Library.BiomassCohorts;
using Landis.Core;
using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.Succession;
using Landis.Extension.Succession.Biomass.Species;

namespace Landis.Extension.DroughtDisturbance
{
    public class SpeciesData
    {

        public static Landis.Extension.Succession.Biomass.Species.AuxParm<List<AgeClass>> MortalityTable;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {

            MortalityTable = parameters.MortalityTable;
        }

        public static bool IsOnsetYear(int year, ISpecies species, IEcoregion ecoregion)
        {

            if (DynamicInputs.AllData.ContainsKey(year))
            {

               DynamicInputs.TimestepData = DynamicInputs.AllData[year];
               foreach (IDynamicInputRecord dynrec in DynamicInputs.TimestepData)
                   if (dynrec.OnsetEcoregion == ecoregion && dynrec.OnsetSpecies == species)
                       return true;

            }

            return false;

        }
    
    }
}
