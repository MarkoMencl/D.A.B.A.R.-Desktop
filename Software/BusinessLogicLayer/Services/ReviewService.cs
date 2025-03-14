using DataAcccessLayer.Repositories;
using EntitiesLayer.Entities;
using EntitiesLayer.HelperEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ReviewService
    {
        public List<ReviewWithUsernames> GetSentReviews(int id)
        {
            using(var repo = new ReviewRepository())
            {
                List<ReviewWithUsernames> sentReviews = repo.GetSentReviews(id).ToList();

                return sentReviews;
            }
        }

        public List<ReviewWithUsernames> GetReceivedReviews(int id)
        {
            using(var repo = new ReviewRepository())
            {
                List<ReviewWithUsernames> receivedReviews = repo.GetReceivedReviews(id).ToList();

                return receivedReviews;
            }
        }

        public void AddReview(Rating review)
        {
            using(var repo = new ReviewRepository())
            {
                repo.Add(review);
            }
        }

        public void UpdateReview(Rating review)
        {
            using(var repo = new ReviewRepository())
            {
                repo.Update(review);
            }
        }
    }
}
