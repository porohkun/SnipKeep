﻿using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
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
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;

namespace SnipKeep
{
    /// <summary>
    /// Interaction logic for SnippetEditor.xaml
    /// </summary>
    public partial class SnippetEditor : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        public event Action<Snippet> SnippetChanged;

        private Snippet _snippet;
        public Snippet Snippet
        {
            get => _snippet;
            set
            {
                if (_snippet != null)
                    _snippet.PropertyChanged -= Snippet_PropertyChanged;
                _snippet = value;
                Snippet_PropertyChanged(_snippet, new PropertyChangedEventArgs("Name"));
                Snippet_PropertyChanged(_snippet, new PropertyChangedEventArgs("Description"));
                Snippet_PropertyChanged(_snippet, new PropertyChangedEventArgs("Tags"));
                Snippet_PropertyChanged(_snippet, new PropertyChangedEventArgs("Parts"));
                Snippet_PropertyChanged(_snippet, new PropertyChangedEventArgs("SelectedPartIndex"));
                IsEnabled = value != null;
                if (_snippet != null)
                    _snippet.PropertyChanged += Snippet_PropertyChanged;
            }
        }

        private void Snippet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                switch (e.PropertyName)
                {
                    case "SelectedPart":
                    case "SelectedPartIndex":
                        textEditor.Text = Text;
                        NotifyPropertyChanged("SelectedPartIndex");
                        NotifyPropertyChanged("Text");
                        NotifyPropertyChanged("Syntax");
                        if (SelectedPart != null)
                            switch (SelectedPart.Syntax)
                            {
                                case "Java":
                                case "C#":
                                case "C++":
                                    textEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(textEditor.Options);
                                    _foldingStrategy = new BraceFoldingStrategy();
                                    break;
                                case "XmlDoc":
                                case "HTML":
                                case "ASP/XHTML":
                                case "XML":
                                    textEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
                                    _foldingStrategy = new MyXmlFoldingStrategy();
                                    break;
                            }
                        break;
                    case "Name": NotifyPropertyChanged("SnipName"); break;
                    case "Description": NotifyPropertyChanged("Description"); break;
                    case "Text": NotifyPropertyChanged("Text"); break;
                    case "Tags": NotifyPropertyChanged("Tags"); break;
                    case "Parts":
                        {
                            Parts.Clear();
                            if (_snippet != null)
                                foreach (var part in _snippet.Parts)
                                    Parts.Add(part);
                            break;
                        }
                    case "Saved":
                        var snippet = (Snippet)sender;
                        if (!snippet.Saved)
                            SnippetChanged?.Invoke(snippet);
                        break;
                }
        }

        public SnippetPart SelectedPart
        {
            get => _snippet?.SelectedPart;
            set
            {
                if (_snippet != null && value != null)
                    _snippet.SelectedPart = value;
            }
        }
        public int SelectedPartIndex
        {
            get => Parts.IndexOf(SelectedPart);
            set => SelectedPart = value == -1 ? null : Parts[value];
        }
        public string SnipName
        {
            get => _snippet == null ? "" : _snippet.Name;
            set
            {
                if (_snippet != null)
                    _snippet.Name = value;
            }
        }
        public string Description
        {
            get => _snippet == null ? "" : _snippet.Description;
            set
            {
                if (_snippet != null)
                    _snippet.Description = value;
            }
        }
        public string Text
        {
            get => _snippet == null ? "" : _snippet.Text;
            set
            {
                if (_snippet != null)
                    _snippet.Text = value;
            }
        }
        public IHighlightingDefinition Syntax => HighlightingManager.Instance.GetDefinition(_snippet?.SelectedPart == null ? "" : _snippet.SelectedPart.Syntax);

        public List<Label> Tags
        {
            get => (List<Label>)_snippet?.Tags;
            set
            {
                if (_snippet != null)
                {
                    NotifyPropertyChanged("Tags");
                }
            }
        }
        public ObservableCollection<SnippetPart> Parts { get; } = new ObservableCollection<SnippetPart>();

        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

        public SnippetEditor()
        {
            MenuItems = MenuItemViewModel.GetMenuItems(CommandBinding_NewPart);

            InitializeComponent();
            DataContext = this;

            _foldingManager = FoldingManager.Install(textEditor.TextArea);

            _foldingUpdateTimer = new DispatcherTimer();
            _foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            _foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            _foldingUpdateTimer.Start();
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            Text = textEditor.Text;
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
        IFoldingStrategy _foldingStrategy;

        void UpdateFoldings()
        {
            _foldingStrategy?.UpdateFoldings(_foldingManager, textEditor.Document);
        }

        #endregion

        private void CommandBinding_NewPart(string syntax)
        {
            _snippet?.AddPart(syntax);
        }

    }
}
