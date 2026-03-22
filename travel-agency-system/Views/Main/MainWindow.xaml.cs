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
using travel_agency_system.Models;
using travel_agency_system.Services;

namespace travel_agency_system.Views.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadStartPage();
        }

        private void LoadStartPage()
        {
            var user = UserManager.GetInstance.CurrentUser;

            if (user is Admin)
            {
                MainFrame.Navigate(new AdminPage());
            }
            else
            {
                
                MainFrame.Navigate(new CustomerCatalogPage());
            }
        }    }
}
