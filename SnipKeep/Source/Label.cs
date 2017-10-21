using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SnipKeep
{
    public class Label : INotifyPropertyChanged, IComparable, IComparable<Label>
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        #region IComparable, IComparable<Label> Members

        public int CompareTo(object obj)
        {
            return CompareTo((Label)obj);
        }

        public int CompareTo(Label other)
        {
            return Name.CompareTo(other.Name);
        }

        #endregion

        public static ObservableCollection<Label> Labels { get; } = new ObservableCollection<Label>();

        private string _name;
        private bool _selected;
        private readonly List<Snippet> _snippets;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected == value) return;
                _selected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Selected"));
            }
        }
        public int Count => _snippets.Count;

        private Label()
        {
            _snippets = new List<Snippet>();
        }

        internal void AddSnippet(Snippet snip)
        {
            if (_snippets.Contains(snip)) return;
            _snippets.Add(snip);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        internal void RemoveSnippet(Snippet snip)
        {
            if (!_snippets.Contains(snip)) return;
            _snippets.Remove(snip);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        internal static Label GetLabelByName(string tag)
        {
            var label = Labels.FirstOrDefault(l => l.Name == tag);
            if (label != null) return label;
            label = new Label() { Name = tag };
            Labels.Add(label);
            return label;
        }
    }
}
