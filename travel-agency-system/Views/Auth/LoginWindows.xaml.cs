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
using System.Windows.Shapes;
using travel_agency_system.Services;

namespace travel_agency_system.Views.Auth
{
    /// <summary>
    /// Interaction logic for LoginWindows.xaml
    /// </summary>
    public partial class LoginWindows : Window
    {
        public LoginWindows()
        {
            InitializeComponent();
            AuthFrame.Navigate(new LoginPage());
            this.Loaded += LoginWindows_Loaded;
        }

        private async void LoginWindows_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await UserManager.GetInstance.InitializeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження бази даних: {ex.Message}",
                                "Критична помилка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
