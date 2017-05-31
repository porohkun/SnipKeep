﻿using System;
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
            set { _snippet = value; UpdateBindings(); }
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
        public string Filename
        {
            get { return _snippet == null ? "" : _snippet.Filename; }
            set
            {
                if (_snippet != null)
                {
                    _snippet.Filename = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filename"));
                }
            }
        }
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
            
        }

        private void UpdateBindings()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SnipName"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filename"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Tags"));
        }

        private void TagsControl_TagAdded(Label tag)
        {
            _snippet.AddTag(tag);
        }

        private void TagsControl_TagRemoved(Label tag)
        {
            _snippet.RemoveTag(tag);
        }
    }
}
