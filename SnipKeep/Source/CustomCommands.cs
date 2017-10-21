using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SnipKeep
{
    internal static class CustomCommands
    {
        public static RoutedCommand NewSnippet = new RoutedCommand();
        public static RoutedCommand CopyCode = new RoutedCommand();
        public static RoutedCommand DeleteSnippet = new RoutedCommand();
    }
}
