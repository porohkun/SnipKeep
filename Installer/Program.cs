using MimiJson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installer
{
    class Program
    {
        public static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Porohkun", "SnipKeep");
        static string UpdatePath => Path.Combine(AppDataPath, "Update");

        static void Main(string[] args)
        {
            try
            {
                var path = args[0];
                var procId = int.Parse(args[1]);
                bool exceptions = false;
                Console.WriteLine("Waiting for app closing...");
                Process proc = null;
                do
                {
                    try
                    {
                        proc = Process.GetProcessById(procId);
                    }
                    catch
                    {
                        proc = null;
                    }
                } while (proc != null);
                Console.WriteLine("done.");

                var updFilePath = Path.Combine(UpdatePath, "update-config.json");
                var json = JsonValue.ParseFile(updFilePath);
                var version = Version.Parse(json["version"]);

                foreach (var jfile in json["files"])
                {
                    var updPath = Path.Combine(UpdatePath, jfile["path"], jfile["name"]);
                    var filePath = Path.Combine(path, jfile["path"], jfile["name"]);
                    switch (jfile["action"].String)
                    {
                        case "replace":
                        case "add":
                            Console.WriteLine("Copying file " + jfile["name"]);
                            exceptions = exceptions || CatchAction(() => Directory.CreateDirectory(Path.GetDirectoryName(filePath)));
                            exceptions = exceptions || CatchAction(() => File.Copy(updPath, filePath, true));
                            break;
                        case "del":
                        case "delete":
                            Console.WriteLine("Deleting file " + jfile["name"]);
                            exceptions = exceptions || CatchAction(() => File.Delete(filePath));
                            break;
                    }
                }

                Console.WriteLine("updating reg version");
                exceptions = exceptions || CatchAction(() => TestVersion.SetVersion("SnipKeep", $"{version.Major}.{version.Minor}.{version.Build}{version.Revision:D2}"));

                Console.WriteLine("all done.");
                if (exceptions)
                    Console.ReadKey();

                try
                {
                    ProcessStartInfo info = new ProcessStartInfo(Path.Combine(path, "SnipKeep.exe"));
                    info.Arguments = "/d";
                    Process.Start(info);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }
        }

        public static bool CatchAction(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return true;
            }
            return false;
        }
    }
}
