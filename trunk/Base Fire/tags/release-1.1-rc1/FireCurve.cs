//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf


namespace Landis.Fire
{
	/// <summary>
	/// Fire Curve parameters for an ecoregion.
	/// </summary>
	public interface IFireCurve
	{
		/// <summary>
		/// Severity 1 (5 point scale).
		/// </summary>
		int Severity1
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Severity 2 (5 point scale).
		/// </summary>
		int Severity2
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Severity 3 (5 point scale).
		/// </summary>
		int Severity3
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Severity 4 (5 point scale).
		/// </summary>
		int Severity4
		{
			get;
		}
		//---------------------------------------------------------------------

		/// <summary>
		/// Severity 5 (5 point scale).
		/// </summary>
		int Severity5
		{
			get;
		}
	}
}


namespace Landis.Fire
{
	public class FireCurve
		: IFireCurve
	{
		private int severity1;
		private int severity2;
		private int severity3;
		private int severity4;
		private int severity5;

		//---------------------------------------------------------------------
		/// <summary>
		/// Severity 1, time in years.
		/// </summary>
		public int Severity1
		{
			get {
				return severity1;
			}
		}

		//---------------------------------------------------------------------
		/// <summary>
		/// Severity 2 , time in years.
		/// </summary>
		public int Severity2
		{
			get {
				return severity2;
			}
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Severity 3, time in years.
		/// </summary>
		public int Severity3
		{
			get {
				return severity3;
			}
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Severity 4, time in years.
		/// </summary>
		public int Severity4
		{
			get {
				return severity4;
			}
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Severity 5, time in years.
		/// </summary>
		public int Severity5
		{
			get {
				return severity5;
			}
		}
		//---------------------------------------------------------------------

		public FireCurve(
		int severity1,
		int severity2,
		int severity3,
		int severity4,
		int severity5)
		{
		this.severity1 = severity1;
		this.severity2 = severity2;
		this.severity3 = severity3;
		this.severity4 = severity4;
		this.severity5 = severity5;
		}

		//---------------------------------------------------------------------

		public FireCurve()
		{
		this.severity1 = 0;
		this.severity2 = 0;
		this.severity3 = 0;
		this.severity4 = 0;
		this.severity5 = 0;
		}
	}
}
