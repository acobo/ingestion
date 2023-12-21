using Accord.Audio;
using LineExtractor.Extractors;
using MathNet.Numerics.Data.Matlab;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ReactiveUI.Fody.Helpers;
using System.IO;
using OpenCvSharp.WpfExtensions;
using System.Collections.ObjectModel;
using ReactiveUI;
using LineExtractor.Data;
using System.Security.Cryptography;
using LineExtractor.Preprocessing;

namespace LineExtractor.ViewModels
{
    //
    //
    public class VehicleTraceAnnotationViewModel : ViewModelBase
    {
        //[Reactive] public ObservableCollection<VehicleTrace> Traces { get; set; } = new ObservableCollection<VehicleTrace>();
        [Reactive] public VehicleTrace? SelectedTrace { get; set; }

        [Reactive] public WriteableBitmap SignalBitmap { get; set; }
        [Reactive] public WriteableBitmap OriginalSignalBitmap { get; set; }
        [Reactive] public WriteableBitmap VideoBitmap { get; set; }

        [Reactive] public WriteableBitmap NotesBitmap { get; set; }

        VideoCapture mVideoCapture;

        [Reactive] public Matrix<double> Signal { get; private set; }
        [Reactive] public Matrix<double> OriginalSignal { get; private set; }
        [Reactive] public int SignalLength { get; private set; }

        Mat VideoFrame { get; set; }
        Mat NotesImage { get; set; }
        Mat SignalImage { get; set; }
        Mat OriginalSignalImage { get; set; }


        [Reactive] public DasSignal CurrentSignal { get; private set; }
        [Reactive] public VehicleTrace? CurrentTrace { get; private set; }

        [Reactive] public string CursorPosText { get; set; }

        [Reactive] public string FilteringStrategy { get; set; }

        [Reactive] public SignalVisualizationFilterStrategy VisualizationFilterStrategy { get; set; }

        public VehicleTraceAnnotationViewModel(MainViewModel root) : base(root)
        {
            this.PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(SelectedTrace):
                        RedrawNotes();
                        break;
                    case nameof(FilteringStrategy):
                        ApplyImageFilter();
                        break;
                }
            };
        }

        public void LoadSignal(string sfn, string vfn)
        {
            //Cargamos fichero .mat
            //var mat = MatlabReader.Read<double>(sfn, "Fp");
            var mat = MatlabReader.ReadAll<double>(sfn, "Fp", "muestreo_distancia", "muestreo_tiempo");
            //transpose for easier handling
            var matT = mat["Fp"].Transpose();
            //
            Signal = matT;
            SignalLength = Signal.ColumnCount;

            OriginalSignal = matT.Clone();
            //Traces = new ObservableCollection<VehicleTrace>();


            DasSignal dasSignal = new DasSignal()
            {
                FileName = sfn,
                SamplingDistance = mat["muestreo_distancia"][0, 0],
                SamplingFrequency = mat["muestreo_tiempo"][0, 0],
                Signal = OriginalSignal,
                VideoFileName = vfn,
                //Traces = this.Traces
            };
            CurrentSignal = dasSignal;

            //load video if file exists
            if (File.Exists(vfn))
            {
                //Cargamos video
                mVideoCapture = new VideoCapture(vfn);
                //Obtenemos el primer frame
                mVideoCapture.PosMsec = 5000;
                var frame = new Mat();
                mVideoCapture.Read(frame);
                //Obtenemos los datos del video
                VideoBitmap = frame.ToWriteableBitmap();
            }

            GenerateBitmapsFromSignal(Signal);
            SetupNotesBitmaps();
            ApplyImageFilter();

            //Miramos a ver si existe un fichero con el mismo nombre pero extension .json
            var jsonFileName = Path.ChangeExtension(sfn, ".json");
            if (File.Exists(jsonFileName))
            {
                LoadTraces(jsonFileName);
            }
        }

        private void LoadTraces(string fileName)
        {
            var json = System.IO.File.ReadAllText(fileName);
            var traces = JsonConvert.DeserializeObject<List<VehicleTrace>>(json);
            CurrentSignal.Traces = new ObservableCollection<VehicleTrace>(traces);
            RedrawNotes();
        }

        public void Start()
        {
            var fn = @"E:\das\2021-11-30_Medidas\medida_211130_11_55_19.mat";
            var vfn = @"E:\das\2021-11-30_Medidas\medida_211130_11_55_19.mp4";

            //new SignalLoaderViewModel(this).LoadSignal(fn);

            LoadSignal(fn, vfn);
            //GenerateBitmapsFromSignal(Signal);
            //SetupNotesBitmaps();

            //ApplyImageFilter();
        }

        void GenerateBitmapsFromSignal(Matrix<double> s)
        {
            //Obtenemos los datos de la señal            
            SignalBitmap = GenerateBitmap8(s);
            OriginalSignalBitmap = GenerateBitmap8(OriginalSignal);

            SignalImage = SignalBitmap.ToMat();
            //SignalBitmap = SignalImage.ToWriteableBitmap();
        }

        void SetupNotesBitmaps()
        {
            //Creamos el bitmap de anotaciones
            NotesImage = new Mat(new Size(SignalBitmap.PixelWidth, SignalBitmap.PixelHeight), MatType.CV_8UC4, new Scalar(0, 0, 0, 0));
            //
            RedrawNotes();
        }

        void RedrawNotes()
        {
            //Clear notes
            NotesImage.SetTo(new Scalar(0, 0, 0, 0));
            //Dibujamos una linea horizontal roja en la posicion 1127
            //NotesImage.Line(new Point(0, 1127), new Point(NotesImage.Width, 1127), new Scalar(0, 0, 255, 255), 3);
            var camPoint = 1075;
            NotesImage.Line(new Point(0, camPoint), new Point(NotesImage.Width, camPoint), new Scalar(0, 0, 255, 255), 3);

            //Dibujamos trazas
            foreach (var trace in CurrentSignal.Traces)
            {
                trace.DrawTrace(NotesImage, new Scalar(255, 0, 0, 255));
            }
            //y la seleccionada
            if (SelectedTrace != null)
            {
                SelectedTrace.DrawTrace(NotesImage, new Scalar(0, 255, 0, 255));
            }

            if (CurrentTrace != null)
            {
                CurrentTrace.DrawTrace(NotesImage, new Scalar(0, 255, 255, 255));
            }

            //mostramos
            NotesBitmap = NotesImage.ToWriteableBitmap();
        }

        void ApplyImageFilter()
        {
            //autolevels
            //SignalBitmap = SignalImage.EqualizeHist().ToWriteableBitmap();
            var umbra = 120;
            //var t = SignalImage.InRange(124, 130);
            var t = SignalImage.MedianBlur(5).InRange(112, 142);

            var t2 = (255 - t);
            t = t2;// SignalImage & t2;

            SignalBitmap = t.ToWriteableBitmap(); //Filtering.ApplySobelFilter(t).ToWriteableBitmap();
        }

        void ApplySecondFilter()
        {   
            //
        }

        public void ApplyThreshold(Matrix<double> mat)
        {
            //threshold
            mat = mat.Map(v => v > 0.5 ? 1.0 : 0.0);
        }

        //Generate image from matrix
        public WriteableBitmap GenerateBitmap16(Matrix<double> mat)
        {
            //generate 16bit grayscale bitmap
            var bmp = new WriteableBitmap(mat.ColumnCount, mat.RowCount, 96, 96, PixelFormats.Gray16, null);
            //autoscale mat to 16bit grayscale
            var min = mat.Enumerate().Min();
            var max = mat.Enumerate().Max();
            var range = max - min;
            var mat2 = mat.Subtract(min).Divide(range);
            //copy mat2 to bmp
            var pixels = new ushort[mat2.ColumnCount * mat2.RowCount];
            for (int i = 0; i < mat2.RowCount; i++)
            {
                for (int j = 0; j < mat2.ColumnCount; j++)
                {
                    var v = mat2[i, j];
                    var v2 = (ushort)(v * ushort.MaxValue);
                    pixels[i * mat2.ColumnCount + j] = v2;
                }
            }
            bmp.WritePixels(new System.Windows.Int32Rect(0, 0, mat2.ColumnCount, mat2.RowCount), pixels, mat2.ColumnCount * sizeof(ushort), 0);
            return bmp;

            ////write bitmap as png
            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bmp));
            //using (var stream = System.IO.File.Create(@"c:\temp\das.png"))
            //{
            //    encoder.Save(stream);
            //}
        }

        public WriteableBitmap GenerateBitmap8(Matrix<double> mat)
        {
            //generate 8bit grayscale bitmap
            var bmp = new WriteableBitmap(mat.ColumnCount, mat.RowCount, 96, 96, PixelFormats.Gray8, null);
            //autoscale mat to 8bit grayscale
            var min = mat.Enumerate().Min();
            var max = mat.Enumerate().Max();
            var range = max - min;
            var mat2 = mat.Subtract(min).Divide(range);

            //copy mat2 to bmp
            var pixels = new byte[mat2.ColumnCount * mat2.RowCount];
            for (int i = 0; i < mat2.RowCount; i++)
            {
                for (int j = 0; j < mat2.ColumnCount; j++)
                {
                    var v = mat2[i, j];
                    var v2 = (byte)(v * byte.MaxValue);
                    pixels[i * mat2.ColumnCount + j] = v2;
                }
            }
            bmp.WritePixels(new System.Windows.Int32Rect(0, 0, mat2.ColumnCount, mat2.RowCount), pixels, mat2.ColumnCount * sizeof(byte), 0);
            return bmp;
            ////write bitmap as png
            //PngBitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bmp));
            //using (var stream = System.IO.File.Create(@"c:\temp\das.png"))
            //{
            //    encoder.Save(stream);
            //}
        }

        public void SetVideoFrame(int signalSample, double frequency = 200.0)
        {
            //compute msg
            var posMsg = (int)(signalSample / frequency * 1000.0);
            //set video frame
            mVideoCapture.PosMsec = posMsg;
            var frame = new Mat();
            mVideoCapture.Read(frame);
            VideoBitmap = frame.ToWriteableBitmap();
        }

        public void AddPoint(int n, int l)
        {
            if (CurrentTrace == null)
                CurrentTrace = new VehicleTrace() { Signal = OriginalSignal };

            CurrentTrace.AddPoint(new Point(n, l));

            RedrawNotes();
        }

        public void RemoveLastPoint()
        {
            if (CurrentTrace == null)
                return;
            CurrentTrace.RemoveLastPoint();
            RedrawNotes();
        }

        //public void SetVideoFrame(int frame)
        //{
        //    mVideoCapture.PosFrames = frame;
        //    var mat = new Mat();
        //    mVideoCapture.Read(mat);
        //    VideoBitmap = mat.ToWriteableBitmap();
        //}

        public RelayCommand NewTraceCommand => new RelayCommand(p =>
        {
            VehicleTrace trace = new VehicleTrace() { Signal = OriginalSignal };
            CurrentTrace = trace;
            RedrawNotes();
        });

        public void ExtractSignals(string fn)
        {
            List<VehicleSignal> signals = new List<VehicleSignal>();
            //Para cada traza
            foreach (var trace in CurrentSignal.Traces)
            {
                //extraemos las señales
                //var traceSignals = new BasicSignalExtractor().ExtractSignal(CurrentSignal.Signal, trace).Cast<VehicleSignal>().ToList();
                var traceSignals = new EnergySignalExtractor(0.005).ExtractSignal(CurrentSignal.Signal, trace).Cast<VehicleSignal>().ToList();
                //guardamos las señales
                signals.AddRange(traceSignals);
            }
            //guardamos las señales en JSON
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(signals);
            System.IO.File.WriteAllText(fn, json);
        }

        public RelayCommand ExtractSignalsCommand => new RelayCommand(p =>
        {
            //chose a s.json file to save
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".json";
            dlg.Filter = "JSON Files (*.json)|*.json";
            var result = dlg.ShowDialog();
            if (result == true)
            {
                var file = dlg.FileName;
                //extract signals
                ExtractSignals(file);
            }
            return;



            //extract signals
            if (Signal != null && CurrentTrace != null)
            {
                var signals = new BasicSignalExtractor().ExtractSignal(OriginalSignal, CurrentTrace);
                //Guardamos las señales
                var sf = StorageManager.GetStoragePath();
                //creamos el directorio
                var dir = System.IO.Path.Combine(sf, "signals");
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                //guardamos las señales en JSON con el nombre del archivo el vehicle id
                var json = JsonConvert.SerializeObject(signals);
                var file = System.IO.Path.Combine(dir, CurrentTrace.VehicleID + ".json");
                System.IO.File.WriteAllText(file, json);
            }
        });

        public RelayCommand AssignVehicleToTraceCommand => new RelayCommand(p =>
        {
            if (VideoBitmap != null && CurrentTrace != null)
            {
                CurrentTrace.VehicleBitmap = VideoBitmap;
            }
        });



        public RelayCommand OpenTracesCommand => new RelayCommand(p =>
        {
            //vamos a proceder a abrir un json con las trazas
            var ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "JSON files (*.json)|*.json";
            if (ofd.ShowDialog() == true)
            {
                LoadTraces(ofd.FileName);
            }
        });



        public RelayCommand SaveTracesCommand => new RelayCommand(p =>
        {
            //vamos a proceder a guardar un json con las trazas
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                RestoreDirectory = true,
                Filter = "JSON files (*.json)|*.json"

            };
            if (sfd.ShowDialog() == true)
            {
                var json = JsonConvert.SerializeObject(CurrentSignal.Traces);
                System.IO.File.WriteAllText(sfd.FileName, json);
            }
        });

        public RelayCommand AddTraceCommand => new RelayCommand(p =>
        {
            //Añadimos current trace a la lista actual y la seleccionamos
            if (CurrentTrace != null)
            {
                //solo la añadimos si tiene al menos 2 puntos
                if (CurrentTrace.PointsCount < 2)
                    return;

                CurrentSignal.Traces.Add(CurrentTrace);
                SelectedTrace = CurrentTrace;
                CurrentTrace = null;
            }
        });

        /// <summary>
        /// borra la traza seleccionada
        /// </summary>
        public RelayCommand DeleteTraceCommand => new RelayCommand(p =>
        {
            if (SelectedTrace != null)
            {
                CurrentSignal.Traces.Remove(SelectedTrace);
                CurrentTrace = null;
            }
        });

        public RelayCommand OpenSignalCommand => new RelayCommand(p =>
        {
            //abrimos el archivo de señal .mat
            var ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "MAT files (*.mat)|*.mat";
            if (ofd.ShowDialog() == true)
            {
                //el fichero de video seria igual pero acabado en mp4
                var videoFile = System.IO.Path.ChangeExtension(ofd.FileName, "mp4");
                //cargamos la señal
                LoadSignal(ofd.FileName, videoFile);
            }
        });

    }
}

