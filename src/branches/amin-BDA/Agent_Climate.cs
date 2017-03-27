using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Climate;
using System.IO;
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
            Climate.GetPDSI(1980);
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

            //if (this.TimeToNextEpidemic <= this.TimeSinceLastEpidemic && PlugIn.ModelCore.CurrentTime <= this.EndYear)
            //{

            // this.TimeSinceLastEpidemic = 0;
            //BDA-Climate
            // TimeToNext function does not apply for Climate
            // if OutbreakPattern <> Climate
            //    activeAgent.TimeToNextEpidemic = TimeToNext(activeAgent, BDAtimestep);
            // else  skip -- TimeToNext is handled below and must be run every time step
            //BDA-Climate



            this.TimeToNextEpidemic = TimeToNext(BDAtimestep);


            int timeOfNext = PlugIn.ModelCore.CurrentTime + this.TimeToNextEpidemic;
            SiteVars.TimeOfNext.ActiveSiteValues = timeOfNext;
            Console.Write("Time-to-Next:" + this.TimeToNextEpidemic + ", MinROS:" + this.MinROS + ", MaxROS:" + this.MaxROS + "\n");
            //Console.Read();


            if (this.TimeToNextEpidemic == 0)
            {
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
                    ROS = (int)(PlugIn.ModelCore.GenerateUniform() *
                          (double)(this.MaxROS - this.MinROS)) + 1 +
                          this.MinROS;

                }
            }

            else
            {
                //activeAgent.TimeSinceLastEpidemic += BDAtimestep;
                ROS = this.MinROS;
            }
            //}
            //else
            //{
            //    //activeAgent.TimeSinceLastEpidemic += BDAtimestep;
            //    ROS = this.MinROS;
            //}

            //BDA-Climate
            // If OutbreakPattern = Climate
            // Calculate climate variable for current timestep
            // Update TimeToNext, TimeOfNext if necessary
            // Function outlined below (CalculateClimate)
            //BDA-Climate

            return ROS;

        }


        public override int TimeToNext(int TimeStep)
        {

            //int timeofNext =0;
            //Climate.GetPDSI

            //    private void CalculateClimate(IAgent agent)
            //{
            // Read climate data from agent.ClimateLibrary
            // Extract agent.VariableName data
            // Calculate ecoregion values (EcoClimate) from (agent.StartMonth to agent.EndMonth using agent.Function)
            // Calculate landscape value (LandscapeClimate) as area-weighted average of EcoClimate
            // if (LandscapeClimate passes agent.LogicalTest)
            // Record current timestep in agent.OutbreakList
            // if OutbreakList contains any values where (current timestep - value ) < OutbreakLag
            //   { agent.TimtToNext = current timestep - value}
            // else
            //   {  agent.TimeToNext = agent.OutbreakLag}
            // Note:
            //  This is intended to make not override if outbreak is already set to happen in < OutbreakLag years
            //
            // Update TimeOfNext as above 

            Console.Write("Landscape_PDSI: " + Climate.LandscapAnnualPDSI[PlugIn.ModelCore.CurrentTime - 1] + "\n");
            //Console.Read();

            if (Climate.LandscapAnnualPDSI[PlugIn.ModelCore.CurrentTime - 1] > this.threshold_Lowerbound && Climate.LandscapAnnualPDSI[PlugIn.ModelCore.CurrentTime - 1] < this.threshold_Upperbound)
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
            return OutbreakList.First.Value - PlugIn.ModelCore.CurrentTime;




            //Should get start year in order to get
            ////if (this.variableName.ToString().ToLower() == "annualpdsi")
            ////{

            ////    int numberOftimeStaps = 0;
            ////    int numberOfEcoregions = 0;

            ////    int index = 0;
            ////    double ecoAverage = 0;

            ////    //List<int> levels = Climate.AnnualPDSI.AsEnumerable().Select(al => al.Field<int>("TimeStep")).Distinct().ToList().Max();
            ////    //numberOftimeStaps = levels.Max();
            ////    numberOftimeStaps = Climate.AnnualPDSI.AsEnumerable().Select(al => al.Field<int>("TimeStep")).Distinct().ToList().Max();

            ////    //List<int> ecos = Climate.AnnualPDSI.AsEnumerable().Select(a2 => a2.Field<int>("Ecorigion")).Distinct().ToList().Max();
            ////    //numberOfEcoregions = ecos.Max();
            ////    numberOfEcoregions = Climate.AnnualPDSI.AsEnumerable().Select(a2 => a2.Field<int>("Ecorigion")).Distinct().ToList().Max();

            ////    Climate.LandscapeAnnualPDSI = new double[numberOftimeStaps];

            ////    for (int timeStep = 1; timeStep <= numberOftimeStaps; timeStep++)
            ////    {
            ////        index = timeStep;
            ////        for (int eco = 1; eco <= numberOfEcoregions; eco++)
            ////        {
            ////            if (index <= Climate.AnnualPDSI.Rows.Count && (Int32)Climate.AnnualPDSI.Rows[index - 1][0] == timeStep && (Int32)Climate.AnnualPDSI.Rows[index - 1][1] == eco)
            ////            {
            ////                ecoAverage += (double)Climate.AnnualPDSI.Rows[index - 1][2];// get the valuse of annualPDSI

            ////                if (eco == numberOfEcoregions)
            ////                {
            ////                    ecoAverage = ecoAverage / numberOfEcoregions;
            ////                    Climate.LandscapAnnualPDSI[timeStep - 1] = ecoAverage;
            ////                    //Can be printed
            ////                    //file.WriteLine(timeStep + ", " + ecoAverage) ;
            ////                    if (ecoAverage < this.threshold_Lowerbound || ecoAverage > this.threshold_Upperbound)
            ////                    {
            ////                        OutbreakList.Add(timeStep);

            ////                        if (OutbreakList.Count != 0)
            ////                        {
            ////                            for (int j = 0; j < OutbreakList.Count; j++)
            ////                            {
            ////                                if (timeStep - OutbreakList[j] < this.OutbreakLag ) //&&  timeStep - OutbreakList[j] > 0)
            ////                                    this.TimeToNextEpidemic = timeStep - OutbreakList[j];
            ////                                else
            ////                                    this.TimeToNextEpidemic = this.OutbreakLag;

            ////                                //update time of next when in boundry
            ////                            }
            ////                        }

            ////                    }
            ////                    ecoAverage = 0;
            ////                }
            ////            }
            ////            index = index + numberOftimeStaps;
            ////        }
            ////    }



            // Check Outbreak (upper and lower Threshold)
            //if (LandscapeClimate passes agent.LogicalTest)
            // Record current timestep in agent.OutbreakList
            // if OutbreakList contains any values where (current timestep - value ) < OutbreakLag
            //   { agent.TimtToNext = current timestep - value}
            // else
            //   {  agent.TimeToNext = agent.OutbreakLag}
            // Note:
            //  This is intended to make not override if outbreak is already set to happen in < OutbreakLag years
            //
            // Update TimeOfNext as above 

            //Should get start year in order to get
            //????? what is start year here?
            //if (ecoAverage > -0.2)//check with threshold
            //{
            //    OutbreakList.Add(timeStep);
            //}
            //if()

        }
    }
}

