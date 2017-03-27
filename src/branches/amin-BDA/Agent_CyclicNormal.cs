using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Extension.BaseBDA
{
    public class Agent_CyclicNormal:Agent
    {
        private double mean;
        private double stdDev;
        private int timeSinceLastEpidemic;

        public Agent_CyclicNormal(int sppCount, int ecoCount)
            : base(sppCount, ecoCount)
        {
            
        }
        //---------------------------------------------------------------------
        public double Mean
        {
            get
            {
                return mean;
            }
            set
            {
                mean = value;
            }
        }
        //---------------------------------------------------------------------
        public double STdDev
        {
            get
            {
                return stdDev;
            }
            set
            {
                stdDev = value;
            }
        }
        //---------------------------------------------------------------------
        public int TimeSinceLastEpidemic
        {
            get
            {
                return timeSinceLastEpidemic;
            }
            set
            {
                timeSinceLastEpidemic = value;
            }
        }
        //---------------------------------------------------------------------
        public override int TimeToNext(int Timestep)
        {
                int timeToNext = 0;
                //if (this.RandFunc == OutbreakPattern.CyclicUniform)
                //{
                //    int MaxI = (int)Math.Round(this.MaxInterval);
                //    int MinI = (int)Math.Round(this.MinInterval);
                //    double randNum = PlugIn.ModelCore.GenerateUniform();
                //    timeToNext = (MinI) + (int)(randNum * (MaxI - MinI));
                //}
                //else if (this.RandFunc == OutbreakPattern.CyclicNormal)
                //{

                    PlugIn.ModelCore.NormalDistribution.Mu = this.NormMean;
                    PlugIn.ModelCore.NormalDistribution.Sigma = this.NormStDev;

                    int randNum = (int)PlugIn.ModelCore.NormalDistribution.NextDouble();

                    timeToNext = randNum;

                    // Interval times are always rounded up to the next time step increment.
                    // This bias can be removed by reducing times by half the time step.
                    timeToNext = timeToNext - (Timestep / 2);

                    if (timeToNext < 0) timeToNext = 0;
                //}

                return timeToNext;

        }
        //---------------------------------------------------------------------
        public override int RegionalOutbreakStatus(int BDAtimestep)
        {
            int ROS = 0;

            if (this.TimeToNextEpidemic <= this.TimeSinceLastEpidemic && PlugIn.ModelCore.CurrentTime <= this.EndYear)
            {

                this.TimeSinceLastEpidemic = 0;
                //BDA-Climate
                // TimeToNext function does not apply for Climate
                // if OutbreakPattern <> Climate
                //    activeAgent.TimeToNextEpidemic = TimeToNext(activeAgent, BDAtimestep);
                // else  skip -- TimeToNext is handled below and must be run every time step
                //BDA-Climate
                this.TimeToNextEpidemic = TimeToNext( BDAtimestep);


                int timeOfNext = PlugIn.ModelCore.CurrentTime + this.TimeToNextEpidemic;
                SiteVars.TimeOfNext.ActiveSiteValues = timeOfNext;

                //calculate ROS
                if (this.TempType == TemporalType.pulse)
                    ROS = this.MaxROS;
                else if (this.TempType == TemporalType.variablepulse)
                {
                    //randomly select an ROS netween ROSmin and ROSmax
                    //ROS = (int) (Landis.Util.Random.GenerateUniform() *
                    //      (double) (activeAgent.MaxROS - activeAgent.MinROS + 1)) +
                    //      activeAgent.MinROS;

                    // Correction suggested by Brian Miranda, March 2008
                    ROS = (int)(PlugIn.ModelCore.GenerateUniform() * (double)(this.MaxROS - this.MinROS)) + 1 + this.MinROS;

                }

            }
            else
            {
                //activeAgent.TimeSinceLastEpidemic += BDAtimestep;
                ROS = this.MinROS;
            }

            //BDA-Climate
            // If OutbreakPattern = Climate
            // Calculate climate variable for current timestep
            // Update TimeToNext, TimeOfNext if necessary
            // Function outlined below (CalculateClimate)
            //BDA-Climate

            return ROS;

        }
    }
}
