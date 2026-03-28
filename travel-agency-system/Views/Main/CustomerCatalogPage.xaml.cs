using System;
using System.Collections.Generic;
using System.Linq;
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
using travel_agency_system.Interfaces;
using System.Xml.Serialization;

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
        private readonly DataSearchEngine<TravelPackage> _searchEngine = new();
        private readonly ITourFilterService _tourFilterService= new TourFilterService();
        private  List<TravelPackage> _allToursCache = new();

        private bool _isMasking = false;

        public CustomerCatalogPage()
        {
            InitializeComponent();
            _currentCustomer = UserManager.GetInstance.CurrentUser as Customer;

            _tourFilterService.OnFilterCompleted += (filteredTours) =>
            {
                string query = TxtSearch?.Text?.Trim() ?? string.Empty;
                _searchEngine.Search(filteredTours, query);
            };

            _searchEngine.OnSearchCompleted += (finalTours) =>
            {
                DgTours.ItemsSource = finalTours;
            };
        }

        private void ApplyFiltersAndSearch()
        {
            if (_allToursCache == null || !_allToursCache.Any()) return;
            FilterCategory selectedCategory = CmbFilterCategory != null ? (FilterCategory)CmbFilterCategory.SelectedIndex : FilterCategory.All;
            SortOrder selectedOrder = CmbSortOrder != null ? (SortOrder)CmbSortOrder.SelectedIndex : SortOrder.Ascending;

            string? minVal = TxtMinValue?.Text?.Trim();
            string? maxVal = TxtMaxValue?.Text?.Trim();

            var options = new TourFilterOptions
            {
                Category = selectedCategory,
                Order = selectedOrder,
                MinValue = string.IsNullOrEmpty(minVal) ? null : minVal,
                MaxValue = string.IsNullOrEmpty(maxVal) ? null : maxVal
            };

            _tourFilterService.ApplyFilters(_allToursCache, options);
        }

        private async void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCustomer == null) return;
            TopUpPanel.Visibility = Visibility.Collapsed;
            HistoryPanel.Visibility = Visibility.Visible;

            try
            {
                var myTransactions = await _transactionManager.GetTransactionsByCustomerAsync(_currentCustomer.Id);
                DgHistory.ItemsSource = myTransactions.OrderByDescending(t => t.TransactionDate).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            RootDialog.IsOpen = true;
        }

        private void BtnTopUp_Click(object sender, RoutedEventArgs e)
        {
            HistoryPanel.Visibility = Visibility.Collapsed;
            TopUpPanel.Visibility = Visibility.Visible;
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

            _allToursCache = await _tourManager.GetAllToursAsync();
            DgTours.ItemsSource = _allToursCache;
            ApplyFiltersAndSearch();
            UpdateBalanceUI();
        }

        private async void BtnConfirmTopUp_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCustomer == null) return;

            if (double.TryParse(TxtTopUpAmount.Text, out double amount) && amount > 0)
            {
                _currentCustomer.TopUp(amount);
                await UserManager.GetInstance.UpdateCustomerAsync(_currentCustomer);
                UpdateBalanceUI();

                RootDialog.IsOpen = false;

                MessageBox.Show($"Balance successfully topped up by {amount:F2}$!",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
            {
                MessageBox.Show($"Error updating balance: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                    _allToursCache = await _tourManager.GetAllToursAsync();
                    ApplyFiltersAndSearch();

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

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            ApplyFiltersAndSearch();
        }

        private void FilterText_Changed(object sender, TextChangedEventArgs e)
        {
            if (CmbFilterCategory?.SelectedIndex == (int)FilterCategory.Date && sender is TextBox tb)
            {
                ApplyDateMasking(tb);
            }
            ApplyFiltersAndSearch();
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {

            if (CmbFilterCategory != null && TxtMinValue!=null && TxtMaxValue!=null) 
            {
                TxtMaxValue.TextChanged -= FilterText_Changed;
                TxtMinValue.TextChanged -= FilterText_Changed;

                TxtMinValue.Clear();
                TxtMaxValue.Clear();
                TxtMaxValue.IsEnabled = true;
                TxtMinValue.IsEnabled = true;
                if (CmbFilterCategory.SelectedIndex == (int)FilterCategory.Date)
                {
                    MaterialDesignThemes.Wpf.HintAssist.SetHint(TxtMinValue, "dd.mm.yyyy");
                    MaterialDesignThemes.Wpf.HintAssist.SetHint(TxtMaxValue, "dd.mm.yyyy");
                }
                else if (CmbFilterCategory.SelectedIndex == (int)FilterCategory.Price)
                {
                    MaterialDesignThemes.Wpf.HintAssist.SetHint(TxtMinValue, "Min Price");
                    MaterialDesignThemes.Wpf.HintAssist.SetHint(TxtMaxValue, "Max Price");
                }
                else
                {
                    MaterialDesignThemes.Wpf.HintAssist.SetHint(TxtMinValue, "Min Value");
                    MaterialDesignThemes.Wpf.HintAssist.SetHint(TxtMaxValue, "Max Value");
                    TxtMaxValue.IsEnabled = false;
                    TxtMinValue.IsEnabled = false;
                }
                TxtMinValue.TextChanged += FilterText_Changed;
                TxtMaxValue.TextChanged += FilterText_Changed;
            }
            ApplyFiltersAndSearch();
        }

        private void ApplyDateMasking(TextBox tb)
        {
            if (_isMasking) return;
            _isMasking = true;
            string rawText = tb.Text.Replace(".", "");
            rawText = new string(rawText.Where(char.IsDigit).ToArray());

            if (rawText.Length > 8) rawText = rawText.Substring(0, 8);

            string maskedText = "";
            for (int i = 0; i < rawText.Length; i++)
            {
                if (i == 2 || i == 4) maskedText += ".";
                maskedText += rawText[i];
            }

            tb.Text = maskedText;
            tb.CaretIndex = maskedText.Length;

            _isMasking = false;
        }
    }
}