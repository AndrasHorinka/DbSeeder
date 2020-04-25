using DbSeeder.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;

/// <summary>
/// DirectoryStructure are the methods to retrieve children, fix name - it stores a list of DirectoryItems
/// DirectoryItemType - is an enum - storing the Drive/folder/file --> like Method
/// A directoryItem stands for the Query -- 
///     storing info about the query and the methods to be executed independent from the UI
///     It stores DirectoryItemType (a.k.a Method), URI
///     
/// </summary>
namespace DbSeeder.WPF.Model
{
    /// <summary>
    /// A view model for each Queries
    /// </summary>
    public class QueryViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///  The event that is fired when an event occurs at property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public QueryViewModel()
        {
            Items = new ObservableCollection<JsonFieldViewModel>();
            SetMethodCommand = new RelayCommand(Method_Set);
            RegexCheckedCommand = new RelayCommand(Regex_Checked);
            UniqueCheckedCommand = new RelayCommand(Unique_Checked);

            GenerateSampleData();
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Snapshot of time when window was opened
        /// </summary>
        public DateTime CurrentMoment
        {
            get
            {
                return DateTime.Now;
            }
        }

        private string _QueryName = "Give your reference name to the query";
        /// <summary>
        /// The reference name for the query - given by the user
        /// </summary>
        public string QueryName
        {
            get
            {
                return _QueryName;
            }
            set
            {
                // return if no change occurs
                if (_QueryName == value) return;

                _QueryName = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(QueryName)));
            }
        }

        private ObservableCollection<JsonFieldViewModel> _Items;
        /// <summary>
        /// A list of each keys in the JSON, their types and their values
        /// </summary>
        public ObservableCollection<JsonFieldViewModel> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                // return if no change occurs
                if (_Items == value) return;

                _Items = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Items)));
            }
        }

        private string _DefaultUri;
        /// <summary>
        /// The default URL with keys included
        /// </summary>
        public string DefaultUri
        {
            get
            {
                return _DefaultUri;
            }
            set
            {
                if (_DefaultUri == value) return;

                _DefaultUri = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(DefaultUri)));
            }
        }

        private HttpMethod _Method;
        /// <summary>
        /// The method to be used for the seeding
        /// </summary>
        public HttpMethod Method
        {
            get
            {
                return _Method;
            }
            set
            {
                if (_Method == value) return;

                _Method = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Method)));
            }
        }

        /// <summary>
        /// The new Field to be added to 
        /// </summary>
        public JsonFieldViewModel NewField { get; set; }

        #endregion

        #region Helper Functions

        private void Unique_Checked()
        {

        }

        private void Regex_Checked()
        {

        }

        private void Method_Set()
        {

        }

        #endregion

        #region Commands

        public ICommand SetMethodCommand { get; set; }
        public ICommand RegexCheckedCommand { get; set; }
        public ICommand UniqueCheckedCommand { get; set; }

        #endregion

        private void GenerateSampleData()
        {
            for (var i = 0; i < 18; i = i+3)
            {
                // Simple Field
                var jsonField = new JsonFieldViewModel(FieldTypes.Field, $"Key {i}");

                // Map
                var jsonField2 = new JsonFieldViewModel(FieldTypes.Map, $"Key {i+1}");
                    // Map for map
                var contentForJsonField2a = new JsonFieldViewModel(FieldTypes.Map, $"Field {i+1} - subMap");
                    // Regex Field for Map
                var contentForJsonField2b = new JsonFieldViewModel(FieldTypes.Field, $"Field {i + 1} - subField")
                {
                    IsRegex = true,
                    RegexExpression = "[aA-zZ]*"
                };
                    // Unique field for Map
                var contentForJsonField2c = new JsonFieldViewModel(FieldTypes.Field, $"Field {i + 1} - submap - subfield")
                {
                    IsUnique = true
                };

                jsonField2.Children.Add(contentForJsonField2a);
                jsonField2.Children.Add(contentForJsonField2b);
                jsonField2.Children.Add(contentForJsonField2c);
                
                // Array
                var jsonField3 = new JsonFieldViewModel(FieldTypes.Array, $"Key {i+2}");
                    // Field for Array
                var contentForJsonField3a = new JsonFieldViewModel(FieldTypes.Field, $"Field {i + 2} - subField");
                    // Array for array
                var contentForJsonField3b = new JsonFieldViewModel(FieldTypes.Array, $"Field {i + 2} - subArray");

                jsonField3.Children.Add(contentForJsonField3a);
                jsonField3.Children.Add(contentForJsonField3b);
            }

        }
    }
}
