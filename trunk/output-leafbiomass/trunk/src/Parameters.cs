using Landis.Species;
using System.Collections.Generic;

namespace Landis.Output.Biomass
{
    public interface IParameters
    {
        int Timestep {get;}
        IEnumerable<ISpecies> SelectedSpecies {get;}
        string SpeciesMapNames {get;}
        bool MakeMaps {get;}
        bool MakeTable {get;}
    }
    /// <summary>
    /// The parameters for the plug-in.
    /// </summary>
    public class Parameters
        : IParameters
    {
        private int timestep;
        private IEnumerable<ISpecies> selectedSpecies;
        private string speciesMapNames;
        private bool makeMaps;
        private bool makeTable;
        //---------------------------------------------------------------------

        public int Timestep
        {
            get {
                return timestep;
            }
        }

        //---------------------------------------------------------------------

        public IEnumerable<ISpecies> SelectedSpecies
        {
            get {
                return selectedSpecies;
            }
        }

        //---------------------------------------------------------------------

        public string SpeciesMapNames
        {
            get {
                return speciesMapNames;
            }
        }

        //---------------------------------------------------------------------

        public bool MakeMaps
        {
            get {
                return makeMaps;
            }
        }
        //---------------------------------------------------------------------

        public bool MakeTable
        {
            get {
                return makeTable;
            }
        }
        //---------------------------------------------------------------------

        public Parameters(int                   timestep,
                          IEnumerable<ISpecies> selectedSpecies,
                          string                speciesMapNames,
                          bool makeMaps,
                          bool makeTable
                          )
        {
            this.timestep = timestep;
            this.selectedSpecies = selectedSpecies;
            this.speciesMapNames = speciesMapNames;
            this.makeMaps = makeMaps;
            this.makeTable = makeTable;
        }
    }
}
