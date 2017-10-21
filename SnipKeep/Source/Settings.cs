using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SnipKeep
{
    public class Settings
    {
        public static Version CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version;
        public static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Porohkun", "SnipKeep");
        public static string AppPath => AppDomain.CurrentDomain.BaseDirectory;
        public static string LibraryPath => Path.Combine(AppPath, "Library");
    }
}