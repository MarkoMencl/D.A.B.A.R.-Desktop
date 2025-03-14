using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Managers;
using System.Windows.Media;
using System.Windows.Input;

namespace PresentationLayer
{
    public partial class UcFavorites : UserControl
    {
        private readonly FavoriteService favoriteService;
        private readonly AdService adService;

        public UcFavorites()
        {
            InitializeComponent();
            favoriteService = new FavoriteService();
            adService = new AdService();
            Loaded += UcFavorites_Loaded;
        }

        private void UcFavorites_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFavorites();
        }

        private void LoadFavorites()
        {
            var currentUser = SessionManager.GetCurrentUser();
            if (currentUser == null)
            {
                MessageBox.Show("You must be logged in to view favorites.", "Login Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var favoriteAds = favoriteService.GetFavoritesByUserId(currentUser.id);

            FavoritesItemsControl.ItemsSource = favoriteAds.Select(ad => new
            {
                Id = ad.ad_id,
                Title = ad.Ad?.title ?? "No Title",
                Description = ad.Ad?.description ?? "No Description",
                ImageSource = ad.Ad?.ImageAdCollections?.FirstOrDefault()?.Image?.bitmap != null
                    ? ConvertBase64ToImageSource(Convert.ToBase64String(ad.Ad.ImageAdCollections.First().Image.bitmap))
                    : null
            }).ToList();
        }

        private void RemoveFromFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int adId = Convert.ToInt32(button.Tag);
                int userId = SessionManager.GetCurrentUser().id;

                FavoriteService favoriteService = new FavoriteService();
                bool isRemoved = favoriteService.RemoveFromFavorites(userId, adId);

                if (isRemoved)
                {
                    MessageBox.Show("Ad removed from favorites successfully!", "Favorite Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to remove the ad from favorites.", "Favorite Status", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                LoadFavorites(); 
            }
        }

        private void OnFavoriteItemClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null && grid.Tag != null)
            {
                int adId = Convert.ToInt32(grid.Tag);
                var parentWindow = Window.GetWindow(this) as MainWindow;
                parentWindow.MainContentControl.Content = new AdDetailsUC(adId);
            }
        }




        private ImageSource ConvertBase64ToImageSource(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            try
            {
                byte[] bytes = Convert.FromBase64String(base64String);
                using (var stream = new System.IO.MemoryStream(bytes))
                {
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
