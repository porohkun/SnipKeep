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

        private static ObservableCollection<Label> _labels = new ObservableCollection<Label>();
        public static ObservableCollection<Label> Labels { get { return _labels; } }

        private string _name;
        private List<Snippet> _snippets;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
        public int Count { get { return _snippets.Count; } }

        public Label()
        {
            _snippets = new List<Snippet>();
        }
        
        internal void AddSnippet(Snippet snip)
        {
            if (!_snippets.Contains(snip))
            {
                _snippets.Add(snip);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
        }

        internal void RemoveSnippet(Snippet snip)
        {
            if (_snippets.Contains(snip))
            {
                _snippets.Remove(snip);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
        }

    }
}
