using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using System.Windows.Threading;

namespace SnipKeep
{
    /// <summary>
    /// Interaction logic for SnippetEditor.xaml
    /// </summary>
    public partial class SnippetEditor : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private Snippet _snippet;
        public Snippet Snippet
        {
            get { return _snippet; }
            set
            { _snippet = value; UpdateBindings(); IsEnabled = value != null; }
        }
        public string SnipName
        {
            get { return _snippet == null ? "" : _snippet.Name; }
            set
            {
                if (_snippet != null)
                {
                    _snippet.Name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SnipName"));
                }
            }
        }
        public string Description
        {
            get { return _snippet == null ? "" : _snippet.Description; }
            set
            {
                if (_snippet != null)
                {
                    _snippet.Description = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                }
            }
        }
        //public string Filename
        //{
        //    get { return _snippet == null ? "" : _snippet.Filename; }
        //    set
        //    {
        //        if (_snippet != null)
        //        {
        //            _snippet.Filename = value;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filename"));
        //        }
        //    }
        //}
        public string Text
        {
            get { return _snippet == null ? "" : _snippet.Text; }
            set
            {
                if (_snippet != null)
                {
                    _snippet.Text = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
                }
            }
        }
        public List<Label> Tags
        {
            get { return _snippet == null ? null : (List<Label>)_snippet.Tags; }
            set
            {
                if (_snippet != null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tags"));
                }
            }
        }

        public SnippetEditor()
        {
            InitializeComponent();
            DataContext = this;
            textEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(textEditor.Options);
            _foldingStrategy = new BraceFoldingStrategy();
            _foldingManager = FoldingManager.Install(textEditor.TextArea);
            textEditor.TextChanged += TextEditor_TextChanged;

            _foldingUpdateTimer = new DispatcherTimer();
            _foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            _foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            _foldingUpdateTimer.Start();
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            Text = textEditor.Text;
        }

        private void UpdateBindings()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SnipName"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filename"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tags"));
            textEditor.Text = Text;
        }

        private void TagsControl_TagAdded(Label tag)
        {
            _snippet.AddTag(tag);
        }

        private void TagsControl_TagRemoved(Label tag)
        {
            _snippet.RemoveTag(tag);
        }

        #region context menu

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Cut();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Copy();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Paste();
        }

        private void Indent_Click(object sender, RoutedEventArgs e)
        {
            (textEditor.TextArea.IndentationStrategy as CSharpIndentationStrategy).Indent(new TextDocumentAccessor(textEditor.Document), true);
        }

        #endregion

        #region foldings

        DispatcherTimer _foldingUpdateTimer;
        FoldingManager _foldingManager;
        BraceFoldingStrategy _foldingStrategy;

        void UpdateFoldings()
        {
            _foldingStrategy.UpdateFoldings(_foldingManager, textEditor.Document);
        }

        #endregion

    }
}
