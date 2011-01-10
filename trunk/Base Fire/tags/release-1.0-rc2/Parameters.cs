namespace Landis.Fire
{
	/// <summary>
	/// Parameters for the plug-in.
	/// </summary>
	public interface IParameters
	{
		/// <summary>
		/// Timestep (years)
		/// </summary>
		int Timestep
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Fire event parameters for each ecoregion.
		/// </summary>
		/// <remarks>
		/// Use Ecoregion.Index property to index this array.
		/// </remarks>
		IFireParameters[] FireParameters
		{
			get;
		}

		//---------------------------------------------------------------------
		/// <summary>
		/// Definitions of Fire Curves.
		/// </summary>
		IFireCurve[] FireCurves
		{
			get;
		}

		//---------------------------------------------------------------------
		/// <summary>
		/// Definitions of Wind Curves.
		/// </summary>
		IWindCurve[] WindCurves
		{
			get;
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Definitions of Fire damages.
		/// </summary>
		IDamageTable[] FireDamages
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Template for the filenames for output maps.
		/// </summary>
		string MapNamesTemplate
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Name of log file.
		/// </summary>
		string LogFileName
		{
			get;
		}
	}
}


namespace Landis.Fire
{
	/// <summary>
	/// Parameters for the plug-in.
	/// </summary>
	public class Parameters
		: IParameters
	{
		private int timestep;
		private IFireParameters[] eventParameters;
		private IFireCurve[] fireCurves;
		private IWindCurve[] windCurves;
		private IDamageTable[] damages;
		private string mapNamesTemplate;
		private string logFileName;

		//---------------------------------------------------------------------

		/// <summary>
		/// Timestep (years)
		/// </summary>
		public int Timestep
		{
			get {
				return timestep;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Fire event parameters for each ecoregion.
		/// </summary>
		/// <remarks>
		/// Use Ecoregion.Index property to index this array.
		/// </remarks>
		public IFireParameters[] FireParameters
		{
			get {
				return eventParameters;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Definitions of Fire Curve ages.
		/// </summary>
		public IFireCurve[] FireCurves
		{
			get {
				return fireCurves;
			}
		}

		/// <summary>
		/// Definitions of Wind Curve ages.
		/// </summary>
		public IWindCurve[] WindCurves
		{
			get {
				return windCurves;
			}
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Definitions of Fire severities.
		/// </summary>
		public IDamageTable[] FireDamages
		{
			get {
				return damages;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Template for the filenames for output maps.
		/// </summary>
		public string MapNamesTemplate
		{
			get {
				return mapNamesTemplate;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Name of log file.
		/// </summary>
		public string LogFileName
		{
			get {
				return logFileName;
			}
		}

		//---------------------------------------------------------------------

		public Parameters(int                timestep,
		                  IFireParameters[] eventParameters,
		                  IFireCurve[] fireCurves,
		                  IWindCurve[] windCurves,
		                  IDamageTable[] damages,
		                  string             mapNameTemplate,
		                  string             logFileName)
		{
			this.timestep = timestep;
			this.eventParameters = eventParameters;
			this.fireCurves = fireCurves;
			this.windCurves = windCurves;
			this.damages = damages;
			this.mapNamesTemplate = mapNameTemplate;
			this.logFileName = logFileName;
		}
	}
}
