using BusinessLogicLayer.Managers;
using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using EntitiesLayer.HelperEntities;
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
    /// <summary>
    /// Interaction logic for UcReview.xaml
    /// </summary>
    public partial class UcReview : UserControl
    {
        private Rating Review;

        public UcReview(ReviewWithUsernames review, bool isUpdateable)
        {
            InitializeComponent();
            Review = review.Review;

            txtblUsername.Text = review.User.username;
            txtblReviewContent.Text = review.Review.comment;
            txtblGrade.Text = review.Review.value.ToString() + "/5";

            ImageService service = new ImageService();

            var user = SessionManager.GetCurrentUser();

            if (isUpdateable)
            {
                btnUpdateReview.Visibility = Visibility.Visible;
            }

            if(review.User.image_id != null)
            {
                var image = service.GetUserImage((int)review.User.image_id);

                if (image != null && image.bitmap != null)
                {
                    using (var stream = new System.IO.MemoryStream(image.bitmap))
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = stream;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        imgUserImage.Source = bitmapImage;
                    }
                }
            }
        }

        private void btnUpdateReview_Click(object sender, RoutedEventArgs e)
        {
            User user = SessionManager.GetCurrentUser();

            var ucReview = new UcReviewPopupDialogue(Review);

            ucReview.CloseAction = () => popupAddReview.IsOpen = false;
            popupAddReview.Child = ucReview;

            popupAddReview.IsOpen = true;
        }
    }
}
