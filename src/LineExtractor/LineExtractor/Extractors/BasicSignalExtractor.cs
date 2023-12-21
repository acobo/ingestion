using LineExtractor.Data;
using MathNet.Numerics.LinearAlgebra;
using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace LineExtractor.Extractors
{
    public class BasicSignalExtractor : ISignalExtractor
    {
        public int Spread = 100;

        public IEnumerable<object> ExtractSignal(Matrix<double> signal, VehicleTrace trace)
        {
            var minX = trace.MinX;
            var maxX = trace.MaxX;

            var signalLength = maxX - minX;
            var signalMatrix = Matrix<double>.Build.Dense(signalLength, signal.ColumnCount);

            List<VehicleSignal> signals = new();

            //recorremos la traza interpolada
            foreach (var p in trace.InterpolatedPoints())
            {
                var fromX = p.X - Spread;
                if (fromX < 0)
                    continue;
                var toX = p.X + Spread;
                if (toX >= signal.ColumnCount)
                    continue;

                var columnCount = toX - fromX ;

                //obtenemos un slice de la señal en y
                var slice = signal.SubMatrix((int)p.Y, 1, fromX, columnCount);
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