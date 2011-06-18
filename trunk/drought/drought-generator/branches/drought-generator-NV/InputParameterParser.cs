
using System.Collections.Generic;

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Extension.DroughtGenerator
{
    /// <summary>
    /// A parser that reads the plug-in's parameters from text input.
    /// </summary>
    class InputParametersParser
        : TextParser<IInputParameters>
    {
        //---------------------------------------------------------------------
        //public InputParameterParser()
        //{
         //   RegisterForInputValues();
        //}
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

            InputVar<double> mu = new InputVar<double>("Mu");
            ReadVar(mu);
            parameters.Mu = mu.Value;

            InputVar<double> sigma = new InputVar<double>("Sigma");
            ReadVar(sigma);
            parameters.Sigma = sigma.Value;

            InputVar<string> mapNames = new InputVar<string>("MapName");
            ReadVar(mapNames);
            parameters.MapNamesTemplate = mapNames.Value;

            InputVar<string> logFile = new InputVar<string>("LogFile");
            ReadVar(logFile);
            parameters.LogFileName = logFile.Value;

            return parameters;

        }

    }
}
