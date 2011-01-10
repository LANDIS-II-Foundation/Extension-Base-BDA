using Landis.Cohorts;
using Landis.Landscape;
using Landis.RasterIO;
using Landis.Util;
using Landis.Ecoregions;
using System.Collections.Generic;
using System.IO;

namespace Landis.Fire
{
	///<summary>
	/// A disturbance plug-in that simulates Fire disturbance.
	/// </summary>
	public class PlugIn
		: Landis.PlugIns.IDisturbance, Landis.PlugIns.I2PhaseInitialization
	{
		private int timestep;
		private int nextTimeToRun;
		private string mapNameTemplate;
		private StreamWriter log;

		//---------------------------------------------------------------------

		/// <summary>
		/// The name that users refer to the plug-in by.
		/// </summary>
		public string Name
		{
			get {
				return "Fire";
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The next timestep when the plug-in should run.
		/// </summary>
		public int NextTimeToRun
		{
			get {
				return nextTimeToRun;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes the plug-in with a data file.
		/// </summary>
		/// <param name="dataFile">
		/// Path to the file with initialization data.
		/// </param>
		/// <param name="startTime">
		/// Initial timestep (year): the timestep that will be passed to the
		/// first call to the component's Run method.
		/// </param>
		public void Initialize(string dataFile,
		                       int    startTime)
		{
			ParameterParser.EcoregionsDataset = Model.Ecoregions;
			ParameterParser parser = new ParameterParser();
			IParameters parameters = Data.Load<IParameters>(dataFile, parser);

			timestep = parameters.Timestep;
			nextTimeToRun = startTime - 1 + timestep;
			mapNameTemplate = parameters.MapNamesTemplate;

			SiteVars.Initialize(Model.GetSuccession<AgeCohort.ICohort>().Cohorts);
			Event.Initialize(parameters.FireParameters,
					         parameters.FireCurves,
					         parameters.WindCurves,
			                 parameters.FireDamages);

			UI.WriteLine("Opening Fire log file \"{0}\" ...", parameters.LogFileName);
			log = Data.CreateTextFile(parameters.LogFileName);
			log.AutoFlush = true;
			log.Write("Time,Initiation Site,Sites Checked,Cohorts Killed,Mean Severity,");
			foreach (IEcoregion ecoregion in Model.Ecoregions)
			{
				if(ecoregion.Active)
					log.Write("{0},", ecoregion.Name);
			}
			log.WriteLine("");
		}
		
		public void InitializePhase2() //InitializePhase2
		{
	       		SiteVars.InitializeTimeOfLastWind();
		}

		//---------------------------------------------------------------------

		///<summary>
		/// Run the plug-in at a particular timestep.
		///</summary>
		public void Run(int currentTimestep)
		{
			UI.WriteLine("Processing landscape for Fire events ...");

			nextTimeToRun += timestep;

			SiteVars.Event.SiteValues = null;
			SiteVars.Severity.ActiveSiteValues = 0;

			int eventCount = 0;
			foreach (ActiveSite site in Model.Landscape) {
			
				Event FireEvent = Event.Initiate(site, currentTimestep, timestep);
				if (FireEvent != null) {
					LogEvent(currentTimestep, FireEvent);
					eventCount++;
				}
			}
			UI.WriteLine("  Fire events: {0}", eventCount);

			//  Write Fire severity map
			IOutputRaster<SeverityPixel> map = CreateMap(currentTimestep);
			using (map) {
				SeverityPixel pixel = new SeverityPixel();
				foreach (Site site in Model.Landscape.AllSites) {
					if (site.IsActive) {
						if (Model.SiteVars.Disturbed[site])
							pixel.Band0 = (byte) (SiteVars.Severity[site] + 1);
						else
							pixel.Band0 = 1;
					}
					else {
						//	Inactive site
						pixel.Band0 = 0;
					}
					map.WritePixel(pixel);
				}
			}
		}

		//---------------------------------------------------------------------

		private void LogEvent(int   currentTime,
		                      Event FireEvent)
		{
			if(FireEvent.Severity > 0) 
			{
				log.Write("{0},\"{1}\",{2},{3},{4:0.0}",
			              currentTime,
			              FireEvent.StartLocation,
			              FireEvent.NumSiteChecked,
			              FireEvent.CohortsKilled,
			              FireEvent.Severity);
				foreach (IEcoregion ecoregion in Model.Ecoregions)
				{
					if(ecoregion.Active)
						log.Write(",{0}", FireEvent.SitesInEvent[ecoregion.Index]);
				}
				log.WriteLine("");
			}
		}

		//---------------------------------------------------------------------

		private IOutputRaster<SeverityPixel> CreateMap(int currentTime)
		{
			string path = MapNames.ReplaceTemplateVars(mapNameTemplate, currentTime);
			UI.WriteLine("Writing Fire severity map to {0} ...", path);
			return Util.Raster.Create<SeverityPixel>(path,
			                                         Model.LandscapeMapDims,
			                                         Model.LandscapeMapMetadata);
		}
	}
}

namespace Landis.Fire
{
	public class SeverityPixel
		: SingleBandPixel<byte>
	{
	}
}


