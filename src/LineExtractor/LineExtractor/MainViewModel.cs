using LineExtractor.Extractors;
using LineExtractor.ViewModels;
using MathNet.Numerics.Data.Matlab;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using Numpy;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using PandasNet;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using static Tensorflow.CollectionDef.Types;

namespace LineExtractor
{
    public class MainViewModel : ReactiveObject
    {
        public MainWindow Window { get; }
        public StorageManager StorageManager { get; } = new StorageManager();

        [Reactive] public ViewModelBase CurrentView { get; set; }

        public MainViewModel(MainWindow window)
        {
            Window = window;

            //CurrentView = new VehicleTraceAnnotationViewModel(this);           

        }

        internal void Start()
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(System.Reflection.Assembly.GetExecutingAssembly());

            CurrentView = new NodeEditorViewModel(this);
        }
    }
}
