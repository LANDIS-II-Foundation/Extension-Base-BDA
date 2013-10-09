using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;

namespace Landis.Extension.BaseBDA
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
                //String outputFolder = OutputPath.ReplaceTemplateVars("", FINISH ME LATER);
                FolderName = System.IO.Directory.GetCurrentDirectory().Split("\\".ToCharArray()).Last(),
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime,
                ProjectionFilePath = "Projection.?" //How do we get projections???
            };

            Extension = new ExtensionMetadata(mCore){
                Name = PlugIn.ExtensionName,
                TimeInterval = Timestep, //change this to PlugIn.TimeStep for other extensions
                ScenarioReplicationMetadata = scenRep
            };

            //---------------------------------------
            //          table outputs:   
            //---------------------------------------

             PlugIn.eventLog = new MetadataTable<EventsLog>(logFileName);

            OutputMetadata tblOut_events = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "EventLog",
                FilePath = PlugIn.eventLog.FilePath//,
                //MetadataFilePath = @"Base-Wind\EventLog.xml"
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
                    Name = "Outbreak Severity",
                    FilePath = @mapTypePath,
                    Map_DataType = MapDataType.Nominal,
                    Map_Unit = "categorical",
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
                        Map_DataType = MapDataType.Nominal,
                        Map_Unit = "categorical",
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
                        Map_DataType = MapDataType.Nominal,
                        Map_Unit = "categorical",
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
