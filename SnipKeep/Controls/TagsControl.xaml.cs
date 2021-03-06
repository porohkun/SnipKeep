﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
          DependencyProperty.Register("Tags", typeof(List<Label>), typeof(TagsControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTagsPropertyChanged)) { BindsTwoWayByDefault = true });

        public List<Label> Tags { get; set; }

        public event Action<Label> TagAdded;
        public event Action<Label> TagRemoved;

        public TagsControl()
        {
            InitializeComponent();
        }

        private static void OnTagsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tags = d as TagsControl;

            tags.SetValue(TagsProperty, e.NewValue);
            tags.Tags = (List<Label>)e.NewValue;

            tags.RefreshTags();
        }

        private void RefreshTags()
        {
            if (Tags == null) return;
            for (var i = 0; i < Tags.Count; i++)
            {
                if (i < panel.Children.Count - 1)
                    SetTagToButton(panel.Children[i] as Button, Tags[i]);
                else
                    panel.Children.Insert(i, NewButton(Tags[i]));
            }
            while (Tags.Count < panel.Children.Count - 1)
                panel.Children.RemoveAt(panel.Children.Count - 2);
            newTagBox.Editor.Text = "";
        }

        private void SetTagToButton(Button button, Label label)
        {
            button.Content = label.Name;
            button.Tag = label;
        }

        private Button NewButton(Label label)
        {
            var button = new Button();
            button.Margin = new Thickness(3);
            button.Loaded += Button_Loaded;
            button.Click += Button_Click;
            SetTagToButton(button, label);
            button.ContextMenu = Resources["popup"] as ContextMenu;
            return button;
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            ((Border)button.Template.FindName("border", button)).CornerRadius = new CornerRadius(4);
            ((ContentPresenter)button.Template.FindName("contentPresenter", button)).Margin = new Thickness(3, 0, 3, 0);
        }

        private void ButtonPopup_Click(object sender, RoutedEventArgs e)
        {
            TagRemoved?.Invoke((((sender as MenuItem).Parent as ContextMenu).PlacementTarget as Button).Tag as Label);
            RefreshTags();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var popup = (sender as Button).ContextMenu;
            popup.PlacementTarget = (UIElement)sender;
            popup.Visibility = Visibility.Visible;
            popup.IsOpen = true;
        }

        private void newTagBox_Loaded(object sender, RoutedEventArgs e)
        {
            newTagBox.Editor.TextChanged += Editor_TextChanged;
            newTagBox.Editor.KeyDown += Editor_KeyDown;
        }

        private void ProcessTags(string text)
        {
            var tags = text.Split(',', ';', ':', ' ', '/', '\\', '|').Distinct();
            foreach (var tag in tags)
                if (!string.IsNullOrEmpty(tag))
                {
                    var lab = Label.GetLabelByName(tag);
                    TagAdded?.Invoke(lab);
                }
            Label.Labels.Sort();
            RefreshTags();
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Tags != null)
            {
                var text = ((TextBox)sender).Text;
                if (Regex.IsMatch(text, "[,;:/\\| ]", RegexOptions.IgnoreCase))
                {
                    ProcessTags(text);
                }
            }
        }

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessTags(((TextBox)sender).Text);
            }
        }

        private void newTagBox_SelectionAdapterCommit()
        {
            ProcessTags(newTagBox.Editor.Text);
        }
    }
}
