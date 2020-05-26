using System.Windows;
using DbSeeder.WPF.ViewModels;

namespace DbSeeder.WPF.View
{
    /// <summary>
    /// Interaction logic for JSONBuilderView.xaml
    /// </summary>
    public partial class JSONBuilderView : Window
    {
        public JSONBuilderView()
        {
            InitializeComponent();
            DataContext = new JsonBuilderViewModel();
        }
    }
}
