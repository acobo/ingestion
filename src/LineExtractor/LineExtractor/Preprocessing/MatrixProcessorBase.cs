using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.Preprocessing
{
    public abstract class MatrixProcessorBase : IMatrixProcessor
    {
        public string Name { get; }

        public MatrixProcessorBase(string name)
        {
            Name = name;
        }

        public abstract Matrix<double> Process(Matrix<double> input);
    }

    public class IdentityMatrixProcessor: MatrixProcessorBase
    {
        public IdentityMatrixProcessor() : base("Identity")
        {
        }

        public override Matrix<double> Process(Matrix<double> input)
        {
            return input;
        }
    }
}
