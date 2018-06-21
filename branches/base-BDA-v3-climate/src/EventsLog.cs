using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.BaseBDA
{
    public class EventsLog
    {
        //log.Write("CurrentTime, ROS, AgentName, NumCohortsKilled, NumSitesDamaged, MeanSeverity");

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Time {set; get;}

        [DataFieldAttribute(Desc = "Rate of Spread")]
        public int ROS { set; get; }

        [DataFieldAttribute(Desc = "Agent Name")]
        public string AgentName { set; get; }

        //[DataFieldAttribute(Unit = FiledUnits.None, Desc = "Total Number of Sites in Event")]
        //public int TotalSites { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Cohorts Killed")]
        public int CohortsKilled { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Damaged Sites in Event")]
        public int DamagedSites { set; get; }

        [DataFieldAttribute(Desc = "Mean Severity (1-5)", Format="0.00")]
        public double MeanSeverity { set; get; }

    }
}
