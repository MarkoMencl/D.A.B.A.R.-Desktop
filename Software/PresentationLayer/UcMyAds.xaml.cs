using System;
using System.Collections.Generic;
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

using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using System.IO;
using BusinessLogicLayer.Managers;

namespace PresentationLayer
{
    public partial class UcMyAds : UserControl
    {
        private readonly AdService adService;

        public UcMyAds()
        {
            InitializeComponent();
            adService = new AdService();
            LoadUserAds();
        }

        private void LoadUserAds()
        {
            var user = SessionManager.GetCurrentUser();
            if (user == null)
            {
                MessageBox.Show("Niste prijavljeni.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<Ad> myAds = adService.GetUserAds(user.id);

            var displayAds = myAds.Select(ad => new
            {
                ad.id,
                ad.title,
                ad.description,
                ad.price,
                ImageSource = ConvertImageToDisplay(ad.ImageAdCollections.FirstOrDefault()?.Image.bitmap)
            }).ToList();

            MyAdsItemsControl.ItemsSource = displayAds;
        }


        private BitmapImage ConvertImageToDisplay(byte[] imageData)
        {
            if (imageData == null) return null;

            using (var stream = new MemoryStream(imageData))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
        }

        private void DeleteAd_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int adId)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite obrisati oglas?", "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    bool isDeleted = adService.DeleteAd(adId);

                    if (isDeleted)
                    {
                        MessageBox.Show("Oglas uspješno obrisan!", "Brisanje uspješno", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUserAds(); 
                    }
                    else
                    {
                        MessageBox.Show("Brisanje oglasa nije uspjelo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void UpdateAd_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int adId)
            {
                
                Ad adToUpdate = adService.GetAdByIdAsync(adId); 
                if (adToUpdate == null)
                {
                    MessageBox.Show("Oglas nije pronađen.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                
                UcUpdateAd updateAdControl = new UcUpdateAd(adToUpdate);   

                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainContentControl.Content = updateAdControl;
                }
            }
        }


    }
}
