using DataAcccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogicLayer.Services
{
    public class FavoriteService
    {
        public string AddToFavorites(int userId, int adId)
        {
            if (userId <= 0)
                return "You must be logged in to add favorites.";

            using (var repo = new FavoriteRepository())
            {
                if (repo.IsFavoriteExists(userId, adId))
                    return "This ad is already in your favorites.";

                var favorite = new FavoriteAdCollection
                {
                    user_id = userId,
                    ad_id = adId
                };

                bool success = repo.AddFavorite(favorite);
                return success ? "Ad added to favorites successfully!" : "Failed to add the ad to favorites.";
            }
        }

        public List<FavoriteAdCollection> GetFavoritesByUserId(int userId)
        {
            using (var repo = new FavoriteRepository())
            {
                return repo.GetFavoritesByUserId(userId).ToList();
            }
        }


        public bool RemoveFromFavorites(int userId, int adId)
        {
            using (var repo = new FavoriteRepository())
            {
                return repo.RemoveFavorite(userId, adId);
            }
        }
    }
}
