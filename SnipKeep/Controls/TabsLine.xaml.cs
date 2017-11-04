using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnipKeep.Controls
{
    /// <summary>
    /// Interaction logic for TabsLine.xaml
    /// </summary>
    public partial class TabsLine : ListView
    {
        [Category("Behavior")]
        public event RoutedEventHandler NewTabClick;

        //public static readonly DependencyProperty TagsProperty =
        //    DependencyProperty.Register("Tags", typeof(object), typeof(TabsLine), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTagsPropertyChanged)) { BindsTwoWayByDefault = true });

        //public object Tags { get; set; }

        public TabsLine()
        {
            InitializeComponent(); 
        }

        private void NewTabClick_OnClick(object sender, RoutedEventArgs e)
        {
            NewTabClick?.Invoke(this, e);
        }
    }
}
