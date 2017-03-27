//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo

using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;
//using Landis.Util;
using System.Collections.Generic;
using System.Text;

namespace Landis.Extension.BudwormBDA
{

    /// <summary>
    /// A parser that reads the extension parameters from text input.
    /// </summary>
    public class AgentParameterParser
        : TextParser<IAgent>
    {

        public static IEcoregionDataset EcoregionsDataset = PlugIn.ModelCore.Ecoregions;
        public static ISpeciesDataset SpeciesDataset = PlugIn.ModelCore.Species; //null;

        //---------------------------------------------------------------------

        /*public override string LandisDataValue
        {
            get {
                return "A Single BDA Agent";
            }
        }*/

        //---------------------------------------------------------------------

        public AgentParameterParser()
        {
            RegisterForInputValues();
        }

        //---------------------------------------------------------------------

        protected override IAgent Parse()
        {
            //PlugIn.ModelCore.Log.WriteLine("Parsing 1; sppCnt={0}", Model.Species.Count);
            Agent agentParameters = new Agent(PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count, (int) DisturbanceType.Null);  //The last disturb Type is Null

            InputVar<string> agentName = new InputVar<string>("BDAAgentName");
            ReadVar(agentName);
            agentParameters.AgentName = agentName.Value;

            InputVar<int> bdpc = new InputVar<int>("BDPCalibrator");
            ReadVar(bdpc);
            agentParameters.BDPCalibrator = bdpc.Value;

            InputVar<SRDmode> srd = new InputVar<SRDmode>("SRDMode");
            ReadVar(srd);
            agentParameters.SRDmode = srd.Value;

            InputVar<int> tSLE = new InputVar<int>("TimeSinceLastEpidemic");
            ReadVar(tSLE);
            agentParameters.TimeSinceLastEpidemic = tSLE.Value;

            InputVar<TemporalType> tt = new InputVar<TemporalType>("TemporalType");
            ReadVar(tt);
            agentParameters.TempType = tt.Value;

            InputVar<RandomFunction> rf = new InputVar<RandomFunction>("RandomFunction");
            ReadVar(rf);
            agentParameters.RandFunc = rf.Value;

            InputVar<double> rp1 = new InputVar<double>("RandomParameter1");
            ReadVar(rp1);
            agentParameters.RandomParameter1 = rp1.Value;

            InputVar<double> rp2 = new InputVar<double>("RandomParameter2");
            ReadVar(rp2);
            agentParameters.RandomParameter2 = rp2.Value;

            InputVar<int> minROS = new InputVar<int>("MinROS");
            ReadVar(minROS);
            agentParameters.MinROS = minROS.Value;

            InputVar<int> maxROS = new InputVar<int>("MaxROS");
            ReadVar(maxROS);
            agentParameters.MaxROS = maxROS.Value;

            InputVar<bool> d = new InputVar<bool>("Dispersal");
            ReadVar(d);
            agentParameters.Dispersal = d.Value;

            InputVar<int> dr = new InputVar<int>("DispersalRate");
            ReadVar(dr);
            agentParameters.DispersalRate = dr.Value;

            InputVar<double> et = new InputVar<double>("EpidemicThresh");
            ReadVar(et);
            agentParameters.EpidemicThresh = et.Value;

            InputVar<int> ien = new InputVar<int>("InitialEpicenterNum");
            ReadVar(ien);
            agentParameters.EpicenterNum = ien.Value;

            InputVar<double> oec = new InputVar<double>("OutbreakEpicenterCoeff");
            ReadVar(oec);
            agentParameters.OutbreakEpicenterCoeff = oec.Value;

            InputVar<bool> se = new InputVar<bool>("SeedEpicenter");
            ReadVar(se);
            agentParameters.SeedEpicenter = se.Value;

            InputVar<double> sec = new InputVar<double>("SeedEpicenterCoeff");
            ReadVar(sec);
            agentParameters.SeedEpicenterCoeff = sec.Value;

            InputVar<DispersalTemplate> dispt = new InputVar<DispersalTemplate>("DispersalTemplate");
            ReadVar(dispt);
            agentParameters.DispersalTemp = dispt.Value;

            InputVar<bool> nf = new InputVar<bool>("NeighborFlag");
            ReadVar(nf);
            agentParameters.NeighborFlag = nf.Value;

            InputVar<NeighborSpeed> nspeed = new InputVar<NeighborSpeed>("NeighborSpeedUp");
            ReadVar(nspeed);
            agentParameters.NeighborSpeedUp = nspeed.Value;

            InputVar<int> nr = new InputVar<int>("NeighborRadius");
            ReadVar(nr);
            agentParameters.NeighborRadius = nr.Value;

            InputVar<NeighborShape> ns = new InputVar<NeighborShape>("NeighborShape");
            ReadVar(ns);
            agentParameters.ShapeOfNeighbor = ns.Value;

            InputVar<double> nw = new InputVar<double>("NeighborWeight");
            ReadVar(nw);
            agentParameters.NeighborWeight = nw.Value;

            //Added for budworm-BDA
            InputVar<double> class2_SV = new InputVar<double>("Class2_SV");
            ReadVar(class2_SV);
            agentParameters.Class2_SV = class2_SV.Value;

            InputVar<double> class3_SV = new InputVar<double>("Class3_SV");
            ReadVar(class3_SV);
            agentParameters.Class3_SV = class3_SV.Value;

            InputVar<int> bfAgeCutoff = new InputVar<int>("BFAgeCutoff");
            ReadVar(bfAgeCutoff);
            agentParameters.BFAgeCutoff = bfAgeCutoff.Value;

            //--------- Read In Ecoreigon Table ---------------------------------------
            PlugIn.ModelCore.Log.WriteLine("Begin parsing ECOREGION table.");

            InputVar<string> ecoName = new InputVar<string>("Ecoregion Name");
            InputVar<double> ecoModifier = new InputVar<double>("Ecoregion Modifier");

            Dictionary <string, int> lineNumbers = new Dictionary<string, int>();
            const string DistParms = "DisturbanceModifiers";

            while (! AtEndOfInput && CurrentName != DistParms) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(ecoName, currentLine);
                IEcoregion ecoregion = EcoregionsDataset[ecoName.Value.Actual];
                if (ecoregion == null)
                    throw new InputValueException(ecoName.Value.String,
                                                  "{0} is not an ecoregion name.",
                                                  ecoName.Value.String);
                int lineNumber;
                if (lineNumbers.TryGetValue(ecoregion.Name, out lineNumber))
                    throw new InputValueException(ecoName.Value.String,
                                                  "The ecoregion {0} was previously used on line {1}",
                                                  ecoName.Value.String, lineNumber);
                else
                    lineNumbers[ecoregion.Name] = LineNumber;

                IEcoParameters ecoParms = new EcoParameters();
                agentParameters.EcoParameters[ecoregion.Index] = ecoParms;

                ReadValue(ecoModifier, currentLine);
                ecoParms.EcoModifier = ecoModifier.Value;

                CheckNoDataAfter("the " + ecoModifier.Name + " column",
                                 currentLine);
                GetNextLine();
            }

            //--------- Read In Disturbance Modifier Table -------------------------------
            PlugIn.ModelCore.Log.WriteLine("Begin parsing DISTURBANCE table.");

            ReadName(DistParms);

            InputVar<DisturbanceType> distType = new InputVar<DisturbanceType>("Disturbance Type");
            InputVar<int> duration = new InputVar<int>("Duration");
            InputVar<double> distModifier = new InputVar<double>("Disturbance Modifier");

            lineNumbers = new Dictionary<string, int>();
            const string SppParms = "BDASpeciesParameters";

            while (! AtEndOfInput && CurrentName != SppParms) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(distType, currentLine);
                int dt = (int) distType.Value.Actual;

                IDistParameters distParms = new DistParameters();
                agentParameters.DistParameters[dt] = distParms;

                ReadValue(duration, currentLine);
                distParms.Duration = duration.Value;

                ReadValue(distModifier, currentLine);
                distParms.DistModifier = distModifier.Value;

                CheckNoDataAfter("the " + distModifier.Name + " column",
                                 currentLine);
                GetNextLine();
            }
            //--------- Read In Species Table ---------------------------------------
            PlugIn.ModelCore.Log.WriteLine("Begin parsing SPECIES table.");

            ReadName(SppParms);

            //const string FireCurves = "FireCurveTable";
            InputVar<string> sppName = new InputVar<string>("Species");
            InputVar<int> minorHostAge = new InputVar<int>("Minor Host Age");
            InputVar<double> minorHostSRD = new InputVar<double>("Minor Host SRDProb");
            InputVar<int> secondaryHostAge = new InputVar<int>("Second Host Age");
            InputVar<double> secondaryHostSRD = new InputVar<double>("Secondary Host SRDProb");
            InputVar<int> primaryHostAge = new InputVar<int>("Primary Host Age");
            InputVar<double> primaryHostSRD = new InputVar<double>("Primary Host SRDProb");
            InputVar<int> resistantHostAge = new InputVar<int>("Resistant Host Age");
            InputVar<double> resistantHostVuln = new InputVar<double>("Resistant Host VulnProb");
            InputVar<int> tolerantHostAge = new InputVar<int>("Tolerant Host Age");
            InputVar<double> tolerantHostVuln = new InputVar<double>("Tolerant Host VulnProb");
            InputVar<int> vulnerableHostAge = new InputVar<int>("Vulnerable Host Age");
            InputVar<double> vulnerableHostVuln = new InputVar<double>("Vulnerable Host VulnProb");
            InputVar<bool> cfsConifer = new InputVar<bool>("CFS Conifer type:  yes/no");

            const string NegSpp = "IgnoredSpecies";

            while ((! AtEndOfInput) && (CurrentName != NegSpp)) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(sppName, currentLine);
                ISpecies species = SpeciesDataset[sppName.Value.Actual];
                if (species == null)
                    throw new InputValueException(sppName.Value.String,
                                                  "{0} is not a species name.",
                                                  sppName.Value.String);
                int lineNumber;
                if (lineNumbers.TryGetValue(species.Name, out lineNumber))
                    throw new InputValueException(sppName.Value.String,
                                                  "The species {0} was previously used on line {1}",
                                                  sppName.Value.String, lineNumber);
                else
                    lineNumbers[species.Name] = LineNumber;

                ISppParameters sppParms = new SppParameters();
                agentParameters.SppParameters[species.Index] = sppParms;

                ReadValue(minorHostAge, currentLine);
                sppParms.MinorHostAge = minorHostAge.Value;

                ReadValue(minorHostSRD, currentLine);
                sppParms.MinorHostSRD = minorHostSRD.Value;

                ReadValue(secondaryHostAge, currentLine);
                sppParms.SecondaryHostAge = secondaryHostAge.Value;

                ReadValue(secondaryHostSRD, currentLine);
                sppParms.SecondaryHostSRD = secondaryHostSRD.Value;

                ReadValue(primaryHostAge, currentLine);
                sppParms.PrimaryHostAge = primaryHostAge.Value;

                ReadValue(primaryHostSRD, currentLine);
                sppParms.PrimaryHostSRD = primaryHostSRD.Value;

                ReadValue(resistantHostAge, currentLine);
                sppParms.ResistantHostAge = resistantHostAge.Value;

                ReadValue(resistantHostVuln, currentLine);
                sppParms.ResistantHostVuln = resistantHostVuln.Value;

                ReadValue(tolerantHostAge, currentLine);
                sppParms.TolerantHostAge = tolerantHostAge.Value;

                ReadValue(tolerantHostVuln, currentLine);
                sppParms.TolerantHostVuln = tolerantHostVuln.Value;

                ReadValue(vulnerableHostAge, currentLine);
                sppParms.VulnerableHostAge = vulnerableHostAge.Value;

                ReadValue(vulnerableHostVuln, currentLine);
                sppParms.VulnerableHostVuln = vulnerableHostVuln.Value;

                ReadValue(cfsConifer, currentLine);
                sppParms.CFSConifer = cfsConifer.Value;

                CheckNoDataAfter("the " + cfsConifer.Name + " column",
                                 currentLine);


                GetNextLine();
            }

            //--------- Read In Ignored Species List ---------------------------------------

            List<ISpecies> negSppList = new List<ISpecies>();
            if (!AtEndOfInput)
            {
                ReadName(NegSpp);
                InputVar<string> negSppName = new InputVar<string>("Ignored Spp Name");

                while (!AtEndOfInput)
                {
                    StringReader currentLine = new StringReader(CurrentLine);

                    ReadValue(negSppName, currentLine);
                    ISpecies species = SpeciesDataset[negSppName.Value.Actual];
                    if (species == null)
                        throw new InputValueException(negSppName.Value.String,
                                                      "{0} is not a species name.",
                                                      negSppName.Value.String);
                    int lineNumber;
                    if (lineNumbers.TryGetValue(species.Name, out lineNumber))
                        PlugIn.ModelCore.Log.WriteLine("WARNING: The species {0} was previously used on line {1}.  Being listed in the IgnoredSpecies list will override any settings in the Host table.", negSppName.Value.String, lineNumber);
                    else
                        lineNumbers[species.Name] = LineNumber;

                    negSppList.Add(species);

                    GetNextLine();

                }
            }
            agentParameters.NegSppList = negSppList;


            return agentParameters; //.GetComplete();
        }

        public static SRDmode SRDParse(string word)
        {
            if (word == "max")
                return SRDmode.max;
            else if (word == "mean")
                return SRDmode.mean;
            throw new System.FormatException("Valid algorithms: max, mean");
        }

        public static TemporalType TTParse(string word)
        {
            if (word == "pulse")
                return TemporalType.pulse;
            else if (word == "variablepulse")
                return TemporalType.variablepulse;
            throw new System.FormatException("Valid algorithms: pulse, continuous, variablepulse");
        }

        public static RandomFunction RFParse(string word)
        {
            if (word == "RFnormal")
                return RandomFunction.RFnormal;
            else if (word == "RFuniform")
                return RandomFunction.RFuniform;
            throw new System.FormatException("Valid algorithms: RFnormal and RFuniform");
        }

        public static DisturbanceType DTParse(string word)
        {
            if (word == "Wind")
                return DisturbanceType.Wind;
            else if (word == "Fire")
                return DisturbanceType.Fire;
            else if (word == "Harvest")
                return DisturbanceType.Harvest;
            throw new System.FormatException("Valid algorithms: Wind, Fire, Harvest");
        }
        public static DispersalTemplate DispTParse(string word)
        {
            if (word == "MaxRadius")
                return DispersalTemplate.MaxRadius;
            else if (word == "4N")
                return DispersalTemplate.N4;
            else if (word == "8N")
                return DispersalTemplate.N8;
            else if (word == "12N")
                return DispersalTemplate.N12;
            else if (word == "24N")
                return DispersalTemplate.N24;
            throw new System.FormatException("Valid algorithms: MaxRadius, 4N, 8N, 12N, 24N");
        }
        public static NeighborShape NSParse(string word)
        {
            if (word == "uniform")
                return NeighborShape.uniform;
            else if (word == "linear")
                return NeighborShape.linear;
            else if (word == "gaussian")
                return NeighborShape.gaussian;
            throw new System.FormatException("Valid algorithms: uniform, linear, gaussian");
        }
        public static NeighborSpeed NSpeedParse(string word)
        {
            if (word == "none")
                return NeighborSpeed.none;
            else if (word == "2x")
                return NeighborSpeed.X2;
            else if (word == "3x")
                return NeighborSpeed.X3;
            else if (word == "4x")
                return NeighborSpeed.X4;
            throw new System.FormatException("Valid algorithms:  none, 2x, 3x, 4x");
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Registers the appropriate method for reading input values.
        /// </summary>
        public static void RegisterForInputValues()
        {
            Type.SetDescription<SRDmode>("Site Resources Dominance Mode");
            InputValues.Register<SRDmode>(SRDParse);

            Type.SetDescription<TemporalType>("Temporal Type");
            InputValues.Register<TemporalType>(TTParse);

            Type.SetDescription<RandomFunction>("Random Function");
            InputValues.Register<RandomFunction>(RFParse);

            Type.SetDescription<DisturbanceType>("Disturbance Type");
            InputValues.Register<DisturbanceType>(DTParse);

            Type.SetDescription<DispersalTemplate>("Dispersal Template");
            InputValues.Register<DispersalTemplate>(DispTParse);

            Type.SetDescription<NeighborShape>("Neighbor Shape");
            InputValues.Register<NeighborShape>(NSParse);

            Type.SetDescription<NeighborSpeed>("Neighbor Speed");
            InputValues.Register<NeighborSpeed>(NSpeedParse);
        }
    }
}
