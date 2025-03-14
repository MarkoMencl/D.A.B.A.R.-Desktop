using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using BusinessLogicLayer.Managers;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using System.CodeDom;
using System.ComponentModel;

namespace PresentationLayer
{
    public partial class UcUpdateAd : UserControl
    {
        private readonly AdService adService = new AdService();
        private Ad adToUpdate;
        public ObservableCollection<Category> Categories { get; set; }

        public UcUpdateAd(Ad ad)
        {
            InitializeComponent();
            adToUpdate = ad;
            Categories = new ObservableCollection<Category>();
            cmbStatus.ItemsSource = new string[]
            {
                LocaleManager.GetLocalizedString("Used"),
                LocaleManager.GetLocalizedString("AsNew"),
                LocaleManager.GetLocalizedString("New")
            };
            FetchCategories();
        }

        private async void FetchCategories()
        {
            var categoryService = new CategoryService();
            var categoriesFromManager = await categoryService.GetCategoriesAsync();
            Categories.Clear();
            foreach (var category in categoriesFromManager)
            {
                category.localizationkey = LocaleManager.GetLocalizedString(category.localizationkey);
                Categories.Add(category);
            }
            cmbCategory.ItemsSource = Categories;
            cmbCategory.DisplayMemberPath = "localizationkey";
            PopulateFields();
        }

        private void PopulateFields()
        {
            txtTitle.Text = adToUpdate.title;
            txtDescription.Text = adToUpdate.description;
            txtPrice.Text = adToUpdate.price.ToString();
            var selectedCategory = Categories.FirstOrDefault(c => c.id == adToUpdate.category_id);
            if (selectedCategory != null)
                cmbCategory.SelectedItem = selectedCategory;
            string statusText = adToUpdate.status == 2 ? LocaleManager.GetLocalizedString("New")
                              : adToUpdate.status == 1 ? LocaleManager.GetLocalizedString("AsNew")
                              : LocaleManager.GetLocalizedString("Used");
            cmbStatus.SelectedItem = statusText;
            imgSelected.Source = ConvertImageToDisplay(adToUpdate.ImageAdCollections.FirstOrDefault()?.Image.bitmap);
        }

        private BitmapImage ConvertImageToDisplay(byte[] imageData)
        {
            if (imageData == null)
                return null;
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

        private async void btnUpdateAd_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            Category selectedCategory = cmbCategory.SelectedItem as Category;
            string statusText = cmbStatus.SelectedItem as string;
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || selectedCategory == null || string.IsNullOrEmpty(statusText))
            {
                MessageBox.Show("Sva polja su obavezna.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            decimal price;
            if (!decimal.TryParse(txtPrice.Text.Trim(), out price))
            {
                MessageBox.Show("Cijena nije ispravna.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int categoryId = selectedCategory.id;
            int statusInt = statusText == LocaleManager.GetLocalizedString("New") ? 2 :
                            statusText == LocaleManager.GetLocalizedString("AsNew") ? 1 : 0;
            BitmapImage image = imgSelected.Source as BitmapImage;
            byte[] imageBytes = null;
            string imageFormat = null;
            int? imageSize = null;
            if (image != null)
            {
                imageBytes = ConvertBitmapImageToByteArray(image);
                imageFormat = "png";
                imageSize = imageBytes.Length;
            }
            bool isSuccessful = await adService.UpdateAdAsync(adToUpdate.id, title, description, (double)price, categoryId, statusInt, imageBytes, imageFormat, imageSize);
            if (isSuccessful)
            {
                MessageBox.Show("Oglas uspješno ažuriran!", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                    mainWindow.MainContentControl.Content = new UcMyAds();
                RefreshScreen();
            }
            else
            {
                MessageBox.Show("Ažuriranje oglasa nije uspjelo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                RefreshScreen();
            }
        }

        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            RefreshScreen();
        }

        private void RefreshScreen()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var newProfile = new UserProfileUC();
                newProfile.UcProfile.Content = new UcMyAds();

                mainWindow.MainContentControl.Content = newProfile;
            }
        }

        private void btnChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                imgSelected.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private byte[] ConvertBitmapImageToByteArray(BitmapImage image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        private string GetImageFormat(BitmapImage image)
        {
            return "png";
        }
    }
}
