using Edu.Wisc.Forest.Flel.Util;
using Landis.Species;
using System.Collections.Generic;

namespace Landis.Output.Biomass
{
    /// <summary>
    /// An editable set of parameters for the plug-in.
    /// </summary>
    public class EditableParameters
        : IEditable<IParameters>
    {
        private InputValue<int> timestep;
        private IEnumerable<ISpecies> selectedSpecies;
        private InputValue<string> speciesMapNames;
        private InputValue<bool> makeMaps;
        private InputValue<bool> makeTable;

        //---------------------------------------------------------------------

        public InputValue<int> Timestep
        {
            get {
                return timestep;
            }

            set {
                if (value != null)
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > 0");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------

        public IEnumerable<ISpecies> SelectedSpecies
        {
            get {
                return selectedSpecies;
            }

            set {
                selectedSpecies = value;
            }
        }

        //---------------------------------------------------------------------

        public InputValue<string> SpeciesMapNames
        {
            get {
                return speciesMapNames;
            }

            set {
                if (value != null) {
                    Biomass.SpeciesMapNames.CheckTemplateVars(value.Actual);
                }
                speciesMapNames = value;
            }
        }
        //---------------------------------------------------------------------

        public InputValue<bool> MakeMaps
        {
            get {
                return makeMaps;
            }
            set {
                makeMaps = value;
            }
        }

        //---------------------------------------------------------------------

        public InputValue<bool> MakeTable
        {
            get {
                return makeTable;
            }
            set {
                makeTable = value;
            }
        }
        //---------------------------------------------------------------------

        public EditableParameters()
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Indicates whether the set of parameters is complete.  One or both
        /// groups of species and pool parameters must be complete.
        /// </summary>
        public bool IsComplete
        {
            get {
                if (timestep == null)
                    return false;
                bool speciesParmsComplete = (selectedSpecies != null) &&
                                            (speciesMapNames != null);
                if (speciesParmsComplete)
                    return true;
                return false;
            }
        }

        //---------------------------------------------------------------------

        public IParameters GetComplete()
        {
            if (IsComplete) {
                if (selectedSpecies == null)
                    return new Parameters(timestep.Actual,
                                          selectedSpecies,
                                          null,
                                          makeMaps.Actual,
                                          makeTable.Actual
                                          );
                else
                    return new Parameters(timestep.Actual,
                                          selectedSpecies,
                                          speciesMapNames.Actual,
                                          makeMaps.Actual,
                                          makeTable.Actual
                                          );
            }
            else
                return null;
        }
    }
}
