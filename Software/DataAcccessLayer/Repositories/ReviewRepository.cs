using EntitiesLayer.Entities;
using EntitiesLayer.HelperEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcccessLayer.Repositories
{
    public class ReviewRepository : Repository<Rating>
    {
        public ReviewRepository() : base(new DabarModel())
        {
        }

        public IQueryable<ReviewWithUsernames> GetSentReviews(int id)
        {
            var query = from sr in Entities
                        join receiver in Context.Users on sr.user_id_ratee equals receiver.id
                        where sr.user_id_rater == id
                        select new ReviewWithUsernames
                        {
                            Review = sr,
                            User = receiver
                        };

            return query;
        }

        public IQueryable<ReviewWithUsernames> GetReceivedReviews(int id)
        {
            var query = from sr in Entities
                        join sender in Context.Users on sr.user_id_rater equals sender.id
                        where sr.user_id_ratee == id
                        select new ReviewWithUsernames
                        {
                            Review = sr,
                            User = sender
                        };

            return query;
        }

        public override int Update(Rating entity, bool saveChanges = true)
        {
            var review = Entities.SingleOrDefault(r => r.id == entity.id);

            review.value = entity.value;
            review.comment = entity.comment;

            if (saveChanges)
            {
                return SaveChanges();
            }

            return 0;
        }
    }
}
