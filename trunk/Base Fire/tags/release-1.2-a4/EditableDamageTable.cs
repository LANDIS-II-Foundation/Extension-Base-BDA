//  Copyright 2005 University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo
//  License:  Available at  
//  http://landis.forest.wisc.edu/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Fire
{

    /// <summary>
    /// Editable definition of a Fire severity.
    /// </summary>
    public interface IEditableDamageTable
        : IEditable<IDamageTable>
    {
        /// <summary>
        /// The maximum cohort ages (as % of species longevity) that the
        /// damage class applies to.
        /// </summary>
        InputValue<Percentage> MaxAge
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The difference between fire severity and species fire tolerance.
        /// </summary>
        InputValue<int> SeverTolerDifference
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Editable definition of a Fire severity.
    /// </summary>
    public class EditableDamageTable
        : IEditableDamageTable
    {
        private InputValue<Percentage> maxAge;
        private InputValue<int> severTolerDifference;

        //---------------------------------------------------------------------


        /// <summary>
        /// The maximum cohort ages (as % of species longevity) that the
        /// damage class applies to.
        /// </summary>
        public InputValue<Percentage> MaxAge
        {
            get {
                return maxAge;
            }

            set {
                if (value != null) {
                    ValidateAge(value);
                }
                maxAge = value;
            }
        }

        //---------------------------------------------------------------------

        private void ValidateAge(InputValue<Percentage> age)
        {
            if (age.Actual < 0.0 || age.Actual > 1.0)
                throw new InputValueException(age.String,
                                              "Value must be between 0% and 100%");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The difference between fire severity and species fire tolerance.
        /// </summary>
        public InputValue<int> SeverTolerDifference
        {
            get {
                return severTolerDifference;
            }

            set {
                if (value != null) {
                    if (value.Actual < -4 || value.Actual > 4)
                        throw new InputValueException(value.String,
                                                      "Value must be between -4 and 4");
                }
                severTolerDifference = value;
            }
        }

        //---------------------------------------------------------------------

        public EditableDamageTable()
        {
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                foreach (object parameter in new object[]{ 
                                                           maxAge,
                                                           severTolerDifference }) {
                    if (parameter == null)
                        return false;
                }
                return true;
            }
        }

        //---------------------------------------------------------------------

        public IDamageTable GetComplete()
        {
            if (IsComplete)
                return new DamageTable(
                                    maxAge.Actual,
                                    severTolerDifference.Actual);
            else
                return null;
        }
    }
}
