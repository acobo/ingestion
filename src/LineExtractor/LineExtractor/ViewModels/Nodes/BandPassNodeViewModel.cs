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

namespace LineExtractor.ViewModels.Nodes
{
    public class BandPassNodeViewModel : NodeViewModel
    {        
        public ValueNodeInputViewModel<double?> FreqMin { get; }
        public ValueNodeInputViewModel<double?> FreqMax { get; }

        public ValueNodeInputViewModel<DasSignal> Input { get; }
        public ValueNodeOutputViewModel<DasSignal> Output { get; }

        static BandPassNodeViewModel()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<BandPassNodeViewModel>));
        }

        public BandPassNodeViewModel()
        {
            Name = "Bandpass Filter";

            Input = new ValueNodeInputViewModel<DasSignal>()
            {
                Name = "Input",
            };
            Inputs.Add(Input);

            Output = new ValueNodeOutputViewModel<DasSignal>()
            {
                Name = "Output",
                Value = null
            };
            Outputs.Add(Output);         
            
            FreqMin = new ValueNodeInputViewModel<double?>()
            {
                Name = "FreqMin (Hz)",
                Editor = new ValueEditorViewModel<double?>()
                {
                    Value = null,                                        
                }
            };
            Inputs.Add(FreqMin);

            FreqMax = new ValueNodeInputViewModel<double?>()
            {
                Name = "FreqMax (Hz)",
                Editor = new ValueEditorViewModel<double?>()
                {
                    Value = null,
                }
            };
            Inputs.Add(FreqMax);
        }
    }
}
