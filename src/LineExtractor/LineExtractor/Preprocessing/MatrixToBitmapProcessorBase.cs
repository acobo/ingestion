using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.Preprocessing
{
    public abstract class MatrixToBitmapProcessorBase : IMatrixToBitmapProcessor
    {
        public string Name { get; }

        public MatrixToBitmapProcessorBase(string name)
        {
            Name = name;
        }
        public abstract OpenCvSharp.Mat Process(Matrix<double> input);
    }

    public class TaskMatrixToBitmapProcessor : MatrixToBitmapProcessorBase
    {
        private readonly Func<Matrix<double>, OpenCvSharp.Mat> _func;

        public TaskMatrixToBitmapProcessor(string name, Func<Matrix<double>, OpenCvSharp.Mat> func) : base(name)
        {
            _func = func;
        }

        public override OpenCvSharp.Mat Process(Matrix<double> input)
        {
            return _func(input);
        }
    }

    public class DefaultMatrixToBitmapProcessor : TaskMatrixToBitmapProcessor
    {
        public DefaultMatrixToBitmapProcessor() : base("Default bitmap", DefaultProcess)
        {
        }

        private static OpenCvSharp.Mat DefaultProcess(Matrix<double> input)
        {
            var mat = new OpenCvSharp.Mat(input.RowCount, input.ColumnCount, OpenCvSharp.MatType.CV_8UC1);
            for (int i = 0; i < input.RowCount; i++)
            {
                for (int j = 0; j < input.ColumnCount; j++)
                {
                    mat.Set(i, j, input[i, j]);
                }
            }
            return mat;
        }
    }
}
