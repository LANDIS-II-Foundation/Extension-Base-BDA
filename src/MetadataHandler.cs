﻿//  Authors:  Robert M. Scheller, Brian Miranda
//  BDA originally programmed by Wei (Vera) Li at University of Missouri-Columbia in 2004.


using System.Collections.Generic;
using Landis.Library.Metadata;
using Landis.Core;

namespace Landis.Extension.ClimateBDA
{
    public static class MetadataHandler
    {
        
        public static ExtensionMetadata Extension {get; set;}

        public static void InitializeMetadata(int Timestep, 
            string severityMapFileName, 
            string srdMapFileName, 
            string nrdMapFileName, 
            string logFileName, 
            IEnumerable<IAgent> manyAgentParameters, 
            ICore mCore)
        {
            ScenarioReplicationMetadata scenRep = new ScenarioReplicationMetadata() {
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime,
            };

            Extension = new ExtensionMetadata(mCore){
                Name = PlugIn.ExtensionName,
                TimeInterval = Timestep, //change this to PlugIn.TimeStep for other extensions
                ScenarioReplicationMetadata = scenRep
            };

            //---------------------------------------
            //          table outputs:   
            //---------------------------------------

            var logDir = System.IO.Path.GetDirectoryName(logFileName);
            if (!string.IsNullOrWhiteSpace(logDir))
            {
                System.IO.Directory.CreateDirectory(logDir);
            }
            PlugIn.EventLog = new MetadataTable<EventsLog>(logFileName);

            OutputMetadata tblOut_events = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "EventLog",
                FilePath = PlugIn.EventLog.FilePath,
                Visualize = false,
            };
            tblOut_events.RetriveFields(typeof(EventsLog));
            Extension.OutputMetadatas.Add(tblOut_events);

            //---------------------------------------            
            //          map outputs:         
            //---------------------------------------

            foreach (IAgent activeAgent in manyAgentParameters)
            {
                string mapTypePath = MapNames.ReplaceTemplateVarsMetadata(severityMapFileName, activeAgent.AgentName);

                OutputMetadata mapOut_Severity = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = System.String.Format(activeAgent.AgentName + " Outbreak Severity"),
                    FilePath = @mapTypePath,
                    Map_DataType = MapDataType.Ordinal,
                    Map_Unit = FieldUnits.Severity_Rank,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_Severity);

                if (srdMapFileName != null)
                {
                    mapTypePath = MapNames.ReplaceTemplateVarsMetadata(srdMapFileName, activeAgent.AgentName);
                    OutputMetadata mapOut_SRD = new OutputMetadata()
                    {
                        Type = OutputType.Map,
                        Name = "Site Resource Dominance",
                        FilePath = @mapTypePath,
                        Map_DataType = MapDataType.Continuous,
                        Map_Unit = FieldUnits.Percentage,
                        Visualize = false,
                    };
                    Extension.OutputMetadatas.Add(mapOut_SRD);
                }

                if (nrdMapFileName != null)
                {
                    mapTypePath = MapNames.ReplaceTemplateVarsMetadata(nrdMapFileName, activeAgent.AgentName);
                    OutputMetadata mapOut_NRD = new OutputMetadata()
                    {
                        Type = OutputType.Map,
                        Name = "Neighborhood Resource Dominance",
                        FilePath = @mapTypePath,
                        Map_DataType = MapDataType.Continuous,
                        Map_Unit = FieldUnits.Percentage,
                        Visualize = false,
                    };
                    Extension.OutputMetadatas.Add(mapOut_NRD);
                }
            }
            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);




        }
    }
}
