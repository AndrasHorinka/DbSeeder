using DbSeeder.WPF.View;
using System;
using System.Diagnostics;
using System.Windows;

namespace DbSeeder.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
            }

        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/AndrasHorinka/DbSeeder",
                UseShellExecute = true
            };
            Process.Start(psi);

        }

        private void NewQueryButton_Click(object sender, RoutedEventArgs e)
        {
            var queryDetail = new QueryDetail(this);
            queryDetail.ShowDialog();
        }
    }
}
