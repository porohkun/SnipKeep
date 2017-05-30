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

namespace SnipKeep
{
    /// <summary>
    /// Interaction logic for TagsControl.xaml
    /// </summary>
    public partial class TagsControl : UserControl
    {
        public static readonly DependencyProperty TagsProperty =
          DependencyProperty.Register("Tags", typeof(List<Label>), typeof(TagsControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTagsPropertyChanged)));

        public List<Label> Tags { get; set; }

        
        public TagsControl()
        {
            InitializeComponent();
        }

        private static void OnTagsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tags = d as TagsControl;
            
            tags.SetValue(TagsProperty, e.NewValue);
            tags.Tags = (List<Label>)e.NewValue;
        }

    }
}
