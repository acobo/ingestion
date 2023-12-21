using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LineExtractor.Data
{
    public class VehicleTrace
    {
        [JsonIgnore]
        public Matrix<double> Signal { get; set; }
        public List<Point> Points { get; set; } = new();
        [JsonIgnore]
        public WriteableBitmap VehicleBitmap { get; internal set; }

        public int MinX => Points.Count > 0 ? Points[0].X : 0;
        public int MaxX => Points.Count > 0 ? Points[^1].X : 0;

        public Guid VehicleID { get; set; } = Guid.NewGuid();

        public int PointsCount => Points.Count;

        public void DrawTrace(Mat bitmap, Scalar color)
        {
            if (Points.Count < 2)
                return;
            //Sort points by x
            var sorted = Points.OrderBy(p => p.X).ToList();

            //Spline interpolation
            var interpol = MathNet.Numerics.Interpolate.CubicSpline(sorted.Select(p => (double)p.X).ToArray(), sorted.Select(p => (double)p.Y).ToArray());

            var minX = sorted[0].X;
            var maxX = sorted[^1].X;

            for (int x = minX; x < maxX; x++)
            {
                var y = (int)interpol.Interpolate(x);
                if (y < 0) continue;
                if (y >= bitmap.Rows) continue;

                var p = new Point(x, y);
                bitmap.Line(p, p, color, 5);
                //bitmap.Set(y, x, new Scalar(255, 255, 0, 255));
            }
        }

        //public Point[] GetPoints()
        //{
        //    var interpol = MathNet.Numerics.Interpolate.CubicSpline(Points.Select(p => (double)p.X).ToArray(), Points.Select(p => (double)p.Y).ToArray());
        //    return null;
        //}

        /// <summary>
        /// Implementamos ienumerable para poder usar el foreach
        /// </summary>
        /// <param name="p"></param>
        /// 
        public IEnumerable<Point> InterpolatedPoints()
        {
            var interpol = MathNet.Numerics.Interpolate.CubicSpline(Points.Select(p => (double)p.X).ToArray(), Points.Select(p => (double)p.Y).ToArray());
            var lastY = 0;
            for (int x = MinX; x < MaxX; x++)
            {
                //solo queremos uno por cambio de y
                var y = (int)Math.Round(interpol.Interpolate(x));
                if (y < 0) continue;
                //if (y >= bitmap) continue;                
                if (y == lastY) continue;
                lastY = y;

                yield return new Point(x, y);
            }            
        }

        public void AddPoint(Point p)
        {
            Points.Add(p);
            Points.Sort((p1, p2) => p1.X.CompareTo(p2.X));
        }

        public void RemoveLastPoint()
        {
            if (Points.Count > 0)
                Points.RemoveAt(Points.Count - 1);
        }
    }
}
