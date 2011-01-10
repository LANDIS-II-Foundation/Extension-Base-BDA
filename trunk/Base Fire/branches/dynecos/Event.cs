//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Grids;
using Landis.AgeCohort;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;
using Landis.Util;
using System.Collections.Generic;
//using log4net;

namespace Landis.Fire
{
    public class Event
        : AgeCohort.ICohortDisturbance
    {
        private static RelativeLocation[] neighborhood;
        private static List<IDamageTable> damages;
        private static ILandscapeCohorts cohorts;

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

        PlugInType AgeCohort.IDisturbance.Type
        {
            get {
                return PlugIn.Type;
            }
        }

        //---------------------------------------------------------------------

        ActiveSite AgeCohort.IDisturbance.CurrentSite
        {
            get {
                return currentSite;
            }
        }

        //---------------------------------------------------------------------

        public static void Initialize(List<IDamageTable>  damages)
        {
            Event.damages = damages;

            cohorts = Model.Core.SuccessionCohorts as ILandscapeCohorts;
            if (cohorts == null)
                throw new System.ApplicationException("Error: Cohorts don't support age-cohort interface");
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
            //IFireParameters eventParms = ecoregion.FireParameters;
            
            //Adjust ignition probability (measured on an annual basis) for the 
            //user determined fire time step.
            double ignitionProb = ecoregion.IgnitionProbability * timestep;

            //The initial site must exceed the probability of initiation and
            //have a severity > 0 and exceed the ignition threshold:
            if (Random.GenerateUniform() <= ignitionProb
                && Random.GenerateUniform() <= ComputeFireInitSpreadProb(site, currentTime)
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
            double sizeGenerated = Random.GenerateExponential(ecoregion.MeanSize);
            //UI.WriteLine("Max={0}, Min={1}, Mean={2}, Eco={3}.", ecoregion.MaxSize, ecoregion.MinSize, ecoregion.MeanSize, ecoregion.Name);
            //double sizeGenerated = Random.GenerateLogNormal(ecoregion.MeanSize);
            if (sizeGenerated < ecoregion.MinSize)
                return (int) (ecoregion.MinSize/Model.Core.CellArea);
            else if (sizeGenerated > ecoregion.MaxSize)
            {
                return (int) (ecoregion.MaxSize / Model.Core.CellArea);
            }    
            else
                return (int) (sizeGenerated/Model.Core.CellArea);
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

            //logger.Debug(string.Format("New Fire event at {0}",initiationSite.Location));
        }

        //---------------------------------------------------------------------

        private void Spread(int currentTime)
        {
            int windDirection = (int) (Util.Random.GenerateUniform() * 8);
            double windSpeed = Random.GenerateUniform();
            
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
                currentSite = site as ActiveSite;

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
                    siteSeverity = CalcSeverity(currentSite, currentTime);
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

                    siteCohortsKilled = Damage(currentSite);
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
                        Random.Shuffle(neighbors);
                        foreach (Site neighbor in neighbors) 
                        {
                            if (!neighbor.IsActive)
                                continue;
                            if (SiteVars.Event[neighbor] != null)
                                continue;
                            if (sitesToConsider.Contains(neighbor))
                                continue;
                            if (Random.GenerateUniform() <= ComputeFireInitSpreadProb(neighbor as ActiveSite, currentTime))
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
                    && Random.GenerateUniform() < windProb)
                {
                    neighbors.Add(neighbor);
                    success++;
                }
                index++;
            }
            //logger.Debug(string.Format("Successfully added {0} neighbors.", success));
            
            //Next, add the 9th neighbor, a neighbor one cell beyond the 
            //8 nearest neighbors.
            //array index 0 = north; 1 = northeast, 2 = east,...,8 = northwest
            int[] vertical  ={2,2,0,-2,-2,-2,0,2};
            int[] horizontal={0,2,2,2,0,-2,-2,-2};

            RelativeLocation relativeLoc9 = 
                new RelativeLocation(vertical[windDirection], horizontal[windDirection]);
            Site neighbor9 = site.GetNeighbor(relativeLoc9);
            if (neighbor9 != null && Random.GenerateUniform() < windSpeed)
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

        private int Damage(ActiveSite site)
        {
            int previousCohortsKilled = this.cohortsKilled;
            cohorts[site].DamageBy(this);
            return this.cohortsKilled - previousCohortsKilled;
        }

        //---------------------------------------------------------------------

        //  A filter to determine which cohorts are removed.

        bool AgeCohort.ICohortDisturbance.Damage(AgeCohort.ICohort cohort)
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
                //for (int i = damages.Length-1; i >= 0; --i) 
                foreach(IDamageTable damage in damages)
                {
                    //IDamageTable damage = damages[i];
                    if (siteSeverity - cohort.Species.FireTolerance >= damage.SeverTolerDifference) 
                    {
                        if (damage.MaxAge >= ageAsPercent) {
                            //logger.Debug(string.Format("  cohort {0}:{1} killed, damage class {2}", cohort.Species.Name, cohort.Age, damage.SeverTolerDifference));
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
