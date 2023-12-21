using MathNet.Numerics.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor
{
    public class VehicleSignal
    {   
        public Guid VehicleID { get; set; }  
        public DateTime? TimeStamp { get; set; }
        public double? Speed { get; set; }
        

        public double? Distance { get; set; }
        public double[] Signal { get; set; }
        public double SamplingFrequency { get; set; }
    }
}
