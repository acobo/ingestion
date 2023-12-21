using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.Helpers
{
    public static class MatrixExtensions
    {
        public static double Energy(this Matrix<double> signal, int start, int end)
        {
            double energy = 0;

            for(int i = start; i < end; i++)
            {
                var value = signal[0, i];
                energy += value * value;
            }   
            return energy;
        }
    }
}
