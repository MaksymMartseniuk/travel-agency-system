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
using travel_agency_system.Services;
using System.Text.RegularExpressions;

namespace travel_agency_system.Views.Main
{
    /// <summary>
    /// Interaction logic for CustomerCatalogPage.xaml
    /// </summary>
    public partial class CustomerCatalogPage : Page
    {
        private Customer? _currentCustomer;
        private readonly TourManager _tourManager = new TourManager();
        private readonly TransactionManager _transactionManager = new TransactionManager();
        public CustomerCatalogPage()
        {
            InitializeComponent();
            _currentCustomer = UserManager.GetInstance.CurrentUser as Customer;

        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnTopUp_Click(object sender, RoutedEventArgs e)
        {
            TxtTopUpAmount.Clear();
            RootDialog.IsOpen = true;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            UserManager.GetInstance.Logout();
            Window.GetWindow(this)?.Close();

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _currentCustomer = UserManager.GetInstance.CurrentUser as Customer;
            var tours = await _tourManager.GetAllToursAsync();
            DgTours.ItemsSource = tours;
            UpdateBalanceUI();
        }

        private async void BtnConfirmTopUp_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(TxtTopUpAmount.Text, out double amount) && amount > 0)
            {
                if (_currentCustomer != null)
                {
                    _currentCustomer.TopUp(amount);
                    await UserManager.GetInstance.UpdateCustomerAsync(_currentCustomer);
                    UpdateBalanceUI();
                    
                    RootDialog.IsOpen = false;

                    MessageBox.Show($"Balance successfully topped up by {amount:F2}$!",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid amount greater than 0.",
                    "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void UpdateBalanceUI()
        {
            try
            {
                txtUserBalance.Text = $"Balance: {_currentCustomer?.Balance:F2}$";
            }
            catch (Exception ex) 
            {   MessageBox.Show($"Помилка оновлення балансу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private async void BtnBookSelected_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCustomer == null) return;
            var selectedTour = DgTours.SelectedItem as TravelPackage;

            if (selectedTour == null)
            {
                MessageBox.Show("Please select a tour from the table first.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Do you want to book '{selectedTour.Name}' for {selectedTour.Price}$?",
                                 "Confirm Booking", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _currentCustomer.MakePurchase(selectedTour.Price);
                    await _transactionManager.RecordTransactionAsync(_currentCustomer.Id, selectedTour);
                    await UserManager.GetInstance.UpdateCustomerAsync(_currentCustomer);

                    UpdateBalanceUI();

                    MessageBox.Show($"Success! You have booked '{selectedTour.Name}'.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Payment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"System error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
