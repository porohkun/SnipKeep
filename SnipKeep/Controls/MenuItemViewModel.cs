using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SnipKeep
{
    public class MenuItemViewModel
    {
        private readonly ICommand _command;
        public ICommand Command => _command;

        public string Header { get; set; }
        public string Syntax { get; set; }

        public MenuItemViewModel(Action<string> action)
        {
            _command = new CommandViewModel(() => action(Syntax));
        }

        public static ObservableCollection<MenuItemViewModel> GetMenuItems(Action<string> action)
        {
            return new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel(action) { Header = "Plain text", Syntax = "Text"},
                new MenuItemViewModel(action) { Header = "C#", Syntax = "C#"},
                new MenuItemViewModel(action) { Header = "C++", Syntax = "C++"},
                new MenuItemViewModel(action) { Header = "XML/XAML", Syntax = "XML" },
                new MenuItemViewModel(action) { Header = "HTML", Syntax = "HTML" },
                new MenuItemViewModel(action) { Header = "ASP/XHTML", Syntax = "ASP/XHTML" },
                new MenuItemViewModel(action) { Header = "JSON", Syntax = "Java" },
                new MenuItemViewModel(action) { Header = "Java", Syntax = "Java" },
                new MenuItemViewModel(action) { Header = "SQL", Syntax = "SQL" }
            };
        }

    }

    public class CommandViewModel : ICommand
    {
        private readonly Action _action;

        public CommandViewModel(Action action)
        {
            _action = action;
        }

        public void Execute(object o)
        {
            _action();
        }

        public bool CanExecute(object o)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
