using MathNet.Numerics.LinearAlgebra;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.Preprocessing
{
    public interface IMatrixToBitmapProcessor
    {
        Mat Process(Matrix<double> input);
    }
}
