using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MimiJson;
using System.Collections.ObjectModel;

namespace SnipKeep
{
    public class Snippet : INotifyPropertyChanged, IComparable, IComparable<Snippet>
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
        #region IComparable, IComparable<Snippet> Members

        public int CompareTo(object obj)
        {
            return CompareTo((Snippet)obj);
        }

        public int CompareTo(Snippet other)
        {
            return Id.CompareTo(other.Id);
        }

        #endregion

        public static ObservableCollection<Snippet> Snippets { get; } = new ObservableCollection<Snippet>();

        private bool _saved;

        private DateTime _saveTime = DateTime.MinValue;
        private string _name = "";
        private string _description = "";
        private readonly List<SnippetPart> _parts = new List<SnippetPart>();
        private SnippetPart _selectedPart;
        private readonly List<Label> _tags = new List<Label>();

        public Library Library { get; private set; }

        public string Id { get; }

        public bool Saved
        {
            get => _saved && (_parts.Count <= 0 || _parts.Select(p => p.Saved).Aggregate((s1, s2) => s1 && s2));
            set
            {
                if (_saved == value) return;
                _saved = value;
                if (_saved)
                    SaveTime = DateTime.UtcNow;
                NotifyPropertyChanged("Saved");
            }
        }

        public DateTime SaveTime
        {
            get => _saveTime;
            private set
            {
                if (_saveTime == value) return;
                _saveTime = value;
                NotifyPropertyChanged("SaveTime");
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                Saved = false;
                NotifyPropertyChanged("Name");
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
                NotifyPropertyChanged("Description");
            }
        }

        public string Text
        {
            get => _selectedPart != null ? _selectedPart.Text : "";
            set
            {
                if (_selectedPart == null || _selectedPart.Text == value) return;
                _selectedPart.Text = value;
            }
        }

        public SnippetPart SelectedPart
        {
            get => _selectedPart;
            set
            {
                if (_selectedPart == value) return;
                _selectedPart = value;
                NotifyPropertyChanged("SelectedPart");
            }
        }

        public IEnumerable<SnippetPart> Parts => _parts;
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
                NotifyPropertyChanged("TagsString");
            }
        }

        public Snippet(string id, Library lib)
        {
            Id = id;
            Library = lib;
        }

        public Snippet(string id, Library lib, DateTime saveTime, string name, string description, IEnumerable<Label> tags, IEnumerable<SnippetPart> parts) : this(id, lib)
        {
            _saveTime = saveTime;
            _name = name;
            _description = description;
            SetTags(tags, true);
            SetParts(parts, true);
            _saved = true;
            SelectedPart = _parts.FirstOrDefault();
        }

        public void Update(string name, string description, IEnumerable<Label> tags, IEnumerable<SnippetPart> parts)
        {
            var selected = SelectedPart;
            Name = name;
            Description = description;
            SetTags(tags, true);
            NotifyPropertyChanged("Tags");
            SetParts(parts, true);
            NotifyPropertyChanged("Parts");
            SelectedPart = _parts.FirstOrDefault(p => p.Id == selected.Id) ?? _parts.FirstOrDefault();
        }

        internal void Delete()
        {
            Library.RemoveSnippet(this);
        }

        public bool AddTag(Label tag, bool silent = false)
        {
            if (_tags.Contains(tag)) return false;
            _tags.Add(tag);
            _tags.Sort();
            tag.AddSnippet(this);
            if (!silent)
            {
                Saved = false;
                NotifyPropertyChanged("Tags");
                NotifyPropertyChanged("TagsString");
            }
            return true;
        }

        public bool RemoveTag(Label tag, bool silent = false)
        {
            if (!_tags.Contains(tag)) return false;
            _tags.Remove(tag);
            tag.RemoveSnippet(this);
            if (!silent)
            {
                Saved = false;
                NotifyPropertyChanged("Tags");
                NotifyPropertyChanged("TagsString");
            }
            return true;
        }

        public void SetTags(IEnumerable<Label> tags, bool silent = false)
        {
            while (_tags.Count > 0)
                RemoveTag(_tags[0], true);
            foreach (var tag in tags)
                AddTag(tag, true);
            if (!silent)
            {
                Saved = false;
                NotifyPropertyChanged("Tags");
                NotifyPropertyChanged("TagsString");
            }
        }

        private void AddPart(SnippetPart part, bool silent = false)
        {
            _parts.Add(part);
            part.PropertyChanged += Part_PropertyChanged;
            if (!silent)
            {
                Saved = false;
                NotifyPropertyChanged("Parts");
            }
            SelectedPart = part;
        }

        public void AddPart(string syntax, bool silent = false)
        {
            var part = new SnippetPart(Library.GetNewId(), syntax);
            AddPart(part, silent);
        }

        public void RemovePart(SnippetPart part, bool silent = false)
        {
            part.PropertyChanged -= Part_PropertyChanged;
            _parts.Remove(part);
            if (!silent)
            {
                Saved = false;
                NotifyPropertyChanged("Parts");
            }
        }

        public void SetParts(IEnumerable<SnippetPart> parts, bool silent = false)
        {
            while (_parts.Count > 0)
                RemovePart(_parts[0], true);
            foreach (var part in parts)
                AddPart(part, true);
            if (!silent)
            {
                Saved = false;
                NotifyPropertyChanged("Parts");
            }
        }

        private void Part_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                Saved = false;
                if (sender == _selectedPart)
                    NotifyPropertyChanged("Text");
            }
        }
    }
}
