using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineExtractor.ViewModels
{
    public class ViewModelBase: ReactiveObject
    {
        public MainViewModel Root { get; }

        public ViewModelBase(MainViewModel root)
        {
            Root = root;
        }
    }
}
