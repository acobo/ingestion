using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LineExtractor
{
    public class Vehicle
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Plate { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }        
    }
}
