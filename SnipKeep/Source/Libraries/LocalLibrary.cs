using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SnipKeep
{
    public class LocalLibrary : Library
    {
        public override ImageSource IconSource { get { return Icons.Sources["database.png"]; } }
        public override string Name { get { return "Local library"; } }

        public LocalLibrary(string libraryPath)
        {
            this._libraryPath = libraryPath;
            try
            {
                LoadLibrary();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public override void SaveLibrary()
        {
            RegenPatches();
            foreach (var snippet in _snippets)
                if (!snippet.Saved)
                    snippet.Save();
        }

        public override void LoadLibrary()
        {
            SaveLibrary();
            foreach (var snippet in _snippets)
                snippet.UpdateFromSaved();
            var files = Directory.GetFiles(SnippetsPath, "*.*", SearchOption.TopDirectoryOnly).Select(f => Path.GetFileName(f));
            foreach (var file in files.Where(f => Path.GetExtension(f) != ".meta").Except(_snippets.Select(s => s.Filename)))
            {
                var snip = new Snippet(this, file);
                _snippets.Add(snip);
                Snippet.Snippets.Add(snip);
            }
            Snippet.Snippets.Sort();
            OnPropertyChanged(this, new PropertyChangedEventArgs("Count"));
        }

    }
}
