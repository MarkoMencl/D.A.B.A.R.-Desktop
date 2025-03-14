using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BusinessLogicLayer.Services;
using EntitiesLayer.HelperEntities;
using EntitiesLayer.Entities;
using BusinessLogicLayer.Managers;
using System.Windows.Input;
using Entities.SharedServices;

namespace PresentationLayer
{
    public partial class UcSearchAds : UserControl
    {
        private readonly string _query;
        private readonly int? _categoryId;
        private readonly AdService adService;
        private readonly LocationService locationService;
        private readonly CategoryService categoryService;
        private readonly FavoriteService favoriteService;

        public UcSearchAds(string query = "", int? categoryId = null)
        {
            InitializeComponent();
            _query = query;
            _categoryId = categoryId;
            adService = new AdService();
            locationService = new LocationService();
            categoryService = new CategoryService();
            favoriteService = new FavoriteService();
            Loaded += UcSearchAds_Loaded;
        }

        private async void UcSearchAds_Loaded(object sender, RoutedEventArgs e)
        {
            await SetCategoryTitle();
            PopulateLocationFilter();
            await LoadAdsAsync();
        }

        private async Task SetCategoryTitle()
        {
            if (_categoryId.HasValue)
            {
                var category = await categoryService.GetCategoryByIdAsync(_categoryId.Value);
                CategoryTitle.Text = $"Ads from {LocalizationService.GetLocalizedString(category?.localizationkey) ?? "Unknown Category"}";
            }
            else
            {
                CategoryTitle.Text = "Ads from All Categories";
            }
        }

        private void PopulateLocationFilter()
        {
            var locations = locationService.GetLocations();
            var defaultLocation = new Location { id = 0, name = LocalizationService.GetLocalizedString("AllLocations") };
            var list = new List<Location> { defaultLocation };
            list.AddRange(locations);
            FilterComboBox.ItemsSource = list;
            FilterComboBox.DisplayMemberPath = "name";
            FilterComboBox.SelectedValuePath = "id";
            if (FilterComboBox.Items.Count > 0)
                FilterComboBox.SelectedIndex = 0;
        }

        private async Task LoadAdsAsync()
        {
            var extendedAds = await adService.GetFilteredSortedAdsAsync(_query, _categoryId, null, 0);
            AdsItemsControl.ItemsSource = extendedAds.Select(ad => new SearchAdViewModel
            {
                Id = ad.Id,
                ImageSource = ConvertBase64ToImageSource(ad.ImageBase64),
                Title = ad.Title,
                Description = ad.Description,
                UserId = ad.UserId,
                Views = ad.Views,
                Price = ad.Price,
                DateOfPublication = ad.Date_of_publication,
                Location = ad.Location
            }).ToList();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            int sortOption = SortComboBox.SelectedIndex;
            int? locationFilterId = FilterComboBox.SelectedValue as int?;
            if (locationFilterId.HasValue && locationFilterId.Value == 0)
                locationFilterId = null;

            var extendedAds = await adService.GetFilteredSortedAdsAsync(_query, _categoryId, locationFilterId, sortOption);
            AdsItemsControl.ItemsSource = extendedAds.Select(ad => new SearchAdViewModel
            {
                Id = ad.Id,
                ImageSource = ConvertBase64ToImageSource(ad.ImageBase64),
                Title = ad.Title,
                Description = ad.Description,
                UserId = ad.UserId,
                Views = ad.Views,
                Price = ad.Price,
                DateOfPublication = ad.Date_of_publication,
                Location = ad.Location
            }).ToList();
        }

        private void AddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                int adId = Convert.ToInt32(button.Tag);
                var currentUser = SessionManager.GetCurrentUser();

                if (currentUser == null)
                {
                    MessageBox.Show("You must be logged in to add favorites.", "Login Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                FavoriteService favoriteService = new FavoriteService();
                string resultMessage = favoriteService.AddToFavorites(currentUser.id, adId);

                MessageBox.Show(resultMessage, "Favorite Status", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void OnAdClick(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border != null && border.Tag != null)
            {
                int adId = Convert.ToInt32(border.Tag);
                var parentWindow = Window.GetWindow(this) as MainWindow;
                parentWindow.MainContentControl.Content = new AdDetailsUC(adId);
            }
        }


        private ImageSource ConvertBase64ToImageSource(string base64String)
            {
                if (string.IsNullOrEmpty(base64String) || base64String == "default_image_path")
                    return null;
                try
                {
                    string base64Data = base64String;
                    if (base64Data.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
                    {
                        var index = base64Data.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
                        if (index >= 0)
                            base64Data = base64Data.Substring(index + 7);
                    }
                    byte[] bytes = Convert.FromBase64String(base64Data);
                    using (var stream = new System.IO.MemoryStream(bytes))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                }
                catch (FormatException)
                {
                    return null;
                }
            }

        private class SearchAdViewModel
        {
            public int Id { get; set; }
            public ImageSource ImageSource { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int UserId { get; set; }
            public int Views { get; set; }
            public double Price { get; set; }
            public DateTime DateOfPublication { get; set; }
            public string Location { get; set; }
           }
    }
}
