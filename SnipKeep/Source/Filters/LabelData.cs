using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SnipKeep
{
    public class LabelData : IFilterData
    {
        private ImageSource _iconSource;
        public ImageSource IconSource { get { if (_iconSource == null) _iconSource = Icons.Sources["label.png"]; return _iconSource; } }
        public string Name { get; set; }
        public int Count { get { return 3; } }
        public ObservableCollection<IFilterData> Items { get { return null; } }

    }
}
