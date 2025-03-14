using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAcccessLayer.Repositories
{
    public class ImageRepository : Repository<Image>
    {
        public ImageRepository() : base(new DabarModel())
        {
        }
        public int Insert(Image image)
        {
            Entities.Add(image);
            SaveChanges();
            Context.Entry(image).Reload();
            return image.id;
        }


        public Image GetImageById(int id)
        {
            return Entities.FirstOrDefault(img => img.id == id);
        }

        public override int Update(Image entity, bool saveChanges = true)
        {
            var existingImage = Entities.FirstOrDefault(img => img.id == entity.id);
            if (existingImage != null)
            {
                existingImage.bitmap = entity.bitmap;
                existingImage.format = entity.format;
                existingImage.size = entity.size;

                if (saveChanges)
                {
                    SaveChanges();
                }

                return existingImage.id;
            }

            return 0;
        }

        public int? GetLastInsertedImageId(int id)
        {
            using (var db = new DabarModel())
            {
                return db.Images
                         .Where(i => i.id == id)
                         .OrderByDescending(i => i.id)
                         .Select(i => (int?)i.id)
                         .FirstOrDefault();
            }
        }

        public List<Image> GetImagesByAdId(int adId)
        {
            using (var db = new DabarModel())
            {
                return db.Images
                         .Where(img => img.ImageAdCollections.Any(iac => iac.ad_id == adId))
                         .ToList();
            }
        }

        public async Task<Image> GetFirstImageByAdIdAsync(int adId)
        {
            return await Context.Images
                .Join(Context.ImageAdCollections,
                      img => img.id,
                      imgAd => imgAd.image_id,
                      (img, imgAd) => new { img, imgAd })
                .Where(joined => joined.imgAd.ad_id == adId)
                .Select(joined => joined.img)
                .FirstOrDefaultAsync();
        }

    }
}
