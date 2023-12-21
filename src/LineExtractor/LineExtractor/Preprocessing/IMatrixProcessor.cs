using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.Preprocessing
{
    public interface IMatrixProcessor
    {
        Matrix<double> Process(Matrix<double> input);
    }
}
