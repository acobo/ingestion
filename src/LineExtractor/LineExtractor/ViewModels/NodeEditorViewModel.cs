using LineExtractor.ViewModels.Nodes;
using NodeNetwork.Toolkit.NodeList;
using NodeNetwork.ViewModels;
using NodeNetwork;
using NodeNetwork.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using DynamicData.Binding;
using DynamicData.Alias;

namespace LineExtractor.ViewModels
{
    public class NodeEditorViewModel : ViewModelBase
    {
        public NodeListViewModel NodeList { get; } = new NodeListViewModel();
        public NetworkViewModel Network { get; } = new NetworkViewModel();


        public NodeEditorViewModel(MainViewModel root) : base(root)
        {
            NodeList.AddNodeType(()=> new BandPassNodeViewModel());
            NodeList.AddNodeType(() => new OutputNodeViewModel());
            NodeList.AddNodeType(() => new DasSignalNodeViewModel());

            Network.Validator = network =>
            {
                bool containsLoops = GraphAlgorithms.FindLoops(network).Any();
                if (containsLoops)
                {
                    return new NetworkValidationResult(false, false, new ErrorMessageViewModel("Network contains loops!"));
                }

                bool nullInput = GraphAlgorithms.FindStartingNodes(Network)
                    .OfType<DasSignalNodeViewModel>()
                    .Any(n => n.Output.Value == null);
                if (nullInput)
                {
                    return new NetworkValidationResult(false, false, new ErrorMessageViewModel("Network contains null input!"));
                }
                

                //bool containsDivisionByZero = GraphAlgorithms.GetConnectedNodesBubbling(output)
                //    .OfType<DivisionNodeViewModel>()
                //    .Any(n => n.Input2.Value == 0);
                //if (containsDivisionByZero)
                //{
                //    return new NetworkValidationResult(false, true, new ErrorMessageViewModel("Network contains division by zero!"));
                //}
                
                return new NetworkValidationResult(true, true, null);
            };
        }
    }
}
