using LineExtractor.Data;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.Extractors
{
    /// <summary>
    /// permite extraer un path de señal de cada traza
    /// </summary>
    public class QuadSignalExtractor : ISignalExtractor
    {
        public IEnumerable<object> ExtractSignal(Matrix<double> signal, VehicleTrace trace)
        {
            //extraemos un quad (un cuadradito) de cada traza
            throw new NotImplementedException();
        }
    }
}
