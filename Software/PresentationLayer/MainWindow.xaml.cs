using BusinessLogicLayer.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using EntitiesLayer.Entities;

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.KeyDown += MainWindow_KeyDown;

            UpdateNavbar();
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                OpenPdfInBrowser();
            }
        }

        private void OpenPdfInBrowser()
        {
            string pdfPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Korisnička_dokumentacija_aplikacije_D_A_B_A_R_.pdf");

            if (File.Exists(pdfPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening PDF: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("PDF file not found!");
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UcMainPage();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UserProfileUC();
        }

        private void btnAddAd_Click(object sender, RoutedEventArgs e)
        {
            var user = SessionManager.GetCurrentUser();
            if (user == null)
            {
                MessageBox.Show(LocaleManager.GetLocalizedString("YouHaveToBeLoggedIn"));
                return;
            }
            MainContentControl.Content = new UcAddAd();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchTextBox.Text;
            MainContentControl.Content = new UcSearchAds(query);
        }

        private void FavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UcFavorites();
        }

        private void ProfileStatsButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UcProfileStats();
        }


        private void Image_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UcMainPage();
        }

        private void btnOdjava_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.RemoveCurrentUser();
            MainContentControl.Content = new UcMainPage();
            UpdateNavbar();
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UcRegistration();
            UpdateNavbar();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UcLogin();
            UpdateNavbar();
        }

        public void UpdateNavbar()
        {
            var user = SessionManager.GetCurrentUser();

            // Check for user session
            if (user == null)
            {
                btnStatistics.Visibility = Visibility.Collapsed;
                btnFavourites.Visibility = Visibility.Collapsed;
                btnMessages.Visibility = Visibility.Collapsed;
                btnProfile.Visibility = Visibility.Collapsed;
                btnOdjava.Visibility = Visibility.Collapsed;
                btnLogin.Visibility = Visibility.Collapsed;
                btnRegistration.Visibility = Visibility.Collapsed;

                if (MainContentControl.Content is UcLogin)
                {
                    btnRegistration.Visibility = Visibility.Visible;
                    btnLogin.Visibility = Visibility.Collapsed;
                }
                else if (MainContentControl.Content is UcRegistration)
                {
                    btnRegistration.Visibility = Visibility.Collapsed;
                    btnLogin.Visibility = Visibility.Visible;
                }
                else
                {
                    btnLogin.Visibility = Visibility.Visible;
                }
            }
            else
            {
                btnStatistics.Visibility = Visibility.Visible;
                btnFavourites.Visibility = Visibility.Visible;
                btnMessages.Visibility = Visibility.Visible;
                btnProfile.Visibility = Visibility.Visible;
                btnOdjava.Visibility = Visibility.Visible;

                btnRegistration.Visibility = Visibility.Collapsed;
                btnLogin.Visibility = Visibility.Collapsed;
            }

            btnStatistics.Content = LocaleManager.GetLocalizedString("Statistics");
            btnFavourites.Content = LocaleManager.GetLocalizedString("Favorites");
            btnMessages.Content = LocaleManager.GetLocalizedString("Messages");
            btnProfile.Content = LocaleManager.GetLocalizedString("Profile");
            btnOdjava.Content = LocaleManager.GetLocalizedString("Logout");
            btnLogin.Content = LocaleManager.GetLocalizedString("LoginTitle");
            btnRegistration.Content = LocaleManager.GetLocalizedString("Register");
            BtnTextBlock.Text = LocaleManager.GetLocalizedString("AddAd");
            SearchTextBox.Text = LocaleManager.GetLocalizedString("Search");
        }


        private void btnMessages_Click(object sender, RoutedEventArgs e)
        {
            MainContentControl.Content = new UcMessageMain();
        }
    }
}
