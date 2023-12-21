using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.Preprocessing
{
    /// <summary>
    /// Input: señal
    /// Salida: quantizacion en imagen
    /// </summary>
    public abstract class SignalVisualizationFilterStrategy
    {
        public string Name { get; set; }        
        
    }
}
