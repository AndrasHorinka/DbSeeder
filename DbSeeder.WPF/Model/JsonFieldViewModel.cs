using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using DbSeeder.WPF.Services;

namespace DbSeeder.WPF.Model
{
    public class JsonFieldViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///  The event that is fired when an event occurs at property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Fields and Properties

        private string _FieldName = string.Empty;
        /// <summary>
        /// Name of the key in the JSON
        /// </summary>
        public string FieldName
        {
            get { return _FieldName; }
            set
            {
                // return if no change occurs
                if (_FieldName == value) return;

                // update to new value and launch event
                _FieldName = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(FieldName)));
            }
        }

        private FieldTypes _FieldType;
        /// <summary>
        /// Type of the Key in the JSON, like field/dict/array
        /// </summary>
        public FieldTypes FieldType
        {
            get { return _FieldType; }
            set
            {
                // return if no change occurs
                if (_FieldType == value) return;

                // update to new value and launch event
                _FieldType = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(FieldType)));
            }
        }

        private string _ParentField = string.Empty;
        /// <summary>
        /// Name of the parent key in the JSON
        /// </summary>
        public string ParentField
        {
            get { return _ParentField; }
            set
            {
                // return if no change occurs
                if (_ParentField == value) return;

                // update to new value and launch event
                _ParentField = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ParentField)));
            }
        }

        private ObservableCollection<JsonFieldViewModel> _Children;
        /// <summary>
        /// Value of the key in the JSON, can be string/bol/int/long if field - or field/dict/array if dict/array
        /// </summary>
        public ObservableCollection<JsonFieldViewModel> Children
        {
            get { return _Children; }
            set
            {
                // return if no change occurs
                if (_Children == value) return;

                // update to new value and launch event
                _Children = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Children)));
            }
        }

        private bool _IsRegex = false;
        /// <summary>
        /// If FieldValue is auto-generated based on regex
        /// </summary>
        public bool IsRegex
        {
            get { return _IsRegex; }
            set
            {
                // return if no change occurs
                if (_IsRegex == value) return;

                // update to new value and launch event
                _IsRegex = !_IsRegex;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsRegex)));
            }
        }

        private string _RegexExpression;
        /// <summary>
        /// The regex expression
        /// </summary>
        public string RegexExpression
        {
            get { return _RegexExpression; }
            set
            {
                // return if no change occurs
                if (_RegexExpression == value) return;

                // update to new value and launch event
                _RegexExpression = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(RegexExpression)));
            }
        }

        private bool _IsUnique = false;
        /// <summary>
        /// If FieldValue must be unique
        /// </summary>
        public bool IsUnique
        {
            get { return _IsUnique; }
            set
            {
                // return if no change occurs
                if (_IsUnique == value) return;

                // update to new value and launch event
                _IsUnique = !_IsUnique;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsUnique)));
            }
        }

        // Indicates if this item is expanded
        public bool IsExpanded
        {
            get { return Children?.Count(f => f.CanExpand && f != null) > 0; }
            set
            {
                // if UI instructs to expand
                if (value)
                {
                    // Show each children
                    Expand();
                }
                // if UI instructs to collapse
                else ClearChildren();
            }
        }

        /// <summary>
        /// Indicates if this item can be expanded (in case of Map or Array)
        /// </summary>
        public bool CanExpand { get { return FieldType != FieldTypes.Field; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public JsonFieldViewModel(FieldTypes fieldType, string fieldName)
        {
            ExpandCommand = new RelayCommand(Expand);
            FieldType = fieldType;
            FieldName = fieldName;
        }

        #endregion

        #region Helper Methods

        // Reset Children
        private void ClearChildren()
        {
            this.Children = new ObservableCollection<JsonFieldViewModel>();

            // Show the expand arrow if FieldType is a map or array
            if (CanExpand) Children.Add(null);
        }

        // Expand this item to get all children
        private void Expand()
        {
            if (!CanExpand) return;
            // 45 min
            // TODO: How to add children? Its not a living code, but already there. 
            //Children = new ObservableCollection<JsonFieldViewModel>();
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to expand this item
        /// </summary>
        public ICommand ExpandCommand { get; set; }

        #endregion

        // Create a Form in WPF that represents add Fieldkknhjhijyc fg bgyudrtm
    }
}
