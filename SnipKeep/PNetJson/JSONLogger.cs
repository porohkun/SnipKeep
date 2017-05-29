using System.Diagnostics;

namespace PNetJson
{
    static class JSONLogger
    {
        public static void Log(string str)
        {
            Debug.WriteLine(str);
        }
        public static void Error(string str)
        {
            Debug.WriteLine(str);
        }
    }
}
