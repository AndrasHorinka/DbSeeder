using DbSeeder.WPF.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DbSeeder.WPF.View
{
    /// <summary>
    /// Interaction logic for AddField.xaml
    /// </summary>
    public partial class AddField : Window
    {
        private string KeyType { get; set; }
        private string FieldType { get; set; }
        private Window ParentWindow { get; set; }

        // TODO: instead of parent ParentWindow should create an object that stores fields --> and that should be passed
        public AddField(Window parent)
        {
            ParentWindow = parent;
            ParentWindow.Visibility = Visibility.Hidden;
            InitializeComponent();
        }

        /// <summary>
        /// Exit from current window without saving it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ParentWindow.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// Saves the entry made and return it to Parent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Entry(object sender, RoutedEventArgs e)
        {

            // ERROR CHECK - if not all mandatory fields are required
            if (string.IsNullOrWhiteSpace(KeyType) ||
                string.IsNullOrWhiteSpace(KeyName.Text) ||
                (KeyType.Equals("Field") && string.IsNullOrWhiteSpace(FieldType))
                )
            {
                ErrorMsgSlot.Foreground = new SolidColorBrush(Color.FromRgb(255, 228, 220));
                ErrorMsgSlot.Text = $"Cannot save - fields are invalid!";
                ErrorMsgSlot.Visibility = Visibility.Visible;

                DispatcherTimer time = new DispatcherTimer();
                time.Interval = TimeSpan.FromSeconds(2);
                time.Start();
                time.Tick += delegate
                {
                    ErrorMsgSlot.Visibility = Visibility.Hidden;
                    time.Stop();
                };
            }

            switch (KeyType)
            {
                case "String":
                    var jsonFieldString = new JsonField<string>(KeyName.Text, string.Empty);
                    break;

                case "Boolean":
                    var jsonFieldBool = new JsonField<bool>(KeyName.Text, true);
                    break;

                case "Integer":
                    var jsonFieldInt = new JsonField<int>(KeyName.Text, 0);
                    break;

                case "Float":
                    var jsonFieldFloat = new JsonField<float>(KeyName.Text, 0);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Store which KeyType selector is active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeySelectors_Checked(object sender, RoutedEventArgs e)
        {
            KeyType = ((RadioButton)sender).Content.ToString();
            if (KeyType != "Field")
            {
                FieldType = null;
            }
        }

        /// <summary>
        /// Store which FieldType selector is active, if KeyType is Field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FieldTypeSelectors_Checked(object sender, RoutedEventArgs e)
        {
            FieldType = ((RadioButton)sender).Content.ToString();
        }
    }
}
