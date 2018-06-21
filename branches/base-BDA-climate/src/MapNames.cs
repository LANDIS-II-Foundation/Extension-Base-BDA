//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller,   James B. Domingo

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Extension.BaseBDA
{
    /// <summary>
    /// Methods for working with the template for map filenames.
    /// </summary>
    public static class MapNames
    {
        public const string AgentNameVar = "agentName";
        public const string TimestepVar = "timestep";

        private static IDictionary<string, bool> knownVars;
        private static IDictionary<string, string> varValues;

        //---------------------------------------------------------------------
        static MapNames()
        {
            knownVars = new Dictionary<string, bool>();
            knownVars[AgentNameVar] = true;
            knownVars[TimestepVar] = true;

            varValues = new Dictionary<string, string>();
        }

        //---------------------------------------------------------------------
        public static void CheckTemplateVars(string template)
        {
            OutputPath.CheckTemplateVars(template, knownVars);
        }

        //---------------------------------------------------------------------
        public static string ReplaceTemplateVars(string template,
                                                 string agentName,
                                                 int    timestep)
        {
            varValues[AgentNameVar] = agentName;
            varValues[TimestepVar] = timestep.ToString();
            return OutputPath.ReplaceTemplateVars(template, varValues);
        }
        //---------------------------------------------------------------------
        public static string ReplaceTemplateVars(string template,
                                                 string agentName)
        {
            varValues[AgentNameVar] = agentName;
            return OutputPath.ReplaceTemplateVars(template, varValues);
        }
    }
}
