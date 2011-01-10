//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Fire
{
	/// <summary>
	/// Definition of a Fire Damage Class.
	/// </summary>
	public interface IDamageTable
	{
		/// <summary>
		/// The maximum cohort ages (as % of species longevity) that the
		/// damage class applies to.
		/// </summary>
		double MaxAge
		{get;}

		//---------------------------------------------------------------------

		/// <summary>
		/// The difference between fire severity and species fire tolerance.
		/// </summary>
		int SeverTolerDifference
		{get;}

	}
}


namespace Landis.Fire
{
	/// <summary>
	/// Definition of a Fire Damage class.
	/// </summary>
	public class DamageTable
		: IDamageTable
	{
		private double maxAge;
		private int severTolerDifference;

		//---------------------------------------------------------------------

		/// <summary>
		/// The maximum cohort ages (as % of species longevity) that the
		/// damage class applies to.
		/// </summary>
		public double MaxAge
		{
			get {return maxAge;}
		}

		/// <summary>
		/// The difference between fire severity and species fire tolerance.
		/// </summary>
		public int SeverTolerDifference
		{
			get {return severTolerDifference;}
		}

		//---------------------------------------------------------------------

		public DamageTable(
		                double maxAge,
		                int  severTolerDifference)
		{
			this.maxAge = maxAge;
			this.severTolerDifference = severTolerDifference;
		}
	}
}
