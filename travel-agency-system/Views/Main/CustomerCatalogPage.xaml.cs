using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using travel_agency_system.Models;

namespace travel_agency_system.Views.Main
{
    /// <summary>
    /// Interaction logic for CustomerCatalogPage.xaml
    /// </summary>
    public partial class CustomerCatalogPage : Page
    {
        public CustomerCatalogPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => {
                var testData = new List<TravelPackage>
        {
            new MountainTour(Guid.NewGuid(), "Test Mountain", 500, "Desc", TimeSpan.FromDays(5), DateTime.Now, true),
            new BeachTour(Guid.NewGuid(), "Test Beach", 300, "Desc", TimeSpan.FromDays(7), DateTime.Now, "Sandy")
        };

                ToursList.ItemsSource = testData;

                // Перевірка в консолі: якщо виведе 0, значить дані не прийшли
                Console.WriteLine($"Items count: {ToursList.Items.Count}");
            };
        }
    }
}
