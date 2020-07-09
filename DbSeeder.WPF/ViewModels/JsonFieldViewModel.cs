using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using DbSeeder.WPF.Services;

namespace DbSeeder.WPF.ViewModels
{
    public class JsonFieldViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///  The event that is fired when an event occurs at property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Constructor

        public JsonFieldViewModel(JsonFieldViewModel owner = null)
        {
            // Initialize Commands
            ShowAddChildrenZone = new RelayCommand(showAddChildrenZone);
            DiscardAddChildrenZone = new RelayCommand(discardAddChildrenZone);
            AddChildField = new RelayCommand(addChildField);
            DeleteField = new RelayCommand(deleteField);

            // Initialize Nullable Types
            keyType = string.Empty;
            keyName = string.Empty;
            regex = string.Empty;
            fieldType = string.Empty;
            Owner = owner;
            Children = new ObservableCollection<JsonFieldViewModel>();

            // Initialize nonNullable Types
            ChildCounter = 0;
            addChildrenZoneIsVisible = false;
            isVisible = true;
            isExpanded = false;
            isUnique = false;
            editedFieldZoneBackground = new SolidColorBrush(Colors.White);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property to store Parent/Owner of given child. Can be null - if so it is in the root.
        /// </summary>
        internal JsonFieldViewModel Owner { get; set; }

        private ObservableCollection<JsonFieldViewModel> children;
        /// <summary>
        /// Collection to store Child elements of given Field. Cannot be null (initialized in constructor) - but can store null - if no child added
        /// </summary>
        public ObservableCollection<JsonFieldViewModel> Children
        {
            get => children;
            private set 
            {
                if (!CanExpand) return;

                if (value is null) return;

                if (value == children) return;

                children = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Children)));
            }
        }

        /// <summary>
        /// Counter to store how many children are nested below
        /// </summary>
        public int ChildCounter { get; internal set; }

        private string keyName;
        /// <summary>
        /// Property to store the name of the given key
        /// </summary>
        public string KeyName
        {
            get => keyName;
            set
            {
                // False checks
                if (value is null) return;
                if (value.ToString().Equals(keyName)) return;

                keyName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KeyName)));
            }
        }

        private string keyType;
        /// <summary>
        /// Property to store the type of the key (Field / Map / Array)
        /// </summary>
        public string KeyType
        {
            get => keyType;
            set
            {
                // False checks
                if (value is null) return;
                if (value.ToString().Equals(keyType)) return;

                keyType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(KeyType)));
            }
        }

        private string regex;
        /// <summary>
        /// Property to store regular expression of given field - if KeyType is field.
        /// </summary>
        public string Regex
        {
            get => regex;
            set
            {
                // False checks
                if (value is null) return;
                if (value.ToString().Equals(regex)) return;

                regex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Regex)));
            }
        }

        private string fieldType;
        /// <summary>
        /// Property to store the type of field - if KeyType is field.
        /// Possible types are: string, bool, int, float
        /// This is obsolete if KeyType is Map or Array
        /// </summary>
        public string FieldType
        {
            get => fieldType;
            set
            {
                // False checks
                if (value is null) return;
                if (value.ToString().Equals(fieldType)) return;

                // TODO: Check if value is of allowed type (string, bool, int, float)
                fieldType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FieldType)));
            }
        }

        private bool isUnique;
        /// <summary>
        /// Property to store if the given field should be unique - if KeyType is field.
        /// This is obsolete if KeyType is Map or Array
        /// </summary>
        public bool IsUnique
        {
            get => isUnique;
            set
            {
                // False check
                if (value == isUnique) return;

                isUnique = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsUnique)));
            }
        }

        private bool isVisible;
        /// <summary>
        /// Property to store if the given field should be visible or not
        /// </summary>
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible == value) return;

                isVisible = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsVisible)));
            }
        }

        private bool isExpanded;
        /// <summary>
        ///  Property to store if the given field should be expanded or not
        /// </summary>
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (isExpanded == value) return;

                isExpanded = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
            }
        }

        /// <summary>
        /// Property to store if the item can be expanded, which is true if it has any children
        /// </summary>
        public bool CanExpand
        {
            get
            {
                return !keyType.Equals("Field", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        private JsonFieldViewModel child;
        /// <summary>
        /// Property to related information about a new Child
        /// </summary>
        public JsonFieldViewModel Child
        {
            get => child;
            set
            {
                if (value is null) return;
                if (value == child) return;

                child = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Child)));
            }
        }

        #endregion

        #region Backend methods

        /// <summary>
        /// Increase counter by one up until root
        /// </summary>
        internal async void ChildAdded()
        {
            ChildCounter++;
            if (Owner != null) await Task.Run(() => Owner.ChildAdded());
        }

        public void DeleteChild(JsonFieldViewModel child)
        {
            if (child is null) return;

            if (Owner is null)
            {
                // Develop an observer to that will remove the element
            }
            else
            {
                // TODO: replace it with Observer method call
                Owner.Children.Remove(child);
            }
        }

        private bool IsFieldValid()
        {
            if (Child is null) return false;
            if (string.IsNullOrWhiteSpace(KeyName)) return false;
            if (string.IsNullOrWhiteSpace(KeyType)) return false;

            if (KeyType.Equals("Field"))
            {
                if (string.IsNullOrWhiteSpace(FieldType)) return false;
            }

            return true;
        }

        #endregion

        #region AddChildrenUIControl

        private SolidColorBrush editedFieldZoneBackground;
        /// <summary>
        /// Property to flag if given JSON field is being edited
        /// </summary>
        public SolidColorBrush EditedFieldZoneBackground
        {
            get => editedFieldZoneBackground;
            set
            {
                if (value == editedFieldZoneBackground) return;

                editedFieldZoneBackground = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(EditedFieldZoneBackground)));

            }
        }

        /// <summary>
        /// Property to handle the visibility of Add New Child button
        /// </summary>
        public bool NewChildButtonVisiblity
        {
            get
            {
                return CanExpand && !AddChildrenZoneIsVisible;
            }
        }

        private bool addChildrenZoneIsVisible;
        /// <summary>
        /// Property to flag if AddChildrenZone should be visible or not
        /// </summary>
        public bool AddChildrenZoneIsVisible
        {
            get
            {
                if (!CanExpand) return false;

                return addChildrenZoneIsVisible;
            }
            set
            {
                if (value == addChildrenZoneIsVisible) return;

                addChildrenZoneIsVisible = value;
                if (value)
                {
                    MarkFieldEdited();
                }
                else
                {
                    UnMarkFieldEdited();
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddChildrenZoneIsVisible)));
            }
        }

        /// <summary>
        /// Command that shows AddChildrenZone
        /// </summary>
        public ICommand ShowAddChildrenZone { get; set; }
        private void showAddChildrenZone()
        {
            Child = new JsonFieldViewModel()
            {
                Owner = this,
            };

            AddChildrenZoneIsVisible = true;
            if (Owner != null)
            {
                foreach (var sibling in Owner.Children)
                {
                    if (sibling is null) continue;

                    if (sibling.Equals(this)) continue;
                    sibling.AddChildrenZoneIsVisible = false;
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewChildButtonVisiblity)));
        }

        /// <summary>
        /// Command that hides AddChildrenZone
        /// </summary>
        public ICommand DiscardAddChildrenZone { get; set; }
        private void discardAddChildrenZone()
        {
            AddChildrenZoneIsVisible = false;
            child = null;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewChildButtonVisiblity)));
        }

        /// <summary>
        /// Command that add a new field as child to the selected element
        /// </summary>
        public ICommand AddChildField { get; set; }
        private void addChildField()
        {
            if (IsFieldValid())
            {
                Children.Add(Child);
                ChildAdded();
            }
            
            discardAddChildrenZone();
            return;
        }

        /// <summary>
        /// Command that removes certain field and all of its children
        /// </summary>
        public ICommand DeleteField { get; set; }
        private void deleteField()
        {
            DeleteChild(this);
            // should take an argument which field it is to be deleted
        }

        private void MarkFieldEdited()
        {
            EditedFieldZoneBackground = new SolidColorBrush(Colors.AntiqueWhite);
        }

        private void UnMarkFieldEdited()
        {
            EditedFieldZoneBackground = new SolidColorBrush(Colors.White);
        }

        #endregion

        #region Sample Generation

        internal void GenerateSample(int index, int depth)
        {
            string[] keyTypes = { "Field", "Map", "Array" };
            string[] fieldTypes = { "string", "bool", "float", "int" };
            bool[] bools = { true, false };
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];

            Random rnd = new Random();

            for (int i = 0; i < index; i++)
            {
                keyType = depth <= 4 ? keyTypes[rnd.Next(3)] : "Field";

                JsonFieldViewModel field = new JsonFieldViewModel(this)
                {
                    KeyName = $"key lvl {index} - {index*10+i}",
                    KeyType = keyType
                };

                Children.Add(field);

                if (!(string.Equals(field.KeyType, "Field", StringComparison.OrdinalIgnoreCase)))
                {
                    if (depth <= 4)
                    {
                        field.GenerateSample(i, ++depth);
                    }
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
        }

        #endregion
    }
}
