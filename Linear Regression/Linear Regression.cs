/*  CTRADER GURU --> Indicator Template 1.0.6

    Homepage    : https://ctrader.guru/
    Telegram    : https://t.me/ctraderguru
    Twitter     : https://twitter.com/cTraderGURU/
    Facebook    : https://www.facebook.com/ctrader.guru/
    YouTube     : https://www.youtube.com/channel/UCKkgbw09Fifj65W5t5lHeCQ
    GitHub      : https://github.com/ctrader-guru

*/

using cAlgo.API;
using System;

namespace cAlgo
{

    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class LinearRegression : Indicator
    {

        #region Identity

        public const string NAME = "Linear Regression";

        public const string VERSION = "1.0.1";

        #endregion

        #region Params

        [Parameter(NAME + " " + VERSION, Group = "Identity", DefaultValue = "https://www.google.com/search?q=ctrader+guru+linear+regression")]
        public string ProductInfo { get; set; }

        [Parameter("Period", Group = "Params", DefaultValue = 120)]
        public int Period { get; set; }

        [Parameter("Color", Group = "Styles", DefaultValue = "Gray")]
        public string MyColor { get; set; }

        [Parameter("Line", Group = "Styles", DefaultValue = 1.0)]
        public int LineThickness { get; set; }

        [Parameter("Standard deviation", Group = "Styles", DefaultValue = true)]
        public bool ShowDeviantion { get; set; }

        [Parameter("Center", Group = "Styles", DefaultValue = true)]
        public bool ShowCenter { get; set; }

        [Parameter("Channel", Group = "Styles", DefaultValue = true)]
        public bool ShowChannel { get; set; }

        #endregion

        #region Indicator Events

        protected override void Initialize()
        {

            Print("{0} : {1}", NAME, VERSION);

        }

        public override void Calculate(int index)
        {

            if (IsLastBar)
                LinearRegressionTo(Bars.ClosePrices);

        }

        #endregion

        #region Private Methods

        private void LinearRegressionTo(DataSeries series)
        {

            double sum_x = 0, sum_x2 = 0, sum_y = 0, sum_xy = 0;

            int start = series.Count - Period;
            int end = series.Count - 1;

            for (int i = start; i <= end; i++)
            {
                sum_x += 1.0 * i;
                sum_x2 += 1.0 * i * i;
                sum_y += series[i];
                sum_xy += series[i] * i;
            }

            double a = (Period * sum_xy - sum_x * sum_y) / (Period * sum_x2 - sum_x * sum_x);
            double b = (sum_y - a * sum_x) / Period;

            double maxDeviation = 0;
            double sumDevation = 0;

            for (int i = start; i <= end; i++)
            {
                double price = a * i + b;
                maxDeviation = Math.Max(Math.Abs(series[i] - price), maxDeviation);
                sumDevation += Math.Pow(series[i] - price, 2.0);
            }

            double stdDeviation = Math.Sqrt(sumDevation / Period);

            end += 20;

            double pr1 = a * start + b;
            double pr2 = a * end + b;

            if (ShowCenter)
            {
                Chart.DrawTrendLine("center", start, pr1, end, pr2, Color.FromName(MyColor), LineThickness, LineStyle.Lines);
            }

            if (ShowChannel)
            {
                Chart.DrawTrendLine("top", start, pr1 + maxDeviation, end, pr2 + maxDeviation, Color.FromName(MyColor), LineThickness, LineStyle.Solid);
                Chart.DrawTrendLine("bottom", start, pr1 - maxDeviation, end, pr2 - maxDeviation, Color.FromName(MyColor), LineThickness, LineStyle.Solid);
            }

            if (ShowDeviantion)
            {
                Chart.DrawTrendLine("dev-top", start, pr1 + stdDeviation, end, pr2 + stdDeviation, Color.FromName(MyColor), LineThickness, LineStyle.DotsVeryRare);
                Chart.DrawTrendLine("dev-bottom", start, pr1 - stdDeviation, end, pr2 - stdDeviation, Color.FromName(MyColor), LineThickness, LineStyle.DotsVeryRare);
            }

        }

        #endregion

    }

}
