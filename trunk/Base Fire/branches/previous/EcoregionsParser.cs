//  Copyright 2006 University of Wisconsin
//  Author:  James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;
using System.Text;
using System.Collections.Generic;

namespace Landis.Fire
{
    /// <summary>
    /// A parser that reads ecoregion definitions from a text file.
    /// </summary>
    public class EcoregionsParser
        : Landis.TextParser<IEcoregionDataset>
    {
        public override string LandisDataValue
        {
            get {
                return "Fire Ecoregions";
            }
        }

        //---------------------------------------------------------------------

        public EcoregionsParser()
        {
        }

        //---------------------------------------------------------------------

        protected override IEcoregionDataset Parse()
        {
            ReadLandisDataVar();
            
            IEditableEcoregionDataset dataset = new EditableEcoregionDataset();

            Dictionary <string, int> nameLineNumbers = new Dictionary<string, int>();
            Dictionary <ushort, int> mapCodeLineNumbers = new Dictionary<ushort, int>();

            InputVar<string> name = new InputVar<string>("Name");
            InputVar<string> description = new InputVar<string>("Description");
            InputVar<ushort> mapCode = new InputVar<ushort>("Map Code");

            while (! AtEndOfInput) {
                IEditableEcoregionParameters parameters = new EditableEcoregionParameters();

                dataset.Add(parameters);

                StringReader currentLine = new StringReader(CurrentLine);

                int lineNumber;

                ReadValue(mapCode, currentLine);
                if (mapCodeLineNumbers.TryGetValue(mapCode.Value.Actual, out lineNumber))
                    throw new InputValueException(mapCode.Value.String,
                                                  "The map code {0} was previously used on line {1}",
                                                  mapCode.Value.Actual, lineNumber);
                else
                    mapCodeLineNumbers[mapCode.Value.Actual] = LineNumber;
                parameters.MapCode = mapCode.Value;

                ReadValue(name, currentLine);
                if (nameLineNumbers.TryGetValue(name.Value.Actual, out lineNumber))
                    throw new InputValueException(name.Value.String,
                                                  "The name \"{0}\" was previously used on line {1}",
                                                  name.Value.Actual, lineNumber);
                else
                    nameLineNumbers[name.Value.Actual] = LineNumber;
                parameters.Name = name.Value;

                ReadValue(description, currentLine);
                parameters.Description = description.Value;

                CheckNoDataAfter("the " + description.Name + " column",
                                 currentLine);
                GetNextLine();
            }

            return dataset.GetComplete();
        }
    }
}
