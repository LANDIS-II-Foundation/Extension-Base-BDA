using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.BaseBDA
{
    public class PDSI_Log
    {
        //log.Write("CurrentTime, ROS, AgentName, NumCohortsKilled, NumSitesDamaged, MeanSeverity");

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "...")]
        public int Time {set; get;}

        [DataFieldAttribute(Desc = "Palmer Drought Severity Index")]
        public double PDSI { set; get; }

    }
}
