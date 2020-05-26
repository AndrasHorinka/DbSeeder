using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public JsonFieldViewModel()
        {
            #region Initialize Commands

            ShowAddChildrenZone = new RelayCommand(showAddChildrenZone);
            DiscardAddChildrenZone = new RelayCommand(discardAddChildrenZone);
            AddChildField = new RelayCommand(addChilField);
            DeleteField = new RelayCommand(deleteField);

            #endregion

            #region Initialize Nullable Types

            Children = new ObservableCollection<JsonFieldViewModel>()
            {
                null
            };

            #endregion

            #region Initialize nonNullable Types

            ChildCounter = 0;
            AddChildrenZoneIsVisible = true; // Change it to false

            #endregion

            #region Initialize Samples



            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property to store Parent/Owner of given child. Can be null - if so it is in the root.
        /// </summary>
        internal JsonFieldViewModel Owner { get; set; }

        /// <summary>
        /// Collection to store Child elements of given Field. Cannot be null (initialized in constructor) - but can store null - if no child added
        /// </summary>
        public ObservableCollection<JsonFieldViewModel> Children { get; set; }

        /// <summary>
        /// Counter to store how many children are nested below
        /// </summary>
        public int ChildCounter { get; private set; }

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
        /// Property to store regular expression of given field
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

        #endregion

        #region AddChildrenUIControl

        private bool addChildrenZoneIsVisible;
        /// <summary>
        /// Property to flag if AddChildrenZone should be visible or not
        /// </summary>
        public bool AddChildrenZoneIsVisible
        {
            get => addChildrenZoneIsVisible;
            set
            {
                if (value == addChildrenZoneIsVisible) return;

                addChildrenZoneIsVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AddChildrenZoneIsVisible)));
            }
        }

        /// <summary>
        /// Command that shows AddChildrenZone
        /// </summary>
        public ICommand ShowAddChildrenZone;
        private void showAddChildrenZone()
        {
            AddChildrenZoneIsVisible = true;
        }

        /// <summary>
        /// Command that hides AddChildrenZone
        /// </summary>
        public ICommand DiscardAddChildrenZone;
        private void discardAddChildrenZone()
        {
            AddChildrenZoneIsVisible = false;
            ChildFieldName = null;
            ChildRegex = null;
            ChildIsUnique = false;
        }

        /// <summary>
        /// Command that add a new field as child to the selected element
        /// </summary>
        public ICommand AddChildField;
        private void addChilField()
        {
            // should take an argument which is the owner field
            JsonFieldViewModel newField = new JsonFieldViewModel()
            {
                Owner = this,
            };

        }

        /// <summary>
        /// Command that removes certain field and all of its children
        /// </summary>
        public ICommand DeleteField;
        private void deleteField()
        {
            // should take an argument which field it is to be deleted
        }



        /// <summary>
        /// Property to store the name of the newField
        /// </summary>
        public string ChildFieldName { get; set; }

        /// <summary>
        /// Property to store the regular expression
        /// </summary>
        public string ChildRegex { get; set; }

        /// <summary>
        /// Property to store if value must be unique or not
        /// </summary>
        public bool ChildIsUnique { get; set; } = false;



        #endregion
    }
}
