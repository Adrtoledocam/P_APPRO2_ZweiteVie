using Syncfusion.Maui.Toolkit.Charts;

namespace P_APPRO2_ZweiteVieApp.Pages.Controls
{
    public class LegendExt : ChartLegend
    {
        protected override double GetMaximumSizeCoefficient()
        {
            return 0.5;
        }
    }
}
