//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;
using System;
using Landis.Library.Climate;

namespace Landis.Extension.BaseBDA
{
    public enum TemporalType {pulse,  variablepulse};
    public enum OutbreakPattern { CyclicNormal, CyclicUniform, Climate };
    public enum SRDmode {max, mean};
    public enum DispersalTemplate {MaxRadius, N4, N8, N12, N24};
    public enum NeighborShape {uniform, linear, gaussian};
    public enum NeighborSpeed {none, X2, X3, X4};
    public enum Zone {Nozone, Lastzone, Newzone};
    public enum Function { Test1, Test2, Test3 };


    /// <summary>
    /// Interface to the Parameters for the extension
    /// </summary>
    public interface IAgent
    {
        string AgentName{get;set;}
        int BDPCalibrator{get;set;}
        int StartYear { get; set; }
        int EndYear { get; set; }

        //-- ROS --
        int TimeSinceLastEpidemic{get;set;}
        int TimeToNextEpidemic{get;set;}
        TemporalType TempType{get;set;}
        OutbreakPattern RandFunc{get;set;}
        SRDmode SRDmode{get;set;}
        double NormMean { get; set; }
        double NormStDev { get; set; }
        double MaxInterval{get;set;}
        double MinInterval{get;set;}
        int MinROS{get;set;}
        int MaxROS{get;set;}
        
        //BDA-Climate
        //Add new parameters
        // ClimateLibrary
        // VariableName
        // StartMonth
        // EndMonth
        // Function
        // LogicalTest
        // OutbreakLag
        // TimeSinceLastClimate
        // OutbreakList (a list of timesteps when outbreaks occur)
        //BDA-Climate


       

        //-- DISPERSAL -------------
        bool Dispersal{get;set;}
        int DispersalRate{get;set;}
        double EpidemicThresh{get;set;}
        int EpicenterNum{get;set;}
        bool SeedEpicenter{get;set;}
        double OutbreakEpicenterCoeff{get;set;}
        double SeedEpicenterCoeff{get;set;}
        DispersalTemplate DispersalTemp{get;set;}
        IEnumerable<RelativeLocation> DispersalNeighbors{get;set;}

        // Neighborhood Resource Dominance parameters
        bool NeighborFlag{get;set;}
        NeighborSpeed NeighborSpeedUp{get;set;}
        int NeighborRadius{get; set;}
        NeighborShape ShapeOfNeighbor{get;set;}
        double NeighborWeight{get;set;}
        IEnumerable<RelativeLocationWeighted> ResourceNeighbors{get;set;}
        double Class2_SV { get; }
        double Class3_SV { get; }
        IEnumerable<ISpecies> NegSppList { get;set;}
        //IEnumerable<ISpecies> AdvRegenSppList { get; set; }
        //int AdvRegenAgeCutoff { get; }

        ISppParameters[] SppParameters { get; set; }
        IEcoParameters[] EcoParameters { get; set; }
        //IDistParameters[] DistParameters { get; set; }
        List<IDisturbanceType> DisturbanceTypes { get;  }
        ISiteVar<byte> Severity { get; set; }
        ISiteVar<Zone> OutbreakZone { get; set; }


        //void CalculateClimate(); //renamed to TimeToNext()
        int RegionalOutbreakStatus( int BDAtimestep);
        int TimeToNext(int Timestep);
    }
}


namespace Landis.Extension.BaseBDA
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public abstract class Agent
        : IAgent
    {


        private string agentName;
        private int bdpCalibrator;
        private int startYear;
        private int endYear;

        //-- ROS --
        private int minROS;
        private int maxROS;
        private TemporalType tempType;
        private OutbreakPattern randFunc;
        //---------

        private int timeSinceLastEpidemic;
        private int timeToNextEpidemic;
        private SRDmode srdMode;
        private double normMean;
        private double normStDev;
        private double maxInterval;
        private double minInterval;


        //BDA-Climate
        //Add new parameters
        // ClimateLibrary
        // VariableName
        // StartMonth
        // EndMonth
        // Function
        // LogicalTest
        // OutbreakLag
        // TimeSinceLastClimate
        // OutbreakList
        //BDA-Climate




        //-- DISPERSAL -------------
        private bool dispersal;
        private int dispersalRate;
        private double epidemicThresh;
        private int epicenterNum;
        private bool seedEpicenter;
        private double outbreakEpicenterCoeff;
        private double seedEpicenterCoeff;
        private DispersalTemplate dispersalTemp;
        private IEnumerable<RelativeLocation> dispersalNeighbors;

        // Neighborhood Resource Dominance parameters
        private bool neighborFlag;
        private NeighborSpeed neighborSpeedUp;
        private int neighborRadius;
        private NeighborShape shapeOfNeighbor;
        private double neighborWeight;
        private IEnumerable<RelativeLocationWeighted> resourceNeighbors;
        private double class2_SV;
        private double class3_SV;
        private IEnumerable<ISpecies> negSppList;
        private IEnumerable<ISpecies> advRegenSppList;
        private int advRegenAgeCutoff;

        private ISppParameters[] sppParameters;
        private IEcoParameters[] ecoParameters;
        //private IDistParameters[] distParameters;
        private List<IDisturbanceType> disturbanceTypes;
        private ISiteVar<byte> severity;
        private ISiteVar<Zone> outbreakZone;
        //---------------------------------------------------------------------
        public string AgentName
        {
            get
            {
                return agentName;
            }
            set
            {
                agentName = value;
            }
        }
        //---------------------------------------------------------------------
        public int BDPCalibrator
        {
            get
            {
                return bdpCalibrator;
            }
            set
            {
                bdpCalibrator = value;
            }
        }
        //---------------------------------------------------------------------
        public int StartYear
        {
            get
            {
                return startYear;
            }
            set
            {
                startYear = value;
            }
        }
        //---------------------------------------------------------------------
        public int EndYear
        {
            get
            {
                return endYear;
            }
            set
            {
                endYear = value;
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
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must = or be > 0.");
                if (value > 10000)
                    throw new InputValueException(value.ToString(),
                        "Value must < 10000.");
                timeSinceLastEpidemic = value;
            }
        }
        //---------------------------------------------------------------------
        public int TimeToNextEpidemic
        {
            get
            {
                return timeToNextEpidemic;
            }
            set
            {
                timeToNextEpidemic = value;
            }
        }
        //---------------------------------------------------------------------
        public TemporalType TempType
        {
            get
            {
                return tempType;
            }
            set
            {
                tempType = value;
            }
        }
        //---------------------------------------------------------------------
        public OutbreakPattern RandFunc
        {
            get
            {
                return randFunc;
            }
            set
            {
                randFunc = value;
            }
        }
        //---------------------------------------------------------------------
        public SRDmode SRDmode
        {
            get
            {
                return srdMode;
            }
            set
            {
                srdMode = value;
            }
        }
        //---------------------------------------------------------------------
        public double NormMean
        {
            get
            {
                return normMean;
            }
            set
            {
                normMean = value;
            }
        }
        //---------------------------------------------------------------------
        public double NormStDev
        {
            get
            {
                return normStDev;
            }
            set
            {
                normStDev = value;
            }
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
        public int MinROS
        {
            get
            {
                return minROS;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must = or be > 0.");
                if (maxROS > 0 && value > maxROS)
                    throw new InputValueException(value.ToString(),
                        "Value must < or = MaxROS.");

                minROS = value;
            }
        }
        //---------------------------------------------------------------------
        public int MaxROS
        {
            get
            {
                return maxROS;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must = or be > 0.");
                if (minROS > 0 && value < minROS)
                    throw new InputValueException(value.ToString(),
                        "Value must > or = MinROS.");
                maxROS = value;
            }
        }
        //---------------------------------------------------------------------
        public ISppParameters[] SppParameters
        {
            get
            {
                return sppParameters;
            }
            set
            {
                sppParameters = value;
            }
        }
        //---------------------------------------------------------------------
        public IEcoParameters[] EcoParameters
        {
            get
            {
                return ecoParameters;
            }
            set
            {
                ecoParameters = value;
            }
        }
        //---------------------------------------------------------------------
        //public IDistParameters[] DistParameters
        //{
        //    get {
        //        return distParameters;
        //    }
        //    set {
        //        distParameters = value;
        //    }
        //}
        //---------------------------------------------------------------------
        /// <summary>
        /// Disturbances that can alter the SRD value
        /// </summary>
        public List<IDisturbanceType> DisturbanceTypes
        {
            get
            {
                return disturbanceTypes;
            }
        }
        //---------------------------------------------------------------------
        public ISiteVar<byte> Severity
        {
            get
            {
                return severity;
            }
            set
            {
                severity = value;
            }
        }
        public ISiteVar<Zone> OutbreakZone
        {
            get
            {
                return outbreakZone;
            }
            set
            {
                outbreakZone = value;
            }
        }

        //---------------------------------------------------------------------
        public bool Dispersal
        {
            get
            {
                return dispersal;
            }
            set
            {
                dispersal = value;
            }
        }
        //---------------------------------------------------------------------
        public int DispersalRate
        {
            get
            {
                return dispersalRate;
            }
            set
            {
                if (value <= 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be > 0.");
                dispersalRate = value;
            }
        }
        //---------------------------------------------------------------------
        public double EpidemicThresh
        {
            get
            {
                return epidemicThresh;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(),
                         "Value must be > or = 0 and < or = 1.");
                epidemicThresh = value;
            }
        }
        //---------------------------------------------------------------------
        public int EpicenterNum
        {
            get
            {
                return epicenterNum;
            }
            set
            {
                epicenterNum = value;
            }
        }
        //---------------------------------------------------------------------
        public bool SeedEpicenter
        {
            get
            {
                return seedEpicenter;
            }
            set
            {
                seedEpicenter = value;
            }
        }
        //---------------------------------------------------------------------
        public double OutbreakEpicenterCoeff
        {
            get
            {
                return outbreakEpicenterCoeff;
            }
            set
            {
                outbreakEpicenterCoeff = value;
            }
        }
        //---------------------------------------------------------------------
        public double SeedEpicenterCoeff
        {
            get
            {
                return seedEpicenterCoeff;
            }
            set
            {
                seedEpicenterCoeff = value;
            }
        }
        //---------------------------------------------------------------------
        public DispersalTemplate DispersalTemp
        {
            get
            {
                return dispersalTemp;
            }
            set
            {
                dispersalTemp = value;
            }
        }
        //---------------------------------------------------------------------
        public IEnumerable<RelativeLocation> DispersalNeighbors
        {
            get
            {
                return dispersalNeighbors;
            }
            set
            {
                dispersalNeighbors = value;
            }
        }
        //---------------------------------------------------------------------
        public bool NeighborFlag
        {
            get
            {
                return neighborFlag;
            }
            set
            {
                neighborFlag = value;
            }
        }
        //---------------------------------------------------------------------
        public NeighborSpeed NeighborSpeedUp
        {
            get
            {
                return neighborSpeedUp;
            }
            set
            {
                neighborSpeedUp = value;
            }
        }
        //---------------------------------------------------------------------
        public int NeighborRadius
        {
            get
            {
                return neighborRadius;
            }
            set
            {
                if (value <= 0)
                    throw new InputValueException(value.ToString(),
                        "Value must be > 0.");
                neighborRadius = value;
            }
        }
        //---------------------------------------------------------------------
        public NeighborShape ShapeOfNeighbor
        {
            get
            {
                return shapeOfNeighbor;
            }
            set
            {
                shapeOfNeighbor = value;
            }
        }
        //---------------------------------------------------------------------
        public double NeighborWeight
        {
            get
            {
                return neighborWeight;
            }
            set
            {
                if (value < 0)
                    throw new InputValueException(value.ToString(),
                        "Value must = or be > 0.");
                neighborWeight = value;
            }
        }
        //---------------------------------------------------------------------
        public IEnumerable<RelativeLocationWeighted> ResourceNeighbors
        {
            get
            {
                return resourceNeighbors;
            }
            set
            {
                resourceNeighbors = value;
            }
        }
        //---------------------------------------------------------------------
        public double Class2_SV
        {
            get
            {
                return class2_SV;
            }
            set
            {
                class2_SV = value;
            }
        }
        //---------------------------------------------------------------------
        public double Class3_SV
        {
            get
            {
                return class3_SV;
            }
            set
            {
                class3_SV = value;
            }
        }
        //---------------------------------------------------------------------
        public IEnumerable<ISpecies> NegSppList
        {
            get
            {
                return negSppList;
            }
            set
            {
                negSppList = value;
            }
        }
        //---------------------------------------------------------------------
        public IEnumerable<ISpecies> AdvRegenSppList
        {
            get
            {
                return advRegenSppList;
            }
            set
            {
                advRegenSppList = value;
            }
        }
        //---------------------------------------------------------------------
        public int AdvRegenAgeCutoff
        {
            get
            {
                return advRegenAgeCutoff;
            }
            set
            {
                advRegenAgeCutoff = value;
            }
        }
        //---------------------------------------------------------------------






        /// <summary>
        /// Objects and Lists must be initialized.
        /// </summary>
        public Agent(int sppCount, int ecoCount)
        {
            SppParameters = new ISppParameters[sppCount];
            EcoParameters = new IEcoParameters[ecoCount];
            //DistParameters = new IDistParameters[distCount];
            disturbanceTypes = new List<IDisturbanceType>();
            negSppList = new List<ISpecies>();
            //advRegenSppList = new List<ISpecies>();
            dispersalNeighbors = new List<RelativeLocation>();
            resourceNeighbors = new List<RelativeLocationWeighted>();
            severity = PlugIn.ModelCore.Landscape.NewSiteVar<byte>();
            outbreakZone = PlugIn.ModelCore.Landscape.NewSiteVar<Zone>();

            for (int i = 0; i < sppCount; i++)
                SppParameters[i] = new SppParameters();
            for (int i = 0; i < ecoCount; i++)
                EcoParameters[i] = new EcoParameters();
            //for (int i = 0; i < distCount; i++)
            //   DistParameters[i] = new DistParameters();
        }

        //BDA-Climate
        //---------------------------------------------------------------------
        //public virtual void CalculateClimate()
        //{
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

            //Should get start year in order to get

            

        //}

        // copied from PlugIn
        //private static int RegionalOutbreakStatus(IAgent activeAgent, int BDAtimestep)
        public virtual int RegionalOutbreakStatus(int BDAtimestep)
        {
            int ROS = 0;

            //if (this.TimeToNextEpidemic <= this.TimeSinceLastEpidemic && PlugIn.ModelCore.CurrentTime <= this.EndYear)
            //{

            //    this.TimeSinceLastEpidemic = 0;
            //    //BDA-Climate
            //    // TimeToNext function does not apply for Climate
            //    // if OutbreakPattern <> Climate
            //    //    activeAgent.TimeToNextEpidemic = TimeToNext(activeAgent, BDAtimestep);
            //    // else  skip -- TimeToNext is handled below and must be run every time step
            //    //BDA-Climate

            //    // Amin BDA-Climate----------------
            //    if (this.RandFunc.ToString().ToLower() != "climate")
            //    {
            //        this.TimeToNextEpidemic = this.TimeToNext(BDAtimestep);


            //        //--------------------
            //        //activeAgent.TimeToNextEpidemic = TimeToNext(activeAgent, BDAtimestep);

            //        int timeOfNext = PlugIn.ModelCore.CurrentTime + this.TimeToNextEpidemic;
            //        SiteVars.TimeOfNext.ActiveSiteValues = timeOfNext;
            //        //-----------------------------
            //        //calculate ROS
            //        if (this.TempType == TemporalType.pulse)
            //            ROS = this.MaxROS;
            //        else if (this.TempType == TemporalType.variablepulse)
            //        {
            //            //randomly select an ROS netween ROSmin and ROSmax
            //            //ROS = (int) (Landis.Util.Random.GenerateUniform() *
            //            //      (double) (activeAgent.MaxROS - activeAgent.MinROS + 1)) +
            //            //      activeAgent.MinROS;

            //            // Correction suggested by Brian Miranda, March 2008
            //            ROS = (int)(PlugIn.ModelCore.GenerateUniform() *
            //                  (double)(this.MaxROS - this.MinROS)) + 1 +
            //                  this.MinROS;

            //        }

            //    }
            //    else
            //    {
            //        //activeAgent.TimeSinceLastEpidemic += BDAtimestep;
            //        ROS = this.MinROS;
            //    }
            //}
            ////BDA-Climate
            //// If OutbreakPattern = Climate
            //// Calculate climate variable for current timestep
            //// Update TimeToNext, TimeOfNext if necessary
            //// Function outlined below (CalculateClimate)
            ////BDA-Climate
            
            //else if (this.RandFunc.ToString().ToLower() == "climate")
            //{
            //    this.CalculateClimate();
            //    //((Agent_Climate)this).CalculateClimate();
            //}

            return ROS;

        }
        public virtual int TimeToNext(int Timestep)
        {
                int timeToNext = 0;
            //    if (this.RandFunc == OutbreakPattern.CyclicUniform)
            //    {
            //        int MaxI = (int)Math.Round(this.MaxInterval);
            //        int MinI = (int)Math.Round(this.MinInterval);
            //        double randNum = PlugIn.ModelCore.GenerateUniform();
            //        timeToNext = (MinI) + (int)(randNum * (MaxI - MinI));
            //    }
            //    else if (this.RandFunc == OutbreakPattern.CyclicNormal)
            //    {

            //        PlugIn.ModelCore.NormalDistribution.Mu = this.NormMean;
            //        PlugIn.ModelCore.NormalDistribution.Sigma = this.NormStDev;

            //        int randNum = (int)PlugIn.ModelCore.NormalDistribution.NextDouble();

            //        timeToNext = randNum;

            //        // Interval times are always rounded up to the next time step increment.
            //        // This bias can be removed by reducing times by half the time step.
            //        timeToNext = timeToNext - (Timestep / 2);

            //        if (timeToNext < 0) timeToNext = 0;
            //    }

                return timeToNext;
            //}
        }
    }
    public class RelativeLocationWeighted
    {
        private RelativeLocation location;
        private double weight;

        //---------------------------------------------------------------------
        public RelativeLocation Location
        {
            get {
                return location;
            }
            set {
                location = value;
            }
        }

        public double Weight
        {
            get {
                return weight;
            }
            set {
                weight = value;
            }
        }

        public RelativeLocationWeighted (RelativeLocation location, double weight)
        {
            this.location = location;
            this.weight = weight;
        }





    }
}
