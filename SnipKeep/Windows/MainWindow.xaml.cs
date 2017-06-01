using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class MainWindow : Window
    {
        private ObservableCollection<Library> _libraries;
        public ObservableCollection<Library> Libraries
        {
            get
            {
                if (_libraries == null)
                    _libraries = Library.Loaded;
                return _libraries;
            }
        }

        private ObservableCollection<Label> _labels;
        public ObservableCollection<Label> Labels
        {
            get
            {
                if (_labels == null)
                    _labels = Label.Labels;
                return _labels;
            }
        }

        private ObservableCollection<Snippet> _snippets;
        public ObservableCollection<Snippet> Snippets
        {
            get
            {
                if (_snippets == null)
                    _snippets = Snippet.Snippets;
                return _snippets;
            }
        }

        #region generator

        private string lorem = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboTanks.Battle
{
    public class MovingSubsystem
    {
        private int _tickLag = 0;
        public int TickLag { get { return _tickLag; } private set { _tickLag = value >= 0 ? value : 0; } }

        internal Func<bool> OnRotateRight;
        internal Func<bool> OnRotateLeft;
        internal Func<bool> OnMoveForward;
        internal Func<bool> OnMoveBackward;

        private bool MakeMovingAction(Func<bool> action, int tickLag)
        {
            if (TickLag > 0) return false;
            bool result = false;
            if (action != null)
                result = action();
            if (result)
                TickLag += tickLag;
            return result;
        }

        public bool RotateRight()
        {
            return MakeMovingAction(OnRotateRight, Configuration.RotationTime);
        }

        public bool RotateLeft()
        {
            return MakeMovingAction(OnRotateLeft, Configuration.RotationTime);
        }

        public bool MoveForward()
        {
            return MakeMovingAction(OnMoveForward, Configuration.MovingForwardTime);
        }

        public bool MoveBackward()
        {
            return MakeMovingAction(OnMoveBackward, Configuration.MovingBackwardTime);
        }

        public bool RotateRight(out int lag)
        {
            var result = RotateRight();
            lag = _tickLag;
            return result;
        }

        public bool RotateLeft(out int lag)
        {
            var result = RotateLeft();
            lag = _tickLag;
            return result;
        }

        public bool MoveForward(out int lag)
        {
            var result = MoveForward();
            lag = _tickLag;
            return result;
        }

        public bool MoveBackward(out int lag)
        {
            var result = MoveBackward();
            lag = _tickLag;
            return result;
        }

        internal void Update()
        {
            TickLag -= 1;
        }
    }
}
";

        #endregion

        #region tree

        List<TreeViewItem> selectedItems = new List<TreeViewItem>();
        bool CtrlPressed { get { return System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl); } }

        // deselects the tree item
        void Deselect(TreeViewItem treeViewItem)
        {
            treeViewItem.Background = Brushes.White;// change background and foreground colors
            treeViewItem.Foreground = Brushes.Black;
            selectedItems.Remove(treeViewItem); // remove the item from the selected items set
        }

        // changes the state of the tree item:
        // selects it if it has not been selected and
        // deselects it otherwise
        void ChangeSelectedState(TreeViewItem treeViewItem)
        {
            if (!selectedItems.Contains(treeViewItem))
            { // select
                treeViewItem.Background = Brushes.Black; // change background and foreground colors
                treeViewItem.Foreground = Brushes.White;
                selectedItems.Add(treeViewItem); // add the item to selected items
            }
            else
            { // deselect
                Deselect(treeViewItem);
            }
        }
        #endregion

        public MainWindow()
        {
            Icons.Load();
            InitializeComponent();
            DataContext = this;

            //var words = lorem.Replace("\r\n", "").Replace(";", " ").Replace(",", " ").Replace("0", " ").Replace("1", " ").Replace(":", " ").Replace("{", " ").Replace("}", " ").Replace("(", " ").Replace(")", " ").Replace(".", " ").Replace("=", " ").Replace("<", " ").Replace(">", " ").Replace("?", " ").Replace("!", " ").Replace("+", " ").Replace("-", " ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Replace("  ", " ");
            //var w = words.Split(' ').Where(e => e.Length > 0).Distinct();
            //foreach (var tag in w)
            //{
            //    Labels.Add(new Label() { Name = tag });
            //}
            //Labels.Sort();
            //var rnd = new Random();
            //for (int i = 0; i < 1000; i++)
            //{
            //    var snippet = new Snippet(Library.Loaded[0])
            //    {
            //        Text = lorem.Substring(0, rnd.Next(lorem.Length / 2, lorem.Length))
            //    };
            //    for (int t = 0; t < rnd.Next(2, 5); t++)
            //        snippet.AddTag((Label)Labels[rnd.Next(0, Labels.Count)]);
            //    snippet.Save();
            //    Snippets.Add(snippet);
            //}
            //Snippets.Sort();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Snippets);
            view.Filter = SnippetFilter;
        }

        private bool SnippetFilter(object item)
        {
            var snipped = item as Snippet;
            if (labelList.SelectedItems.Count == 0) return true;
            foreach (Label selected in labelList.SelectedItems)
                if (!snipped.Tags.Contains(selected))
                    return false;
            return true;
        }

        private void labelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(Snippets).Refresh();

        }

        private void snippetsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            editor.Snippet = (Snippet)snippetsList.SelectedItem;
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
            Libraries[0].LoadLibrary();
        }

        private void CommandBinding_SaveLib(object sender, ExecutedRoutedEventArgs e)
        {
            Libraries[0].SaveLibrary();
        }
        
        private void CommandBinding_NewSnippet(object sender, ExecutedRoutedEventArgs e)
        {
            var lib = librariesList.SelectedItem == null ? Library.Loaded[0] : (Library)librariesList.SelectedItem;
            var snip = lib.CreateSnippet();
            Snippets.Sort();
            snippetsList.SelectedItem = snip;
        }

        private void CommandBinding_DeleteSnippet(object sender, ExecutedRoutedEventArgs e)
        {
            var snip = (Snippet)snippetsList.SelectedItem;
            int i = Snippets.IndexOf(snip);
            snip.Library.RemoveSnippet(snip);
            if (i == Snippets.Count)
                i--;
            if (i >= 0)
                snippetsList.SelectedItem = Snippets[i];
        }

        private void CommandBinding_CopyCode(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(((Snippet)snippetsList.SelectedItem).Text);
        }

        #endregion

    }

}
