using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Landis.Extension.BaseBDA
{
    class ClimateData
    {
        //---------------------------------------------------------------------

        public static DataTable ReadWeatherFile(string path)
        {
            PlugIn.ModelCore.UI.WriteLine("   Loading Climate Data...");

            CSVParser weatherParser = new CSVParser();

            DataTable weatherTable = weatherParser.ParseToDataTable(path);

            return weatherTable;
        }
        //---------------------------------------------------------------------
    }
}
