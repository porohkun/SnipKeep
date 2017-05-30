using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SnipKeep
{
    public class Snippet: INotifyPropertyChanged, IComparable, IComparable<Snippet>
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private bool _saved = false;
        public bool Saved { get { return _saved; } private set { _saved = value; } }

        public string _name;
        public string _description;
        public string _oldFilename;
        public string _filename;
        public string _text;
        private List<LabelData> _tags = new List<LabelData>();

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    Saved = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    Saved = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                }
            }
        }
        public string Filename
        {
            get { return _filename; }
            set
            {
                if (_filename != value)
                {
                    if (_oldFilename == "")
                        _oldFilename = _filename;
                    _filename = value;
                    Saved = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filename"));
                }
            }
        }
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    Saved = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
                }
            }
        }
        public IEnumerable<LabelData> Tags { get { return _tags; } }
        public string TagsString
        {
            get
            {
                var sb = new StringBuilder();
                for (int i = 0; i < _tags.Count; i++)
                {
                    sb.Append(_tags[i].Name);
                    if (i < _tags.Count - 1)
                        sb.Append(", ");
                }
                return sb.ToString();
            }
        }

        public Library Library { get; private set; }

        public Snippet(Library lib)
        {
            Library = lib;
        }

        public bool AddTag(LabelData tag)
        {
            if (_tags.Contains(tag)) return false;
            _tags.Add(tag);
            _tags.Sort();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TagsString"));
            return true;
        }

        public bool RemoveTag(LabelData tag)
        {
            if (!_tags.Contains(tag)) return false;
            _tags.Remove(tag);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TagsString"));
            return true;
        }

        public void Save()
        {
            if (Filename == null || Filename == "")
            {
                Name = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                Filename = Name + ".cs";
            }
        }

        public int CompareTo(object obj)
        {
            return CompareTo((Snippet)obj);
        }

        public int CompareTo(Snippet other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
