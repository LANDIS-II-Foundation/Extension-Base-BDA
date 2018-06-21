//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;
using System.Data;

namespace Landis.Extension.BaseBDA
{
    public enum TemporalType {pulse,  variablepulse};
    public enum OutbreakPattern {CyclicNormal, CyclicUniform, Climate};
    public enum SRDmode {max, mean};
    public enum DispersalTemplate {MaxRadius, N4, N8, N12, N24};
    public enum NeighborShape {uniform, linear, gaussian};
    public enum NeighborSpeed {none, X2, X3, X4};
    public enum Zone {Nozone, Lastzone, Newzone};
    


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
        // - Climate - 
        string ClimateVarName { get; set; }
        string ClimateVarSource { get; set; }
        float ClimateThresh_Lowerbound { get; set; }
        float ClimateThresh_Upperbound { get; set; }
        int ClimateLag { get; set; }
        int TimeSinceLastClimate { get; set; }
        DataTable ClimateDataTable { get; set; }
        LinkedList<int> OutbreakList { get; set; }
        
        //-- DISPERSAL -------------
        bool Dispersal{get;set;}
        int DispersalRate{get;set;}
        double EpidemicThresh{get;set;}
        int EpicenterNum{get;set;}
        bool SeedEpicenter{get;set;}
        double OutbreakEpicenterCoeff{get;set;}
        double OutbreakEpicenterThresh { get; set; }
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
    }
}


namespace Landis.Extension.BaseBDA
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public class Agent
        : IAgent
    {
        private string agentName;
        private int bdpCalibrator;
        private int startYear;
        private int endYear;

        //-- ROS --
        private int timeSinceLastEpidemic;
        private int timeToNextEpidemic;
        private TemporalType tempType;
        private OutbreakPattern randFunc;
        private SRDmode srdMode;
        private double normMean;
        private double normStDev;
        private double maxInterval;
        private double minInterval;
        private int minROS;
        private int maxROS;
        // - Climate - 
        private string climateVarName;
        private string climateVarSource;
        private float climateThresh_Lowerbound;
        private float climateThresh_Upperbound;
        private int climateLag;
        private int timeSinceLastClimate;
        private DataTable climateDataTable;
        public LinkedList<int> outbreakList = new LinkedList<int>();
        
        //-- DISPERSAL -------------
        private bool dispersal;
        private int dispersalRate;
        private double epidemicThresh;
        private int epicenterNum;
        private bool seedEpicenter;
        private double outbreakEpicenterCoeff;
        private double outbreakEpicenterThresh;
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
            get {
                return agentName;
            }
            set {
                agentName = value;
            }
        }
        //---------------------------------------------------------------------
        public int BDPCalibrator
        {
            get {
                return bdpCalibrator;
            }
            set {
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
            get {
                return timeSinceLastEpidemic;
            }
            set {
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
            get {
                return timeToNextEpidemic;
            }
            set {
                timeToNextEpidemic = value;
            }
        }
        //---------------------------------------------------------------------
        public TemporalType TempType
        {
            get {
                return tempType;
            }
            set {
                tempType = value;
            }
        }
        //---------------------------------------------------------------------
        public OutbreakPattern RandFunc
        {
            get {
                return randFunc;
            }
            set {
                randFunc = value;
            }
        }
        //---------------------------------------------------------------------
        public SRDmode SRDmode
        {
            get {
                return srdMode;
            }
            set {
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
            get {
                return maxInterval;
            }
            set {
                maxInterval = value;
            }
        }
        //---------------------------------------------------------------------
        public double MinInterval
        {
            get {
                return minInterval;
            }
            set {
                minInterval = value;
            }
        }
        //---------------------------------------------------------------------
        public int MinROS
        {
            get {
                return minROS;
            }
            set {
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
            get {
                return maxROS;
            }
            set {
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
        // - Climate - 
        public string ClimateVarName
        {
            get
            {
                return climateVarName;
            }
            set
            {
                climateVarName = value;
            }
        }
        //---------------------------------------------------------------------
        public string ClimateVarSource
        {
            get
            {
                return climateVarSource;
            }
            set
            {
                climateVarSource = value;
            }
        }
        //---------------------------------------------------------------------
        public float ClimateThresh_Lowerbound
        {
            get
            {
                return climateThresh_Lowerbound;
            }
            set
            {
                climateThresh_Lowerbound = value;
            }
        }
        //---------------------------------------------------------------------
        public float ClimateThresh_Upperbound
        {
            get
            {
                return climateThresh_Upperbound;
            }
            set
            {
                climateThresh_Upperbound = value;
            }
        }
        //---------------------------------------------------------------------
        public int ClimateLag
        {
            get
            {
                return climateLag;
            }
            set
            {
                climateLag = value;
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
        //---------------------------------------------------------------------
        /// <summary>
        /// Climate Data Table.
        /// </summary>
        public DataTable ClimateDataTable
        {
            get
            {
                return climateDataTable;
            }
            set
            {
                climateDataTable = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Outbreak List.
        /// </summary>
        public LinkedList<int> OutbreakList
        {
            get
            {
                return outbreakList;
            }
            set
            {
                outbreakList = value;
            }
        }

        //---------------------------------------------------------------------
        public ISppParameters[] SppParameters
        {
            get {
                return sppParameters;
            }
            set {
                sppParameters = value;
            }
        }
        //---------------------------------------------------------------------
        public IEcoParameters[] EcoParameters
        {
            get {
                return ecoParameters;
            }
            set {
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
            get {
                return severity;
            }
            set {
                severity = value;
            }
        }
        //---------------------------------------------------------------------
        public ISiteVar<Zone> OutbreakZone
        {
            get {
                return outbreakZone;
            }
            set {
                outbreakZone = value;
            }
        }

        //---------------------------------------------------------------------
        public bool Dispersal
        {
            get {
                return dispersal;
            }
            set {
                dispersal = value;
            }
        }
        //---------------------------------------------------------------------
        public int DispersalRate
        {
            get {
                return dispersalRate;
            }
            set {
                if (value <= 0)
                        throw new InputValueException(value.ToString(),
                            "Value must be > 0.");
                dispersalRate = value;
            }
        }
        //---------------------------------------------------------------------
        public double EpidemicThresh
        {
            get {
                return epidemicThresh;
            }
            set {
                if (value < 0.0 || value > 1.0)
                       throw new InputValueException(value.ToString(),
                            "Value must be > or = 0 and < or = 1.");
                epidemicThresh = value;
            }
        }
        //---------------------------------------------------------------------
        public int EpicenterNum
        {
            get {
                return epicenterNum;
            }
            set {
                epicenterNum = value;
            }
        }
        //---------------------------------------------------------------------
        public bool SeedEpicenter
        {
            get {
                return seedEpicenter;
            }
            set {
                seedEpicenter = value;
            }
        }
        //---------------------------------------------------------------------
        public double OutbreakEpicenterCoeff
        {
            get {
                return outbreakEpicenterCoeff;
            }
            set {
                outbreakEpicenterCoeff = value;
            }
        }
        //---------------------------------------------------------------------
        public double OutbreakEpicenterThresh
        {
            get
            {
                return outbreakEpicenterThresh;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(),
                         "Value must be > or = 0 and < or = 1.");
                outbreakEpicenterThresh = value;
            }
        }
        //---------------------------------------------------------------------
        public double SeedEpicenterCoeff
        {
            get {
                return seedEpicenterCoeff;
            }
            set {
                seedEpicenterCoeff = value;
            }
        }
        //---------------------------------------------------------------------
        public DispersalTemplate DispersalTemp
        {
            get {
                return dispersalTemp;
            }
            set {
                dispersalTemp = value;
            }
        }
        //---------------------------------------------------------------------
        public IEnumerable<RelativeLocation> DispersalNeighbors
        {
            get {
                return dispersalNeighbors;
            }
            set {
                dispersalNeighbors = value;
            }
        }
        //---------------------------------------------------------------------
        public bool NeighborFlag
        {
            get {
                return neighborFlag;
            }
            set {
                neighborFlag = value;
            }
        }
        //---------------------------------------------------------------------
        public NeighborSpeed NeighborSpeedUp
        {
            get {
                return neighborSpeedUp;
            }
            set {
                 neighborSpeedUp = value;
            }
        }
        //---------------------------------------------------------------------
        public int NeighborRadius
        {
            get {
                return neighborRadius;
            }
            set {
                if (value <= 0)
                        throw new InputValueException(value.ToString(),
                            "Value must be > 0.");
                neighborRadius = value;
            }
        }
        //---------------------------------------------------------------------
        public NeighborShape ShapeOfNeighbor
        {
            get {
                return shapeOfNeighbor;
            }
            set {
                shapeOfNeighbor = value;
            }
        }
        //---------------------------------------------------------------------
        public double NeighborWeight
        {
            get {
                return neighborWeight;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(),
                            "Value must = or be > 0.");
                neighborWeight = value;
            }
        }
        //---------------------------------------------------------------------
        public IEnumerable<RelativeLocationWeighted> ResourceNeighbors
        {
            get {
                return resourceNeighbors;
            }
            set {
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
            severity       = PlugIn.ModelCore.Landscape.NewSiteVar<byte>();
            outbreakZone   = PlugIn.ModelCore.Landscape.NewSiteVar<Zone>();
            climateDataTable = new DataTable();

            for (int i = 0; i < sppCount; i++)
                SppParameters[i] = new SppParameters();
            for (int i = 0; i < ecoCount; i++)
                EcoParameters[i] = new EcoParameters();
            //for (int i = 0; i < distCount; i++)
            //   DistParameters[i] = new DistParameters();
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
