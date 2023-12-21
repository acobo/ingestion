using LineExtractor.Data;
using MathNet.Numerics.LinearAlgebra;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor
{
    public interface ISignalExtractor
    {
        /// <summary>
        /// Extrae la señal de un vehiculo a partir de la señal original y la traza del vehiculo
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="trace"></param>
        /// <param name="bitmap"></param>
        IEnumerable<object> ExtractSignal(Matrix<double> signal, VehicleTrace trace);
    }
}
