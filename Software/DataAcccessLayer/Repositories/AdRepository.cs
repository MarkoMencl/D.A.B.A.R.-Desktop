using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAcccessLayer;
using EntitiesLayer.Entities;

namespace DataAcccessLayer.Repositories
{
    public class AdRepository : Repository<Ad>
    {
        public AdRepository() : base(new DabarModel()) { }


        public Ad GetAdById(int id)
        {
            return Entities
                .Include(a => a.User)
                .Include(a => a.ImageAdCollections.Select(iac => iac.Image))
                .FirstOrDefault(x => x.id == id);
        }


        public int AddAd(Ad ad)
        {
            Add(ad);
            SaveChanges();
            return ad.id;
        }

        public bool DeleteAd(int adId)
        {
            var ad = Entities
                .Include(a => a.ImageAdCollections.Select(iac => iac.Image))
                .FirstOrDefault(a => a.id == adId);

            if (ad == null)
                return false;

            foreach (var imageAd in ad.ImageAdCollections)
            {
                if (imageAd.Image != null)
                {
                    Context.Images.Remove(imageAd.Image);
                }
            }

            Context.ImageAdCollections.RemoveRange(ad.ImageAdCollections);
            Entities.Remove(ad);

            return SaveChanges() > 0;
        }


        public IQueryable<Ad> GetUserAds(int userId)
        {
            
            return Entities
                .Where(ad => ad.user_id == userId)
                .Include(ad => ad.ImageAdCollections.Select(iac => iac.Image)); 
            
        }

        public void LinkImageToAd(int adId, int imageId)
        {
            var imageAd = new ImageAdCollection
            {
                ad_id = adId,
                image_id = imageId
            };

            Context.Set<ImageAdCollection>().Add(imageAd);
            SaveChanges();
        }

        public override int Update(Ad entity, bool saveChanges = true)
        {
            var existing = GetAdById(entity.id);
            if (existing == null)
                return 0;

            Context.Entry(existing).CurrentValues.SetValues(entity);
            return saveChanges ? SaveChanges() : 0;
        }

    }
}

