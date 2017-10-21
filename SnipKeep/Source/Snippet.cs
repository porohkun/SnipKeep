using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using PNetJson;
using System.Collections.ObjectModel;

namespace SnipKeep
{
    public class Snippet : INotifyPropertyChanged, IComparable, IComparable<Snippet>
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        #region IComparable, IComparable<Snippet> Members

        public int CompareTo(object obj)
        {
            return CompareTo((Snippet)obj);
        }

        public int CompareTo(Snippet other)
        {
            return Name.CompareTo(other.Name);
        }

        #endregion

        public static ObservableCollection<Snippet> Snippets { get; } = new ObservableCollection<Snippet>();

        public bool Saved { get; private set; }

        private string _name = "";
        private string _description = "";
        //private string _oldFilename;
        private string _filename;
        private string _path => Path.Combine(Library.SnippetsPath, Filename);
        private string _metaPath => Path.Combine(Library.SnippetsPath, Path.GetFileNameWithoutExtension(Filename) + ".meta");
        private string _text = "";
        private List<Label> _tags = new List<Label>();

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                Saved = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                if (_description == value) return;
                _description = value;
                Saved = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
            }
        }

        public string Filename
        {
            get => _filename;
            //set
            //{
            //    if (_filename != value)
            //    {
            //        if (_oldFilename == "")
            //            _oldFilename = _filename;
            //        _filename = value;
            //        Saved = false;
            //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filename"));
            //    }
            //}
        }
        public string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                Saved = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
            }
        }
        public IEnumerable<Label> Tags => _tags;

        public string TagsString
        {
            get
            {
                var sb = new StringBuilder();
                for (var i = 0; i < _tags.Count; i++)
                {
                    sb.Append(_tags[i].Name);
                    if (i < _tags.Count - 1)
                        sb.Append(", ");
                }
                return sb.ToString();
            }
            set
            {
                if (value == TagsString) return;
                var line = value.Replace(" ", "").Split(',').ToArray();
                _tags.Clear();
                _tags.AddRange(Label.Labels.Where(l => line.Contains(l.Name)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TagsString"));
            }
        }

        public Library Library { get; private set; }

        internal Snippet(Library lib)
        {
            Library = lib;
            RegenFilename();
        }

        public Snippet(Library lib, string filename) : this(lib)
        {
            _filename = filename;
            UpdateFromSaved(true);
        }

        internal void UpdateFromSaved(bool load = false)
        {
            Text = File.ReadAllText(_path);
            var json = JSONValue.Load(_metaPath);
            _name = json["name"];
            Description = json["description"];
            foreach (var tagname in json["tags"])
                AddTag(Label.GetLabelByName(tagname), true);
            if (!load)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tags"));
        }

        internal void Delete()
        {
            if (File.Exists(_path))
                File.Delete(_path);
            if (File.Exists(_metaPath))
                File.Delete(_metaPath);
        }

        internal void RegenFilename()
        {
            _filename = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".cs";
        }

        public bool AddTag(Label tag, bool silent = false)
        {
            if (_tags.Contains(tag)) return false;
            _tags.Add(tag);
            _tags.Sort();
            tag.AddSnippet(this);
            if (!silent)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tags"));
            return true;
        }

        public bool RemoveTag(Label tag)
        {
            if (!_tags.Contains(tag)) return false;
            _tags.Remove(tag);
            tag.RemoveSnippet(this);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tags"));
            return true;
        }

        public void Save()
        {
            if (Saved) return;
            File.WriteAllText(_path, Text);
            JSONValue json = new JSONObject(
                new JOPair("name", Name),
                new JOPair("tags", new JSONArray(Tags.Select(t => (JSONValue)t.Name))),
                new JOPair("description", Description)
            );
            json.Save(_metaPath);
            Saved = true;
        }

    }
}
