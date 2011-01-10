using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Fire
{
    /// <summary>
    /// Editable dataset of ecoregion parameters.
    /// </summary>
    public interface IEditableEcoregionDataset
        : IEditable<IEcoregionDataset>, IList<IEditableEcoregionParameters>
    {
        //---------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the parameters for a ecoregion.
        /// </summary>
        /// <param name="name">
        /// The ecoregion's name.
        /// </param>
        /// <remarks>
        /// If there are no parameters for the specified name, then the get
        /// accessor returns null.  If the value for the set accessor is null,
        /// then the parameters for the specified name are removed from the
        /// dataset (it is not an error if the name is not in the dataset).
        /// </remarks>
        IEditableEcoregionParameters this[string name]
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Finds an ecoregion's parameters by its map code.
        /// </summary>
        /// <param name="mapCode">
        /// The ecoregion's map code.
        /// </param>
        IEditableEcoregionParameters Find(ushort mapCode);
    }
}
