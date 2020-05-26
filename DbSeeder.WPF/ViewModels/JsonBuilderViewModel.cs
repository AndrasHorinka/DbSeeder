using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Data;

using static System.DateTime;

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

        /// <summary>
        /// Snapshot of time when window was opened
        /// </summary>
        public DateTime CurrentMoment => Now;

        private string queryName = string.Empty;
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(JsonFieldViewModels)));
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
            #region Initialize NullableTypes

            JsonFieldViewModels = new ObservableCollection<JsonFieldViewModel>();
            ResetJsonField();

            #endregion
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
    }

}
