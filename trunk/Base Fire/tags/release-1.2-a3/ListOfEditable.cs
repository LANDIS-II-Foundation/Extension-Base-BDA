//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Fire
{
    /// <summary>
    /// List of editable items.
    /// </summary>
    public class ListOfEditable<TEditableItem, TItem>
        : List<TEditableItem>
        where TEditableItem : IEditable<TItem>
    {
        /// <summary>
        /// Indicates if each item in the list is complete.
        /// </summary>
        public bool IsEachItemComplete
        {
            get {
                foreach (TEditableItem item in this)
                    if (! item.IsComplete)
                        return false;
                return true;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with the default capacity.
        /// </summary>
        public ListOfEditable()
            : base()
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with a specific capacity.
        /// </summary>
        public ListOfEditable(int capacity)
            : base(capacity)
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with the elements copied from a
        /// collection.
        /// </summary>
        public ListOfEditable(IEnumerable<TEditableItem> collection)
            : base(collection)
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets an array of complete items.
        /// </summary>
        public TItem[] GetComplete()
        {
            if (IsEachItemComplete) {
                TItem[] completeItems = new TItem[this.Count];
                for (int i = 0; i < this.Count; i++)
                    completeItems[i] = this[i].GetComplete();
                return completeItems;
            }
            else
                return null;
        }
    }
}
