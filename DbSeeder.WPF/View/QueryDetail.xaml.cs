using System;
using System.Windows;
using System.Windows.Controls;

namespace DbSeeder.WPF.View
{
    /// <summary>
    /// Interaction logic for QueryDetail.xaml
    /// </summary>
    public partial class QueryDetail : Window
    {
        public Window ParentWindow { get; set; }
        public string QueryName { get; set; }
        public DateTime Now { get; set; }

        private const string UriExample = @"Example: https://localhost:5001/{uriParam1}/{uriParam2}";

        public QueryDetail(Window parent)
        {
            ParentWindow = parent;
            ParentWindow.Visibility = Visibility.Hidden;
            InitializeComponent();
            DateField.Text = DateTime.Now.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ParentWindow.Visibility = Visibility.Visible;
        }



        private void Uri_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = UriExample;
            }
        }
    }
}
