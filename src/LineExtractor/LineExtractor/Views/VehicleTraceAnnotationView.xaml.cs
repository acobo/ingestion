using LineExtractor.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LineExtractor.Views
{
    /// <summary>
    /// Interaction logic for VehicleTraceAnnotationView.xaml
    /// </summary>
    public partial class VehicleTraceAnnotationView : ReactiveUserControl<VehicleTraceAnnotationViewModel>
    {
        public VehicleTraceAnnotationView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                DataContext = ViewModel;
                ViewModel.Start();
            });
        }

        private void ImageGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(this.ImageGrid);
            Debug.WriteLine($"{pos.X} {pos.Y}");
            //Ponemos el frame
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ViewModel.SetVideoFrame((int)pos.X);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                ViewModel.AddPoint((int)pos.X, (int)pos.Y);
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                ViewModel.RemoveLastPoint();
            }
        }

        private void ImageGrid_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(this.ImageGrid);
            ViewModel.CursorPosText = $"l: {(int)pos.X} t: {(int)pos.Y}";
        }
    }
}
