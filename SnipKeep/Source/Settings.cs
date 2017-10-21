using MimiJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SnipKeep
{
    public class Settings
    {
        public static Version CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version;
        public static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Porohkun", "SnipKeep");
        public static string AppPath => AppDomain.CurrentDomain.BaseDirectory;
        public static string LibraryPath => Path.Combine(AppDataPath, "Library");
        public static string SettingsPath => Path.Combine(AppDataPath, "settings.json");

        private static double _mainWindowWidth = -1;
        private static double _mainWindowHeight = -1;
        private static WindowState _mainWindowState;

        public static double MainWindowWidth
        {
            get => _mainWindowWidth;
            set { _mainWindowWidth = value; Save(); }
        }
        public static double MainWindowHeight
        {
            get => _mainWindowHeight;
            set { _mainWindowHeight = value; Save(); }
        }

        public static WindowState MainWindowState
        {
            get => _mainWindowState;
            set { _mainWindowState = value; Save(); }
        }


        static Settings()
        {
            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);
            if (File.Exists(SettingsPath))
            {
                try
                {
                    var json = JsonValue.ParseFile(SettingsPath);
                    MainWindowWidth = json["main_window_width"];
                    MainWindowHeight = json["main_window_height"];
                    MainWindowState = (WindowState)(int)json["main_window_state"];
                }
                catch (Exception)
                {
                    Save();
                }
            }
            else
            {
                Save();
            }
        }

        public static void Save()
        {
            try
            {
                var json = new JsonValue(new JsonObject(
                    new JOPair("main_window_width", MainWindowWidth),
                    new JOPair("main_window_height", MainWindowHeight),
                    new JOPair("main_window_state", (int)MainWindowState)
                    ));
                json.ToFile(SettingsPath);
            }
            catch (Exception e)
            {
                MessageBox.Show("Cant save settings file:\r\n" + e.Message);
            }
        }
    }
}