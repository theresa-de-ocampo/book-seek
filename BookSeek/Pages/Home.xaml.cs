using System;
using System.Windows;
using System.Windows.Controls;

namespace BookSeek.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void PosButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Pages/Pos.xaml", UriKind.Relative));
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Pages/Inventory.xaml", UriKind.Relative));
        }

        private void SalesButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("Pages/Audits.xaml", UriKind.Relative));
        }
    }
}