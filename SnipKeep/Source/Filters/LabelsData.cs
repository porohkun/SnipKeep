using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SnipKeep
{
    public class LabelsData : IFilterData
    {
        public ImageSource IconSource { get { return null; } }
        private string _name = "Labels";
        public string Name { get { return _name; } }
        public int Count { get { return 0; } }
        private ObservableCollection<IFilterData> _items = new ObservableCollection<IFilterData>();
        public ObservableCollection<IFilterData> Items { get { return _items; } }

    }
}
