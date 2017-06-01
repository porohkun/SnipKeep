using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SnipKeep
{
    public class GitLibrary : Library
    {
        public override ImageSource IconSource { get { return Icons.Sources["git.png"]; } }

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void LoadLibrary()
        {
            throw new NotImplementedException();
        }

        public override void SaveLibrary()
        {
            throw new NotImplementedException();
        }
    }
}
