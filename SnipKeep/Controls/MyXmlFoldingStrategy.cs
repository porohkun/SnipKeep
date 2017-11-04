using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace SnipKeep
{
    public class MyXmlFoldingStrategy : IFoldingStrategy
    {
        private XmlFoldingStrategy _strategy;
        public MyXmlFoldingStrategy()
        {
            _strategy = new XmlFoldingStrategy();
        }
        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            _strategy.UpdateFoldings(manager, document);
        }
    }
}
