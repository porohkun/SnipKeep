// Copyright Nick Polyak 2008

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiSelectTree
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        // a set of all selected items
        Dictionary<TreeViewItem, string> selectedItems =
            new Dictionary<TreeViewItem, string>();

        // true only while left ctrl key is pressed
        bool CtrlPressed
        {
            get
            {
                return System.Windows.Input.Keyboard.IsKeyDown(Key.LeftCtrl);
            }
        }

        public Window1()
        {
            InitializeComponent();

            MyTreeView.SelectedItemChanged += 
                new RoutedPropertyChangedEventHandler<object>(MyTreeView_SelectedItemChanged);

            MyTreeView.Focusable = true;
        }

        // deselects the tree item
        void Deselect(TreeViewItem treeViewItem)
        {
            treeViewItem.Background = Brushes.White;// change background and foreground colors
            treeViewItem.Foreground = Brushes.Black;
            selectedItems.Remove(treeViewItem); // remove the item from the selected items set
        }

        // changes the state of the tree item:
        // selects it if it has not been selected and
        // deselects it otherwise
        void ChangeSelectedState(TreeViewItem treeViewItem)
        {
            if (!selectedItems.ContainsKey(treeViewItem))
            { // select
                treeViewItem.Background = Brushes.Black; // change background and foreground colors
                treeViewItem.Foreground = Brushes.White;
                selectedItems.Add(treeViewItem, null); // add the item to selected items
            }
            else
            { // deselect
                Deselect(treeViewItem);
            }
        }

        void MyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem treeViewItem = MyTreeView.SelectedItem as TreeViewItem;

            if (treeViewItem == null)
                return;

            // prevent the WPF tree item selection 
            treeViewItem.IsSelected = false;

            treeViewItem.Focus();

            if (!CtrlPressed)
            {
                List<TreeViewItem> selectedTreeViewItemList = new List<TreeViewItem>();
                foreach (TreeViewItem treeViewItem1 in selectedItems.Keys)
                {
                    selectedTreeViewItemList.Add(treeViewItem1);
                }

                foreach (TreeViewItem treeViewItem1 in selectedTreeViewItemList)
                {
                    Deselect(treeViewItem1);
                }
            }

            ChangeSelectedState(treeViewItem);
        }
    }
}
