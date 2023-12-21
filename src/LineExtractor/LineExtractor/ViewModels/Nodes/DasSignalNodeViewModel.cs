using LineExtractor.Data;
using NodeNetwork.Toolkit.ValueNode;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;

namespace LineExtractor.ViewModels.Nodes
{
    /// <summary>
    /// Permite cargar una señal das
    /// </summary>
    public class DasSignalNodeViewModel: NodeViewModel
    {
        public ValueNodeInputViewModel<object> SignalInput { get; } = new ValueNodeInputViewModel<object>();

        public ValueNodeOutputViewModel<DasSignal> Output { get; }

        static DasSignalNodeViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<DasSignalNodeViewModel>));
        }

        public DasSignalNodeViewModel()
        {
            Name = "DasSignal";

            Output = new ValueNodeOutputViewModel<DasSignal>()
            {
                Name = "Output",
                Value = null
            };
            Outputs.Add(Output);
            
            //Output.Value = this.WhenAnyValue(vm => vm.Output.Value)
            //    .Select(value => value);
        }
    }
}
