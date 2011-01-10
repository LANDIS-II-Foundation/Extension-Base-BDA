using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Fire
{
	/// <summary>
	/// Editable parameters for the Fire Curve for a 
	/// collection of ecoregions.
	/// </summary>
	public class FireCurveTable
		: IEditable<IFireCurve[]>
	{
		private IEditableFireCurve[] parameters;

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
		public IEditableFireCurve this[int ecoregionIndex]
		{
			get {
				return parameters[ecoregionIndex];
			}

			set {
				parameters[ecoregionIndex] = value;
			}
		}

		//---------------------------------------------------------------------

		public FireCurveTable(int ecoregionCount)
		{
			parameters = new IEditableFireCurve[ecoregionCount];
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				foreach (IEditableFireCurve editableParms in parameters) {
					if (editableParms != null && !editableParms.IsComplete)
						return false;
				}
				return true;
			}
		}

		//---------------------------------------------------------------------

		public IFireCurve[] GetComplete()
		{
			if (IsComplete) {
				IFireCurve[] eventParms = new IFireCurve[parameters.Length];
				for (int i = 0; i < parameters.Length; i++) {
					IEditableFireCurve editableParms = parameters[i];
					if (editableParms != null)
						eventParms[i] = editableParms.GetComplete();
					else
						eventParms[i] = new FireCurve();
				}
				return eventParms;
			}
			else
				return null;
		}
	}
}
