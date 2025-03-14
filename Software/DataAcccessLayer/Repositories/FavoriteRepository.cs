using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DataAcccessLayer;
using EntitiesLayer.Entities;

namespace DataAcccessLayer.Repositories
{
    public class FavoriteRepository : Repository<FavoriteAdCollection>
    {
        public FavoriteRepository() : base(new DabarModel()) { }

        public bool AddFavorite(FavoriteAdCollection favorite)
        {
            Context.Set<FavoriteAdCollection>().Add(favorite);
            return Context.SaveChanges() > 0;
        }


        public bool RemoveFavorite(int userId, int adId)
        {
            var favorite = Entities.FirstOrDefault(f => f.user_id == userId && f.ad_id == adId);
            if (favorite != null)
            {
                Entities.Remove(favorite);
                return SaveChanges() > 0;
            }
            return false;
        }

        public bool IsFavoriteExists(int userId, int adId)
        {
            return Entities.Any(f => f.user_id == userId && f.ad_id == adId);
        }

        public override int Update(FavoriteAdCollection entity, bool saveChanges = true)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<FavoriteAdCollection> GetFavoritesByUserId(int userId)
        {
            return Entities
                .Where(f => f.user_id == userId)
                .Include(f => f.Ad)
                .Include(f => f.Ad.ImageAdCollections.Select(iac => iac.Image));
        }

    }
}
