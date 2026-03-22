using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using travel_agency_system.Interfaces;
using travel_agency_system.Models;
using travel_agency_system.Services;

namespace travel_agency_system.Views.Main
{
    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private readonly TourManager _tourManager = new TourManager();
        private ObservableCollection<TravelPackage> _tours = new ObservableCollection<TravelPackage>();
        public AdminPage()
        {
            InitializeComponent();
        }

        private async void BtnCreateTour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(TxtTourName.Text) || !double.TryParse(TxtTourPrice.Text, out double price))
                {
                    MessageBox.Show("Please enter a valid Tour Name and Price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                ITourBuilder builder = new TourBuilder();

                builder.SetBaseInfo(TxtTourName.Text, price, TxtDescription.Text)
                       .SetTiming(
                           DateStart.SelectedDate ?? DateTime.Now.AddDays(7),
                           int.TryParse(TxtDuration.Text, out int d) ? d : 1
                       );

                if (ChkGuide.IsChecked == true) builder.AddActivity(TourActivity.Guide);
                if (ChkBeach.IsChecked == true) builder.AddActivity(TourActivity.Beach);
                if (ChkSpa.IsChecked == true) builder.AddActivity(TourActivity.Spa);
                if (ChkSkiing.IsChecked == true) builder.AddActivity(TourActivity.Skiing);

                TravelPackage newTour = builder.Build();

                await _tourManager.AddTourAsync(newTour);

                _tours.Add(newTour);

                MessageBox.Show("Tour created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserManager.GetInstance.Logout();
            Window.GetWindow(this)?.Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadToursAsync();
        }
        private async Task LoadToursAsync()
        {
            var list = await _tourManager.GetAllToursAsync();
            _tours = new ObservableCollection<TravelPackage>(list);

            if (DgTours != null)
            {
                DgTours.ItemsSource = _tours;
            }
        }

        private void ClearForm()
        {
            TxtTourName.Clear();
            TxtTourPrice.Clear();
            TxtDescription.Clear();
            TxtDuration.Clear();
            DateStart.SelectedDate = null;
            ChkGuide.IsChecked = ChkBeach.IsChecked = ChkSpa.IsChecked = ChkSkiing.IsChecked = false;
        }

    }
}
