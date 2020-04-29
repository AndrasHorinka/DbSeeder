using DbSeeder.WPF.Model;
using System.Windows;

namespace DbSeeder.WPF.View
{
    /// <summary>
    /// Interaction logic for QueryView.xaml
    /// </summary>
    public partial class QueryView : Window
    {
        #region Constructors

        public QueryView()
        {
            InitializeComponent();
            DataContext = new QueryViewModel();
        }

        #endregion
    }
}
