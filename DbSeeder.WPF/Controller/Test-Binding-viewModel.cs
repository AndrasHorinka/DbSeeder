using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DbSeeder.WPF.Controller
{
    public class Test_Binding_viewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public ObservableCollection<FirstLevelObject> Items { get; set; }

        // Constructor
        public Test_Binding_viewModel()
        {
            Items = new ObservableCollection<FirstLevelObject>();
            SeedContent();
        }

        /// <summary>
        /// Seed Dictionary
        /// </summary>
        private void SeedContent()
        {
            for (int i = 0; i < 3; i = i+10)
            {
                var flo = new FirstLevelObject
                {
                    FieldName = $"key {i+3}",
                    FieldValue = i,
                    Children = new ObservableCollection<FirstLevelObject>()
                };
                        
                var flo1Sub1 = new FirstLevelObject
                {
                    FieldName = $"key {i + 1}",
                    FieldValue = i + 1
                };
                

                var flo1Sub2 = new FirstLevelObject
                {
                    FieldName = $"key {i + 2}",
                    FieldValue = i + 2
                };
                
                flo.Children.Add(flo1Sub1); 
                flo.Children.Add(flo1Sub2);
                Items.Add(flo);



                var flo2 = new FirstLevelObject
                {
                    FieldName = $"key {i + 3}",
                    FieldValue = "Alpha",
                    Children = new ObservableCollection<FirstLevelObject>()
                };

                var flo2Sub1 = new FirstLevelObject
                {
                    FieldName = $"key {i + 4}",
                    FieldValue = "Beta",
                };
                
                var flo2Sub2 = new FirstLevelObject
                {
                    FieldName = $"key {i + 5}",
                    FieldValue = "omega",
                };

                flo2.Children.Add(flo2Sub1);
                flo2.Children.Add(flo2Sub2);
                Items.Add(flo2);



                var flo3 = new FirstLevelObject
                {
                    FieldName = $"key {i + 6}",
                    FieldValue = 6.6,
                    Children = new ObservableCollection<FirstLevelObject>()
                };

                var flo3Sub1 = new FirstLevelObject
                {
                    FieldName = $"key {i + 7}",
                    FieldValue = 7.7
                };
                var flo3Sub2 = new FirstLevelObject
                {
                    FieldName = $"key {i + 8}",
                    FieldValue = 8.8
                };

                flo3.Children.Add(flo3Sub1);
                flo3.Children.Add(flo3Sub2);
                Items.Add(flo3);
            }
        }
    }

    public class FirstLevelObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public ObservableCollection<FirstLevelObject> Children { get; set; }

        public string FieldName { get; set; }
        public object FieldValue { get; set; }
    }
}
