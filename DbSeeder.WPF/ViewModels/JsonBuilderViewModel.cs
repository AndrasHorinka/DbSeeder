using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Data;

using DbSeeder.WPF.Delegates;

namespace DbSeeder.WPF.ViewModels
{
    /// <summary>
    /// This is the new ViewModel
    /// </summary>
    public class JsonBuilderViewModel : INotifyPropertyChanged
    {
        #region Property Changed Event

        /// <summary>
        ///  The event that is fired when an event occurs at property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Fields and Properties

        private readonly JsonFieldDelegate DeleteFieldHandler;
        private readonly JsonFieldDelegate CollapseFieldsHandler;
        private readonly JsonFieldDelegate AddJsonFieldHandler;
        private List<JsonFieldViewModel> AllJsonFields;

        private string queryName;
        /// <summary>
        /// Property to store the reference name of the query
        /// </summary>
        public string QueryName
        {
            get => queryName;
            set
            {
                // return if no change occurs
                if (queryName == value) return;
                if (value is null) return;

                queryName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QueryName)));
            }
        }

        private string defaultUri;
        /// <summary>
        /// The default URL with keys included
        /// </summary>
        public string DefaultUri
        {
            get => defaultUri;
            set
            {
                if (defaultUri == value) return;

                defaultUri = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DefaultUri)));
            }
        }

        private HttpMethod method;
        /// <summary>
        /// The method to be used for the seeding
        /// </summary>
        public HttpMethod Method
        {
            get => method;
            set
            {
                if (method == value) return;
                
                method = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Method)));
            }
        }

        /// <summary>
        /// Helper Property to seed ComboBox
        /// </summary>
        public IEnumerable<HttpMethod> AvailableHttpMethods =>
            new List<HttpMethod>
            {
                HttpMethod.Post,
                HttpMethod.Patch,
                HttpMethod.Put,
                HttpMethod.Delete
            };

        private ObservableCollection<JsonFieldViewModel> jsonFieldViewModels;
        /// <summary>
        /// Property to store each fields of the json
        /// </summary>
        public ObservableCollection<JsonFieldViewModel> JsonFieldViewModels
        {
            get => jsonFieldViewModels;
            set
            {
                if (value is null) return;

                if (value == jsonFieldViewModels) return;

                jsonFieldViewModels = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(JsonFieldViewModels)));
            }
        }

        private CollectionViewSource jsonFields;
        /// <summary>
        /// Property to store Json to be bound the UI to
        /// </summary>
        public ICollectionView JsonField => jsonFields.View;

        #endregion

        #region Constructor

        public JsonBuilderViewModel()
        {
            // Initialize NullableTypes
            jsonFieldViewModels = new ObservableCollection<JsonFieldViewModel>();
            ResetJsonField();
            AllJsonFields = new List<JsonFieldViewModel>();
            
            // Delegates
            DeleteFieldHandler = DeleteJsonField;
            CollapseFieldsHandler = CollapseJsonField;
            AddJsonFieldHandler = AddJsonField;

            // Initialize nonNullable Types
            method = HttpMethod.Post;

            // Initialise Samples
            GenerateSample();
        }

        #endregion

        #region Helper Methods

        private void ResetJsonField()
        {
            jsonFields = new CollectionViewSource();
            jsonFields.Source = JsonFieldViewModels;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JsonField)));
        }

        #endregion

        #region Sample Generation

        private void GenerateSample()
        {
            string[] keyTypes = {"Field", "Map", "Array"};
            string[] fieldTypes = {"string", "bool", "float", "int"};
            bool[] bools = {true, false};
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];

            Random rnd = new Random();

            for (int i = 1; i < 10; i++)
            {
                JsonFieldViewModel field = new JsonFieldViewModel(CollapseFieldsHandler, DeleteFieldHandler, AddJsonFieldHandler, null, null)
                {
                    KeyName = $"key lvl 1 {i}",
                    KeyType = keyTypes[rnd.Next(3)],
                };

                JsonFieldViewModels.Add(field);
                AllJsonFields.Add(field);

                if (!(string.Equals(field.KeyType, "Field", StringComparison.CurrentCultureIgnoreCase)))
                {
                    field.GenerateSample(i, 0);
                    continue;
                }

                field.FieldType = fieldTypes[rnd.Next(4)];
                field.IsUnique = bools[rnd.Next(2)];

                for (int s = 0; s < stringChars.Length; s++)
                {
                    stringChars[s] = chars[rnd.Next(chars.Length)];
                }

                field.Regex = new string(stringChars);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JsonFieldViewModels)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JsonField)));
        }

        #endregion

        #region Delegate Methods

        public void DeleteJsonField(JsonFieldViewModel jsonField)
        {
            if (jsonField is null) throw new ArgumentNullException();

            JsonFieldViewModels.Remove(jsonField);
            AllJsonFields.Remove(jsonField);
        }

        public void CollapseJsonField(JsonFieldViewModel jsonField)
        {
            if (jsonField is null) throw new ArgumentNullException();

            foreach (var field in AllJsonFields)
            {
                if (field == jsonField) continue;

                field.AddChildrenZoneIsVisible = false;
            }
        }

        public void AddJsonField(JsonFieldViewModel jsonField)
        {
            if (jsonField is null) throw new ArgumentNullException();

            AllJsonFields.Add(jsonField);
        }

        #endregion
    }
}
