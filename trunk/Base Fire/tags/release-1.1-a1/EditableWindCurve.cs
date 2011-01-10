//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf


using Edu.Wisc.Forest.Flel.Util;

// Validate Ecoregion Parameter Table
// The interface (IEditableWindCurve) is listed first.
namespace Landis.Fire
{
	/// <summary>
	/// Editable parameters for the Fire Curve for an 
	/// ecoregion.
	/// </summary>
	public interface IEditableWindCurve
		: IEditable<IWindCurve>
	{
		/// <summary>
		/// Severity 1 Age (years)
		/// </summary>
		InputValue<int> Severity1
		{get;set;}

		//---------------------------------------------------------------------

		/// <summary>
		/// Severity2 Age (years)
		/// </summary>
		InputValue<int> Severity2
		{get;set;}

		//---------------------------------------------------------------------

		/// <summary>
		/// Severity3 Age (years)
		/// </summary>
		InputValue<int> Severity3
		{get;set;}

		//---------------------------------------------------------------------
		/// <summary>
		/// Severity4 Age (years)
		/// </summary>
		InputValue<int> Severity4
		{get;set;}
		//---------------------------------------------------------------------
		/// <summary>
		/// Severity5 Age (years)
		/// </summary>
		InputValue<int> Severity5
		{get;set;}
	}
}


namespace Landis.Fire
{
	/// <summary>
	/// Editable Fire Curve for an ecoregion.
	/// </summary>
	public class EditableWindCurve
		: IEditableWindCurve
	{
		private InputValue<int> severity1;
		private InputValue<int> severity2;
		private InputValue<int> severity3;
		private InputValue<int> severity4;
		private InputValue<int> severity5;

		//---------------------------------------------------------------------

		/// <summary>
		/// Severity 1 Age (years)
		/// </summary>
		public InputValue<int> Severity1
		{
			get {
				return severity1;
			}

			set {
				if (value != null) {
					if (value.Actual < -1)
						throw new InputValueException(value.String,
						                              "Value must be = or > -1.");
					if (severity1 != null && value.Actual != -1 && value.Actual > severity2.Actual)
						throw new InputValueException(value.String,
						                              "Value must be = -1 or < next highest severity.");
				}
				severity1 = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Severity 2 Age (years)
		/// </summary>
		public InputValue<int> Severity2
		{
			get {
				return severity2;
			}

			set {
				if (value != null) {
					if (value.Actual < -1)
						throw new InputValueException(value.String,
						                              "Value must be = or > -1.");
					if (severity2 != null && value.Actual != -1 && value.Actual > severity3.Actual)
						throw new InputValueException(value.String,
						                              "Value must be = -1 or < next highest severity.");
				}
				severity2 = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Severity 3 Age (years)
		/// </summary>
		public InputValue<int> Severity3
		{
			get {
				return severity3;
			}

			set {
				if (value != null) {
					if (value.Actual < -1)
						throw new InputValueException(value.String,
						                              "Value must be = or > -1.");
					if (severity3 != null && value.Actual != -1 && value.Actual > severity4.Actual)
						throw new InputValueException(value.String,
						                              "Value must be = -1 or < next highest severity.");
				}
				severity3 = value;
			}
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Severity 4 Age (years)
		/// </summary>
		public InputValue<int> Severity4
		{
			get {
				return severity4;
			}

			set {
				if (value != null) {
					if (value.Actual < -1)
						throw new InputValueException(value.String,
						                              "Value must be = or > -1.");
					if (severity4 != null && value.Actual != -1 && value.Actual > severity5.Actual)
						throw new InputValueException(value.String,
						                              "Value must be = -1 or < next highest severity.");
				}
				severity4 = value;
			}
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Severity 5 Age (years)
		/// </summary>
		public InputValue<int> Severity5
		{
			get {
				return severity5;
			}

			set {
				if (value != null) {
					if (value.Actual < -1)
						throw new InputValueException(value.String,
						                              "Value must be = or > -1.");
				}
				severity5 = value;
			}
		}
		//---------------------------------------------------------------------

		public EditableWindCurve()
		{
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				foreach (object parameter in new object[]{ 
				severity1,
				severity2,
				severity3,
				severity4,
				severity5}) {
					if (parameter == null)
						return false;
				}
				return true;
			}
		}

		//---------------------------------------------------------------------

		public IWindCurve GetComplete()
		{
			if (IsComplete)
				return new WindCurve(
				severity1.Actual,
				severity2.Actual,
				severity3.Actual,
				severity4.Actual,
				severity5.Actual);
			else
				return null;
		}
	}
}


