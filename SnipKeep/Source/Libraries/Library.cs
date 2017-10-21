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

        protected List<Snippet> _snippets = new List<Snippet>();

        public abstract ImageSource IconSource { get; }
        public abstract string Name { get; }
        public int Count => _snippets.Count;
        public string LibraryPath => _libraryPath;
        public string SnippetsPath => Path.Combine(_libraryPath, "Snippets");

        public virtual Snippet CreateSnippet()
        {
            var snip = new Snippet(this) { Name = "New snippet" };
            var unique = false;
            while (!unique)
            {
                unique = true;
                if (_snippets.All(snippet => snippet.Filename != snip.Filename)) continue;
                unique = false;
                snip.RegenFilename();
            }
            if (Clipboard.ContainsText())
                snip.Text = Clipboard.GetText();
            _snippets.Add(snip);
            Snippet.Snippets.Add(snip);
            snip.Save();
            OnPropertyChanged(this, new PropertyChangedEventArgs("Count"));
            return snip;
        }

        public virtual void RemoveSnippet(Snippet snip)
        {
            _snippets.Remove(snip);
            snip.Delete();
            Snippet.Snippets.Remove(snip);
            OnPropertyChanged(this, new PropertyChangedEventArgs("Count"));
        }

        public abstract void SaveLibrary();
        public abstract void LoadLibrary();
        protected void RegenPatches()
        {
            if (!Directory.Exists(SnippetsPath))
                Directory.CreateDirectory(SnippetsPath);
        }
    }
}
