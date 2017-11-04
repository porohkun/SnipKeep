using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using MimiJson;

namespace SnipKeep
{
    public abstract class Library : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        #endregion
        public static ObservableCollection<Library> Loaded = new ObservableCollection<Library> { new LocalLibrary(Settings.LibraryPath) };

        protected string _libraryPath;

        private readonly List<string> _ids = new List<string>();
        protected readonly List<Snippet> _snippets = new List<Snippet>();

        public abstract ImageSource IconSource { get; }
        public abstract string Name { get; }
        public int Count => _snippets.Count;
        public string LibraryPath => _libraryPath;
        public string SnippetsPath => Path.Combine(_libraryPath, "Snippets");

        public string GetNewId()
        {
            string id;
            do
            {
                id = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            }
            while (_ids.Contains(id));
            _ids.Add(id);
            return id;
        }

        public Snippet GetSnippet(string id)
        {
            return _snippets.FirstOrDefault(s => s.Id == id);
        }

        public virtual Snippet CreateSnippet(string syntax)
        {
            var snip = new Snippet(GetNewId(), this) { Name = "New snippet" };
            snip.AddPart(syntax);
            if (Clipboard.ContainsText())
                snip.Text = Clipboard.GetText();
            _snippets.Add(snip);
            Snippet.Snippets.Add(snip);
            OnPropertyChanged(this, new PropertyChangedEventArgs("Count"));
            return snip;
        }

        public virtual void RemoveSnippet(Snippet snip)
        {
            _snippets.Remove(snip);
            Snippet.Snippets.Remove(snip);
            OnPropertyChanged(this, new PropertyChangedEventArgs("Count"));
        }

        protected abstract void Remove(Snippet snippets);

        public void SaveLibrary()
        {
            Save(_snippets.Where(s => !s.Saved));
        }

        protected abstract void Save(IEnumerable<Snippet> snippet);

        public void LoadLibrary()
        {
            SaveLibrary();
            var loaded = new List<Snippet>();
            var snipIds = _snippets.Select(s => s.Id).ToArray();
            foreach (var operation in Load())
            {
                if (!snipIds.Contains(operation.Id))
                {
                    var snippet = operation.Load();
                    _snippets.Add(snippet);
                    Snippet.Snippets.Add(snippet);
                    _ids.Add(snippet.Id);
                    foreach (var part in snippet.Parts)
                        _ids.Add(part.Id);
                    loaded.Add(snippet);
                }
                else
                {
                    var snippet = GetSnippet(operation.Id);
                    if (snippet.SaveTime < operation.SaveTime)
                    {
                        operation.Update(snippet);
                        foreach (var part in snippet.Parts)
                            if (!_ids.Contains(part.Id))
                                _ids.Add(part.Id);
                    }
                    else
                    {
                        snippet.Saved = false;
                    }
                    loaded.Add(snippet);
                }
            }
            var forDel = _snippets.Except(loaded).ToArray();
            foreach (var snippet in forDel)
                RemoveSnippet(snippet);
            SaveLibrary();
            OnPropertyChanged(this, new PropertyChangedEventArgs("Count"));
        }

        protected abstract IEnumerable<ILoadOperation> Load();

        protected interface ILoadOperation
        {
            string Id { get; }
            DateTime SaveTime { get; }
            Snippet Load();
            void Update(Snippet snippet);
        }
    }
}
