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
        public static string UpdatePath => Path.Combine(AppDataPath, "Update");
        public static string AppPath => AppDomain.CurrentDomain.BaseDirectory;
        public static string LibraryPath => Path.Combine(AppDataPath, "Library");
        public static string SettingsPath => Path.Combine(AppDataPath, "settings.json");
        public static string UpdateConfigUrl => "https://porohkun.github.io/SnipKeep/update-config.json";

        private static bool _autoUpdate = true;
        public static event EventHandler AutoUpdateChanged;
        public static bool AutoUpdate
        {
            get => _autoUpdate;
            set
            {
                if (value != _autoUpdate)
                {
                    _autoUpdate = value;
                    AutoUpdateChanged?.Invoke(null, EventArgs.Empty);
                    Save();
                }
            }
        }

        #region GUI

        public static string Title => "SnipKeep" +
#if DEBUG
                                      " [DEBUG]" +
#endif
                                      " v." + CurrentVersion;

        private static double _gui_MainWindow_Width = 800;
        public static event EventHandler GUI_MainWindow_WidthChanged;
        public static double GUI_MainWindow_Width
        {
            get => _gui_MainWindow_Width;
            set
            {
                if (value != _gui_MainWindow_Width)
                {
                    _gui_MainWindow_Width = value;
                    GUI_MainWindow_WidthChanged?.Invoke(null, EventArgs.Empty);
                    Save();
                }
            }
        }

        private static double _gui_MainWindow_Height = 400;
        public static event EventHandler GUI_MainWindow_HeightChanged;
        public static double GUI_MainWindow_Height
        {
            get => _gui_MainWindow_Height;
            set
            {
                if (value != _gui_MainWindow_Height)
                {
                    _gui_MainWindow_Height = value;
                    GUI_MainWindow_HeightChanged?.Invoke(null, EventArgs.Empty);
                    Save();
                }
            }
        }

        private static double _gui_MainWindow_LeftPanelWidth = 250;
        public static event EventHandler GUI_MainWindow_LeftPanelWidthChanged;
        public static double GUI_MainWindow_LeftPanelWidth
        {
            get => _gui_MainWindow_LeftPanelWidth;
            set
            {
                if (value != _gui_MainWindow_LeftPanelWidth)
                {
                    _gui_MainWindow_LeftPanelWidth = value;
                    GUI_MainWindow_LeftPanelWidthChanged?.Invoke(null, EventArgs.Empty);
                    Save();
                }
            }
        }

        private static double _gui_MainWindow_RightPanelWidth = 250;
        public static event EventHandler GUI_MainWindow_RightPanelWidthChanged;
        public static double GUI_MainWindow_RightPanelWidth
        {
            get => _gui_MainWindow_RightPanelWidth;
            set
            {
                if (value != _gui_MainWindow_RightPanelWidth)
                {
                    _gui_MainWindow_RightPanelWidth = value;
                    GUI_MainWindow_RightPanelWidthChanged?.Invoke(null, EventArgs.Empty);
                    Save();
                }
            }
        }

        private static WindowState _gui_mainWindow_State;
        public static event EventHandler GUI_MainWindow_StateChanged;
        public static WindowState GUI_MainWindow_State
        {
            get => _gui_mainWindow_State;
            set
            {
                if (value != _gui_mainWindow_State)
                {
                    _gui_mainWindow_State = value;
                    GUI_MainWindow_HeightChanged?.Invoke(null, EventArgs.Empty);
                    Save();
                }
            }
        }

        #endregion


        static Settings()
        {
            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);
            if (File.Exists(SettingsPath))
            {
                try
                {
                    var json = JsonValue.ParseFile(SettingsPath);
                    AutoUpdate = json["auto_update"];
                    _gui_MainWindow_Width = json["gui_main_window_width"];
                    _gui_MainWindow_Height = json["gui_main_window_height"];
                    _gui_MainWindow_LeftPanelWidth = json["gui_main_window_left_panel_width"];
                    _gui_MainWindow_RightPanelWidth = json["gui_main_window_right_panel_width"];
                    _gui_mainWindow_State = (WindowState)(int)json["gui_main_window_state"];
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
                    new JOPair("auto_update", AutoUpdate),
                    new JOPair("gui_main_window_width", GUI_MainWindow_Width),
                    new JOPair("gui_main_window_height", GUI_MainWindow_Height),
                    new JOPair("gui_main_window_left_panel_width", GUI_MainWindow_LeftPanelWidth),
                    new JOPair("gui_main_window_right_panel_width", GUI_MainWindow_RightPanelWidth),
                    new JOPair("main_window_state", (int)GUI_MainWindow_State)
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