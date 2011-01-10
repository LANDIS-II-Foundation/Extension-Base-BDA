using Landis.Landscape;

namespace Landis.Fire
{
	public static class SiteVars
	{
		private static ISiteVar<Event> eventVar;
		private static ISiteVar<int> timeOfLastFire;
		private static ISiteVar<int> timeOfLastWind;
		private static ISiteVar<byte> severity;


		//---------------------------------------------------------------------

		public static void Initialize()
		{
			eventVar           = Model.Landscape.NewSiteVar<Event>(InactiveSiteMode.DistinctValues);
			timeOfLastFire  = Model.Landscape.NewSiteVar<int>();
			severity           = Model.Landscape.NewSiteVar<byte>();

			//Initialize TimeSinceLastFire to the maximum cohort age:
			ISiteCohorts<AgeCohort.ICohort> cohorts;
			foreach (ActiveSite site in Model.Landscape) 
			{
				cohorts = Model.GetSuccession<AgeCohort.ICohort>().Cohorts[site];
				ushort maxAge   = AgeCohort.Util.GetMaxAge(cohorts);
				SiteVars.timeOfLastFire[site] = maxAge * -1;
			}

		}

		//---------------------------------------------------------------------

		public static void InitializeTimeOfLastWind()
		{

			timeOfLastWind  = Model.SiteVars.GetVar<int>("Wind.TimeOfLastEvent");

		}
		//---------------------------------------------------------------------

		public static ISiteVar<Event> Event
		{
			get {
				return eventVar;
			}
		}

		//---------------------------------------------------------------------

		public static ISiteVar<int> TimeOfLastFire
		{
			get {
				return timeOfLastFire;
			}
		}

		public static ISiteVar<int> TimeOfLastWind
		{
			get {
				return timeOfLastWind;
			}
		}
		//---------------------------------------------------------------------

		public static ISiteVar<byte> Severity
		{
			get {
				return severity;
			}
		}

	}
}
