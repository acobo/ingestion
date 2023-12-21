using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.Helpers
{
    public class FilterHelpers
    {
        //Create a mathnet bandpass filter
        public static MathNet.Filtering.IOnlineFilter CreateBandPassFilter(double lowFreq, double highFreq, double sampleRate)
        {
            var low = lowFreq / sampleRate;
            var high = highFreq / sampleRate;
            var bandPass = MathNet.Filtering.OnlineFilter.CreateBandpass(MathNet.Filtering.ImpulseResponse.Finite, low, high, 0.1);
            return bandPass;
        }
    }
}
