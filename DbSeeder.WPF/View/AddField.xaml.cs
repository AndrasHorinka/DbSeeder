using System.Windows;

namespace DbSeeder.WPF.View
{
    /// <summary>
    /// Interaction logic for AddField.xaml
    /// </summary>
    public partial class AddField : Window
    {
        public Window ParentWindow { get; set; }

        // TODO: instead of parent ParentWindow should create an object that stores fields --> and that should be passed
        public AddField(Window parent)
        {
            ParentWindow = parent;
            ParentWindow.Visibility = Visibility.Hidden;
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ParentWindow.Visibility = Visibility.Visible;
        }
    }
}
