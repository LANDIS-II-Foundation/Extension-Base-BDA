//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo

using Landis.Core;
using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;
using System.Text;

namespace Landis.Extension.BaseBDA
{
    /// <summary>
    /// A parser that reads the extension parameters from text input.
    /// </summary>
    public class InputParameterParser
        : Landis.TextParser<IInputParameters>
    {
        public static IEcoregionDataset EcoregionsDataset = null;


        //---------------------------------------------------------------------
        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }

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
            ReadOptionalVar(srdMapNames);
            parameters.SRDMapNames = srdMapNames.Value;

            InputVar<string> nrdMapNames = new InputVar<string>("NRDMapNames");
            ReadOptionalVar(nrdMapNames);
            parameters.NRDMapNames = nrdMapNames.Value;

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

            IAgent agentParameters = Landis.Data.Load<IAgent>(agentFileName.Value, agentParser);
            agentParameterList.Add(agentParameters);

            while (!AtEndOfInput) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(agentFileName, currentLine);

                agentParameters = Landis.Data.Load<IAgent>(agentFileName.Value, agentParser);

                agentParameterList.Add(agentParameters);

                GetNextLine();

            }

            foreach(IAgent activeAgent in agentParameterList)
            {
                if(agentParameters == null)
                    PlugIn.ModelCore.UI.WriteLine("PARSE:  Agent Parameters NOT loading correctly.");
                else
                    PlugIn.ModelCore.UI.WriteLine("Name of Agent = {0}", agentParameters.AgentName);

            }
            parameters.ManyAgentParameters = agentParameterList;

            return parameters; //.GetComplete();

        }
    }
}
