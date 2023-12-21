using LineExtractor.Data;
using LineExtractor.Helpers;
using MathNet.Numerics.LinearAlgebra;
using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace LineExtractor.Extractors
{
    public class EnergySignalExtractor : ISignalExtractor
    {
        public double SamplingPeriod { get; }
        public double SearchRadiusSg = 2; //el radio de busqueda en segundos
        public double SpreadSg = 2.0; // el spread en segundos

        public EnergySignalExtractor(double samplingPeriod = 0.05)
        {
            SamplingPeriod = samplingPeriod;
        }

        public IEnumerable<object> ExtractSignal(Matrix<double> signal, VehicleTrace trace)
        {
            var minX = trace.MinX;
            var maxX = trace.MaxX;

            var signalLength = maxX - minX;
            var signalMatrix = Matrix<double>.Build.Dense(signalLength, signal.ColumnCount);

            List<VehicleSignal> signals = new();

            var signalSize = (int)Math.Round(SpreadSg / SamplingPeriod);
            var signalHalfSize = signalSize / 2;
            var searchRadius = (int)Math.Round(SearchRadiusSg / SamplingPeriod);

            //recorremos la traza interpolada
            foreach (var p in trace.InterpolatedPoints())
            {
                //Vamos a buscar el spread con el maximo de energia
                var fromXSearch = p.X - searchRadius;
                if (fromXSearch < 0) continue;

                var displacementMax = 2 * searchRadius;

                var toXSearch = fromXSearch + displacementMax;
                if ((toXSearch + signalSize) >= signal.ColumnCount)
                    continue;

                var indexSelected = fromXSearch;
                var maxEnergy = double.MinValue;
                var maxEnergyIndex = toXSearch;

                //Vamos calculando la energia a lo largo de la ventana de desplazamiento
                while (indexSelected <= toXSearch)
                {
                    var fromX = indexSelected;
                    var toX = fromX + signalSize;

                    var energy = signal.Energy(fromX, toX);
                    if (energy > maxEnergy)
                    {
                        maxEnergy = energy;
                        maxEnergyIndex = indexSelected;
                    }
                    indexSelected++;
                }
                //obtenemos un slice de la señal en y
                var slice = signal.SubMatrix((int)p.Y, 1, maxEnergyIndex, signalSize);
                //convert to array
                var sliceArray = slice.ToRowMajorArray();

                VehicleSignal v = new VehicleSignal
                {
                    VehicleID = trace.VehicleID,
                    Signal = sliceArray,
                    SamplingFrequency = 200,
                    Distance = p.Y,
                };

                signals.Add(v);
            }
            return signals;
        }
    }
}