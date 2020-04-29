using DbSeeder.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;

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
            NewField = new JsonFieldViewModel();
            AddNewJsonFieldCommand = new RelayCommand(Add_Item);
            ToggleAddFieldZoneVisibility = new RelayCommand(Toggle_AddFieldVisibility);
            Keys = new List<string>
            {
                "root"
            };

            GenerateSampleData();
        }

        #endregion

        #region Fields and Properties

        private IList<string> _Keys;
        public IList<string> Keys
        {
            get
            {
                return _Keys;
            }
            set
            {
                if (_Keys == value) return;

                _Keys = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Keys)));
            }
        }

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
            private set
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

        private bool _AddFieldZoneVisibility = false;
        /// <summary>
        /// Visibility controller for AddFieldZone
        /// </summary>
        public bool AddFieldZoneVisibility
        {
            get
            {
                return _AddFieldZoneVisibility;
            }
            set
            {
                if (_AddFieldZoneVisibility == value) return;

                _AddFieldZoneVisibility = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AddFieldZoneVisibility)));
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
            NewField.IsUnique = !NewField.IsUnique;
        }

        private void Regex_Checked()
        {
            NewField.IsRegex = !NewField.IsRegex;
        }

        #endregion

        #region Commands

        public ICommand AddNewJsonFieldCommand { get; set; }

        private void Add_Item()
        {
            if (NewField.ParentField.Equals("root", StringComparison.CurrentCultureIgnoreCase))
            {
                Items.Add(NewField);
            }
            else
            {
                // iterate through items of Items
                foreach (JsonFieldViewModel item in Items)
                {
                    // if given item FieldName equals to assigned parentField -- add the new field nested under it
                    if (item.FieldName.Equals(NewField.ParentField, StringComparison.CurrentCultureIgnoreCase))
                    {
                        item.Children.Add(NewField);
                    }
                    else continue;
                }
            }

            // Store the new Key to be able to show it in Parent DropDown selector
            Keys.Add(NewField.FieldName);

            // Reset NewField
            NewField = new JsonFieldViewModel();
            // Trigger PropertyChanged of Items
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Items)));
        }
        
        public ICommand ToggleAddFieldZoneVisibility { get; set; }

        private void Toggle_AddFieldVisibility()
        {
            AddFieldZoneVisibility = !AddFieldZoneVisibility;
        }
        #endregion

        private void GenerateSampleData()
        {
            for (var i = 0; i < 18; i = i+3)
            {
                // Simple Field
                NewField = new JsonFieldViewModel(FieldTypes.Field, $"Key {i}");
                Add_Item();

                // Map
                NewField = new JsonFieldViewModel(FieldTypes.Map, $"Key {i+1}");
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

                NewField.Children.Add(contentForJsonField2a);
                NewField.Children.Add(contentForJsonField2b);
                NewField.Children.Add(contentForJsonField2c);

                Add_Item();

                // Array
                NewField = new JsonFieldViewModel(FieldTypes.Array, $"Key {i+2}");
                    // Field for Array
                var contentForJsonField3a = new JsonFieldViewModel(FieldTypes.Field, $"Field {i + 2} - subField");
                    // Array for array
                var contentForJsonField3b = new JsonFieldViewModel(FieldTypes.Array, $"Field {i + 2} - subArray");

                NewField.Children.Add(contentForJsonField3a);
                NewField.Children.Add(contentForJsonField3b);

                Add_Item();

                NewField = new JsonFieldViewModel();
            }

        }
    }
}
