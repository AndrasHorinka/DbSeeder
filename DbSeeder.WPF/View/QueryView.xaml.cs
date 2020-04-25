using DbSeeder.WPF.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DbSeeder.WPF.View
{
    /// <summary>
    /// Interaction logic for QueryView.xaml
    /// </summary>
    public partial class QueryView : Window
    {

        #region Fields and Properties
        private string KeyType { get; set; }
        private string FieldType { get; set; }
        protected IDictionary<string, object> JsonFields { get; set; }

        #endregion

        #region Constructors

        public QueryView()
        {
            InitializeComponent();
            DataContext = new QueryViewModel();

            //var x = new Dictionary<string, string>()
            //{
            //    {"subKey 4a", "SubKeyType 4a" },
            //    {"subKey 4b", "SubKeyType 4b" }
            //};

            //var y = new List<string>()
            //{
            //    "subList5a",
            //    "subList5b",
            //};

            //JsonFields = new Dictionary<string, object>()
            //{
            //    { "key1", "keyType1" },
            //    { "key2", "keyType2" },
            //    { "key3", "keyType3" },
            //    { "key4", x },
            //    { "key5", y }
            //};
        }
        #endregion

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var counter = 0;
            //foreach (var key in JsonFields.Keys)
            //{
            //    counter++;

            //    var item = new TreeViewItem()
            //    {
            //        Header = key,
            //        IsExpanded = true,
            //        ClipToBounds = true
            //    };

            //    if (counter % 2 == 0)
            //    {
            //        var subItem = new TreeViewItem()
            //        {
            //            Header = $"SubKey of {item.Header}",
            //            IsExpanded = true,
            //            ClipToBounds = true
            //        };

            //        item.Items.Add(subItem);
            //    }

            //    item.Expanded += Key_Expanded;

            //    JsonView.Items.Add(item);
            //}
        }

        private void Key_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
        }

        #endregion

        private void FieldTypeSelectors_Checked(object sender, RoutedEventArgs e)
        {
            KeyType = ((RadioButton)sender).Content.ToString();
            if (KeyType != "Field")
            {
                FieldType = null;
            }
        }
    }
}
