//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

// Validate Fire Parameter Table
namespace Landis.Fire
{
    /// <summary>
    /// Editable parameters (size and frequency) for Fire events in an
    /// ecoregion.
    /// </summary>
    public interface IEditableFireParameters
        : IEditable<IFireParameters>
    {
        /// <summary>
        /// Maximum event size (hectares).
        /// </summary>
        InputValue<int> MaxSize
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Mean event size (hectares).
        /// </summary>
        InputValue<int> MeanSize
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Minimum event size (hectares).
        /// </summary>
        InputValue<int> MinSize
        {
            get;
            set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Ignition Probability
        /// </summary>
        InputValue<float> IgnitionProb
        {
            get;
            set;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fire Spread Age (years).
        /// </summary>
        InputValue<int> FireSpreadAge
        {
            get;
            set;
        }
    }
}


namespace Landis.Fire
{
    /// <summary>
    /// Editable parameters (size and frequency) for Fire events in an
    /// ecoregion.
    /// </summary>
    public class EditableFireParameters
        : IEditableFireParameters
    {
        private InputValue<int> maxSize;
        private InputValue<int> meanSize;
        private InputValue<int> minSize;
        private InputValue<float> ignitionProb;
        private InputValue<int> fireSpreadAge;

        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum event size (hectares).
        /// </summary>
        public InputValue<int> MaxSize
        {
            get {
                return maxSize;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > 0.");
                    if (meanSize != null && value.Actual < meanSize.Actual)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > MeanSize.");
                    if (minSize != null && value.Actual < minSize.Actual)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > MinSize.");
                }
                maxSize = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Mean event size (hectares).
        /// </summary>
        public InputValue<int> MeanSize
        {
            get {
                return meanSize;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > 0.");
                    if (maxSize != null && value.Actual > maxSize.Actual)
                        throw new InputValueException(value.String,
                                                      "Value must be < or = MaxSize.");
                    if (minSize != null && value.Actual < minSize.Actual)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > MinSize.");
                }
                meanSize = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Minimum event size (hectares).
        /// </summary>
        public InputValue<int> MinSize
        {
            get {
                return minSize;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > 0.");
                    if (meanSize != null && value.Actual > meanSize.Actual)
                        throw new InputValueException(value.String,
                                                      "Value must be < or = MeanSize.");
                    if (maxSize != null && value.Actual > maxSize.Actual)
                        throw new InputValueException(value.String,
                                                      "Value must be < or = MaxSize.");
                }
                minSize = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fire ignition probability
        /// </summary>
        public InputValue<float> IgnitionProb
        {
            get {
                return ignitionProb;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0.0 )
                        throw new InputValueException(value.String,
                                                      "Value must be = or > 0.");
                    if (value.Actual > 1.0 )
                        throw new InputValueException(value.String,
                                                      "Value must be = or < 1.0.");
                }
                ignitionProb = value;
            }
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// Fire Spread Age (years).
        /// </summary>
        public InputValue<int> FireSpreadAge
        {
            get {
                return fireSpreadAge;
            }

            set {
                if (value != null) {
                    if (value.Actual < 0)
                        throw new InputValueException(value.String,
                                                      "Value must be = or > 0.");
                }
                fireSpreadAge = value;
            }
        }

        //---------------------------------------------------------------------

        public EditableFireParameters()
        {
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                foreach (object parameter in new object[]{ maxSize,
                                                           meanSize,
                                                           minSize,
                                                           ignitionProb,
                                                           fireSpreadAge}) {
                    if (parameter == null)
                        return false;
                }
                return true;
            }
        }

        //---------------------------------------------------------------------

        public IFireParameters GetComplete()
        {
            if (IsComplete)
                return new FireParameters(maxSize.Actual,
                                           meanSize.Actual,
                                           minSize.Actual,
                                           ignitionProb.Actual,
                                           fireSpreadAge.Actual);
            else
                return null;
        }
    }
}


