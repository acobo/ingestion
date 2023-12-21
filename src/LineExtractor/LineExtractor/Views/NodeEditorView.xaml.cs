using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
    /// Interaction logic for NodeEditorView.xaml
    /// </summary>
    public partial class NodeEditorView : ReactiveUI.ReactiveUserControl<LineExtractor.ViewModels.NodeEditorViewModel>
    {
        public NodeEditorView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.NodeList, v => v.nodeList.ViewModel).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Network, v => v.viewHost.ViewModel).DisposeWith(d);
            });
        }
    }
}
