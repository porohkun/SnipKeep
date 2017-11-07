using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MimiJson;

namespace SnipKeep
{
    public class AppUpdateTask : IAsyncTask
    {
        public string StartStatus => "App update checking...";
        public string FinalStatus => "";
        public string FailureStatus => "Unable to download update";

        private bool _updating = false;

        public AppUpdateTask()
        {

        }

        public IEnumerator<TaskStatus> DoWork()
        {
            var updFilePath = Path.Combine(Settings.UpdatePath, "update-config.json");
            Directory.CreateDirectory(Settings.UpdatePath);

            yield return new TaskStatus(0.02, "App update checking...");
            using (WebClient client = new WebClient())
                client.DownloadFile(Settings.UpdateConfigUrl, updFilePath);

            var json = JsonValue.ParseFile(updFilePath);
            if (Version.Parse(json["version"]) > Settings.CurrentVersion)
            {
                var filesCount = json["files"].Array.Length;
                var progress = 1d;
                foreach (var jfile in json["files"])
                {
                    yield return new TaskStatus(progress, "App update recieving");
                    progress += 0.9d / filesCount;

                    var path = Path.Combine(Settings.UpdatePath, jfile["path"], jfile["name"]);
                    var filePath = Path.Combine(Settings.AppPath, jfile["path"], jfile["name"]);
                    switch (jfile["action"].String)
                    {
                        case "replace":
                        case "add":
                        case "misc":
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                            using (WebClient client = new WebClient())
                                client.DownloadFile(jfile["url"], path);
                            break;
                    }
                }
                _updating = true;
                yield return new TaskStatus(1, "Launching update...");
            }
        }

        public void TaskCompleted()
        {
            if (_updating && MessageBox.Show("SnipKeep update received and ready to install.\r\n\r\nRestart app now?", "SnipKeep have updates.", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                const int ERROR_CANCELLED = 1223; //The operation was canceled by the user.

                ProcessStartInfo info = new ProcessStartInfo(Path.Combine(Settings.UpdatePath, "Installer.exe"));
                info.UseShellExecute = true;
                info.Arguments = string.Format("\"{0}\" {1}", Settings.AppPath.Replace(@"\", @"\\"), Process.GetCurrentProcess().Id);
                if (Environment.OSVersion.Version.Major >= 6)
                    info.Verb = "runas";
                try
                {
                    Process.Start(info);
                    Application.Current.Shutdown();
                }
                catch (Win32Exception ex)
                {
                    if (ex.NativeErrorCode == ERROR_CANCELLED)
                    {
                        if (MessageBox.Show("You have not rights to install update. Try again?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            TaskCompleted();
                    }
                    else throw;
                }
            }
        }

        public void ExceptionCatch(Exception e)
        {
            
        }
    }
}
