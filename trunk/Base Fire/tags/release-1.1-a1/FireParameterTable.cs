//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Fire
{
	/// <summary>
	/// Editable parameters (size and frequency) for Fire events for a
	/// collection of ecoregions.
	/// </summary>
	public class FireParameterTable
		: IEditable<IFireParameters[]>
	{
		private IEditableFireParameters[] parameters;

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of ecoregions in the dataset.
		/// </summary>
		public int Count
		{
			get {
				return parameters.Length;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The event parameters for an ecoregion.
		/// </summary>
		public IEditableFireParameters this[int ecoregionIndex]
		{
			get {
				return parameters[ecoregionIndex];
			}

			set {
				parameters[ecoregionIndex] = value;
			}
		}

		//---------------------------------------------------------------------

		public FireParameterTable(int ecoregionCount)
		{
			parameters = new IEditableFireParameters[ecoregionCount];
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				foreach (IEditableFireParameters editableParms in parameters) {
					if (editableParms != null && !editableParms.IsComplete)
						return false;
				}
				return true;
			}
		}

		//---------------------------------------------------------------------

		public IFireParameters[] GetComplete()
		{
			if (IsComplete) {
				IFireParameters[] eventParms = new IFireParameters[parameters.Length];
				for (int i = 0; i < parameters.Length; i++) {
					IEditableFireParameters editableParms = parameters[i];
					if (editableParms != null)
						eventParms[i] = editableParms.GetComplete();
					else
						eventParms[i] = new FireParameters();
				}
				return eventParms;
			}
			else
				return null;
		}
	}
}
