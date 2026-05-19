using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using System.Xml.Serialization;
using travel_agency_system.Extensions;
using travel_agency_system.Interfaces;
using travel_agency_system.Models;
using travel_agency_system.Services;
using System.Text.Json.Serialization;

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
        private readonly IDataManager<TravelPackage, TourFilterOptions> _dataCoordinator = new DataManager<TravelPackage, TourFilterOptions>();
        private  List<TravelPackage> _allToursCache = new();

        private int _currentPage = 1;
        private int _pageSize = 5;
        private List<TravelPackage> _currentFilteredTours = new();


        private bool _isMasking = false;

        public CustomerCatalogPage()
        {
            InitializeComponent();
            _currentCustomer = UserManager.GetInstance.CurrentUser as Customer;

            _dataCoordinator.OnDataProcessed += (results) =>
            {
                Dispatcher.Invoke(() =>
                {
                    _currentFilteredTours = results.ToList();
                    _currentPage = 1;
                    UpdatePaginationAndStatsUI();
                });
            };
        }

        private async void ApplyFiltersAndSearch()
        {
            if (_allToursCache == null || !_allToursCache.Any()) return;

            var options = GetCurrentOptions();
            string query = TxtSearch?.Text?.Trim() ?? string.Empty;

            await _dataCoordinator.ProcessAsync(_allToursCache, query, options);
        }
        private TourFilterOptions GetCurrentOptions()
        {
            return new TourFilterOptions
            {
                Category = CmbFilterCategory != null ? (FilterCategory)CmbFilterCategory.SelectedIndex : FilterCategory.All,
                Order = CmbSortOrder != null ? (SortOrder)CmbSortOrder.SelectedIndex : SortOrder.Ascending,
                MinValue = string.IsNullOrEmpty(TxtMinValue?.Text) ? null : TxtMinValue.Text.Trim(),
                MaxValue = string.IsNullOrEmpty(TxtMaxValue?.Text) ? null : TxtMaxValue.Text.Trim()
            };
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
            UpdateFilterInputsState();
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
            var selectedItem = DgTours.SelectedItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Please select a tour from the table first.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Guid searchId = Guid.Empty;
            if (selectedItem is DTOs.TravelPackageFullDto fullDto)
            {
                searchId = fullDto.Id;
            }
            else if (selectedItem is DTOs.TravelPackageCompactDto compactDto)
            {
                searchId = compactDto.Id;
            }

            var selectedTour = _allToursCache.FirstOrDefault(t => t.Id == searchId);
            if (selectedTour == null)
            {
                MessageBox.Show("System error: Could not find original tour details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (sender == CmbFilterCategory)
            {
                UpdateFilterInputsState();
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

        private void UpdateFilterInputsState()
        {
            if (CmbFilterCategory == null || TxtMinValue == null || TxtMaxValue == null) return;
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

        private void ViewMode_Changed(object sender, RoutedEventArgs e)
        {
            ApplyFiltersAndSearch();
        }

        private async void BtnCompare_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "Select External Tours Source",
                Filter = "JSON Files (*.json)|*.json"
            };

            if(fileDialog.ShowDialog() == true)
            {
                try
                {
                    List<TravelPackage> externalSourceTours = new List<TravelPackage>();
                    string jsonString = File.ReadAllText(fileDialog.FileName);
                    var options = new JsonSerializerOptions
                    {
                        Converters = { new JsonStringEnumConverter() },
                        PropertyNameCaseInsensitive = true
                    };
                    var loadedData = JsonSerializer.Deserialize<List<TravelPackage>>(jsonString, options);
                    if (loadedData != null) externalSourceTours = loadedData;
                    var currentTourIds = _allToursCache.Select(t => t.Id);
                    var uniqueNewTours = externalSourceTours.ExceptBy(currentTourIds, t => t.Id).ToList();
                    var combinedTours = _allToursCache
                        .Concat(uniqueNewTours)
                        .OrderBy(t => t.Name)
                        .ThenByDescending(t => t.StartDate)
                        .ToList();
                    _allToursCache = combinedTours;
                    await _tourManager.SaveAllToursAsync(_allToursCache);
                    ApplyFiltersAndSearch();
                    MessageBox.Show($"LINQ Set Operations completed!\n\n" +
                                    $"Tours in file: {externalSourceTours.Count}\n" +
                                    $"Duplicates ignored: {externalSourceTours.Count - uniqueNewTours.Count}\n" +
                                    $"New unique tours added: {uniqueNewTours.Count}",
                                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentFilteredTours.Count > _currentPage * _pageSize)
            {
                _currentPage++;
                UpdatePaginationAndStatsUI();
            }
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePaginationAndStatsUI();
            }
        }

        private void UpdatePaginationAndStatsUI()
        {
            if (!_currentFilteredTours.Any())
            {
                DgTours.ItemsSource = null;
                TxtPageNumber.Text = "0";
                TxtTotalCount.Text = "0";
                TxtAvgPrice.Text = "0.00 $";
                BtnPrevPage.IsEnabled = false;
                BtnNextPage.IsEnabled = false;
                return;
            }
            var stats = _currentFilteredTours.GetStatistics();
            TxtTotalCount.Text = stats.TotalCount.ToString();
            TxtAvgPrice.Text = stats.AveragePrice;
            var paginatedTours = _currentFilteredTours.Paginate(_currentPage, _pageSize).ToList();

            if (ChkCompactView?.IsChecked == true)
            {
                DgTours.ItemsSource = paginatedTours.ToCompactView().ToList();
            }
            else
            {
                DgTours.ItemsSource = paginatedTours.ToFullView().ToList();
            }

            TxtPageNumber.Text = _currentPage.ToString();
            BtnPrevPage.IsEnabled = _currentPage > 1;
            BtnNextPage.IsEnabled = _currentFilteredTours.Count > _currentPage * _pageSize;
        }
    }
}