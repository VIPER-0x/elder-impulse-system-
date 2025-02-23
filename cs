using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class ElderImpulseSystemWithFractals : Indicator
    {
        [Parameter("Elder Impulse", DefaultValue = "On", Group = "Elder Impulse")]
        public string AllowElder { get; set; }

        [Parameter("MACD Fast Length", DefaultValue = 12, MinValue = 1, Group = "Elder Impulse")]
        public int MacdFastLength { get; set; }

        [Parameter("MACD Slow Length", DefaultValue = 26, MinValue = 1, Group = "Elder Impulse")]
        public int MacdSlowLength { get; set; }

        [Parameter("MACD Signal Length", DefaultValue = 9, MinValue = 1, Group = "Elder Impulse")]
        public int MacdSignalLength { get; set; }

        [Parameter("EMA Length", DefaultValue = 13, MinValue = 1, Group = "Elder Impulse")]
        public int EmaLength { get; set; }

        [Parameter("Triple MA1", DefaultValue = 20, MinValue = 1, Group = "Triple MA")]
        public int Ma1Length { get; set; }

        [Parameter("Triple MA2", DefaultValue = 50, MinValue = 1, Group = "Triple MA")]
        public int Ma2Length { get; set; }

        [Parameter("Triple MA3", DefaultValue = 200, MinValue = 1, Group = "Triple MA")]
        public int Ma3Length { get; set; }

        [Parameter("Triple MA Type", DefaultValue = "SMA", Group = "Triple MA")]
        public string MaType { get; set; }

        [Parameter("Fractal Periods", DefaultValue = 5, Group = "Fractal", MinValue = 5)]
        public int FractalPeriods { get; set; }

        private MacdCrossOver _macd;
        private ExponentialMovingAverage _ema;
        private MovingAverage _ma1;
        private MovingAverage _ma2;
        private MovingAverage _ma3;
        private Fractals _fractals;

        protected override void Initialize()
        {
            _macd = Indicators.MacdCrossOver(Bars.ClosePrices, MacdFastLength, MacdSlowLength, MacdSignalLength);
            _ema = Indicators.ExponentialMovingAverage(Bars.ClosePrices, EmaLength);

            if (MaType == "SMA")
            {
                _ma1 = Indicators.SimpleMovingAverage(Bars.ClosePrices, Ma1Length);
                _ma2 = Indicators.SimpleMovingAverage(Bars.ClosePrices, Ma2Length);
                _ma3 = Indicators.SimpleMovingAverage(Bars.ClosePrices, Ma3Length);
            }
            else
            {
                _ma1 = Indicators.ExponentialMovingAverage(Bars.ClosePrices, Ma1Length);
                _ma2 = Indicators.ExponentialMovingAverage(Bars.ClosePrices, Ma2Length);
                _ma3 = Indicators.ExponentialMovingAverage(Bars.ClosePrices, Ma3Length);
            }

            _fractals = Indicators.Fractals(FractalPeriods);
        }

        public override void Calculate(int index)
        {
            double macdHistogram = _macd.Histogram.LastValue;
            double macdHistogramPrev = _macd.Histogram.Last(1);
            bool elderBulls = _ema.Result[index] > _ema.Result[index - 1] && macdHistogram > macdHistogramPrev;
            bool elderBears = _ema.Result[index] < _ema.Result[index - 1] && macdHistogram < macdHistogramPrev;
            Color barColor;

            // Swap the colors for the bars
            if (AllowElder == "On")
            {
                barColor = elderBulls ? Color.Green : elderBears ? Color.Red : Color.Blue;
            }
            else
            {
                barColor = Bars.ClosePrices[index] > Bars.OpenPrices[index] ? Color.Green : Color.Red;
            }

            Chart.SetBarColor(index, barColor);
        }
    }
}
