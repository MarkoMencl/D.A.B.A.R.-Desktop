using BusinessLogicLayer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
using EntitiesLayer.Entities;
using System.IO;
using BusinessLogicLayer.Managers;

namespace PresentationLayer
{
    public partial class AdDetailsUC : UserControl
    {
        private int adId;
        private int posterId;
        private readonly AdService adService;

        public AdDetailsUC(int id)
        {
            InitializeComponent();
            adId = id;
            adService = new AdService();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await PrepareAdAsync(adId);
        }

        private async Task PrepareAdAsync(int adId)
        {
            adService.IncrementAdViewsAsync(adId);
            var ad = await FetchIdAsync(adId);
            var images = ad.ImageAdCollections.Select(iac => iac.Image).ToList();
            Console.WriteLine("SLIKE" + images.Count);
            Console.WriteLine("SLIKE" + images[0].ToString());
            posterId = ad.user_id;

            var user = SessionManager.GetCurrentUser();

            if (user != null)
            {
                if (user.id == ad.user_id)
                {
                    btnAddReview.Visibility = Visibility.Hidden;
                    btnSendMessage.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                btnAddReview.Visibility = Visibility.Hidden;
                btnSendMessage.Visibility = Visibility.Hidden;
            }

            await Dispatcher.InvokeAsync(async () =>
            {
                txtAdId.Text = adId.ToString();
                txtPrice.Text = ad.price.ToString();
                txtDescription.Text = ad.description;
                txtContact.Text = ad.User.contact;
                txtEmail.Text = ad.User.email;
                txtStatus.Text = GetStatusText(ad.status);
                txtUser.Text = ad.User.username;
                txtTitle.Text = ad.title;

                if (images.Count > 0)
                {
                    var bitmap = LoadImage(images[0].bitmap);
                    pboAd.Source = bitmap;
                }
                else
                {
                    pboAd.Source = null;
                }

                var loc = await GetLocationAsync(ad.User.location_id);
                txtLocation.Text = loc.name;

                if (ad.User != null && ad.User.Image != null)
                {
                    PFP.Source = LoadImage(ad.User.Image.bitmap);
                }
                else
                {
                    PFP.Source = null;
                }


                galleryStrip.Children.Clear();

                if (ad.ImageAdCollections != null)
                {
                    foreach (var image in ad.ImageAdCollections)
                    {
                        Console.WriteLine("SLIKA" + image.Image.bitmap);
                        System.Windows.Controls.Image img = new System.Windows.Controls.Image
                        {
                            Source = LoadImage(image.Image.bitmap),
                            Width = 100,
                            Height = 100,
                            Stretch = Stretch.UniformToFill,
                            Margin = new Thickness(5),
                            Cursor = Cursors.Hand
                        };

                        img.MouseLeftButtonDown += (s, args) => pboAd.Source = LoadImage(image.Image.bitmap);

                        galleryStrip.Children.Add(img);
                    }
                }
            });

            ccUserReviews.Content = new UcUserReviews(ad.user_id);
        }

        private void AddToFavorites_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
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


        private string GetStatusText(int status)
        {
            switch (status)
            {
                case 0: return LocaleManager.GetLocalizedString("Used");
                case 1: return LocaleManager.GetLocalizedString("AsNew");
                case 2: return LocaleManager.GetLocalizedString("New");
                case 3: return LocaleManager.GetLocalizedString("Sold");
                default: return "Unknown";
            }
        }

        private BitmapImage LoadImage(byte[] imageData)
        {
            using (var ms = new MemoryStream(imageData))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        private async Task<Location> GetLocationAsync(int? location_id)
        {
            LocationService locationService = new LocationService();
            var loc = await locationService.GetLocationByIdAsync((int)location_id);
            return loc;
        }

        private async Task<Ad> FetchIdAsync(int adId)
        {
            var service = new AdService();
            var ad = await Task.Run(() => service.GetAdByIdAsync(adId));
            return ad;
        }

        private async void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            User user = SessionManager.GetCurrentUser();

            if (user != null)
            {
                var service = new AdService();
                var ad = await Task.Run(() => service.GetAdByIdAsync(adId));

                if (ad.user_id != user.id)
                {
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow.MainContentControl.Content = new UcMessageMain(ad.user_id);
                }
                else
                {
                    MessageBox.Show(LocaleManager.GetLocalizedString("UserForbidSelfMessage"));
                }
            }
            else
            {
                MessageBox.Show(LocaleManager.GetLocalizedString("UserNotLoggedIn"));
            }
        }

        private void btnAddReview_Click(object sender, RoutedEventArgs e)
        {
            User user = SessionManager.GetCurrentUser();

            var ucReview = new UcReviewPopupDialogue(user.id, posterId);

            ucReview.CloseAction = () => popupAddReview.IsOpen = false;
            popupAddReview.Child = ucReview;

            popupAddReview.IsOpen = true;
        }
    }
}
