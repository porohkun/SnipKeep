using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SnipKeep
{
    public interface IFilterData
    {
        string Name { get;  }
        ImageSource IconSource { get; }
        int Count { get; }
        ObservableCollection<IFilterData> Items { get; }
    }
}
