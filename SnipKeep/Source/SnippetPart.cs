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

        public bool Saved { get; private set; }

        public string Syntax
        {
            get => _syntax;
            //set
            //{
            //    if (_syntax == value) return;
            //    _syntax = value;
            //    Saved = false;
            //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Syntax"));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
            }
        }

        public SnippetPart(string id, string syntax, string text = "")
        {
            Id = id;
            _syntax = syntax;
            _text = text;
        }
    }
}
