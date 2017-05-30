using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SnipKeep
{
    public class Library
    {
        public static Library[] Loaded = new Library[] { new Library() };

        private List<Snippet> _snippets = new List<Snippet>();

        public Snippet CreateSnippet()
        {
            var snip = new Snippet(this);
            snip.Name = "New snippet";
            if (Clipboard.ContainsText())
                snip.Text = Clipboard.GetText();
            _snippets.Add(snip);
            return snip;
        }

        public void RemoveSnippet(Snippet snip)
        {
            _snippets.Remove(snip);
        }
    }
}
