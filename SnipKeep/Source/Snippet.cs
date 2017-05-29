using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnipKeep
{
    public class Snippet
    {
        private bool _saved = false;
        public bool Saved { get { return _saved; } private set { _saved = value; } }

        public string Name { get; set; }
        public string Filename { get; set; }
        public string Text { get; set; }
        private List<LabelData> _tags = new List<LabelData>();
        public List<LabelData> Tags { get { return _tags; } }
        public string TagsString { get; private set; }

        public Snippet()
        {

        }

        public void Save()
        {
            if (Filename == null || Filename == "")
            {
                Name = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                Filename = Name + ".cs";
            }
            //Tags.Sort();
            TagsString = "";
            for (int i = 0; i < Tags.Count; i++)
            {
                TagsString += Tags[i].Name;
                if (i <= Tags.Count - 1)
                    TagsString += ", ";
            }
        }
    }
}
