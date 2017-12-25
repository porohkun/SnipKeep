using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnipKeep
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(property));
        }

        #endregion

        private ObservableCollection<Library> _libraries;
        public ObservableCollection<Library> Libraries => _libraries ?? (_libraries = Library.Loaded);
        private Library _selectedLibrary = null;

        public Library SelectedLibrary
        {
            get
            {
                if (_selectedLibrary == null)
                    _selectedLibrary = Library.Loaded[0];
                return _selectedLibrary;
            }
            set
            {
                _selectedLibrary = value;
                NotifyPropertyChanged("SelectedLibrary");
            }
        }

        private ObservableCollection<Label> _labels;
        public ObservableCollection<Label> Labels => _labels ?? (_labels = Label.Labels);

        private ObservableCollection<Snippet> _snippets;
        public ObservableCollection<Snippet> Snippets => _snippets ?? (_snippets = Snippet.Snippets);

        public ObservableCollection<MenuItemViewModel> MenuItems { get; }

        public MainWindow()
        {
            MenuItems = MenuItemViewModel.GetMenuItems(CommandBinding_NewSnippet);

            Icons.Load();

            InitializeComponent();
            DataContext = this;

            if (Settings.AutoUpdate)
                AsyncManager.Push(new AppUpdateTask());

            var view = (CollectionView)CollectionViewSource.GetDefaultView(Snippets);
            view.Filter = SnippetFilter;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            foreach (var lib in Libraries)
                lib.SaveLibrary();
        }

        private bool SnippetFilter(object item)
        {
            if (!(item is Snippet snippet)) return false;
            foreach (var label in Label.Labels.Where(l=>l.Selected))
                if (!snippet.Tags.Contains(label))
                    return false;
            return true;
        }

        private void labelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Label label in e.AddedItems)
                label.Selected = true;
            foreach (Label label in e.RemovedItems)
                label.Selected = false;
            CollectionViewSource.GetDefaultView(Snippets).Refresh();
        }

        private Snippet _selectedSnippet;
        public Snippet SelectedSnippet
        {
            get => _selectedSnippet;
            set
            {
                if (_selectedSnippet == value) return;
                _selectedSnippet = value;
                editor.Snippet = value;
                NotifyPropertyChanged("SelectedSnippet");
            }
        }

        #region Commands

        private void CommonCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Close(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void CommandBinding_LoadLib(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedLibrary?.LoadLibrary();
        }

        private void CommandBinding_SaveLib(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedLibrary?.SaveLibrary();
        }

        private void CommandBinding_NewSnippet(string syntax)
        {
            var snip = SelectedLibrary?.CreateSnippet(syntax);
            Snippets.Sort();
            SelectedSnippet = snip;
        }

        private void CommandBinding_DeleteSnippet(object sender, ExecutedRoutedEventArgs e)
        {
            var i = Snippets.IndexOf(SelectedSnippet);
            SelectedSnippet.Library.RemoveSnippet(SelectedSnippet);
            if (i == Snippets.Count)
                i--;
            if (i >= 0)
                SelectedSnippet = Snippets[i];
        }

        private void CommandBinding_CopyCode(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(SelectedSnippet.Text);
        }

        private void CommandBinding_AboutWindow(object sender, ExecutedRoutedEventArgs e)
        {
            var about = new AboutWindow();
            about.ShowDialog();
        }

        #endregion

        private void Editor_OnSnippetChanged(Snippet snippet)
        {
            snippet.Library.SaveLibrary();
        }
    }

}
