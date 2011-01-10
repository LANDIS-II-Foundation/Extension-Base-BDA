namespace Landis.Fire
{
	/// <summary>
	/// Size and frequency parameters for Fire events in an ecoregion.
	/// </summary>
	public interface IFireParameters
	{
		/// <summary>
		/// Maximum event size (hectares).
		/// </summary>
		int MaxSize
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Mean event size (hectares).
		/// </summary>
		int MeanSize
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Minimum event size (hectares).
		/// </summary>
		int MinSize
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Probability of Ignition.
		/// </summary>
		float IgnitionProb
		{
			get;
		}
		//---------------------------------------------------------------------

		/// <summary>
		/// The Fire Spread Age (years).
		/// </summary>
		int FireSpreadAge
		{
			get;
		}
	}
}


namespace Landis.Fire
{
	public class FireParameters
		: IFireParameters
	{
		private int maxSize;
		private int meanSize;
		private int minSize;
		private float ignitionProb;
		private int fireSpreadAge;

		//---------------------------------------------------------------------

		/// <summary>
		/// Maximum event size (hectares).
		/// </summary>
		public int MaxSize
		{
			get {
				return maxSize;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Mean event size (hectares).
		/// </summary>
		public int MeanSize
		{
			get {
				return meanSize;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Minimum event size (hectares).
		/// </summary>
		public int MinSize
		{
			get {
				return minSize;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Fire ignition probability.
		/// </summary>
		public float IgnitionProb
		{
			get {
				return ignitionProb;
			}
		}
		//---------------------------------------------------------------------
		/// <summary>
		/// Fire spread age.
		/// </summary>
		public int FireSpreadAge
		{
			get {
				return fireSpreadAge;
			}
		}

		//---------------------------------------------------------------------

		public FireParameters(int maxSize,
		                       int meanSize,
		                       int minSize,
		                       float ignitionProb,
		                       int fireSpreadAge)
		{
			this.maxSize = maxSize;
			this.meanSize = meanSize;
			this.minSize = minSize;
			this.ignitionProb = ignitionProb;
			this.fireSpreadAge = fireSpreadAge;
		}

		//---------------------------------------------------------------------

		public FireParameters()
		{
			this.maxSize = 0;
			this.meanSize = 0;
			this.minSize = 0;
			this.ignitionProb = 0;
			this.fireSpreadAge = 0;
		}
	}
}
