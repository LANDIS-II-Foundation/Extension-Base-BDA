//  Copyright 2005-2014 Portland State University, University of Wisconsin, US Forest Service
//  Authors:  Robert M. Scheller, Brian Miranda
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.

using System;
using System.Collections.Generic;
using Landis.Library.Climate;
using Landis.Core;
using System.Data;

namespace Landis.Extension.BaseBDA
{
    public class Agent_Climate : Agent
    {
        private IEnumerable<double> climateQueriedValuse;

        private string variableName;
        private float threshold_Lowerbound;
        private float threshold_Upperbound;
        private int outbreakLag;
        private int timeSinceLastClimate;

        public LinkedList<int> OutbreakList = new LinkedList<int>();


        public Agent_Climate(int sppCount, int ecoCount)
            : base(sppCount, ecoCount)
        {
        }
        //---------------------------------------------------------------------
        public IEnumerable<double> ClimateQueriedValuse
        {
            get
            {
                return climateQueriedValuse;
            }
            set
            {
                climateQueriedValuse = value;
            }
        }
        //---------------------------------------------------------------------
        public string VariableName
        {
            get
            {
                return variableName;
            }
            set
            {
                variableName = value;
            }
        }
        //---------------------------------------------------------------------
        public float Threshold_Lowerbound
        {
            get
            {
                return threshold_Lowerbound;
            }
            set
            {
                threshold_Lowerbound = value;
            }
        }
        //---------------------------------------------------------------------
        public float Threshold_Upperbound
        {
            get
            {
                return threshold_Upperbound;
            }
            set
            {
                threshold_Upperbound = value;
            }
        }
        //---------------------------------------------------------------------
        public int OutbreakLag
        {
            get
            {
                return outbreakLag;
            }
            set
            {
                outbreakLag = value;
            }
        }
        //---------------------------------------------------------------------

        public int TimeSinceLastClimate
        {
            get
            {
                return timeSinceLastClimate;
            }
            set
            {
                timeSinceLastClimate = value;
            }
        }


        public override int RegionalOutbreakStatus(int BDAtimestep)
        {
            int ROS = 0;

            this.TimeToNextEpidemic = TimeToNext(BDAtimestep);
            int timeOfNext = PlugIn.ModelCore.CurrentTime + this.TimeToNextEpidemic;
            SiteVars.TimeOfNext.ActiveSiteValues = timeOfNext;
            Console.Write("Time-to-Next:" + this.TimeToNextEpidemic + ", MinROS:" + this.MinROS + ", MaxROS:" + this.MaxROS + "\n");

            if (this.TimeToNextEpidemic == 0)
            {
                //calculate ROS
                if (this.TempType == TemporalType.pulse)
                    ROS = this.MaxROS;
                else if (this.TempType == TemporalType.variablepulse)
                {

                    // Correction suggested by Brian Miranda, March 2008
                    ROS = (int)(PlugIn.ModelCore.GenerateUniform() *
                          (double)(this.MaxROS - this.MinROS)) + 1 +
                          this.MinROS;

                }
            }

            else
            {
                ROS = this.MinROS;
            }

            return ROS;

        }


        public override int TimeToNext(int TimeStep)
        {

            // Note:
            //  This is intended to make not override if outbreak is already set to happen in < OutbreakLag years
            //
            // Update TimeOfNext as above 

            PlugIn.ModelCore.UI.WriteLine("Landscape PDSI= {0}.", Climate.LandscapeAnnualPDSI[PlugIn.ModelCore.CurrentTime - 1]);

            if (Climate.LandscapeAnnualPDSI[PlugIn.ModelCore.CurrentTime - 1] > this.threshold_Lowerbound && Climate.LandscapeAnnualPDSI[PlugIn.ModelCore.CurrentTime - 1] < this.threshold_Upperbound)
            {
                // List of TimeofNext
                OutbreakList.AddLast(PlugIn.ModelCore.CurrentTime + this.outbreakLag);

            }
            else
            {
                if (OutbreakList.Count == 0)
                    return int.MaxValue;
            }

            if (OutbreakList.First.Value - PlugIn.ModelCore.CurrentTime == 0)
            {

                OutbreakList.RemoveFirst();
                return 0;

            }

            LogPDSI(PlugIn.ModelCore.CurrentTime, Climate.LandscapeAnnualPDSI[PlugIn.ModelCore.CurrentTime - 1]);

            return OutbreakList.First.Value - PlugIn.ModelCore.CurrentTime;


        }

        //---------------------------------------------------------------------
        private void LogPDSI(int currentTime, double PDSI)
        {
            PlugIn.PDSILog.Clear();
            PDSI_Log pl = new PDSI_Log();
            pl.Time = currentTime;
            pl.PDSI = PDSI;

            PlugIn.PDSILog.AddObject(pl);
            PlugIn.PDSILog.WriteToFile();
        }

        public void SetPDSI(int ecoCount)
        {
            // GetPDSI(int startYear, int latitude, double fieldCapacity, double wiltingPoint, ClimatePhase climatePhase = ClimatePhase.Future_Climate);
            double[] latitude = new double[ecoCount];
            double[] fieldCapacity = new double[ecoCount];
            double[] wiltingPoint = new double[ecoCount];
            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                latitude[ecoregion.Index] = EcoParameters[ecoregion.Index].Latitude;
                wiltingPoint[ecoregion.Index] = EcoParameters[ecoregion.Index].WiltingPoint;
                fieldCapacity[ecoregion.Index] = EcoParameters[ecoregion.Index].FieldCapacity;
            }
            Climate.SetPDSI(0, latitude, fieldCapacity, wiltingPoint);
        }

    }
}

