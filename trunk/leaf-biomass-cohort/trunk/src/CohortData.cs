namespace Landis.Library.LeafBiomassCohorts
{
    /// <summary>
    /// Data for an individual cohort that is not shared with other cohorts.
    /// </summary>
    public struct CohortData
    {
        /// <summary>
        /// The cohort's age (years).
        /// </summary>
        public ushort Age;

        //---------------------------------------------------------------------

        /// <summary>
        /// The cohort's wood biomass 
        /// </summary>
        public float WoodBiomass;
        //private uint woodBiomass;

        //---------------------------------------------------------------------

        /// <summary>
        /// The cohort's wood biomass 
        /// </summary>
        public float LeafBiomass;
        //private uint leafBiomass;
        //---------------------------------------------------------------------
/*
        public float WoodBiomass
        {
            get {
                return ((float) woodBiomass) / 1000F;
            }
            set {
                woodBiomass = (uint) (value * 1000F);
            }
        }

        //---------------------------------------------------------------------

        public float LeafBiomass
        {
            get {
                return ((float) leafBiomass) / 1000F;
            }
            set {
                leafBiomass = (uint) (value * 1000F);
                //UI.WriteLine("leafB={0}, input={1}.", leafBiomass, value);
            }
        }
        */
        //---------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="age">
        /// The cohort's age.
        /// </param>
        /// <param name="biomass">
        /// The cohort's biomass.
        /// </param>
        public CohortData(ushort age,
                          float woodBiomass,
                          float leafBiomass)
        {
            this.Age = age;
            //this.woodBiomass = 0;
            //this.leafBiomass = 0;
            this.WoodBiomass = woodBiomass;
            this.LeafBiomass = leafBiomass;
        }
    }
}
