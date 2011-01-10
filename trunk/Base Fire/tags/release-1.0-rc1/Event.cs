using Landis.Ecoregions;
using Landis.Landscape;
using Landis.Species;
using Landis.Util;
using System.Collections.Generic;
using log4net;

namespace Landis.Fire
{
	public class Event
	{
		private static RelativeLocation[] neighborhood;
		private static IFireParameters[] FireEventParms;
		private static IFireCurve[] FireCurves;
		private static IWindCurve[] WindCurves;
		private static IDamageTable[] damages;
		private static ILog logger;
		public static Ecoregions.IDataset EcoregionsDataset = Model.Ecoregions;

		private ActiveSite initiationSite;
		private int numSiteChecked;
		private int totalSitesDamaged;
		private int cohortsKilled;
		private double severity;
		private byte siteSeverity;  // used to compute maximum cohort severity
									// at a site
		private int[] sitesInEvent = new int[EcoregionsDataset.Count];



		//---------------------------------------------------------------------

		static Event()
		{
			neighborhood = new RelativeLocation[] 
			{
				new RelativeLocation(-1, 0),	// north
				new RelativeLocation(1, 0),		// south
				new RelativeLocation(0, 1),		// east
				new RelativeLocation(0, -1),		// west
			};

			logger = LogManager.GetLogger(typeof(Event));
		}

		//---------------------------------------------------------------------

		public Location StartLocation
		{
			get {
				return initiationSite.Location;
			}
		}

		//---------------------------------------------------------------------

		public int NumSiteChecked
		{
			get {
				return numSiteChecked;
			}
		}
		//---------------------------------------------------------------------

		public int[] SitesInEvent
		{
			get {
				return sitesInEvent;
			}
		}

		//---------------------------------------------------------------------

		public int CohortsKilled
		{
			get {
				return cohortsKilled;
			}
		}

		//---------------------------------------------------------------------

		public double Severity
		{
			get {
				return severity;
			}
		}

		//---------------------------------------------------------------------

		public static void Initialize(IFireParameters[] eventParameters,
					      IFireCurve[] fireCurves,
					      IWindCurve[] windCurves,
		                              IDamageTable[]        damages)
		{
			FireEventParms = eventParameters;
			FireCurves = fireCurves;
			WindCurves = windCurves;
			Event.damages = damages;
		}

		//---------------------------------------------------------------------
		// The probability of fire initiation (different from ignition) 
		// and spread.  Formula from Jian Yang, University of Missouri-Columbia, 
		// personal communication.

		public static double ComputeFireInitSpreadProb(ActiveSite site, int currentTime)
		{
			IEcoregion ecoregion = Model.SiteVars.Ecoregion[site];
			IFireParameters eventParms = FireEventParms[ecoregion.Index];

			int timeSinceLastFire = currentTime - SiteVars.TimeOfLastFire[site];
			double fireInitSpreadProb = 
				1.0 - System.Math.Exp(timeSinceLastFire * (-1.0 / (double) eventParms.FireSpreadAge));
			
			//logger.Debug(string.Format("timeSinceLastFire={0}, fireInitSpreadProb = {1:0.00}", timeSinceLastFire, fireInitSpreadProb));
			return fireInitSpreadProb;
		
		}

		//---------------------------------------------------------------------

		public static Event Initiate(ActiveSite site,
		                             int  currentTime, int timestep)
		{

			IEcoregion ecoregion = Model.SiteVars.Ecoregion[site];
			IFireParameters eventParms = FireEventParms[ecoregion.Index];
			double ignitionProb = eventParms.IgnitionProb * timestep;

			//The initial site must exceed the probability of initiation and
			//have a severity > 0 and exceed the ignition threshold:
			if (	Random.GenerateUniform() <= ignitionProb
				&& Random.GenerateUniform() <= ComputeFireInitSpreadProb(site, currentTime)
				&& calcSeverity(site, currentTime) > 0) 
			{
				Event FireEvent = new Event(site);
				FireEvent.Spread(currentTime);
				return FireEvent;
			}
			else
			return null;
		}

		//---------------------------------------------------------------------

		public static int ComputeSize(IFireParameters eventParms)
		{
			double sizeGenerated = Random.GenerateExponential(eventParms.MeanSize);
			if (sizeGenerated < eventParms.MinSize)
				return (int) (eventParms.MinSize/Model.CellArea);
			else if (sizeGenerated > eventParms.MaxSize)
				return (int) (eventParms.MaxSize/Model.CellArea);
			else
				return (int) (sizeGenerated/Model.CellArea);
		}

		//---------------------------------------------------------------------

		private Event(ActiveSite initiationSite)
		{
			this.initiationSite = initiationSite;
			foreach(IEcoregion ecoregion in Model.Ecoregions)
				this.sitesInEvent[ecoregion.Index] = 0;
			this.cohortsKilled = 0;
			this.severity = 0;
			this.numSiteChecked = 0;

			logger.Debug(string.Format("New Fire event at {0}",initiationSite.Location));
		}

		//---------------------------------------------------------------------

		private void Spread(int currentTime)
		{
			int windDirection = (int) (Util.Random.GenerateUniform() * 4)+1;
			//int windDirection = 4;
			
			int[] size         = new int[EcoregionsDataset.Count];	// in # of sites
			
			int totalSitesInEvent    = 0;
			long totalSiteSeverities = 0;
			int maxEcoregionSize     = 0;
			int siteCohortsKilled    = 0;

			IEcoregion ecoregion = Model.SiteVars.Ecoregion[initiationSite];
			int ecoIndex = ecoregion.Index;
			size[ecoIndex] = (int)(ComputeSize(FireEventParms[ecoregion.Index])/Model.CellArea);
			if(size[ecoIndex] > maxEcoregionSize) 
				maxEcoregionSize = size[ecoIndex];

			//Create a queue of neighboring sites to which the fire will spread:
			Queue<Site> sitesToConsider = new Queue<Site>();
			sitesToConsider.Enqueue(initiationSite);

			//Fire size cannot be larger than the size calculated for each ecoregion.
			//Fire size cannot be larger than the largest size for any ecoregion.
			while (sitesToConsider.Count > 0 && 
				this.sitesInEvent[ecoIndex] < size[ecoIndex] && 
				totalSitesInEvent < maxEcoregionSize) 
			{
				this.numSiteChecked++;
				Site site = sitesToConsider.Dequeue();
				ecoregion = Model.SiteVars.Ecoregion[site];
				if(ecoregion.Index != ecoIndex && size[ecoregion.Index] < 1)
				{
					ecoIndex = ecoregion.Index;
					size[ecoIndex] = (int)(ComputeSize(FireEventParms[ecoregion.Index]));
					if(size[ecoIndex] > maxEcoregionSize) 
						maxEcoregionSize = size[ecoIndex];
				}
				
				SiteVars.Event[site] = this;
				this.sitesInEvent[ecoIndex]++;
				totalSitesInEvent++;
				
				ActiveSite activeSite = site as ActiveSite;
				siteSeverity = calcSeverity(activeSite, currentTime);
				SiteVars.Severity[activeSite] = siteSeverity;
				if (activeSite != null && siteSeverity>0)
				{
					siteCohortsKilled = Damage(activeSite, currentTime);
					if (siteCohortsKilled > 0) 
					{
						totalSitesDamaged++;
						totalSiteSeverities += siteSeverity;
						Model.SiteVars.Disturbed[activeSite] = true;
					}						
					SiteVars.TimeOfLastFire[activeSite] = currentTime;

					//Next, add site's neighbors in random order to the list of
					//sites to consider.  The neighbors cannot be part of
					//any other Fire event in the current timestep, and
					//cannot already be on the list.

					//Fire can burn into neighbors only if the 
					//spread probability is exceeded.
					List<Site> neighbors = GetNeighbors(site, windDirection);
					Random.Shuffle(neighbors);
					foreach (Site neighbor in neighbors) 
					{
						ActiveSite neighborSite = neighbor as ActiveSite;
						if(Random.GenerateUniform() <= ComputeFireInitSpreadProb(neighborSite, currentTime))
							continue;
						if(SiteVars.Event[neighbor] != null)
							continue;
						if(sitesToConsider.Contains(neighbor))
							continue;
						sitesToConsider.Enqueue(neighbor);
					}	
				}

			}

			if (this.totalSitesDamaged == 0)
				this.severity = 0;
			else
			{
				this.severity = ((double) totalSiteSeverities) / this.totalSitesDamaged;
				logger.Debug(string.Format("Fire Event Completed.  Severity={0:0.00}, SiteDamage={1},CohortsKilled={2}.", 
					this.severity,this.totalSitesDamaged,this.cohortsKilled));
				foreach(IEcoregion eco in Model.Ecoregions)
					logger.Debug(string.Format("EcoName={0}, sitesInEvent={1}.", eco.Name, this.sitesInEvent[eco.Index]));
			}
		}

		//---------------------------------------------------------------------

		private List<Site> GetNeighbors(Site site, int windDirection)
		{
			List<Site> neighbors = new List<Site>(5);
			foreach (RelativeLocation relativeLoc in neighborhood) {
				Site neighbor = site.GetNeighbor(relativeLoc);
				if (neighbor != null)
					neighbors.Add(neighbor);
			}

			int vertical=0;
			int horizontal=0;
			if(windDirection==1) {  //wind is from south
				vertical = -2;
				horizontal = 0;
			}
			if(windDirection==2) {  //wind is from north
				vertical = 2;
				horizontal = 0;
			}
			if(windDirection==3) {  //wind is from east
				vertical = 0;
				horizontal = -2;
			}
			if(windDirection==4) {  //wind is from west
				vertical = 0;
				horizontal = 2;
			}

			RelativeLocation relativeLoc5 = new RelativeLocation(vertical, horizontal);
			Site neighbor5 = site.GetNeighbor(relativeLoc5);
			if (neighbor5 != null)
				neighbors.Add(neighbor5);

			return neighbors;
		}
		
		//---------------------------------------------------------------------

		public static byte calcSeverity(ActiveSite site, int currentTime)
		{
			IEcoregion ecoregion = Model.SiteVars.Ecoregion[site];
			IFireCurve fireCurve = FireCurves[ecoregion.Index];
			IWindCurve windCurve = WindCurves[ecoregion.Index];
			int severity = 0;

			int timeSinceLastFire = currentTime - SiteVars.TimeOfLastFire[site];
			if(fireCurve.Severity1 != -1 && timeSinceLastFire >= fireCurve.Severity1 )
				severity = 1;
			if(fireCurve.Severity2 != -1 && timeSinceLastFire >= fireCurve.Severity2 )
				severity = 2;
			if(fireCurve.Severity3 != -1 && timeSinceLastFire >= fireCurve.Severity3)
				severity = 3;
			if(fireCurve.Severity4 != -1 && timeSinceLastFire >= fireCurve.Severity4)
				severity = 4;
			if(fireCurve.Severity5 != -1 && timeSinceLastFire >= fireCurve.Severity5)
				severity = 5;

			int windSeverity = 0;
			int timeSinceLastWind = 0;
			if(SiteVars.TimeOfLastWind != null)
			{
				timeSinceLastWind = currentTime - SiteVars.TimeOfLastWind[site];
				if(windCurve.Severity1 != -1 && timeSinceLastWind <= windCurve.Severity1 )
					windSeverity = 1;
				if(windCurve.Severity2 != -1 && timeSinceLastWind <= windCurve.Severity2 )
					windSeverity = 2;
				if(windCurve.Severity3 != -1 && timeSinceLastWind <= windCurve.Severity3 )
					windSeverity = 3;
				if(windCurve.Severity4 != -1 && timeSinceLastWind <= windCurve.Severity4 )
					windSeverity = 2;
				if(windCurve.Severity5 != -1 && timeSinceLastWind <= windCurve.Severity5 )
					windSeverity = 5;
			}
			
			if(windSeverity > severity) 
			{
				severity = windSeverity;
				SiteVars.TimeOfLastWind[site] = currentTime;
				logger.Debug(string.Format("Calculated fire severity. timeSinceLastWind={0}, Severity={1}.", timeSinceLastWind, windSeverity));
			} else
				logger.Debug(string.Format("Calculated fire severity. timeSinceLastFire={0}, Severity={1}.", timeSinceLastFire, severity));
			return (byte) severity;
		}


		//---------------------------------------------------------------------

		private int Damage(ActiveSite site, int currentTime)
		{
			ISiteCohorts<AgeCohort.ICohort> cohorts = Model.GetSuccession<AgeCohort.ICohort>().Cohorts[site];
			int previousCohortsKilled = this.cohortsKilled;
			cohorts.Remove(this.DamageCohort, site);
			return this.cohortsKilled - previousCohortsKilled;
		}

		//---------------------------------------------------------------------
		//DamageCohort is a filter to determine which cohorts are removed.

		private bool DamageCohort(AgeCohort.ICohort cohort)
		{
			//logger.Debug(string.Format("Damaging Cohorts. Severity = {0}", siteSeverity));
			//Fire Severity 5 kills all cohorts:
			if(siteSeverity == 5) 
			{
				this.cohortsKilled++;
				logger.Debug(string.Format("  cohort {0}:{1} killed, severity =5", cohort.Species.Name, cohort.Age));
				return true;
			}
			
			//Otherwise, use damage table to calculate damage.
			//Read table backwards; most severe first.
			float ageAsPercent = cohort.Age / (float) cohort.Species.Longevity;
			for (int i = damages.Length-1; i >= 0; --i) 
			{
				IDamageTable damage = damages[i];
				if (siteSeverity - cohort.Species.FireTolerance == damage.SeverTolerDifference) {
					if (damage.MaxAge >= ageAsPercent) {
						this.cohortsKilled++;
						logger.Debug(string.Format("  cohort {0}:{1} killed, damage class {2}", cohort.Species.Name, cohort.Age, damage.SeverTolerDifference));
						return true;  //Yes, kill me.
					}
					break;  // No need to search further in the table
				}
			}
			return false;
		}
	}
}
