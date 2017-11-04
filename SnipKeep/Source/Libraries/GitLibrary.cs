using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MimiJson;

namespace SnipKeep
{
    public class GitLibrary : Library
    {
        public override ImageSource IconSource => Icons.Sources["git.png"];
        public override string Name { get; }
        protected override void Remove(Snippet snippets)
        {
            throw new NotImplementedException();
        }

        protected override void Save(IEnumerable<Snippet> snippet)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<ILoadOperation> Load()
        {
            throw new NotImplementedException();
        }
    }
}
