using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataAcccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace DataAcccessLayer.Repositories
{
    public class ImageAdCollectionRepository : Repository<ImageAdCollection>
    {
        public ImageAdCollectionRepository() : base(new DabarModel()) { }

        public void Insert(int adId, int imageId)
        {
            var imageAdCollection = new ImageAdCollection
            {
                ad_id = adId,
                image_id = imageId
            };

            Entities.Add(imageAdCollection);
            SaveChanges();
        }

        public override int Update(ImageAdCollection entity, bool saveChanges = true)
        {
            throw new NotImplementedException();
        }
    }
}

