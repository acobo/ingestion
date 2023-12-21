using DynamicData.Binding;
using LineExtractor.Preprocessing;
using LineExtractor.ViewModels;
using MathNet.Numerics.Data.Matlab;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.Data
{
    /// <summary>
    /// Representa a una señal das
    /// </summary>
    public class DasModel : ReactiveObject
    {
        [Reactive] DasSignal Signal { get; set; }

        [Reactive] public IMatrixProcessor Preprocessor { get; set; }
        [Reactive] public IMatrixToBitmapProcessor BitmapProcessor { get; set; }

        public static ObservableCollection<IMatrixProcessor> Processors => _processors;
        public static ObservableCollection<IMatrixToBitmapProcessor> BitmapProcessors => _bitmapProcessors;

        static ObservableCollection<IMatrixProcessor> _processors = new();
        static ObservableCollection<IMatrixToBitmapProcessor> _bitmapProcessors = new();

        static DasModel()
        {
            RegisterProcessors();
        }

        private DasModel()
        {
            //Setup default processors
            Preprocessor = _processors[0];
            BitmapProcessor = _bitmapProcessors[0];
            //
            this.WhenAnyValue(x => x.Preprocessor, x => x.BitmapProcessor).Subscribe(delegate
            {
                Rebuild();
            });

        }

        private void Rebuild()
        {
            throw new NotImplementedException();
        }

        static void RegisterProcessors()
        {
            _processors.Add(new IdentityMatrixProcessor());
            _bitmapProcessors.Add(new DefaultMatrixToBitmapProcessor());
        }

        public static DasModel FromFile(string fn)
        {
            //Leemos el archivo .mat
            if (File.Exists(fn) == false)
                throw new FileNotFoundException("No se encontró el archivo de señal das", fn);
            //check that extension is .mat
            var ext = Path.GetExtension(fn);
            if (ext != ".mat")
                throw new ArgumentException("El archivo de señal das debe tener extensión .mat", nameof(fn));

            var vfn = Path.ChangeExtension(fn, ".mp4");

            //Cargamos fichero .mat
            //var mat = MatlabReader.Read<double>(sfn, "Fp");
            var mat = MatlabReader.ReadAll<double>(fn, "Fp", "muestreo_distancia", "muestreo_tiempo");
            //transpose for easier handling
            var matT = mat["Fp"].Transpose();

            DasSignal dasSignal = new DasSignal()
            {
                FileName = fn,
                SamplingDistance = mat["muestreo_distancia"][0, 0],
                SamplingFrequency = mat["muestreo_tiempo"][0, 0],
                Signal = matT,
                VideoFileName = vfn,
                //Traces = this.Traces
            };

            return new DasModel
            {
                Signal = dasSignal
            };
        }
    }
}
