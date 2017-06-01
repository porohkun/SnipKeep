using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SnipKeep
{
    public static class Icons
    {
        public static Dictionary<string, ImageSource> Sources = new Dictionary<string, ImageSource>();

        public static void Load()
        {
            Load("copy.png");
            Load("cross.png");
            Load("plus.png");
            Load("label.png");
            Load("database.png");
            Load("git.png");
        }

        private static void Load(string icon)
        {
            Sources.Add(icon, new BitmapImage(new Uri(@"/SnipKeep;component/Icons/" + icon, UriKind.Relative)));
        }
    }
}
