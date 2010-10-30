using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Output.Biomass
{
    public static class SelectedDeadPoolsUtil
    {
        static SelectedDeadPoolsUtil()
        {
            Type.SetDescription<SelectedDeadPools>("dead pool");
        }

        //---------------------------------------------------------------------

        public static SelectedDeadPools Parse(string text)
        {
            if (text == "woody")
                return SelectedDeadPools.Woody;
            else if (text == "non-woody")
                return SelectedDeadPools.NonWoody;
            else if (text == "both")
                return SelectedDeadPools.Woody | SelectedDeadPools.NonWoody;
            else
                throw new System.FormatException("Valid values are: woody, non-woody, or both");
        }
    }
}
