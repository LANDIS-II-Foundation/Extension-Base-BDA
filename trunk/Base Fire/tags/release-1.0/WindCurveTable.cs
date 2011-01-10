using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Fire
{
	/// <summary>
	/// Editable parameters for the Fire Curve for a 
	/// collection of ecoregions.
	/// </summary>
	public class WindCurveTable
		: IEditable<IWindCurve[]>
	{
		private IEditableWindCurve[] parameters;

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
		public IEditableWindCurve this[int ecoregionIndex]
		{
			get {
				return parameters[ecoregionIndex];
			}

			set {
				parameters[ecoregionIndex] = value;
			}
		}

		//---------------------------------------------------------------------

		public WindCurveTable(int ecoregionCount)
		{
			parameters = new IEditableWindCurve[ecoregionCount];
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				foreach (IEditableWindCurve editableParms in parameters) {
					if (editableParms != null && !editableParms.IsComplete)
						return false;
				}
				return true;
			}
		}

		//---------------------------------------------------------------------

		public IWindCurve[] GetComplete()
		{
			if (IsComplete) {
				IWindCurve[] eventParms = new IWindCurve[parameters.Length];
				for (int i = 0; i < parameters.Length; i++) {
					IEditableWindCurve editableParms = parameters[i];
					if (editableParms != null)
						eventParms[i] = editableParms.GetComplete();
					else
						eventParms[i] = new WindCurve();
				}
				return eventParms;
			}
			else
				return null;
		}
	}
}
