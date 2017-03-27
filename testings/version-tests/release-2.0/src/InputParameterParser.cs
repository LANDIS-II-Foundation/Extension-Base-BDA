//  Copyright 2005 University of Wisconsin
//  Authors:
//      Robert M. Scheller
//      James B. Domingo
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.
//  Version 1.0
//  License:  Available at
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;
using Landis.Ecoregions;
using Landis.Util;
using System.Collections.Generic;
using System.Text;

namespace Landis.BDA
{
    /// <summary>
    /// A parser that reads the extension parameters from text input.
    /// </summary>
    public class InputParameterParser
        : Landis.TextParser<IInputParameters>
    {
        public static IDataset EcoregionsDataset = null;

        //---------------------------------------------------------------------
        public override string LandisDataValue
        {
            get {
                return "Base BDA";
            }
        }

        //---------------------------------------------------------------------
        public InputParameterParser()
        {
            // FIXME: Hack to ensure that Percentage is registered with InputValues
            //Edu.Wisc.Forest.Flel.Util.Percentage p = new Edu.Wisc.Forest.Flel.Util.Percentage();
        }

        //---------------------------------------------------------------------

        protected override IInputParameters Parse()
        {
            ReadLandisDataVar();

            InputParameters parameters = new InputParameters();

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            //----------------------------------------------------------
            // Read in Map and Log file names.

            InputVar<string> mapNames = new InputVar<string>("MapNames");
            ReadVar(mapNames);
            parameters.MapNamesTemplate = mapNames.Value;

            InputVar<string> srdMapNames = new InputVar<string>("SRDMapNames");
            try
            {
                ReadVar(srdMapNames);
                parameters.SRDMapNames = srdMapNames.Value;
            }
            catch (LineReaderException errString)
            {
                if (!((errString.MultiLineMessage[1].Contains("Found the name \"LogFile\" but expected \"SRDMapNames\"")) || (errString.MultiLineMessage[1].Contains("Found the name \"NRDMapNames\" but expected \"SRDMapNames\""))))
                {
                    throw errString;
                }

            }

            InputVar<string> nrdMapNames = new InputVar<string>("NRDMapNames");
            try
            {
                ReadVar(nrdMapNames);
                parameters.NRDMapNames = nrdMapNames.Value;
            }
            catch (LineReaderException errString)
            {
                if (!(errString.MultiLineMessage[1].Contains("Found the name \"LogFile\" but expected \"NRDMapNames\"")))
                {
                    throw errString;
                }

            }

            InputVar<string> logFile = new InputVar<string>("LogFile");
            ReadVar(logFile);
            parameters.LogFileName = logFile.Value;

            //----------------------------------------------------------
            // Last, read in Agent File names,
            // then parse the data in those files into agent parameters.

            InputVar<string> agentFileName = new InputVar<string>("BDAInputFiles");
            ReadVar(agentFileName);

            List<IAgent> agentParameterList = new List<IAgent>();
            AgentParameterParser agentParser = new AgentParameterParser();

            IAgent agentParameters = Data.Load<IAgent>(agentFileName.Value,agentParser);
            agentParameterList.Add(agentParameters);

            while (!AtEndOfInput) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(agentFileName, currentLine);

                agentParameters = Data.Load<IAgent>(agentFileName.Value,agentParser);

                agentParameterList.Add(agentParameters);

                GetNextLine();

            }

            foreach(IAgent activeAgent in agentParameterList)
            {
                if(agentParameters == null)
                    UI.WriteLine("PARSE:  Agent Parameters NOT loading correctly.");
                else
                    UI.WriteLine("Name of Agent = {0}", agentParameters.AgentName);

            }
            parameters.ManyAgentParameters = agentParameterList;

            return parameters; //.GetComplete();

        }
    }
}
