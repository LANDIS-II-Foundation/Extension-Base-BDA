//  Copyright 2006 University of Wisconsin
//  Author:  James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

namespace Landis.Fire
{
    /// <summary>
    /// The parameters for an ecoregion.
    /// </summary>
    public class EcoregionParameters
        : IEcoregionParameters
    {
        private string name;
        private string description;
        private ushort mapCode;
        private IFireParameters fireParameters;
        private IFireCurve fireCurve;
        private IWindCurve windCurve;

        //---------------------------------------------------------------------

        public string Name
        {
            get {
                return name;
            }
        }

        //---------------------------------------------------------------------

        public string Description
        {
            get {
                return description;
            }
        }

        //---------------------------------------------------------------------

        public ushort MapCode
        {
            get {
                return mapCode;
            }
        }

        //---------------------------------------------------------------------

        public IFireParameters FireParameters
        {
            get {
                return fireParameters;
            }

            set {
                fireParameters = value;
            }
        }

        //---------------------------------------------------------------------

        public IFireCurve FireCurve
        {
            get {
                return fireCurve;
            }

            set {
                fireCurve = value;
            }
        }

        //---------------------------------------------------------------------

        public IWindCurve WindCurve
        {
            get {
                return windCurve;
            }

            set {
                windCurve = value;
            }
        }

        //---------------------------------------------------------------------

        public EcoregionParameters(string name,
                                   string description,
                                   ushort mapCode)
        {
            this.name = name;
            this.description = description;
            this.mapCode = mapCode;

            this.fireParameters = new FireParameters();
            this.fireCurve = new FireCurve();
            this.windCurve = new WindCurve();
        }

        //---------------------------------------------------------------------

        public EcoregionParameters(IEcoregionParameters parameters)
        {
            name           = parameters.Name;
            description    = parameters.Description;
            mapCode        = parameters.MapCode;
            fireParameters = parameters.FireParameters;
            fireCurve      = parameters.FireCurve;
            windCurve      = parameters.WindCurve;
        }
    }
}
