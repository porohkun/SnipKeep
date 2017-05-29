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
        public static Version CurrentVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        public static string AppDataPath { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Porohkun", "SnipKeep"); } }
        public static string AppPath { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static string LibraryPath { get { return Path.Combine(AppPath, "Library"); } }
    }
}