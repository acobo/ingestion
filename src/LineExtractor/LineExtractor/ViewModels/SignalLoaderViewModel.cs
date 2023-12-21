using Accord.Audio;
using Accord.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.ViewModels
{
    /// <summary>
    /// Carga la señal y la preprocesa
    /// </summary>
    public class SignalLoaderViewModel: ViewModelBase
    {
        public SignalLoaderViewModel(MainViewModel root) : base(root)
        {
        }

        public void LoadSignal(string fn)
        {
            //Carga la señal en accord aplica un bandpass filter de 0.1 a 2Hz y luego hace un downsample a 10Hz
            //read mat matlab file
            using var reader = new MatReader(fn, false, false);

            var f = 1 / (reader["muestreo_tiempo"].Value as double[,])[0,0];
            var signal = reader["Fp"].Value as double[,];

            //convert double[,] to double[][]
            var signal2 = new double[signal.GetLength(0)][];
            for (int i = 0; i < signal.GetLength(0); i++)
            {
                signal2[i] = new double[signal.GetLength(1)];
                for (int j = 0; j < signal.GetLength(1); j++)
                {
                    signal2[i][j] = signal[i, j];
                }
            }
        }
    }
}
