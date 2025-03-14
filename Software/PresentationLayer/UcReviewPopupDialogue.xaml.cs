using BusinessLogicLayer.Services;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for UcReviewPopupDialogue.xaml
    /// </summary>
    public partial class UcReviewPopupDialogue : UserControl
    {
        private int ReviewerId;
        private int RevieweeId;

        private Rating Review = null;

        public UcReviewPopupDialogue(int reviewerId, int revieweeId)
        {
            InitializeComponent();
            ReviewerId = reviewerId;
            RevieweeId = revieweeId;
        }

        public UcReviewPopupDialogue(Rating review)
        {
            InitializeComponent();

            txtGrade.Text = review.value.ToString();
            txtDescription.Text = review.comment;
            ReviewerId = review.user_id_rater;
            RevieweeId = review.user_id_ratee;
            Review = new Rating();
            Review = review;
        }

        public Action CloseAction { get; set; }

        private void btnReviewUser_Click(object sender, RoutedEventArgs e)
        {
            ReviewService service = new ReviewService();

            if (IsGradeValid(txtGrade.Text) && txtDescription.Text.Trim().Length != 0)
            {
                if (Review != null)
                {
                    Review.value = Int32.Parse(txtGrade.Text);
                    Review.comment = txtDescription.Text;

                    service.UpdateReview(Review);
                }else
                {
                    Rating review = new Rating();
                    review.user_id_rater = ReviewerId;
                    review.user_id_ratee = RevieweeId;
                    review.comment = txtDescription.Text;
                    review.value = Int32.Parse(txtGrade.Text);

                    service.AddReview(review);
                }

                CloseAction?.Invoke();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            CloseAction?.Invoke();
        }

        private bool IsGradeValid(string input)
        {
            string regex = "^[1-5]$";

            return Regex.IsMatch(input, regex);
        }
    }
}
