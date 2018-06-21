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
        : TextParser<IInputParameters>
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
        public InputParameterParser()
        {
        }

        //---------------------------------------------------------------------

        protected override IInputParameters Parse()
        {

            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != PlugIn.ExtensionName)
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", PlugIn.ExtensionName);

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
            if(ReadOptionalVar(srdMapNames))
                parameters.SRDMapNames = srdMapNames.Value;
            /*try
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
            */
            InputVar<string> nrdMapNames = new InputVar<string>("NRDMapNames");
            if(ReadOptionalVar(nrdMapNames))
                parameters.NRDMapNames = nrdMapNames.Value;
            /*try
            {
                ReadVar(nrdMapNames);
                parameters.NRDMapNames = nrdMapNames.Value;
            }
            catch (LineReaderException errString)
            {
                if (!(errString.MultiLineMessage[1].Contains("Found the name \"VulnMapNames\" but expected \"NRDMapNames\"")))
                {
                    throw errString;
                }

            }
             * */
            InputVar<string> bdpMapNames = new InputVar<string>("BDPMapNames");
            if(ReadOptionalVar(bdpMapNames))
                parameters.BDPMapNames = bdpMapNames.Value;
            /*try
            {
                ReadVar(bdpMapNames);
                parameters.BDPMapNames = bdpMapNames.Value;
            }
            catch (LineReaderException errString)
            {
                if (!(errString.MultiLineMessage[1].Contains("Found the name \"LogFile\" but expected \"VulnMapNames\"")))
                {
                    throw errString;
                }

            }
            */

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
