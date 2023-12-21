using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor
{
    internal static class Filtering
    {
        public static Mat ApplySobelFilter(Mat input)
        {
            var output = new Mat();
            Cv2.Sobel(input, output, MatType.CV_8UC1, 1, 0);
            return output;
        }
    }
}
