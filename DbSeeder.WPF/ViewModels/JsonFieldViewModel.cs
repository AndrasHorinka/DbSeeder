using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using DbSeeder.WPF.Delegates;
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

        public JsonFieldViewModel(JsonFieldDelegate collapseOtherFields, 
            JsonFieldDelegate deleteJsonField, // this is for root fields and to remove Root View Model AllJson collection for collapsing
            JsonFieldDelegate addJsonField,
            JsonFieldDelegate deleteChildDelegate = null, // this is to clear the collection of Owner if owner is not null
            JsonFieldViewModel owner = null) // this is for owner, only if it is not root
        {
            InitializeCommands();
            InitializeNullableTypes(owner);
            InitializeNonNullableTypes();

            // Initialize delegates
            InitializeDelegates(collapseOtherFields, deleteJsonField, addJsonField, deleteChildDelegate, owner);
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Property to store the delegate to delete root fields
        /// </summary>
        public JsonFieldDelegate DeleteFieldFromRootCollection { get; private set; }

        /// <summary>
        /// Property to store the delegate to collapse all other fields
        /// </summary>
        public JsonFieldDelegate CollapseAllOtherFields { get; private set; }

        /// <summary>
        /// A property to store a delegate passed from Parent - to be called on Deletion of THIS
        /// </summary>
        public JsonFieldDelegate DelegateToClearParentCollection { get; private set; }

        /// <summary>
        /// A property to store a delegate to be passed to Children - so they can delete themselves from Parent's collection
        /// </summary>
        public JsonFieldDelegate DelegateForChildrenToClearCollection { get; set; }
        /// <summary>
        /// A method to be used for delegate initialization - to be passed to Children
        /// </summary>
        /// <param name="jsonField"></param>
        private void DeleteJsonField(JsonFieldViewModel jsonField)
        {
            if (jsonField is null) throw new ArgumentNullException();

            Children.Remove(jsonField);
        }

        /// <summary>
        /// A property to store a delegate passed from Root - to be able to collect each JsonFields
        /// </summary>
        public JsonFieldDelegate AddJsonFieldDelegate { get; set; }

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

        private void InitializeCommands()
        {
            ShowAddChildrenZone = new RelayCommand(showAddChildrenZone);
            DiscardAddChildrenZone = new RelayCommand(discardAddChildrenZone);
            AddChildField = new RelayCommand(addChildField);
            DeleteField = new RelayCommand(deleteField);
        }

        private void InitializeNullableTypes(JsonFieldViewModel owner)
        {
            keyType = string.Empty;
            keyName = string.Empty;
            regex = string.Empty;
            fieldType = string.Empty;
            Owner = owner;
            Children = new ObservableCollection<JsonFieldViewModel>();
        }

        private void InitializeNonNullableTypes()
        {
            ChildCounter = 0;
            addChildrenZoneIsVisible = false;
            isVisible = true;
            isExpanded = false;
            isUnique = false;
            editedFieldZoneBackground = new SolidColorBrush(Colors.White);
        }

        private void InitializeDelegates(JsonFieldDelegate collapseOtherFields, JsonFieldDelegate deleteJsonField, JsonFieldDelegate addJsonField, JsonFieldDelegate deleteChildDelegate, JsonFieldViewModel owner)
        {
            // Injected
            if (deleteJsonField is null) throw new ArgumentNullException($"{nameof(DeleteFieldFromRootCollection)} delegate cannot be null");
            DeleteFieldFromRootCollection = deleteJsonField;
            if (collapseOtherFields is null) throw new ArgumentNullException($"{nameof(CollapseAllOtherFields)} delegate cannot be null");
            CollapseAllOtherFields = collapseOtherFields;
            if (addJsonField is null) throw new ArgumentNullException($"{nameof(AddJsonFieldDelegate)} delegate cannot be null");
            AddJsonFieldDelegate = addJsonField;

            // Local
            DelegateForChildrenToClearCollection = DeleteJsonField;

            if (owner != null)
            {
                if (deleteChildDelegate is null) throw new ArgumentNullException($"Root field of the Json needs to be passed with {nameof(JsonFieldDelegate)} delegate");

                DelegateToClearParentCollection = deleteChildDelegate;
            }
        }

        /// <summary>
        /// Increase counter by one up until root
        /// </summary>
        internal async void ChildAdded()
        {
            ChildCounter++;
            if (Owner != null) await Task.Run(() => Owner.ChildAdded());
        }

        /// <summary>
        /// Method that deletes the given field of the JSON
        /// </summary>
        /// <param name="child">JsonFieldViewModel</param>
        public void DeleteJson(JsonFieldViewModel child)
        {
            if (child is null) return;

            // Remove from Parent's collection
            if (Owner != null)
            {
                DelegateToClearParentCollection(this);
            }
        
            // Clear up root
            if (DeleteFieldFromRootCollection is null) throw new InvalidOperationException($"Wanted to delete a root field but {nameof(DeleteFieldFromRootCollection)} delegate is not initialized ");
            DeleteFieldFromRootCollection(this);
        }

        private bool IsFieldValid()
        {
            if (Child is null) return false;
            if (string.IsNullOrWhiteSpace(Child.KeyName)) return false;
            if (string.IsNullOrWhiteSpace(Child.KeyType)) return false;

            if (Child.KeyType.Equals("Field"))
            {
                if (string.IsNullOrWhiteSpace(Child.FieldType)) return false;
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
            Child = new JsonFieldViewModel(CollapseAllOtherFields, DeleteFieldFromRootCollection, AddJsonFieldDelegate, DelegateForChildrenToClearCollection, this);

            AddChildrenZoneIsVisible = true;
            CollapseAllOtherFields(this);

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
                Child.Owner = this;
                Children.Add(Child);
                ChildAdded();
                AddJsonFieldDelegate(Child);
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
            DeleteJson(this);
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

                JsonFieldViewModel field = new JsonFieldViewModel(CollapseAllOtherFields, DeleteFieldFromRootCollection, AddJsonFieldDelegate, DelegateForChildrenToClearCollection, this)
                {
                    KeyName = $"key lvl {index} - {index*10+i}",
                    KeyType = keyType
                };

                Children.Add(field);
                AddJsonFieldDelegate(field);

                if (!(string.Equals(field.KeyType, "Field", StringComparison.OrdinalIgnoreCase)))
                {
                    if (depth <= 4)
                    {
                        //await Task.Run(() => field.GenerateSample(i, ++depth));
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
