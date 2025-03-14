using BusinessLogicLayer.Managers;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using Microsoft.Win32;
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
using Image = EntitiesLayer.Entities.Image;

namespace PresentationLayer
{
    public partial class UserProfileUC : UserControl
    {
        private readonly ImageService imageService;
        private readonly LocationService locationService;
        private byte[] selectedImageBytes;

        public UserProfileUC()
        {
            InitializeComponent();
            locationService = new LocationService();
            imageService = new ImageService();
            LoadLocations();
            LoadUserProfile();
        }

        private void LoadLocations()
        {
            var locations = locationService.GetLocations();
            LocationComboBox.ItemsSource = locations;
        }

        private async void LoadUserProfile()
        {
            var user = SessionManager.GetCurrentUser();

            if (user != null)
            {
                UsernameTextBox.Text = user.username;
                EmailTextBox.Text = user.email;
                ContactTextBox.Text = user.contact;

                var location = null as Location;

                if (user.location_id.HasValue)
                {
                    location = await locationService.GetLocationByIdAsync(user.location_id.Value);
                }

                LocationComboBox.SelectedValuePath = "id";

                if (location != null)
                {
                    LocationComboBox.SelectedValue = location.id;
                }
                else
                {
                    if (LocationComboBox.Items.Count > 0)
                    {
                        LocationComboBox.SelectedIndex = 0;
                    }
                }




                if (!string.IsNullOrEmpty(user.language))
                {
                    foreach (ComboBoxItem item in LanguageComboBox.Items)
                    {
                        if (item.Tag != null && item.Tag.ToString() == user.language)
                        {
                            LanguageComboBox.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (user.image_id != null)
                {
                    var profileImage = imageService.GetUserImage((int)user.image_id);
                    if (profileImage != null && profileImage.bitmap != null)
                    {
                        using (var stream = new System.IO.MemoryStream(profileImage.bitmap))
                        {
                            var bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = stream;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();
                            ProfileImage.Source = bitmapImage;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("User data could not be loaded. Please check your session.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ImagePickerButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;

                selectedImageBytes = System.IO.File.ReadAllBytes(selectedImagePath);

                using (var stream = new System.IO.MemoryStream(selectedImageBytes))
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = stream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    ProfileImage.Source = bitmapImage;
                }
            }
        }

        private void MyAdsButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                UcProfile.Content = new UcMyAds();
            }
        }


        private void LocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedLocation = LocationComboBox.SelectedItem as string;
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = UsernameTextBox.Text.Trim();
                string email = EmailTextBox.Text.Trim();
                string contact = ContactTextBox.Text.Trim();

                var selectedLocation = LocationComboBox.SelectedItem as Location;
                int? locationId = selectedLocation?.id;

                string selectedLanguage = null;
                if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    switch (selectedItem.Content.ToString())
                    {
                        case "Hrvatski":
                            selectedLanguage = "hr";
                            break;
                        case "Engleski":
                            selectedLanguage = "en";
                            break;
                        case "Njemački":
                            selectedLanguage = "de";
                            break;
                    }
                }

                if (LanguageComboBox.SelectedItem is ComboBoxItem SelectedItem)
                {
                    selectedLanguage = SelectedItem.Tag?.ToString();

                    if (string.IsNullOrEmpty(selectedLanguage))
                    {
                        MessageBox.Show("Please select a language.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                if (string.IsNullOrWhiteSpace(email) || locationId == null || string.IsNullOrEmpty(selectedLanguage))
                {
                    MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int? imageId = null;
                var currentUser = SessionManager.GetCurrentUser();

                if (selectedImageBytes != null)
                {
                    if (currentUser.image_id.HasValue)
                    {
                        imageId = currentUser.image_id;
                        var existingImage = imageService.GetUserImage(imageId.Value);

                        if (existingImage != null)
                        {
                            existingImage.bitmap = selectedImageBytes;
                            existingImage.format = "image/jpeg";
                            existingImage.size = selectedImageBytes.Length;
                            imageService.SaveImage(existingImage);
                        }
                    }
                    else
                    {
                        var newImage = new Image
                        {
                            bitmap = selectedImageBytes,
                            format = "image/jpeg",
                            size = selectedImageBytes.Length
                        };
                        imageId = imageService.SaveImage(newImage);
                    }
                }
                else
                {
                    imageId = currentUser.image_id;
                }

                if (!string.IsNullOrEmpty(selectedLanguage))
                {
                    LocaleManager.SetLanguage(selectedLanguage);
                }

                var updatedProfile = new User
                {
                    id = currentUser.id,
                    username = username,
                    email = email,
                    contact = contact,
                    location_id = locationId,
                    language = selectedLanguage,
                    image_id = imageId
                };

                SessionManager.UpdateCurrentUser(updatedProfile);

                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.UpdateNavbar();

                var newUserProfileUC = new UserProfileUC();
                if (this.Parent is ContentControl parentControl)
                {
                    parentControl.Content = newUserProfileUC;
                }

                MessageBox.Show("Profile updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnShowReviews_Click(object sender, RoutedEventArgs e)
        {
            UcProfile.Content = new UcUserReviews();
        }
    }
}
