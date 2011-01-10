//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf


using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Fire
{
	/// <summary>
	/// An editable set of parameters for the plug-in.
	/// </summary>
	public class EditableParameters
		: IEditable<IParameters>
	{
		private InputValue<int> timestep;

		// The xTable classes have a fixed length equal to the number of ecoregions.
		// If an ecoregion is not included in the user input, the relevant values 
		// are given default values, typically zero (0).
		private FireParameterTable eventParameters;
		private FireCurveTable fireCurves;
		private WindCurveTable windCurves;

		// A ListOfEditable table can have variable length and there are no defaults:
		private ListOfEditable<IEditableDamageTable, IDamageTable> damages;
		private InputValue<string> mapNamesTemplate;
		private InputValue<string> logFileName;

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
					                                  "Value must be = or > 0.");
				timestep = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Fire event parameters for each ecoregion.
		/// </summary>
		public FireParameterTable FireParameters
		{
			get {
				return eventParameters;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Definitions of Fire Curves.
		/// </summary>
		public FireCurveTable FireCurve
		{
			get {
				return fireCurves;
			}
		}

		/// <summary>
		/// Definitions of Fire Curves.
		/// </summary>
		public WindCurveTable WindCurve
		{
			get {
				return windCurves;
			}
		}

		//---------------------------------------------------------------------
		/// <summary>
		/// Definitions of Fire damage classes.
		/// </summary>
		public IList<IEditableDamageTable> FireDamages
		{
			get {
				return damages;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Template for pathnames for output maps.
		/// </summary>
		public InputValue<string> MapNamesTemplate
		{
			get {
				return mapNamesTemplate;
			}

			set {
				if (value != null) {
					MapNames.CheckTemplateVars(value.Actual);
				}
				mapNamesTemplate = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Name for the log file.
		/// </summary>
		public InputValue<string> LogFileName
		{
			get {
				return logFileName;
			}

			set {
				if (value != null) {
					// FIXME: check for null or empty path (value.Actual);
				}
				logFileName = value;
			}
		}

		//---------------------------------------------------------------------

		public EditableParameters(int ecoregionCount)
		{
			eventParameters = new FireParameterTable(ecoregionCount);
			fireCurves = new FireCurveTable(ecoregionCount);
			windCurves = new WindCurveTable(ecoregionCount);
			damages = new ListOfEditable<IEditableDamageTable, IDamageTable>();
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				foreach (object parameter in new object[]{ timestep,
				                                           mapNamesTemplate,
				                                           logFileName }) {
					if (parameter == null)
						return false;
				}
				return eventParameters.IsComplete &&
					fireCurves.IsComplete && 
					windCurves.IsComplete &&
				       damages.IsEachItemComplete &&
				       damages.Count >= 1;
			}
		}

		//---------------------------------------------------------------------

		public IParameters GetComplete()
		{
			if (IsComplete)
				return new Parameters(timestep.Actual,
				                      eventParameters.GetComplete(),
				                      fireCurves.GetComplete(),
				                      windCurves.GetComplete(),
				                      damages.GetComplete(),
				                      mapNamesTemplate.Actual,
				                      logFileName.Actual);
			else
				return null;
		}
	}
}
