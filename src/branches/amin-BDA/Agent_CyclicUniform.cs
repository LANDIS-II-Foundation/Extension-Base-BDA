using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Landis.Extension.BaseBDA
{
    public class Agent_CyclicUniform:Agent
    {
        private double maxInterval;
        private double minInterval;
        private int timeSinceLastEpidemic;
        private int generatedTimeToNext;
        private int timeToNext;

        public Agent_CyclicUniform(int sppCount, int ecoCount)
            : base(sppCount, ecoCount)
        {
        }

        //---------------------------------------------------------------------
        public double MaxInterval
        {
            get
            {
                return maxInterval;
            }
            set
            {
                maxInterval = value;
            }
        }
        //---------------------------------------------------------------------
        public double MinInterval
        {
            get
            {
                return minInterval;
            }
            set
            {
                minInterval = value;
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
        public int GeneratedTimeToNext
        {
            get
            {
                return generatedTimeToNext;
            }
            set
            {
                generatedTimeToNext = value;
            }
        }
        //---------------------------------------------------------------------
        public int TTimeToNext
        {
            get
            {
                return timeToNext;
            }
            set
            {
                timeToNext = value;
            }
        }
        //---------------------------------------------------------------------

       public override int TimeToNext(int Timestep)
        {

            if (Timestep == 0)
                generatedTimeToNext = 0;
           
            
            if (generatedTimeToNext == 0)
            {
                 timeToNext = 0;
                //if (this.RandFunc == OutbreakPattern.CyclicUniform)
                //{
                int MaxI = (int)Math.Round(this.MaxInterval);
                int MinI = (int)Math.Round(this.MinInterval);
                double randNum = PlugIn.ModelCore.GenerateUniform();
                Console.Write("Uniform random number {0} \n", randNum);
                timeToNext = (MinI) + (int)(randNum * (MaxI - MinI));
                generatedTimeToNext = timeToNext;
            }
            else
            {
                timeToNext = generatedTimeToNext  - Timestep;
                if (timeToNext == 0)
                    generatedTimeToNext = 0;
            }
                //}
                //else if (this.RandFunc == OutbreakPattern.CyclicNormal)
                //{

                //    PlugIn.ModelCore.NormalDistribution.Mu = this.NormMean;
                //    PlugIn.ModelCore.NormalDistribution.Sigma = this.NormStDev;

                //    int randNum = (int)PlugIn.ModelCore.NormalDistribution.NextDouble();

                //    timeToNext = randNum;

                //    // Interval times are always rounded up to the next time step increment.
                //    // This bias can be removed by reducing times by half the time step.
                //    timeToNext = timeToNext - (Timestep / 2);

                //    if (timeToNext < 0) timeToNext = 0;
                //}

            return timeToNext;
            }


        //---------------------------------------------------------------------
        public override int RegionalOutbreakStatus(int BDAtimestep)
        {
            //generate random number for first time
            
            int ROS = 0;

            if (this.TimeToNextEpidemic <= this.TimeSinceLastEpidemic && PlugIn.ModelCore.CurrentTime <= this.EndYear)
            {

                //this.TimeSinceLastEpidemic = 0;
                //BDA-Climate
                // TimeToNext function does not apply for Climate
                // if OutbreakPattern <> Climate
                //    activeAgent.TimeToNextEpidemic = TimeToNext(activeAgent, BDAtimestep);
                // else  skip -- TimeToNext is handled below and must be run every time step
                //BDA-Climate
                this.TimeToNextEpidemic = TimeToNext(BDAtimestep);


                int timeOfNext = PlugIn.ModelCore.CurrentTime + this.TimeToNextEpidemic;
                SiteVars.TimeOfNext.ActiveSiteValues = timeOfNext;

                if (TimeToNextEpidemic == 0)
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
