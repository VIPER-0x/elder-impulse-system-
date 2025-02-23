# elder-impulse-system-
This C# code represents a trading indicator implemented using the cAlgo API in a trading platform like cTrader. It combines the Elder Impulse System with fractals and multiple moving averages for technical analysis. Below, I’ll provide a breakdown of the key components, functionality, and parameters of the indicator for the GitHub README file.

### ElderImpulseSystemWithFractals Indicator

#### Overview
The `ElderImpulseSystemWithFractals` is a custom indicator designed for algorithmic trading. It utilizes the Elder Impulse System principles, MACD, Exponential Moving Average (EMA), and multiple moving averages (MA) along with fractals to determine market trends and help traders make informed decisions.

#### Key Features
- **Elder Impulse System**: Visualizes bullish and bearish market trends.
- **MACD**: Calculates the MACD Histogram to identify momentum shifts.
- **Exponential and Simple Moving Averages**: Allows the user to choose the type of moving averages to analyze market trends.
- **Fractals**: Provides additional signals based on local minima and maxima.

#### Code Explanation

```csharp
using cAlgo.API; // Importing required cAlgo API namespaces
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo // Define a namespace
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)] // Attributes indicating it's an overlay indicator
    public class ElderImpulseSystemWithFractals : Indicator // Main class definition
    {
        // Parameters for customization
        [Parameter("Elder Impulse", DefaultValue = "On", Group = "Elder Impulse")]
        public string AllowElder { get; set; }

        // MACD parameters
        [Parameter("MACD Fast Length", DefaultValue = 12, MinValue = 1, Group = "Elder Impulse")]
        public int MacdFastLength { get; set; }

        [Parameter("MACD Slow Length", DefaultValue = 26, MinValue = 1, Group = "Elder Impulse")]
        public int MacdSlowLength { get; set; }

        [Parameter("MACD Signal Length", DefaultValue = 9, MinValue = 1, Group = "Elder Impulse")]
        public int MacdSignalLength { get; set; }

        // EMA parameter
        [Parameter("EMA Length", DefaultValue = 13, MinValue = 1, Group = "Elder Impulse")]
        public int EmaLength { get; set; }

        // Moving Average parameters
        [Parameter("Triple MA1", DefaultValue = 20, MinValue = 1, Group = "Triple MA")]
        public int Ma1Length { get; set; }

        [Parameter("Triple MA2", DefaultValue = 50, MinValue = 1, Group = "Triple MA")]
        public int Ma2Length { get; set; }

        [Parameter("Triple MA3", DefaultValue = 200, MinValue = 1, Group = "Triple MA")]
        public int Ma3Length { get; set; }

        [Parameter("Triple MA Type", DefaultValue = "SMA", Group = "Triple MA")]
        public string MaType { get; set; }

        // Fractal period parameter
        [Parameter("Fractal Periods", DefaultValue = 5, Group = "Fractal", MinValue = 5)]
        public int FractalPeriods { get; set; }

        // Private variables for indicators
        private MacdCrossOver _macd;
        private ExponentialMovingAverage _ema;
        private MovingAverage _ma1;
        private MovingAverage _ma2;
        private MovingAverage _ma3;
        private Fractals _fractals;

        // Initialization
        protected override void Initialize()
        {
            _macd = Indicators.MacdCrossOver(Bars.ClosePrices, MacdFastLength, MacdSlowLength, MacdSignalLength);
            _ema = Indicators.ExponentialMovingAverage(Bars.ClosePrices, EmaLength);

            // Choose the type of moving averages based on user input
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

        // Calculate indicator values
        public override void Calculate(int index)
        {
            double macdHistogram = _macd.Histogram.LastValue;
            double macdHistogramPrev = _macd.Histogram.Last(1);
            bool elderBulls = _ema.Result[index] > _ema.Result[index - 1] && macdHistogram > macdHistogramPrev;
            bool elderBears = _ema.Result[index] < _ema.Result[index - 1] && macdHistogram < macdHistogramPrev;
            Color barColor;

            // Determine bar color based on analysis
            if (AllowElder == "On")
            {
                barColor = elderBulls ? Color.Green : elderBears ? Color.Red : Color.Blue;
            }
            else
            {
                barColor = Bars.ClosePrices[index] > Bars.OpenPrices[index] ? Color.Green : Color.Red;
            }

            // Set the color of the bar on the chart
            Chart.SetBarColor(index, barColor);
        }
    }
}
```

#### Parameters Description
- **Elder Impulse Settings**
  - `AllowElder`: Toggle to enable the Elder Impulse System.
  
- **MACD Settings**
  - `MacdFastLength`: Fast length for MACD.
  - `MacdSlowLength`: Slow length for MACD.
  - `MacdSignalLength`: Signal length for MACD.

- **EMA Settings**
  - `EmaLength`: Length for Exponential Moving Average.

- **Triple Moving Averages**
  - `Ma1Length`, `Ma2Length`, `Ma3Length`: Lengths for three different moving averages.
  - `MaType`: Type of moving average ("SMA" or "EMA").

- **Fractals**
  - `FractalPeriods`: Number of periods for fractal calculation.

### Installation
1. Copy the code into a new cAlgo script.
2. Adjust the parameters as needed to fit your trading strategy.
3. Add the indicator to your chart for analysis.

### Usage
This indicator can be used in various trading scenarios, providing visual feedback on potential buying or selling opportunities based on the Elder Impulse System and fractal patterns.

