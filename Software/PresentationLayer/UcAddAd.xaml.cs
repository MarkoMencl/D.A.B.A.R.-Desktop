using BusinessLogicLayer.Managers;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PresentationLayer
{
    public partial class UcAddAd : UserControl
    {
        public ObservableCollection<Category> Categories { get; set; }
        private readonly AdService adService = new AdService();

        public UcAddAd()
        {
            InitializeComponent();
            Categories = new ObservableCollection<Category>();
            cmbStatus.ItemsSource = new List<string> { LocaleManager.GetLocalizedString("Used"), LocaleManager.GetLocalizedString("AsNew"), LocaleManager.GetLocalizedString("New") };
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
        }

        private void btnAddAd_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            Category selectedCategory = cmbCategory.SelectedItem as Category;
            string status = cmbStatus.SelectedItem as string;
            decimal price;
            bool isPriceValid = decimal.TryParse(txtPrice.Text.Trim(), out price);

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || selectedCategory == null ||
                string.IsNullOrEmpty(status) || !isPriceValid)
            {
                MessageBox.Show("All fields are required and must be valid.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int categoryId = selectedCategory.id;
            int statusInt = status == LocaleManager.GetLocalizedString("New") ? 2 :
                            status == LocaleManager.GetLocalizedString("AsNew") ? 1 : 0;

            BitmapImage image = imgSelected.Source as BitmapImage;
            byte[] imageBytes = null;
            string imageFormat = null;
            int imageSize = 0;

            if (image != null)
            {
                try
                {
                    imageBytes = ConvertBitmapImageToByteArray(image);
                    imageFormat = GetImageFormat(image);
                    imageSize = imageBytes.Length;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing image: {ex.Message}", "Image Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var user = SessionManager.GetCurrentUser();
            if (user == null)
            {
                MessageBox.Show("You must be logged in to post an ad.", "Login Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isSuccessful = adService.PostAd(title, description, (double)price, categoryId, statusInt, imageBytes, imageFormat, imageSize);

            if (isSuccessful)
            {
                MessageBox.Show("Ad successfully posted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.MainContentControl.Content = new UcMainPage();
            }
            else
            {
                MessageBox.Show("Failed to post the ad. Check the service layer logs.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
