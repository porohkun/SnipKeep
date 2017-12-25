using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimiJson;

namespace SnipKeep
{
    public class SnippetPart : INotifyPropertyChanged, IComparable, IComparable<SnippetPart>
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
        #region IComparable, IComparable<SnippetPart> Members

        public int CompareTo(object obj)
        {
            return CompareTo((SnippetPart)obj);
        }

        public int CompareTo(SnippetPart other)
        {
            return Id.CompareTo(other.Id);
        }

        #endregion

        private string _syntax;
        private string _text;

        public string Id { get; }

        public bool Saved { get; set; }

        public string Syntax
        {
            get => _syntax;
            //set
            //{
            //    if (_syntax == value) return;
            //    _syntax = value;
            //    Saved = false;
            //    NotifyPropertyChanged("Syntax");
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
                NotifyPropertyChanged("Text");
            }
        }

        public SnippetPart(string id, string syntax, string text = "", bool saved = false)
        {
            Id = id;
            _syntax = syntax;
            _text = text;
            Saved = saved;
        }
    }
}
