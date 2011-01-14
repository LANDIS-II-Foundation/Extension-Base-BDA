//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:    Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;
using System.Collections.Generic;
using Troschuetz.Random;

namespace Landis.Extension.BaseFire
{
    public class Event
        : ICohortDisturbance
    {
        private static RelativeLocation[] neighborhood;
        private static List<IDamageTable> damages;

        private ActiveSite initiationSite;
        private int numSiteChecked;
        private int totalSitesDamaged;
        private int cohortsKilled;
        private double severity;
        private int[] sitesInEvent;

        private ActiveSite currentSite; // current site where cohorts are being damaged
        private byte siteSeverity;      // used to compute maximum cohort severity at a site

        //---------------------------------------------------------------------

        static Event()
        {
            neighborhood = new RelativeLocation[] 
            {
                new RelativeLocation(-1,  0),  // north
                new RelativeLocation(-1,  1),  // northeast
                new RelativeLocation( 0,  1),  // east
                new RelativeLocation( 1,  1),  // southeast
                new RelativeLocation( 1,  0),  // south
                new RelativeLocation( 1, -1),  // southwest
                new RelativeLocation( 0, -1),  // west
                new RelativeLocation(-1, -1),  // northwest
            };
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

        ExtensionType IDisturbance.Type
        {
            get {
                return PlugIn.Type;
            }
        }

        //---------------------------------------------------------------------

        ActiveSite IDisturbance.CurrentSite
        {
            get {
                return currentSite;
            }
        }

        //---------------------------------------------------------------------

        public static void Initialize(List<IDamageTable>  damages)
        {
            Event.damages = damages;
        }

        //---------------------------------------------------------------------

        // The probability of fire initiation (different from ignition)
        // and spread.  Formula from Jian Yang, University of Missouri-Columbia, 
        // personal communication.

        public static double ComputeFireInitSpreadProb(ActiveSite site, int currentTime)
        {
            IFireRegion ecoregion = SiteVars.FireRegion[site];

            int timeSinceLastFire = currentTime - SiteVars.TimeOfLastFire[site];
            double fireInitSpreadProb = 
                1.0 - System.Math.Exp(timeSinceLastFire * (-1.0 / (double) ecoregion.FireSpreadAge));
            
            return fireInitSpreadProb;
        
        }

        //---------------------------------------------------------------------

        public static Event Initiate(ActiveSite site,
                                     int        currentTime,
                                     int        timestep)
        {
            IFireRegion ecoregion = SiteVars.FireRegion[site];
            
            //Adjust ignition probability (measured on an annual basis) for the 
            //user determined fire time step.
            double ignitionProb = ecoregion.IgnitionProbability * timestep;

            //The initial site must exceed the probability of initiation and
            //have a severity > 0 and exceed the ignition threshold:

            if (PlugIn.ModelCore.GenerateUniform() <= ignitionProb
                && PlugIn.ModelCore.GenerateUniform() <= ComputeFireInitSpreadProb(site, currentTime)
                && CalcSeverity(site, currentTime) > 0) 
            {
                Event FireEvent = new Event(site);
                FireEvent.Spread(currentTime);
                return FireEvent;
            }
            else
                return null;
        }

        //---------------------------------------------------------------------

        public static int ComputeSize(IFireRegion ecoregion)
        {
            if (ecoregion.MeanSize <= 0)
                return 0;
            
            PlugIn.ModelCore.ExponentialDistribution.Lambda = 1.0 / ecoregion.MeanSize;
            double sizeGenerated = PlugIn.ModelCore.ExponentialDistribution.NextDouble();
            
            int finalSize;

            if (sizeGenerated < ecoregion.MinSize)
                finalSize = (int)(ecoregion.MinSize / PlugIn.ModelCore.CellArea);
            else if (sizeGenerated > ecoregion.MaxSize)
            {
                finalSize = (int)(ecoregion.MaxSize / PlugIn.ModelCore.CellArea);
            }    
            else
                finalSize = (int)(sizeGenerated / PlugIn.ModelCore.CellArea);

            return finalSize;
        }

        //---------------------------------------------------------------------

        private Event(ActiveSite initiationSite)
        {
            this.initiationSite = initiationSite;
            this.sitesInEvent = new int[FireRegions.Dataset.Count];
            foreach(IFireRegion ecoregion in FireRegions.Dataset)
                this.sitesInEvent[ecoregion.Index] = 0;
            this.cohortsKilled = 0;
            this.severity = 0;
            this.numSiteChecked = 0;
        }

        //---------------------------------------------------------------------

        private void Spread(int currentTime)
        {
            int windDirection = (int)(PlugIn.ModelCore.GenerateUniform() * 8);
            double windSpeed = PlugIn.ModelCore.GenerateUniform();
            
            int[] size = new int[FireRegions.Dataset.Count]; // in # of sites
            
            int totalSitesInEvent    = 0;
            long totalSiteSeverities = 0;
            int maxFireRegionSize     = 0;
            int siteCohortsKilled    = 0;

            IFireRegion ecoregion = SiteVars.FireRegion[initiationSite];
            int ecoIndex = ecoregion.Index;

            size[ecoIndex] = ComputeSize(ecoregion);

            if (size[ecoIndex] > maxFireRegionSize) 
                maxFireRegionSize = size[ecoIndex];

            //Create a queue of neighboring sites to which the fire will spread:
            Queue<Site> sitesToConsider = new Queue<Site>();
            sitesToConsider.Enqueue(initiationSite);

            //Fire size cannot be larger than the size calculated for each ecoregion.
            //Fire size cannot be larger than the largest size for any ecoregion.
            while (sitesToConsider.Count > 0 && 
                this.sitesInEvent[ecoIndex] < size[ecoIndex] && 
                totalSitesInEvent < maxFireRegionSize) 
            {
                this.numSiteChecked++;
                Site site = sitesToConsider.Dequeue();
                currentSite = (ActiveSite) site; //site as ActiveSite;

                ecoregion = SiteVars.FireRegion[site];
                if (ecoregion.Index != ecoIndex) 
                {
                    ecoIndex = ecoregion.Index;
                    if (size[ecoIndex] < 1)
                    {
                        size[ecoIndex] = ComputeSize(ecoregion);
                        if (size[ecoIndex] > maxFireRegionSize) 
                            maxFireRegionSize = size[ecoIndex];
                    }
                }
                
                SiteVars.Event[site] = this;

                if (currentSite != null)
                {
                    siteSeverity = CalcSeverity(currentSite, currentTime);
                }
                else
                    siteSeverity = 0;

                if (siteSeverity > 0)
                {

                    this.sitesInEvent[ecoIndex]++;
                    totalSitesInEvent++;
                        
                    SiteVars.Severity[currentSite] = siteSeverity;
                    SiteVars.TimeOfLastFire[currentSite] = currentTime;

                    totalSiteSeverities += siteSeverity;
                    SiteVars.Disturbed[currentSite] = true;

                    siteCohortsKilled = KillSiteCohorts(currentSite);
                    if (siteCohortsKilled > 0) 
                    {
                        totalSitesDamaged++;
                    }

                    //Next, add site's neighbors in random order to the list of
                    //sites to consider.  The neighbors cannot be part of
                    //any other Fire event in the current timestep, and
                    //cannot already be on the list.

                    //Fire can burn into neighbors only if the 
                    //spread probability is exceeded.
                    List<Site> neighbors = GetNeighbors(site, windDirection, windSpeed);
                    if (neighbors.Count > 0)
                    {
                        neighbors = PlugIn.ModelCore.shuffle(neighbors);

                        foreach (Site neighbor in neighbors) 
                        {
                            if (!neighbor.IsActive)
                                continue;
                            if (SiteVars.Event[neighbor] != null)
                                continue;
                            if (sitesToConsider.Contains(neighbor))
                                continue;
                            if (PlugIn.ModelCore.GenerateUniform() <= ComputeFireInitSpreadProb((ActiveSite)neighbor, currentTime)) //(neighbor as ActiveSite, currentTime))
                                sitesToConsider.Enqueue(neighbor);
                        }
                    }
                }
            }

            if (this.totalSitesDamaged == 0)
                this.severity = 0;
            else
            {
                this.severity = ((double) totalSiteSeverities) / totalSitesInEvent;
            }
        }

        //---------------------------------------------------------------------

        private List<Site> GetNeighbors(Site   site,
                                        int    windDirection,
                                        double windSpeed)
        {
            if (windDirection > 7)
                windDirection = 7;
            double[] windProbs = 
            {
            (((4.0 - windSpeed)/8.0) * (1+windSpeed)), //Primary direction
            (((4.0 - windSpeed)/8.0) * (1+windSpeed)),
            (((4.0 - windSpeed)/8.0)),
            (((4.0 - windSpeed)/8.0) * (1-windSpeed)),
            (((4.0 - windSpeed)/8.0) * (1-windSpeed)), //Opposite of primary direction
            (((4.0 - windSpeed)/8.0) * (1-windSpeed)),
            (((4.0 - windSpeed)/8.0)),
            (((4.0 - windSpeed)/8.0) * (1+windSpeed)),
            };
            
            double windProb = 0.0;
            int index = 0;
            int success = 0;
            List<Site> neighbors = new List<Site>(9);
            foreach (RelativeLocation relativeLoc in neighborhood) 
            {
                Site neighbor = site.GetNeighbor(relativeLoc);

                if (index + windDirection > 7) 
                    windProb = windProbs[index + windDirection - 8];
                else 
                    windProb = windProbs[index + windDirection];
                if (neighbor != null
                    && PlugIn.ModelCore.NextDouble() < windProb)  
                {
                    neighbors.Add(neighbor);
                    success++;
                }
                index++;
            }
            //Next, add the 9th neighbor, a neighbor one cell beyond the 
            //8 nearest neighbors.
            //array index 0 = north; 1 = northeast, 2 = east,...,8 = northwest
            int[] vertical  ={2,2,0,-2,-2,-2,0,2};
            int[] horizontal={0,2,2,2,0,-2,-2,-2};

            RelativeLocation relativeLoc9 = 
                new RelativeLocation(vertical[windDirection], horizontal[windDirection]);
            Site neighbor9 = site.GetNeighbor(relativeLoc9);
            if (neighbor9 != null && PlugIn.ModelCore.GenerateUniform() < windSpeed)
                neighbors.Add(neighbor9);
            return neighbors;
        }
        
        //---------------------------------------------------------------------

        public static byte CalcSeverity(ActiveSite site,
                                        int        currentTime)
        {
            IFireRegion ecoregion = SiteVars.FireRegion[site];
            IFuelCurve fuelCurve = ecoregion.FuelCurve;
            IWindCurve windCurve = ecoregion.WindCurve;
            int severity = 0;

            int timeSinceLastFire = currentTime - SiteVars.TimeOfLastFire[site];
            if (fuelCurve.Severity1 != -1 && timeSinceLastFire >= fuelCurve.Severity1 )
                severity = 1;
            if (fuelCurve.Severity2 != -1 && timeSinceLastFire >= fuelCurve.Severity2 )
                severity = 2;
            if (fuelCurve.Severity3 != -1 && timeSinceLastFire >= fuelCurve.Severity3)
                severity = 3;
            if (fuelCurve.Severity4 != -1 && timeSinceLastFire >= fuelCurve.Severity4)
                severity = 4;
            if (fuelCurve.Severity5 != -1 && timeSinceLastFire >= fuelCurve.Severity5)
                severity = 5;

            int windSeverity = 0;
            int timeSinceLastWind = 0;
            if (SiteVars.TimeOfLastWind != null)
            {
                timeSinceLastWind = currentTime - SiteVars.TimeOfLastWind[site];
                if (windCurve.Severity1 != -1 && timeSinceLastWind <= windCurve.Severity1 )
                    windSeverity = 1;
                if (windCurve.Severity2 != -1 && timeSinceLastWind <= windCurve.Severity2 )
                    windSeverity = 2;
                if (windCurve.Severity3 != -1 && timeSinceLastWind <= windCurve.Severity3 )
                    windSeverity = 3;
                if (windCurve.Severity4 != -1 && timeSinceLastWind <= windCurve.Severity4 )
                    windSeverity = 4;
                if (windCurve.Severity5 != -1 && timeSinceLastWind <= windCurve.Severity5 )
                    windSeverity = 5;
            }
            
            if (windSeverity > severity) 
                severity = windSeverity;
            return (byte) severity;
        }


        //---------------------------------------------------------------------

        private int KillSiteCohorts(ActiveSite site)
        {
            int previousCohortsKilled = this.cohortsKilled;
            SiteVars.Cohorts[site].RemoveMarkedCohorts(this);
            return this.cohortsKilled - previousCohortsKilled;
        }

        //---------------------------------------------------------------------

        //  A filter to determine which cohorts are removed.

        bool ICohortDisturbance.MarkCohortForDeath(ICohort cohort) 
        {
            bool killCohort = false;

            //Fire Severity 5 kills all cohorts:
            if (siteSeverity == 5) 
            {
                //logger.Debug(string.Format("  cohort {0}:{1} killed, severity =5", cohort.Species.Name, cohort.Age));
                killCohort = true;
            }
            else {
                //Otherwise, use damage table to calculate damage.
                //Read table backwards; most severe first.
                float ageAsPercent = (float) cohort.Age / (float) cohort.Species.Longevity;
                PlugIn.ModelCore.Log.WriteLine("Cohort = {0} {1}.", cohort.Age, cohort.Species.Name);
                foreach(IDamageTable damage in damages)
                {
                    if (siteSeverity - cohort.Species.FireTolerance >= damage.SeverTolerDifference) 
                    {
                        if (damage.MaxAge >= ageAsPercent) {
                            killCohort = true;
                        }
                        break;  // No need to search further in the table
                    }
                }
            }

            if (killCohort) {
                this.cohortsKilled++;
            }
            return killCohort;
        }
    }
}
