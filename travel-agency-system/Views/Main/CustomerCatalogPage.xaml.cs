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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _currentCustomer = UserManager.GetInstance.CurrentUser as Customer;
            UpdateBalanceUI();
        }

        private void BtnBook_Click(object sender, RoutedEventArgs e)
        {

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

                    MessageBox.Show($"Рахунок успішно поповнено на {amount:F2}$!",
                        "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, введіть коректну суму більшу за 0.",
                    "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                txtUserBalance.Text = $"Balance: {_currentCustomer.Balance:F2}$";
            }
            catch (Exception ex) 
            {   MessageBox.Show($"Помилка оновлення балансу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}
