using MathNet.Numerics.Data.Matlab;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LineExtractor.Data
{
    /// <summary>
    /// Señál original das
    /// </summary>
    public class DasSignal: ReactiveObject
    {
        public double SamplingDistance { get; set; } = 1;
        public double SamplingFrequency { get; set; } = 0.05;
        public string FileName { get; set; }
        /// <summary>
        /// Fichero de video
        /// </summary>
        public string VideoFileName { get; set; }
        /// <summary>
        /// Punto de medida en la fibra para la cual el video está centrado
        /// </summary>
        public double VideoFrameFiberLength { get; set; }


        [JsonIgnore]
        [Reactive] public Matrix<double> Signal { get; set; }

        /// <summary>
        /// Cada traza corresponde a un vehiculo
        /// </summary>
        [Reactive] public ObservableCollection<VehicleTrace> Traces { get; set; } = new();

        public static DasSignal FromFile(string fn)
        {
            var vfn = Path.ChangeExtension(fn, ".mp4");
            //Cargamos fichero .mat
            //var mat = MatlabReader.Read<double>(sfn, "Fp");
            var mat = MatlabReader.ReadAll<double>(fn, "Fp", "muestreo_distancia", "muestreo_tiempo");
            //transpose for easier handling
            var matT = mat["Fp"].Transpose();

            DasSignal dasSignal = new DasSignal()
            {
                FileName = fn,
                SamplingDistance = mat["muestreo_distancia"][0, 0],
                SamplingFrequency = mat["muestreo_tiempo"][0, 0],
                Signal = matT,
                VideoFileName = vfn,
                //Traces = this.Traces
            };
            return dasSignal;
        }
    }
}
