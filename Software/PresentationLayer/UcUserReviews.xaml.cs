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

namespace PresentationLayer
{
    /// <summary>
    /// Interaction logic for UcUserReviews.xaml
    /// </summary>
    public partial class UcUserReviews : UserControl
    {
        public UcUserReviews()
        {
            InitializeComponent();
            GetReviews();
        }

        public UcUserReviews(int otherUserId)
        {
            InitializeComponent();
            GetReceivedReviews(otherUserId);
            lblSendReviews.Visibility = Visibility.Collapsed;
            scrlSentReviews.Visibility = Visibility.Collapsed;
        }

        private ReviewService service = new ReviewService();

        private void GetReviews()
        {
            stpSentReviews.Children.Clear();
            stpSentReviews.Children.Clear();

            var user = SessionManager.GetCurrentUser();

            if(user != null)
            {
                GetSentReviews(user.id);
                GetReceivedReviews(user.id);
            }
        }

        private void GetSentReviews(int id)
        {
            List<ReviewWithUsernames> reviews = service.GetSentReviews(id);

            foreach (var review in reviews)
            {
                stpSentReviews.Children.Add(new UcReview(review, true));
            }
        }

        private void GetReceivedReviews(int id)
        {
            List<ReviewWithUsernames> reviews = service.GetReceivedReviews(id);

            foreach (var review in reviews)
            {
                stpReceivedReviews.Children.Add(new UcReview(review, false));
            }
        }
    }
}
