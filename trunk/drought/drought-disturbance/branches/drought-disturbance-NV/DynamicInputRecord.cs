//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller

using System.Collections.Generic;
using Landis.Core;

namespace Landis.Extension.DroughtDisturbance
{
    /// <summary>
    /// Values for each ecoregion x species combination.
    /// </summary>
    public interface IDynamicInputRecord
    {

        IEcoregion OnsetEcoregion{get;set;}
        ISpecies OnsetSpecies{get;set;}
    }

    public class DynamicInputRecord
    : IDynamicInputRecord
    {

        private IEcoregion ecoregion;
        private ISpecies species;

        public IEcoregion OnsetEcoregion
        {
            get {
                return ecoregion;
            }
            set {
                ecoregion = value;
            }
        }

        public ISpecies OnsetSpecies
        {
            get {
                return species;
            }
            set {
                species = value;
            }
        }

        public DynamicInputRecord()
        {
        }

    }
}
